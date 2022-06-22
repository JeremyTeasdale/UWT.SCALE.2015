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
    class ContainerHldr
    {
        #region Properties

        private ShippingContainer _cont;
        public ShippingContainer Cont
        {
            get { return _cont; }
            set { _cont = value; }
        }

        private string _item = null;
        public string Item
        {
            get
            {
                if (_item == null)
                {
                    if (Cont.Item != null)
                        _item = Cont.Item;
                    else
                        _item = "MIXED";
                }

                return _item;
            }
        }

        private string _location = null;
        public string Location
        {
            get
            {
                if (_location == null)
                {
                    if (Cont.Location != null)
                        _location = Cont.Location;
                    else
                        _location = "MIXED";
                }
                return _location;
            }
        }

        private string _customerPO = null;
        public string CustomerPO
        {
            get
            {
                if (_customerPO == null)
                {
                    // go to the db
                    if (Cont.InternalShipmentLineNum != null && Cont.InternalShipmentLineNum.CustomerPo != null)
                        _customerPO = Cont.InternalShipmentLineNum.CustomerPo;
                    else
                        _customerPO = "MIXED";
                }
                return _customerPO;
            }
        }

        #endregion

        #region Constructors

        public ContainerHldr(ShippingContainer cont)
        {
            Cont = cont;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0} ({1}) - {2}", Cont.ContainerId, Location, CustomerPO);
        }

        public static int CompareContByItem(ContainerHldr p1, ContainerHldr p2)
        {
            return p1.Item.CompareTo(p2.Item);
        }

        public static int CompareContByLocation(ContainerHldr p1, ContainerHldr p2)
        {
            return p1.Location.CompareTo(p2.Location);
        }

        public static int CompareContByCustomerPO(ContainerHldr p1, ContainerHldr p2)
        {
            return p1.CustomerPO.CompareTo(p2.CustomerPO);
        }


        #endregion
    }
}
