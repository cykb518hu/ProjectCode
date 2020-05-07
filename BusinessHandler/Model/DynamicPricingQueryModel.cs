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
}
