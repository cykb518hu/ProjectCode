using BusinessHandler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace BusinessHandler.MessageHandler
{
    public interface IDynamicPriceRepository
    {
        List<DynamicPricingTableModel> GetDataList(DynamicPricingQueryModel message, out int total);
        List<DynamicPricingStoreModel> GetStoreList();
        List<DynamicPricingStoreDetailModel> GetStoreDetailList(DynamicPricingMapStoreQueryModel message);
        List<DynamicPricingStoreColorModel> GetStoreIdWithColorList(DynamicPricingMapStoreQueryModel message);
        DynamicPricingStoreDetailModel GetStoreDetail(string storeId);
        List<DynamicPriceCategoryModel> GetCategoryList();
    }

    public class DynamicPriceRepository:IDynamicPriceRepository
    {
        public List<DynamicPricingTableModel>  GetDataList(DynamicPricingQueryModel message, out int total)
        {
            var list = new List<DynamicPricingTableModel>();
            string queryString = @"[dbo].[GET_DATA_List]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.dynamicPriceDBconnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;

                StaticSetting.BuildParameters(command, message);

                connection.Open();

                total = Convert.ToInt32(command.ExecuteScalar());

                var orderBy = "";
                switch (message.sortName)
                {
                    case "ProductName":
                        orderBy = "ProductName";
                        break;

                    case "CategoryName":
                        orderBy = "CategoryName";
                        break;
                    case "StoreName":
                        orderBy = "StoreName";
                        break;
                    case "ScrapeDate":
                        orderBy = "DATE";
                        break;
                    case "City":
                        orderBy = "City";
                        break;
                    case "IsSpecial":
                        orderBy = "IsSpecial";
                        break;
                    default:
                        orderBy = "StoreName asc, ProductName";
                        break;
                }
                message.sortOrder = string.IsNullOrWhiteSpace(message.sortOrder) ? "asc" : message.sortOrder;
                orderBy = orderBy + " " + message.sortOrder;

                command.Parameters.AddWithValue("@offset", message.offset);
                command.Parameters.AddWithValue("@limit", message.limit);
                command.Parameters.AddWithValue("@Total", 0);
                command.Parameters.AddWithValue("@OrderByField", orderBy);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new DynamicPricingTableModel();
                        result.ProductId = reader["ProductId"]?.ToString();
                        result.ProductName = reader["ProductName"]?.ToString();
                        result.CategoryName = reader["CategoryName"]?.ToString();
                        result.Brand = reader["Brand"]?.ToString();
                        result.StoreName = reader["StoreName"]?.ToString();
                        result.City= reader["City"]?.ToString();
                        result.StrainType = reader["StrainType"]?.ToString();
                        result.THCPercentage = reader["THCPercentage"]?.ToString();
                        result.CBDPercentage = reader["CBDPercentage"]?.ToString();
                        result.IsSpecial = reader["IsSpecial"]?.ToString();
                        result.ScrapeDate = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd");
                        list.Add(result);

                    }
                }
            }
            GetSubList(list);
            return list;
        }

        public void GetSubList(List<DynamicPricingTableModel> list)
        {
            var idList = string.Empty;
            foreach (var r in list)
            {
                idList += "'" + r.ProductId + "',";
            }
            idList = idList.TrimEnd(',');
            if (string.IsNullOrWhiteSpace(idList))
            {
                return;
            }
            string queryString = @"[dbo].[GET_DATA_SUB_List]";
            var resultList = new List<DynamicPricingTableSubDataModel>();
            using (SqlConnection connection = new SqlConnection(StaticSetting.dynamicPriceDBconnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductIdList", idList);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new DynamicPricingTableSubDataModel();
                        result.ProductId = reader["ProductId"]?.ToString();
                        result.Unit = reader["Unit"]?.ToString();
                        result.Qty = reader["Qty"]?.ToString();
                        result.QtyAvailable = reader["QtyAvailable"]?.ToString();
                        result.MedicalPrice = reader["MedicalPrice"]?.ToString();
                        result.RecreationalPrice = reader["RecreationalPrice"]?.ToString();
                        resultList.Add(result);
                    }
                }
            }
            foreach (var r in list)
            {
                var subList = resultList.Where(x => x.ProductId == r.ProductId).ToList();
                r.SubList = subList;
            }
        }

        public List<DynamicPricingStoreModel> GetStoreList()
        {
            var list = new List<DynamicPricingStoreModel>();
            DynamicPricingMapStoreQueryModel message = new DynamicPricingMapStoreQueryModel();
            var storeList = GetStoreDetailList(message);
            foreach (var r in storeList)
            {
                var result = new DynamicPricingStoreModel();
                result.StoreId = r.StoreId;
                result.StoreName = r.StoreName;
                list.Add(result);
            }
            return list;
        }
        public List<DynamicPricingStoreDetailModel> GetStoreDetailList(DynamicPricingMapStoreQueryModel message)
        {
            var list = new List<DynamicPricingStoreDetailModel>();
            var key = StaticSetting.storeListKey + message.MyLocation + message.City;
            if (HttpRuntime.Cache.Get(key) != null)
            {
                list = (List<DynamicPricingStoreDetailModel>)HttpRuntime.Cache.Get(key);
                return list;
            }
            string queryString = @"[dbo].[GET_STORE_List]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.dynamicPriceDBconnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;

                StaticSetting.BuildParameters(command, message);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new DynamicPricingStoreDetailModel();
                        result.StoreId = reader["StoreId"]?.ToString();
                        result.StoreName = reader["StoreName"]?.ToString();
                        result.City = reader["City"]?.ToString();
                        result.Address = reader["Address"]?.ToString();
                        result.OfferDelivery = reader["OfferDelivery"]?.ToString();
                        result.MedicalOnly = reader["MedicalOnly"]?.ToString();
                        result.MinDeliveryOrder = reader["MinDeliveryOrder"]?.ToString();
                        result.MaxDeliveryOrder = reader["MaxDeliveryOrder"]?.ToString();
                        result.DeliveryFeesUSD = reader["DeliveryFeesUSD"]?.ToString();
                        result.MaxDeliveryDistance = reader["MaxDeliveryDistance"]?.ToString();
                      //  var str = reader["DeliveryHours"]?.ToString();
                        var openHour = JsonConvert.DeserializeObject<StoreOpenDetail>(reader["DeliveryHours"]?.ToString());
                        var openHourDisplay = new StoreOpenDetailDisplay();
                        if(!string.IsNullOrWhiteSpace(openHour.Monday.Active)&& openHour.Monday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Monday = openHour.Monday.Start + " - " + openHour.Monday.End;
                        }
                        else
                        {
                            openHourDisplay.Monday = "Not active";

                        }

                        if (!string.IsNullOrWhiteSpace(openHour.Tuesday.Active) && openHour.Tuesday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Tuesday = openHour.Tuesday.Start + " - " + openHour.Tuesday.End;
                        }
                        else
                        {
                            openHourDisplay.Tuesday = "Not active";

                        }
                        if (!string.IsNullOrWhiteSpace(openHour.Wednesday.Active) && openHour.Wednesday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Wednesday = openHour.Wednesday.Start + " - " + openHour.Wednesday.End;
                        }
                        else
                        {
                            openHourDisplay.Wednesday = "Not active";

                        }
                        if (!string.IsNullOrWhiteSpace(openHour.Thursday.Active) && openHour.Thursday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Thursday = openHour.Thursday.Start + " - " + openHour.Thursday.End;
                        }
                        else
                        {
                            openHourDisplay.Thursday = "Not active";

                        }
                        if (!string.IsNullOrWhiteSpace(openHour.Friday.Active) && openHour.Friday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Friday = openHour.Friday.Start + " - " + openHour.Friday.End;
                        }
                        else
                        {
                            openHourDisplay.Friday = "Not active";

                        }
                        if (!string.IsNullOrWhiteSpace(openHour.Saturday.Active) && openHour.Saturday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Saturday = openHour.Saturday.Start + " - " + openHour.Saturday.End;
                        }
                        else
                        {
                            openHourDisplay.Saturday = "Not active";

                        }
                        if (!string.IsNullOrWhiteSpace(openHour.Sunday.Active) && openHour.Sunday.Active.ToLower().Equals("true"))
                        {
                            openHourDisplay.Sunday = openHour.Sunday.Start + " - " + openHour.Sunday.End;
                        }
                        else
                        {
                            openHourDisplay.Sunday = "Not active";

                        }
                        result.OpenHours = new List<StoreOpenDetailDisplay>();
                        result.OpenHours.Add(openHourDisplay);
                        
                        result.Color = "black";
                        list.Add(result);
                    }
                }
            }
            CalculateStoreColor(list);
            if (message.MyLocation == "Yes")
            {
                list = list.Where(x => x.Color == "yellow").ToList();
            }
            if (message.MyLocation == "No")
            {
                list = list.Where(x => x.Color == "black").ToList();
            }
            HttpRuntime.Cache.Insert(key, list);
            return list;
        }
        public List<DynamicPricingStoreColorModel> GetStoreIdWithColorList(DynamicPricingMapStoreQueryModel message)
        {
            var list = new List<DynamicPricingStoreColorModel>();
            var storeList = GetStoreDetailList(message);
            foreach(var r in storeList)
            {
                var result = new DynamicPricingStoreColorModel();
                result.StoreId = r.StoreId;
                result.Color = r.Color;
                result.City = r.City;
                list.Add(result);
            }
            return list;
        }

        public void CalculateStoreColor(List<DynamicPricingStoreDetailModel> list)
        {

            string queryString = @"select * from MyLocation";
            var resultList =new List<string>();
            using (SqlConnection connection = new SqlConnection(StaticSetting.dynamicPriceDBconnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultList.Add(reader["City"]?.ToString());
                    }
                }
            }
            foreach (var r in list)
            {
                if (resultList.Any(x => x.ToLower().Equals(r.City.ToLower())))
                {
                    r.Color = "yellow";
                }
            }
            
        }


        public DynamicPricingStoreDetailModel GetStoreDetail(string storeId)
        {
            var result = new DynamicPricingStoreDetailModel();
            string queryString = @"select SF.StoreId,SF.StoreName, ST.City,ST.Address,ST.Phone,ST.Email from StoreFront SF LEFT JOIN Store ST
ON SF.StoreId=ST.StoreId 
where SF.StoreId=@StoreId";
            using (SqlConnection connection = new SqlConnection(StaticSetting.dynamicPriceDBconnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@StoreId", storeId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.StoreId = reader["StoreId"]?.ToString();
                        result.StoreName = reader["StoreName"]?.ToString();
                        result.City= reader["City"]?.ToString();
                        result.Address = reader["Address"]?.ToString();
                        result.Phone = reader["Phone"]?.ToString();
                        result.Email = reader["Email"]?.ToString();
                    }
                }
            }
            return result;
        }

        public List<DynamicPriceCategoryModel> GetCategoryList()
        {
            var list = new List<DynamicPriceCategoryModel>();
            string queryString = @"SELECT DISTINCT  CategoryId, CategoryName FROM [dbo].[CategoryRepository] WHERE CategoryName <>''";
            using (SqlConnection connection = new SqlConnection(StaticSetting.dynamicPriceDBconnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new DynamicPriceCategoryModel();
                        result.CategoryId = reader["CategoryId"]?.ToString();
                        result.CategoryName = reader["CategoryName"]?.ToString();
                        list.Add(result);
                    }
                }
            }
            return list;
        }


    }
}
