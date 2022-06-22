using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Manh.ILS.General.Interfaces;
using Manh.ILS.NHibernate.Entities;
using Manh.ILS.Shipping.BL;

using Manh.WMFW.Entities;
using Manh.WMFW.General;

using Manh.WMW.Configs.General;

namespace BHS.UWT.BLL
{


    

    public class DoNotSplitItems : Manh.ILS.Shipping.Interfaces.IPalletBuildingStrategy
    {
        /*
         * 1) I think for each container i'll put it into a wrapper that can track it's customer po and top location
         * 2) then build groups by item/po
         * 3) then combine items by location sequence
         * 4) then try to combine pos by location sequence
         */

        public List<List<ShippingContainer>> Execute(PalletBuildingMasterDetail pbmDetail, FilteredGroup<ShippingContainer> shippingContainerGroup, out List<ShippingContainer> containersNotPalletized)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START"));
            containersNotPalletized = new List<ShippingContainer>();

            List<List<ShippingContainer>> palletListList = new List<List<ShippingContainer>>();
            List<PalletHldr> pallets = new List<PalletHldr>();
            PalletHldr masterPallet = new PalletHldr();
            PalletHldr currentPallet = null;

            string currentItem = "~~GOAT~~";
            string currentPO = "~~GOAT~~";
            string currentLocation = "~~GOAT~~";

            #region Order By PO
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START Order By PO"));

            for (int i = 0; i < shippingContainerGroup.Items.Count; i++)
            {
                ShippingContainer cont = shippingContainerGroup.Items[i];
                ContainerHldr contHldr = new ContainerHldr(cont);
                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", contHldr.ToString()));

                masterPallet.AddContainer(contHldr);
            }

            masterPallet.SortByCustomerPO();

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "END Order By PO"));
            #endregion

            #region Create PO Pallets
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START Build PO Pallets"));

            foreach (ContainerHldr contHldr in masterPallet.Pllt)
            {

                if (contHldr.CustomerPO != currentPO)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: New PO: {0}", contHldr.CustomerPO));
                    currentPO = contHldr.CustomerPO;

                    if (currentPallet != null)
                        pallets.Add(currentPallet);

                    currentPallet = new PalletHldr();
                }

                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", contHldr.ToString()));
                currentPallet.AddContainer(contHldr);
            }

            if (currentPallet != null)
                pallets.Add(currentPallet);

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "END Build PO Pallets"));
            #endregion

            #region Split by Item
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START Split PO Pallets By Item"));
            List<PalletHldr> tempPallets = new List<PalletHldr>();

            // if a given Item/PO combination is too large for a given pallet then split it into item pallets, we will later try to combine things.
            foreach (PalletHldr pallet in pallets)
            {
                if (WillFit(pbmDetail, shippingContainerGroup, pallet.ShippingContList))
                {
                    tempPallets.Add(pallet);
                }
                else
                {
                    SplitPalletByItem(pallet, tempPallets, pbmDetail, shippingContainerGroup);
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: tempPallets.Count: {0}", tempPallets.Count));
                }
            }

            pallets = tempPallets;

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "END Split PO Pallets By Item"));
            #endregion

            #region Combine What You Can
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START Combine What You Can"));
            tempPallets = new List<PalletHldr>();

            Combine(pallets, tempPallets, pbmDetail, shippingContainerGroup);

            pallets = tempPallets;
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "END Combine What You Can"));
            #endregion

            #region Add Pallets to Final List
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START Add Pallets to Final List"));

            foreach (PalletHldr pallet in pallets)
            {
                palletListList.Add(pallet.ShippingContList);
                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", pallet));

            }

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "START Add Pallets to Final List"));
            #endregion

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Execute: {0}", "END"));

            return palletListList;
        }

        private void Combine(List<PalletHldr> itemPallets, List<PalletHldr> tempPallets, PalletBuildingMasterDetail pbmDetail, FilteredGroup<ShippingContainer> shippingContainerGroup)
        {
            #region Combine
            PalletHldr currentPallet = null;
            PalletHldr previousPallet = null;
            PalletHldr testPallet = null;
            List<PalletHldr> rejectPallets = new List<PalletHldr>();

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: {0}", "START Recombine in Location Order"));

            itemPallets.Sort(PalletHldr.ComparePalletByLocation);
            testPallet = new PalletHldr();
            previousPallet = new PalletHldr();
            int count = 0;
            while (itemPallets.Count > 0)
            {
                // get the top of the list
                testPallet.AddPallet(itemPallets[0]);
                previousPallet.AddPallet(itemPallets[0]);
                itemPallets.RemoveAt(0);
                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: Outer Loop::testPallet: {0}", testPallet.ToString()));

                while (itemPallets.Count > 0)
                {
                    currentPallet = itemPallets[0];
                    itemPallets.RemoveAt(0);

                    testPallet.AddPallet(currentPallet);
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: Inner Loop::testPallet: {0}", testPallet.ToString()));

                    // add top of list to current container and test fit
                    if (WillFit(pbmDetail, shippingContainerGroup, testPallet.ShippingContList))
                    {
                        previousPallet.AddPallet(currentPallet);
                        System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: FIT::GOOD Pallet: {0}", currentPallet.ToString()));
                    }
                    else
                    {
                        rejectPallets.Add(currentPallet);
                        System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: NOFIT::rejectPallets: {0}", rejectPallets.Count));
                        System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: NOFIT::BAD Pallet: {0}", currentPallet.ToString()));

                        testPallet.Clear();
                        testPallet.AddPallet(previousPallet);
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: Outer Loop::previousPallet: {0}", previousPallet.ToString()));
                tempPallets.Add(previousPallet);
                itemPallets.AddRange(rejectPallets);
                rejectPallets.Clear();

                testPallet = new PalletHldr();
                previousPallet = new PalletHldr();
                ++count;

                if (count > 1000)
                    break;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.Combine: {0}", "END Recombine in Location Order"));
            #endregion
        }

        private void SplitPalletByItem(PalletHldr pallet, List<PalletHldr> tempPallets, PalletBuildingMasterDetail pbmDetail, FilteredGroup<ShippingContainer> shippingContainerGroup)
        {
            // we will split into item pallets then try to recombine so much as possible
            List<PalletHldr> itemPallets = new List<PalletHldr>();
            string currentItem = "~~GOAT~~";
            PalletHldr currentPallet = null;

            #region Split by item
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: {0}", "START Split By Item"));

            pallet.SortByItem();
            foreach (ContainerHldr contHldr in pallet.Pllt)
            {

                if (contHldr.Item != currentItem)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: New Item: {0}", contHldr.Item));
                    currentItem = contHldr.Item;

                    if (currentPallet != null)
                        itemPallets.Add(currentPallet);

                    currentPallet = new PalletHldr();
                }

                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: {0}", contHldr.ToString()));
                currentPallet.AddContainer(contHldr);
            }

            if (currentPallet != null)
                itemPallets.Add(currentPallet);
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: {0}", "END Split By Item"));
            #endregion

            #region Recombine in Location Order
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: {0}", "START Recombine in Location Order"));
            /*
            itemPallets.Sort(PalletHldr.ComparePalletByLocation);
            testPallet = new PalletHldr();
            previousPallet = new PalletHldr();
            bool firstLoop = true;
            foreach (PalletHldr loopPallet in itemPallets)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: loopPallet: {0}", loopPallet.ToString()));
                testPallet.AddPallet(loopPallet);

                if (firstLoop)
                {
                    firstLoop = false;
                    previousPallet.AddPallet(loopPallet);
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: FIRST::loopPallet: {0}", loopPallet.ToString()));
                }
                else
                {
                    if (WillFit(pbmDetail, shippingContainerGroup, testPallet.ShippingContList))
                    {
                        previousPallet.AddPallet(loopPallet);
                        System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: FIT::loopPallet: {0}", loopPallet.ToString()));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: NOFIT::loopPallet: {0}", loopPallet.ToString()));

                        tempPallets.Add(previousPallet);
                        testPallet = new PalletHldr();
                        testPallet.AddPallet(loopPallet);
                        previousPallet = new PalletHldr();
                        previousPallet.AddPallet(loopPallet);
                    }
                }
            }

            if(testPallet.Pllt.Count != 0)
                tempPallets.Add(previousPallet);
            */

            Combine(itemPallets, tempPallets, pbmDetail, shippingContainerGroup);


            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.SplitPalletByItem: {0}", "END Recombine in Location Order"));
            #endregion
        }

        // we will use the base palletize method to build these loose pallets
        PalletizeByContainerTypeCapacity palletBuilder = null;

        private bool WillFit(PalletBuildingMasterDetail pbmDetail, FilteredGroup<ShippingContainer> shippingContainerGroup, List<ShippingContainer> testPallet)
        {
            List<ShippingContainer> containersNotPalletized = null;

            if (palletBuilder == null)
                palletBuilder = new PalletizeByContainerTypeCapacity();

            shippingContainerGroup = AddToFilterGroup(shippingContainerGroup, testPallet, true);
            List<List<ShippingContainer>> trial = palletBuilder.Execute(pbmDetail, shippingContainerGroup, out containersNotPalletized);
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.PalletBuilding.AnatoliaPalletBuild.WillFit: result: {0} ", trial.Count == 1));

            return trial.Count == 1;
        }

        #region HelperMethods

        private List<ShippingContainer> CopyContainerList(List<ShippingContainer> original)
        {
            List<ShippingContainer> copy = new List<ShippingContainer>();

            foreach (ShippingContainer cont in original)
            {
                copy.Add(cont);
            }

            return copy;
        }

        private List<ShippingContainer> AddToContainerList(List<ShippingContainer> master, List<ShippingContainer> child)
        {
            master.InsertRange(master.Count, child);
            return master;
        }

        private FilteredGroup<ShippingContainer> AddToFilterGroup(FilteredGroup<ShippingContainer> shippingContainerGroup, List<ShippingContainer> containers, bool clear)
        {
            if (clear)
                shippingContainerGroup.Items.Clear();

            foreach (ShippingContainer cont in containers)
            {
                shippingContainerGroup.Items.Add(cont);
            }

            return shippingContainerGroup;
        }

        #endregion

        #region JUNK

        private List<List<ShippingContainer>> BuildDimPalletsDontSplitItems(List<List<ShippingContainer>> contsByItem, PalletBuildingMasterDetail pbmDetail, FilteredGroup<ShippingContainer> shippingContainerGroup)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: {0}", "START"));
            // this list will hold all the pallets we create
            List<List<ShippingContainer>> goodDimensionalPallets = new List<List<ShippingContainer>>();
            List<List<ShippingContainer>> contsByItemClean = new List<List<ShippingContainer>>();
            List<List<ShippingContainer>> trial = new List<List<ShippingContainer>>();
            List<ShippingContainer> containersNotPalletized = null;

            // DEBUG
            foreach (List<ShippingContainer> pallet in contsByItem)
            {
                foreach (ShippingContainer cont in pallet)
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItems: cont.ContainerId: {0}", cont.ContainerId));
            }

            // we will use the base palletize method to build these loose pallets
            PalletizeByContainerTypeCapacity palletBuilder = new PalletizeByContainerTypeCapacity();

            #region Find Issue Items
            // find any item that the non pallet qty exceeds dimensional capacity
            for (int i = 0; i < contsByItem.Count; ++i)
            {
                shippingContainerGroup = AddToFilterGroup(shippingContainerGroup, contsByItem[i], true);

                trial = palletBuilder.Execute(pbmDetail, shippingContainerGroup, out containersNotPalletized);
                // if more than one pallet was created, or if that items containers failed palletizagion, add the item to the good pallet group
                if (trial.Count > 1 || (containersNotPalletized != null && containersNotPalletized.Count > 0))
                {
                    goodDimensionalPallets.Add(contsByItem[i]);
                }
                else
                {
                    contsByItemClean.Add(contsByItem[i]);
                }
            }

            foreach (List<ShippingContainer> pallet in contsByItemClean)
            {
                foreach (ShippingContainer cont in pallet)
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItems: cont.ContainerId: {0}", cont.ContainerId));
            }


            #endregion

            #region Create Loose Pallets
            List<ShippingContainer> curContGroup = new List<ShippingContainer>();
            List<ShippingContainer> previousContGroup = new List<ShippingContainer>();
            List<List<ShippingContainer>> rejectContGroup = new List<List<ShippingContainer>>();
            List<ShippingContainer> lastSingleItemGroup = new List<ShippingContainer>();

            while (contsByItemClean.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: contsByItemClean.Count(Outer): {0}", contsByItemClean.Count));
                // get the top of the list
                lastSingleItemGroup = contsByItemClean[0];
                curContGroup = AddToContainerList(curContGroup, lastSingleItemGroup);
                contsByItemClean.RemoveAt(0);

                previousContGroup = CopyContainerList(curContGroup);

                System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: curContGroup.Count(Outer): {0}, previousContGroup.Count(Outer): {1}", curContGroup.Count, previousContGroup.Count));

                while (contsByItemClean.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: contsByItemClean.Count(Inner): {0}", contsByItemClean.Count));

                    lastSingleItemGroup = contsByItemClean[0];
                    curContGroup = AddToContainerList(curContGroup, lastSingleItemGroup);
                    contsByItemClean.RemoveAt(0);

                    shippingContainerGroup = AddToFilterGroup(shippingContainerGroup, curContGroup, true);
                    trial = palletBuilder.Execute(pbmDetail, shippingContainerGroup, out containersNotPalletized);
                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: curContGroup.Count(Inner): {0}, previousContGroup.Count(Inner): {1}", curContGroup.Count, previousContGroup.Count));

                    System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: trial.Count: {0}", trial.Count));
                    if (trial.Count > 1)
                    {
                        rejectContGroup.Add(lastSingleItemGroup);
                        curContGroup = CopyContainerList(previousContGroup);
                    }
                    else
                    {
                        previousContGroup = CopyContainerList(curContGroup);
                    }

                }

                System.Diagnostics.Debug.WriteLine(string.Format("BHS.Outbound.UnitOfMeasure.BuildDimPalletsDontSplitItemsAccross: curContGroup.Count(EndOuter): {0}, previousContGroup.Count(EndOuter): {1}", curContGroup.Count, previousContGroup.Count));

                goodDimensionalPallets.Add(CopyContainerList(curContGroup));
                curContGroup.Clear();

                contsByItemClean.AddRange(rejectContGroup);
                rejectContGroup.Clear();
            }

            #endregion


            return goodDimensionalPallets;
        }

        #endregion


    }
}
