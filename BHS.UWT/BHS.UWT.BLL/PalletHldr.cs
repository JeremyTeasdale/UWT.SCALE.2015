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
    class PalletHldr
    {
        #region Properties

        private List<ContainerHldr> _pllt;
        public List<ContainerHldr> Pllt
        {
            get
            {
                if (_pllt == null)
                    _pllt = new List<ContainerHldr>();
                return _pllt;
            }
        }

        private string _item;
        public string Item
        {

            get
            {
                return _item;
            }
            set
            {
                if (_item == null || _item.CompareTo(value) > 0)
                    _item = value;
            }
        }
        private string _location;
        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location == null || _location.CompareTo(value) > 0)
                    _location = value;
            }
        }

        private string _customerPO;
        public string CustomerPO
        {
            get
            {
                return _customerPO;
            }
            set
            {
                if (_customerPO == null || _customerPO.CompareTo(value) > 0)
                    _customerPO = value;
            }
        }

        public List<ShippingContainer> ShippingContList
        {
            get
            {
                List<ShippingContainer> list = new List<ShippingContainer>();

                foreach (ContainerHldr cont in Pllt)
                {
                    list.Add(cont.Cont);
                }
                return list;
            }
        }



        #endregion

        #region Constructor

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("Pallet: {0} ({1}) - {2} Count: {3}", this.Item, Location, CustomerPO, Pllt.Count);
        }

        public void AddContainer(ContainerHldr contHldr)
        {
            Pllt.Add(contHldr);

            this.Item = contHldr.Item;
            Location = contHldr.Location;
            CustomerPO = contHldr.CustomerPO;
        }

        public void AddPallet(PalletHldr pallet)
        {
            Pllt.AddRange(pallet.Pllt);

            this.Item = pallet.Item;
            Location = pallet.Location;
            CustomerPO = pallet.CustomerPO;
        }

        public void Clear()
        {
            Pllt.Clear();
            this._item = null;
            this._customerPO = null;
            this._location = null;
        }

        public static int ComparePalletByItem(PalletHldr p1, PalletHldr p2)
        {
            return p1.Item.CompareTo(p2.Item);
        }

        public void SortByItem()
        {
            Pllt.Sort(ContainerHldr.CompareContByItem);
        }

        public static int ComparePalletByLocation(PalletHldr p1, PalletHldr p2)
        {
            return p1.Location.CompareTo(p2.Location);
        }

        public void SortByLocation()
        {
            Pllt.Sort(ContainerHldr.CompareContByLocation);
        }

        public static int ComparePalletByCustomerPO(PalletHldr p1, PalletHldr p2)
        {
            return p1.CustomerPO.CompareTo(p2.CustomerPO);
        }

        public void SortByCustomerPO()
        {
            Pllt.Sort(ContainerHldr.CompareContByCustomerPO);
        }

        #endregion

    }
}
