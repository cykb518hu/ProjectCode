using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessHandler.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessHandler.Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using RestSharp;
using System.Net;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using QRCoder;
using System.Drawing;

namespace BusinessHandler.MessageHandler.Tests
{
    [TestClass()]
    public class UserRepositoryTests
    {
        [TestMethod()]
        public void RegisterTest()
        {
            UserRepository user = new UserRepository();
            UserAccount message = new UserAccount();
            message.Email = "Achilles@11.com";
            message.Password = "dddd";
            message.Cityes = "chengdu,chongqing";
            message.Active = "No";
            user.Register(message);

        }

        [TestMethod()]
        public void QueryXmlTest()
        {
            var filePath = @"C:\TestCode\Document\XmlFile\Midland_Queries.xml";
            XmlHelper helper = new XmlHelper();
            var list = helper.OpenQuery(filePath);

        }
        [TestMethod()]
        public void Testt()
        {
            PaymentResult refund = null;
            try
            {
                refund = CreateExcption();
            }
            catch { }
            if (refund != null)
            {
                var a = 1;
            }
        }

        public PaymentResult CreateExcption()
        {
            var result = new PaymentResult();
            int a = Int32.Parse("11d");

            return result;
        }
        [TestMethod()]
        public void InsertDoc()
        {
            var filePath = @"C:\TestCode\Document\XmlFile";
            XmlHelper xml = new XmlHelper();
            var DocUrlList = Directory.GetFiles(filePath, "*Docs*");

            foreach (var r in DocUrlList)
            {
                var resultList = new List<DocData>();
                resultList = xml.OpenDoc(r);
                string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ToString();
                foreach (var rr in resultList)
                {
                    string queryString = @"UPDATE DOCUMENT SET IMPORTANT='" + rr.Important + "' WHERE DOC_GUID='" + rr.DocId + "'";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        SqlCommand command = new SqlCommand(queryString, connection);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    //string queryString = @"insert into DOCUMENT(DOC_GUID,CITY_NM,DOC_TYPE, DOC_SOURCE,DOC_PATH,CHECKED,IMPORTANT,READABLE) values ('" + rr.DocId + "','" + rr.CityName + "','" + rr.DocType + "','" + rr.DocUrl + "','" + rr.DocFilePath + "','" + rr.IsViewed + "','" + rr.Important + "','" + rr.CanBeRead + "')";
                    //using (SqlConnection connection = new SqlConnection(connectionString))
                    //{

                    //    SqlCommand command = new SqlCommand(queryString, connection);

                    //    connection.Open();
                    //    command.ExecuteNonQuery();
                    //}
                }
            }
        }
        [TestMethod()]
        public void InsertQuery()
        {
            var filePath = @"C:\TestCode\Document\XmlFile";
            XmlHelper xml = new XmlHelper();
            var queriesUrlList = Directory.GetFiles(filePath, "*Queries*");

            foreach (var r in queriesUrlList)
            {
                var xmlDataList = new List<QueryXmlData>();

                XDocument xdoc = XDocument.Load(r);
                xmlDataList = (from lv1 in xdoc.Descendants("Query")
                               select new QueryXmlData
                               {
                                   CityName = lv1.Element("CityId").Value,
                                   DocId = lv1.Element("DocId").Value,

                                   MeetingDate = DateTime.Parse(lv1.Element("MeetingDate").Value),
                                   MeetingDateDisplay = DateTime.Parse(lv1.Element("MeetingDate").Value).ToString("yyyy-MM-dd"),
                                   SearchDate = DateTime.Parse(lv1.Element("SearchDate").Value).ToString("yyyy-MM-dd"),
                                   MeetingTitle = lv1.Element("MeetingTitle").Value,
                                   MeetingLocation = lv1.Element("MeetingLocation").Value,
                                   Entries = (from lv2 in lv1.Element("Entries").Elements("Entry")
                                              select new QueryEntryXmlData
                                              {

                                                  KeyWord = lv2.Element("Keyword").Value,
                                                  PageNumber = lv2.Element("PageNumber").Value,
                                                  ContentList = (from lv3 in lv2.Element("Contents").Elements("Content")
                                                                 select new QueryContentXmlData
                                                                 {
                                                                     Content = lv3.Value,
                                                                     QueryGuid = lv3.Attribute("GUID").Value,
                                                                     Comment = lv3.Attribute("Comment").Value
                                                                 }
                                                               ).ToList()
                                              }

                                   ).ToList()
                               }).ToList();

                string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ToString();
                foreach (var q in xmlDataList)
                {
                    string queryString = @"insert into QUERY(QUERY_GUID,DOC_GUID,MEETING_DATE,MEETING_LOCATION, MEETING_TITLE,SEARCH_DATE) values ('" + Guid.NewGuid().ToString() + "','" + q.DocId + "','" + q.MeetingDate + "','" + q.MeetingLocation + "','" + q.MeetingTitle + "','" + q.SearchDate + "')";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Create the Command and Parameter objects.
                        SqlCommand command = new SqlCommand(queryString, connection);
                        // Open the connection in a try/catch block. 
                        // Create and execute the DataReader, writing the result
                        // set to the console window.
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    var queryGuid = string.Empty;
                    string queryString2 = @"select top 1 QUERY_GUID from QUERY order by USR_CRTN_TS desc ";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Create the Command and Parameter objects.
                        SqlCommand command = new SqlCommand(queryString2, connection);
                        // Open the connection in a try/catch block. 
                        // Create and execute the DataReader, writing the result
                        // set to the console window.
                        connection.Open();
                        queryGuid = command.ExecuteScalar().ToString();
                    }
                    foreach (var d in q.Entries)
                    {
                        foreach (var rr in d.ContentList)
                        {
                            string queryString3 = @"insert into QUERY_ENTRY(ENTRY_GUID, QUERY_GUID,PAGE_NUMBER,KEYWORD,CONTENT, COMMENT) values (@ENTRY_GUID,@QUERY_GUID,@PAGE_NUMBER,@KEYWORD,@CONTENT,@COMMENT)";
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                // Create the Command and Parameter objects.
                                SqlCommand command = new SqlCommand(queryString3, connection);
                                command.Parameters.AddWithValue("@ENTRY_GUID", rr.QueryGuid);
                                command.Parameters.AddWithValue("@QUERY_GUID", queryGuid);
                                command.Parameters.AddWithValue("@PAGE_NUMBER", d.PageNumber);
                                command.Parameters.AddWithValue("@KEYWORD", d.KeyWord);
                                command.Parameters.AddWithValue("@CONTENT", rr.Content);
                                command.Parameters.AddWithValue("@COMMENT", rr.Comment);
                                // Open the connection in a try/catch block. 
                                // Create and execute the DataReader, writing the result
                                // set to the console window.
                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }



        }

        [TestMethod()]
        public void InsertCity()
        {

            var path = @"C:\TestCode\Document\Map\mi_mun_data.xlsx";
            List<MuniciplityCounty> list = new List<MuniciplityCounty>();
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Sheets sheets;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            object oMissiong = System.Reflection.Missing.Value;
            workbook = xlApp.Workbooks.Open(path, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
           oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
            sheets = workbook.Worksheets;

            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Worksheet)sheets.get_Item(1);//读取第一张表  

            int iRowCount = worksheet.UsedRange.Rows.Count;
            int iColCount = worksheet.UsedRange.Columns.Count;
            Range range;
            var str = "";
            for (int iRow = 2; iRow <= 1300; iRow++)
            {
                var o1 = ((Range)worksheet.Cells[iRow, 3]).Text.ToString();
                str += "'" + o1 + "',";
                //var o2 = ((Range)worksheet.Cells[iRow, 2]).Text.ToString();

            }
            var r = 1;
            return;
            string connectionString = ConfigurationManager.ConnectionStrings["TargetDB"].ToString();

            string queryString = @"select * from city";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                connection.Open();
                var reader = command.ExecuteReader();
                if(reader.Read())
                {
                   // var data=
                }

            }

            string connectionString1 = ConfigurationManager.ConnectionStrings["TargetDB"].ToString();
        }

        public CardType Get()
        {
            var roomrates = new List<CardType>();
            return roomrates.FirstOrDefault();
        }
        [TestMethod()]
        public void GetData()
        {
            var host = "https://otp.tools.investis.com/clients/uk/millenium_copthorne/rns/xml-feed.aspx?culture=en-GB";
            RestClient client = new RestClient(host);
            var req = new RestRequest(Method.GET);
            req.AddHeader("Content-Type", "application/xml;charset=UTF-8");
            //req.AddParameter("feed", "regulatory-news");
            string xmlRes = string.Empty;
            try
            {
                var result = client.Execute(req);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    xmlRes = result.Content;
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xmlRes.StartsWith(_byteOrderMarkUtf8))
            {
                xmlRes = xmlRes.Remove(0, _byteOrderMarkUtf8.Length);
            }
            var xDoc = XDocument.Parse(xmlRes);
            // var file = @"C:\Document\testxml.xml";
            // var xDoc = XDocument.Load(file);
        }
        [TestMethod()]
        public void TestPage()
        {
            var blogsList = new List<PaymentResult>();
            blogsList.Add(new PaymentResult { PaymentId = "1", ThisUrl = "11" });
            blogsList.Add(new PaymentResult { PaymentId = "2", ThisUrl = "222" });
            blogsList.Add(new PaymentResult { PaymentId = "3", ThisUrl = "333" });

            blogsList.RemoveAll(x => "1,2".Contains(x.PaymentId));
            return; 
            var paymentId = "2";
            var currentIndex = blogsList.FindIndex(x => x.PaymentId == paymentId);
            var prevUrl = string.Empty;
            var nextUrl = string.Empty;
            if (blogsList.Count > 1)
            {
                if (currentIndex == 0)
                {
                    nextUrl = blogsList[currentIndex + 1].ThisUrl;
                }
                else if (currentIndex == blogsList.Count - 1)
                {
                    prevUrl = blogsList[currentIndex - 1].ThisUrl;
                }
                else
                {
                    prevUrl = blogsList[currentIndex - 1].ThisUrl;
                    nextUrl = blogsList[currentIndex + 1].ThisUrl;
                }
            }
        }
        [TestMethod()]
        public void TestMapColor()
        {
            var list = new List<DocQueryParentModel>();
            list.Add(new DocQueryParentModel { LongName = "City of Portage", Number = 534 });
            list.Add(new DocQueryParentModel { LongName = "City of Eastpointe", Number = 61 });
            list.Add(new DocQueryParentModel { LongName = "City of Sterling Heights", Number = 281 });
            list.Add(new DocQueryParentModel { LongName = "Redford Township", Number = 12 });
            list.Add(new DocQueryParentModel { LongName = "City of Ann Arbor", Number = 38 });
            list = list.OrderByDescending(x => x.Number).ToList();

            var result = new List<MapMunicipalityColor>();

            var colorList = new List<string>();
            colorList.Add("#00FF7F");
            colorList.Add("#00EE76");
            colorList.Add("#00CD66");
            colorList.Add("#008B45");

            int max = list.First().Number;
            int min = list.Last().Number;

            int level = (max - min) / 3;

            foreach (var l in list)
            {
                var data = new MapMunicipalityColor();
                data.MunicipalityName = l.LongName;
                var index = l.Number / level;
                if (index >= colorList.Count)
                {
                    index = colorList.Count - 1;
                }
                data.Color = "#mi_mun[label='" + data.MunicipalityName + "']{polygon-fill: " + colorList[index] + "; line-color: white; }";
                result.Add(data);
            }
        }

        [TestMethod()]
        public void UpdateCityData()
        {

            var path = @"C:\TestCode\Document\Map\mi_munv5.xlsx";
            List<MuniciplityCounty> list = new List<MuniciplityCounty>();
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Sheets sheets;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            object oMissiong = System.Reflection.Missing.Value;
            workbook = xlApp.Workbooks.Open(path, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
           oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
            sheets = workbook.Worksheets;

            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Worksheet)sheets.get_Item(1);//读取第一张表  

            int iRowCount = worksheet.UsedRange.Rows.Count;
            int iColCount = worksheet.UsedRange.Columns.Count;
            for (int iRow = 2; iRow <= iRowCount; iRow++)
            {
                var data = new MuniciplityCounty();
                data.LongNm = ((Range)worksheet.Cells[iRow, 4]).Text.ToString();
                data.County = ((Range)worksheet.Cells[iRow,5]).Text.ToString();
                data.objectId = ((Range)worksheet.Cells[iRow, 1]).Text.ToString();
                list.Add(data);
            }
            List<MuniciplityCounty> dblist = new List<MuniciplityCounty>();
            string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ToString();

            string queryString = @"select * from city";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MuniciplityCounty();
                    data.LongNm = reader["LONG_NM"].ToString();
                    data.County = reader["COUNTY_NM"].ToString();
                    dblist.Add(data);
                }

            }
            List<MuniciplityCounty> duplicateList = new List<MuniciplityCounty>();
            foreach(var r in dblist)
            {
                var subList = list.Where(x => x.LongNm == r.LongNm).ToList();
                if (subList.Any() && subList.Count == 1)
                {
                    var updateStr = "update dbo.CITY  set objectid=" + subList[0].objectId + " where LONG_NM='" + r.LongNm + "' ";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Create the Command and Parameter objects.
                        SqlCommand command = new SqlCommand(updateStr, connection);
                        // Open the connection in a try/catch block. 
                        // Create and execute the DataReader, writing the result
                        // set to the console window.
                        connection.Open();
                        command.ExecuteNonQuery();

                    }
                    //foreach(var s in subList)
                    //{
                    //    s.ShortNm = r.County;
                    //}
                    //duplicateList.AddRange(subList);
                }
            }
            //CreateNewExcel(duplicateList);

        }

        public void CreateNewExcel(List<MuniciplityCounty> duplicateList)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = true;
            Workbook wb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            ws.Cells[1, 2] = "Long Name";
            ws.Cells[1, 3] = "County";
            ws.Cells[1, 4] = "id";

            ws.Cells[1, 5] = "Table County";
            int row = 2;
            foreach (var r in duplicateList)
            {
                ws.Cells[row, 2] = r.LongNm;
                ws.Cells[row, 3] = r.County;
                ws.Cells[row, 4] = r.objectId;

                ws.Cells[row, 5] = r.ShortNm;
                row++;
            }
            var file = @"C:\TestCode\Document\File\";
            file += DateTime.Now.ToString("yyyy-MM-dd") + Guid.NewGuid().ToString() + ".xlsx";
            wb.SaveAs(file, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
        false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        [TestMethod()]
        public void MigrateData()
        {
            List<MuniciplityCounty> dblist = new List<MuniciplityCounty>();
            string connectionString = ConfigurationManager.ConnectionStrings["TargetDB"].ToString();

            string queryString = @"select * from city";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MuniciplityCounty();
                    data.LongNm = reader["LONG_NM"].ToString();
                    data.County = reader["COUNTY_NM"].ToString();
                    data.ShortNm = reader["SHORT_NM"].ToString();
                    data.Typ = reader["TYP"].ToString();
                    data.Municiplity = reader["CITY_NM"].ToString();
                    data.objectId = reader["OBJECTID"].ToString();
                    dblist.Add(data);
                }
            }

            string targetConnectionString = ConfigurationManager.ConnectionStrings["TargetDB"].ToString();
            int i = 0;
            foreach (var r in dblist)
            {
                string updateStr = @"update dbo.CITY set guid='" + Guid.NewGuid().ToString() + "' where city_nm='" + r.Municiplity + "'";
                using (SqlConnection connection = new SqlConnection(targetConnectionString))
                {
                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(updateStr, connection);
                    // Open the connection in a try/catch block. 
                    // Create and execute the DataReader, writing the result
                    // set to the console window.
                    connection.Open();
                    command.ExecuteNonQuery();

                }
            }

            //string targetConnectionString = ConfigurationManager.ConnectionStrings["TargetDB"].ToString();
            //int i = 0;
            //foreach(var r in dblist)
            //{
            //    string updateStr = @"update dbo.CITY set SHORT_NM='" + r.ShortNm + "' , COUNTY_NM='" + r.County + "', TYP='" + r.Typ + "', long_nm='" + r.LongNm + "',states='MI', objectid=" + r.objectId + " where city_nm='" + r.Municiplity + "'";
            //    using (SqlConnection connection = new SqlConnection(targetConnectionString))
            //    {
            //        // Create the Command and Parameter objects.
            //        SqlCommand command = new SqlCommand(updateStr, connection);
            //        // Open the connection in a try/catch block. 
            //        // Create and execute the DataReader, writing the result
            //        // set to the console window.
            //        connection.Open();
            //        command.ExecuteNonQuery();
                 
            //    }
            //}

        }

        [TestMethod()]
        public void TestDateTime()
        {
            var str = "sdlfjldjf future date(2017-01-03)";
            var startIndex = str.IndexOf('(');
            var endIndex= str.IndexOf(')');

            var strDate = str.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        [TestMethod()]
        public void TestGoogleData()
        {
            List<HomeCity> cityList = new List<HomeCity>();

            cityList.Add(new HomeCity {
                CityName = "London",
                CityDescirption = "London test",
                ImageUrl = "http://media.mhb.com/mhb-media/9/6/C/96CB9C49-0EF4-418E-9A7D-E7F71D9FAA08/tpl-half-scratchpad-without.jpg?w=1000&hash=D08B00E543594E4F82B624ACC4A0F7F8",
                BookingUrl= "https://www.millenniumhotels.com/en/bookings/?city=london",
                CityUrl= "https://www.millenniumhotels.com/en/london/",
                ImageAnchor="center center"

            });
            cityList.Add(new HomeCity
            {
                CityName = "Beijing",
                CityDescirption = "Beijing test",
                ImageUrl = "http://media.mhb.com/mhb-media/9/6/C/96CB9C49-0EF4-418E-9A7D-E7F71D9FAA08/tpl-half-scratchpad-without.jpg?w=1000&hash=D08B00E543594E4F82B624ACC4A0F7F8",
                BookingUrl = "https://www.millenniumhotels.com/en/bookings/?city=london",
                CityUrl = "https://www.millenniumhotels.com/en/london/",
                ImageAnchor = "top left"

            });
            cityList.Add(new HomeCity
            {
                CityName = "Chengdu",
                CityDescirption = "Chengdu test",
                ImageUrl = "http://media.mhb.com/mhb-media/9/6/C/96CB9C49-0EF4-418E-9A7D-E7F71D9FAA08/tpl-half-scratchpad-without.jpg?w=1000&hash=D08B00E543594E4F82B624ACC4A0F7F8",
                BookingUrl = "https://www.millenniumhotels.com/en/bookings/?city=london",
                CityUrl = "https://www.millenniumhotels.com/en/london/"

            });
            var data = new HomeCitySection();
            data.HomeCityList = cityList;
            data.AllHotelLink = "https://www.millenniumhotels.com/en/hotel/";
            var str = JsonConvert.SerializeObject(data);
        }

        [TestMethod()]
        public void QRTest()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("http://59.110.217.147:8005/test.html?promoCode=Achilles", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(@"C:\Document\Pen\test.png");
        }
    }

    public class PaymentResult
    {
            public string ThisUrl { get; set; }
            public DateTime DateTime_ { get { return DateTime.Now; } }
        /// <summary>
        /// true: payment failed
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// the payment error message
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// the payment error coce
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// the last event id (the payment status) of the result
        /// </summary>
        public string LastEvent { get; set; }

        /// <summary>
        /// the payment id
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// the GC API response
        /// </summary>
        public string Content { get; set; }
    }

    public class CardType
    {
        public string Name { get; private set; }

        private readonly Regex _reg;

        private CardType(string name, string regex)
        {
            Name = name;
            _reg = new Regex(regex);
        }

        public bool Validate(string cardNumber)
        {
            return !string.IsNullOrEmpty(cardNumber) && cardNumber.LuhnCheck() && _reg.IsMatch(cardNumber);
        }

        public static readonly CardType Visa = new CardType("VISA", @"^4[\d]{12}([\d]{3})?$");

        public static readonly CardType MasterCard = new CardType("MasterCard", @"^5([1-5])[\d]{14}$");

        public static readonly CardType Maestro = new CardType("Maestro", @"^5(0|6|7|8|9)[\d]{9,17}$");

        public static readonly CardType AMEX = new CardType("AMEX", @"^3(4|7)[\d]{13}$");

        public static readonly CardType DCI = new CardType("Diners Club International", @"^3(0(0|5|9)[\d]{11}|6[\d]{12})$");

        public static readonly CardType DCUSC = new CardType("Diners Club US&Canada", @"^3(8|9)[\d]{14}$");

        public static readonly CardType Discover = new CardType("Discover", @"^6((0|4|5)[\d]{14}|22[\d]{13})$");

        public static readonly CardType JCB = new CardType("JCB", @"^35[\d]{14}$");
    }

    public static class CreditCardCheck
    {
        public static bool LuhnCheck(this string cardNumber)
        {
            return LuhnCheck(cardNumber.Select(c => c - '0').ToArray());
        }

        private static bool LuhnCheck(this IReadOnlyCollection<int> digits)
        {
            return GetCheckValue(digits) == 0;
        }

        private static int GetCheckValue(IReadOnlyCollection<int> digits)
        {
            return digits.Select((d, i) => i % 2 == digits.Count % 2 ? ((2 * d) % 10) + d / 5 : d).Sum() % 10;
        }

        public static string CreditCardType(string cardNumber)
        {
            if (CardType.Visa.Validate(cardNumber))
            {
                return CardType.Visa.Name;
            }
            if (CardType.MasterCard.Validate(cardNumber))
            {
                return CardType.MasterCard.Name;
            }
            if (CardType.Maestro.Validate(cardNumber))
            {
                return CardType.Maestro.Name;
            }
            if (CardType.AMEX.Validate(cardNumber))
            {
                return CardType.AMEX.Name;
            }
            if (CardType.DCI.Validate(cardNumber))
            {
                return CardType.DCI.Name;
            }
            if (CardType.DCUSC.Validate(cardNumber))
            {
                return CardType.DCUSC.Name;
            }
            if (CardType.Discover.Validate(cardNumber))
            {
                return CardType.Discover.Name;
            }
            if (CardType.JCB.Validate(cardNumber))
            {
                return CardType.JCB.Name;
            }
            return "";
        }
    }

    public class MuniciplityCounty
    {
        public string Municiplity { get; set; }
        public string ShortNm { get; set; }
        public string County { get; set; }
        public string DeployDate { get; set; }

        public string Typ { get; set; }
        public string LongNm { get; set; }
        public string objectId { get; set; }
    }

    public class HomeCity
    {
        [JsonProperty("cityName")]
        public string CityName { get; set; }

        [JsonProperty("cityDescirption")]
        public string CityDescirption { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("bookingUrl")]
        public string BookingUrl { get; set; }

        [JsonProperty("cityUrl")]
        public string CityUrl { get; set; }

        [JsonProperty("imageAnchor")]
        public string ImageAnchor { get; set; }
    }

    public class HomeCitySection
    {
        [JsonProperty("allHotelLink")]
        public string AllHotelLink { get; set; }

        [JsonProperty("homeCityList")]
        public List<HomeCity> HomeCityList { get; set; }
    }
}