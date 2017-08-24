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
                var resultList = new List<QueryData>();
                resultList = xml.OpenQuery(r);
                string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ToString();
                foreach (var rr in resultList)
                {
                    string queryString = @"insert into QUERY(QUERY_GUID,DOC_GUID,MEETING_DATE,MEETING_LOCATION, MEETING_TITLE,SEARCH_DATE) values ('" + Guid.NewGuid().ToString() + "','" + rr.DocId + "','" + rr.MeetingDate + "','" + rr.MeetingLocation + "','" + rr.MeetingTitle + "','" + rr.ScrapeDate + "')";
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

                    string queryString3 = @"insert into QUERY_ENTRY(ENTRY_GUID, QUERY_GUID,PAGE_NUMBER,KEYWORD,CONTENT, COMMENT) values (@ENTRY_GUID,@QUERY_GUID,@PAGE_NUMBER,@KEYWORD,@CONTENT,@COMMENT)";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Create the Command and Parameter objects.
                        SqlCommand command = new SqlCommand(queryString3, connection);
                        command.Parameters.AddWithValue("@ENTRY_GUID", rr.QueryGuid);
                        command.Parameters.AddWithValue("@QUERY_GUID", queryGuid);
                        command.Parameters.AddWithValue("@PAGE_NUMBER", rr.PageNumber);
                        command.Parameters.AddWithValue("@KEYWORD", rr.KeyWord);
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

        [TestMethod()]
        public void InsertCity()
        {

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
}