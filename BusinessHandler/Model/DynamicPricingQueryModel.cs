using Newtonsoft.Json;
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
        public int limit { get; set; }
        public int offset { get; set; }

        public string MyLocation { get; set; }

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
        public List<StoreOpenDetailDisplay> OpenHours { get; set; }
    }

    public class DynamicPriceCategoryModel
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
    }

    public class StoreOpenDetailDisplay
    {
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public class StoreOpenDetail
    {
        [JsonProperty("Monday")]
        public StoreOpenHours Monday { get; set; }
        [JsonProperty("Tuesday")]
        public StoreOpenHours Tuesday { get; set; }
        [JsonProperty("Wednesday")]
        public StoreOpenHours Wednesday { get; set; }
        [JsonProperty("Thursday")]
        public StoreOpenHours Thursday { get; set; }
        [JsonProperty("Friday")]
        public StoreOpenHours Friday { get; set; }
        [JsonProperty("Saturday")]
        public StoreOpenHours Saturday { get; set; }
        [JsonProperty("Sunday")]
        public StoreOpenHours Sunday { get; set; }
    }
    public class StoreOpenHours
    {
        [JsonProperty("active")]
        public string Active { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }
        [JsonProperty("start")]
        public string Start { get; set; }
    }
}
