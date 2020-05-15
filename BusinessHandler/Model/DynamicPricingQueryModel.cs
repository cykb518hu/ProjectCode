using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.Model
{
    public class DynamicPricingQueryModel
    {
         
        public int limit { get; set; }
        public int offset { get; set; }
        public string sortName { get; set; }
        public string sortOrder { get; set; }

        public string StoreIds { get; set; }

        public string CategoryIds { get; set; }

        public string Brand { get; set; }

        public string City { get; set; }

        public string ProductName { get; set; }
    }

    public class DynamicPricingTableModel
    {
        public string ProductId { get; set; }

        public string ProductName { get; set; }
        public string CategoryName { get; set; }

        public string Brand { get; set; }

        public string StoreName { get; set; }

        public string City { get; set; }

        public string ScrapeDate { get; set; }
        public string StrainType { get; set; }
        public string THCPercentage { get; set; }
        public string CBDPercentage { get; set; }
        public string IsSpecial { get; set; }

        public List<DynamicPricingTableSubDataModel> SubList { get; set; }
    }

    public class DynamicPricingTableSubDataModel
    {
        public string ProductId { get; set; }

        public string Unit { get; set; }
        public string Qty { get; set; }

        public string QtyAvailable { get; set; }

        public string MedicalPrice { get; set; }

        public string RecreationalPrice { get; set; }
    }

    public class DynamicPricingStoreModel
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
    }

    public class DynamicPricingStoreColorModel
    {
        public string StoreId { get; set; }
        public string City { get; set; }
        public string Color { get; set; }
    }

    public class DynamicPricingMapStoreQueryModel
    {

        public string MyLocation { get; set; }

        public string CategoryName { get; set; }

        public string City { get; set; }

    }

    public class DynamicPricingStoreDetailModel
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DeliveryHours { get; set; }
        public string DeliveryFeesUSD { get; set; }
        public string MaxDeliveryDistance { get; set; }
        public string MinDeliveryOrder { get; set; }
        public string MaxDeliveryOrder { get; set; }
        public string MedicalOnly { get; set; }
        public string OfferDelivery { get; set; }
        public string Color { get; set; }
    }

    public class DynamicPriceCategoryModel
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
    }
}
