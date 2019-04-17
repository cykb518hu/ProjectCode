using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BusinessHandler.MessageHandler
{
    public interface IMapDataRepository
    {
        List<MapFilterModel> GetAllCities();
        List<MapFilterModel> GetFilterData(string state);
        List<MapMunicipalityColor> GetMapAreaData(DocQueryMessage message);
        List<MapMeeting> GetMainDataList(DocQueryMessage message, out int total);
        void UpdateMapColor(string cityGuid, string color);

        List<CityOrdinance> GetCityOrdinanceList(DocQueryMessage message,  out int total, string cityGuid = "");

        bool UpdateCityOrdinance(CityOrdinance data);
        List<MapMunicipalityColor> GetCartoSearchResult(string objectIds, string state);

        string GetContentDetail(string contentId, string keyWord);

        Dashboard GetDashboardData(string state);
    }

    public class SqlServerMapDataRepository:IMapDataRepository
    {
        IKeyWord _keyWord;
        public SqlServerMapDataRepository(IKeyWord keyWord)
        {
            _keyWord = keyWord;
        }

        public List<MapFilterModel> GetAllCities()
        {
            var list = new List<MapFilterModel>();
            string queryString = @"select  * from [dbo].[CITY] order by DEPLOYE_DATE desc";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MapFilterModel();
                    data.MunicipalityName = DBNull.Value == reader["CITY_NM"] ? "" : reader["CITY_NM"].ToString();
                    data.CityGuid = DBNull.Value == reader["GUID"] ? "" : reader["GUID"].ToString();
                    list.Add(data);
                }
            }
            return list;
        }
        public List<MapFilterModel> GetFilterData(string state)
        {
            var list = new List<MapFilterModel>();
            var email = StaticSetting.GetUserEmail();
            string queryString = @"select  * from [dbo].[CITY] C INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID WHERE AC.EMAIL=@EMAIL order by DEPLOYE_DATE desc";
            if (!string.IsNullOrWhiteSpace(state))
            {
                queryString = @"select  * from [dbo].[CITY] C INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID WHERE AC.EMAIL=@EMAIL and c.states='" + state + "' order by DEPLOYE_DATE desc";
            }
           
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@EMAIL", email);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MapFilterModel();
                    data.MunicipalityName = DBNull.Value == reader["CITY_NM"] ? "" : reader["CITY_NM"].ToString();
                    data.DeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");
                    data.CountyName = DBNull.Value == reader["COUNTY_NM"] ? "" : reader["COUNTY_NM"].ToString();
                    list.Add(data);
                }
            }
            return list;
        }

        public List<MapMunicipalityColor> GetMapAreaData(DocQueryMessage message)
        {
            var list = new List<MapMunicipalityColor>();

            var queryString = @"[dbo].[GET_Municipality]";

            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;
            
                StaticSetting.BuildParameters(command, message);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new MapMunicipalityColor();
                        data.Color = DBNull.Value == reader["color"] ? "" : reader["color"].ToString();
                        data.Id = DBNull.Value == reader["objectid"] ? 0 : Convert.ToInt32(reader["objectid"]);
                        data.MunicipalityName = DBNull.Value == reader["city_nm"] ? "" : reader["city_nm"].ToString();
                        data.Guid = DBNull.Value == reader["Guid"] ? "" : reader["Guid"].ToString();
                        list.Add(data);
                    }
                }
                return list;
            }

        }


        public List<MapMeeting> GetMainDataList(DocQueryMessage message, out int total)
        {
            var list = new List<MapMeeting>();
            var keyWord = message.KeyWord;
            string queryString = @"[dbo].[GET_DOC_CONTENT]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;

                StaticSetting.BuildParameters(command, message);

                connection.Open();

                total = Convert.ToInt32(command.ExecuteScalar());

                var orderBy = "";
                switch (message.sortName)
                {
                    case "DocType":
                        orderBy = "DOC_TYPE";
                        break;

                    case "MunicipalityDispaly":
                        orderBy = "CITY_NM";
                        break;
                    case "MeetingDateDisplay":
                        orderBy = "MEETING_DATE";
                        break;
                    case "ScrapeDate":
                        orderBy = "SEARCH_DATE";
                        break;
                    case "CityDeployDate":
                        orderBy = "DEPLOYE_DATE";
                        break;
                    default:
                        orderBy = "MEETING_DATE desc, CITY_NM ";
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
                        var result = new MapMeeting();
                        result.DocId = reader["DOC_GUID"].ToString();
                        var docType = reader["DOC_TYPE"].ToString();
                        // result.DocType = @"<p title='" + docType + "'  data-toggle='tooltip' data-placement='top' >" + docType + "</p>";

                        docType = docType.Replace("Commission Meeting", "").Replace("Appeals Meetings", "").Replace("Commission, City", "").Replace("Commissioners", "").Replace("Community Meetings", "").Replace("Force Meetings", "").Replace("of Appeals", "").Replace("Appeals", "").Replace("Commission", "");
                        result.DocType = docType;

                        result.MeetingDateDisplay = DBNull.Value == reader["MEETING_DATE"] ? "--" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                        result.ScrapeDate = DBNull.Value == reader["SEARCH_DATE"] ? "" : Convert.ToDateTime(reader["SEARCH_DATE"]).ToString("yyyy-MM-dd");
                        result.CityDeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");

                        result.IsViewed = "<span class='sp_" + result.DocId + "'>" + (reader["CHECKED"].ToString().Equals("True") ? "Yes" : "No") + "</span>";
                        var important = reader["IMPORTANT"].ToString().Equals("True") ? "Yes" : "No";
                        result.MunicipalityDispaly = @"<a href='" + reader["DOC_SOURCE"].ToString() + "' target='_blank'>" + reader["CITY_NM"].ToString().Replace("MI", "").Replace("Charter", "") + "</a>";
                        result.MinicipalityOperation = @"<div class='btn-group' role='group'><button type='button' class='btn btn-default glyphicon glyphicon-edit' title='Add note'  data-docid='" + result.DocId + "' onclick='OpenDocNoteDetail(this); return false'></button>";

                        //importan means removed
                        if (important.Equals("Yes"))
                        {
                            result.MinicipalityOperation += @"<button type='button' class='btn btn-default glyphicon glyphicon-plus'  data-removed='" + important + "' title='Add data back'   data-docid='" + result.DocId + "'  onclick='RemoveData(this); return false'></button>";
                        }
                        else
                        {
                            //data-toggle='tooltip' data-placement='top'
                            result.MinicipalityOperation += @"<button type='button' class='btn btn-default glyphicon glyphicon-remove'  data-removed='" + important + "' title='Remove data'    data-docid='" + result.DocId + "'  onclick='RemoveData(this); return false'></button>";
                        }
                        result.MinicipalityOperation += "</div>";
                        result.ObjectId = Convert.ToInt32(reader["OBJECTID"]);
                        list.Add(result);

                    }
                }
            }
            GetSubList(list, keyWord);
            return list;
        }

        public  void GetSubList(List<MapMeeting> list,string keyWord )
        {
            var sqlKeyWord = "";
            if (string.IsNullOrWhiteSpace(keyWord) || keyWord.Contains("All"))
            {
                keyWord = _keyWord.GetKeyWords();
            }
            sqlKeyWord = StaticSetting.GetKeyWordForFullSearch(keyWord);

            var docIDList = string.Empty;
            foreach (var r in list)
            {
                docIDList += "'" + r.DocId + "',";
            }
            docIDList = docIDList.TrimEnd(',');
            if (string.IsNullOrWhiteSpace(docIDList))
            {
                return;
            }
            string queryString = @"[dbo].[GET_DOC_NOTES_AMOUNT]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DocIdList", docIDList);
                connection.Open();
                Regex regex = new Regex("btn-default");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var docId = reader["DOC_GUID"].ToString();
                        var count = Convert.ToInt32(reader["count"]);
                        if (count > 0)
                        {
                            var item = list.FirstOrDefault(x => x.DocId == docId);
                            if (item != null)
                            {

                                item.MinicipalityOperation = regex.Replace(item.MinicipalityOperation, "btn-success", 1);

                            }
                        }
                    }
                }
            }

            queryString = @"[dbo].[GET_DOC_CONTENT_SUBLIST]";
            var resultList = new List<MapMeetingKeyWord>();
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DocIdList", docIDList);
                command.Parameters.AddWithValue("@KeyWord", sqlKeyWord);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new MapMeetingKeyWord();
                        result.DocContentId = reader["CONTENT_ID"].ToString();
                        result.PageNumber = DBNull.Value == reader["PAGE_NUMBER"] ? 0 : Convert.ToInt32(reader["PAGE_NUMBER"]);
                        result.Content = DBNull.Value == reader["CONTENT"] ? "" : reader["CONTENT"].ToString();
                        result.DocId= DBNull.Value == reader["DOC_GUID"] ? "" : reader["DOC_GUID"].ToString();
                        //if (result.KeyWord.IndexOf('*') >= 0)
                        //{
                        //    var arr = result.Content.Split(' ');
                        //    for (int i = 0; i < arr.Length; i++)
                        //    {
                        //        if (Regex.IsMatch(arr[i], result.KeyWord, RegexOptions.IgnoreCase))
                        //        {
                        //            arr[i] = string.Format("<b style='color:red'>{0}</b>", arr[i]);
                        //        }
                        //    }
                        //    result.Content = String.Join(" ", arr);
                        //}
                        //else
                        //{
                        //    result.Content = Regex.Replace(result.Content, result.KeyWord, string.Format("<b style='color:red'>{0}</b>", result.KeyWord), RegexOptions.IgnoreCase);
                        //}
                        resultList.Add(result);
                    }
                }
            }
            var finalList=new  List<MapMeetingKeyWord>();
            var keyWordList = keyWord.Split(',').ToList();
            var index = 35;
            CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
            foreach (var r in resultList)
            {
                foreach (var k in keyWordList)
                {
                    var currentIndex = Compare.IndexOf(r.Content, k, CompareOptions.IgnoreCase);
                    //var currentIndex = r.Content.IndexOf(k, Compare.I);
                    if(currentIndex<0)
                    {
                        continue;
                    }
                    var data = new MapMeetingKeyWord();
                    data.DocId = r.DocId;
                    data.DocContentId = r.DocContentId;
                    data.PageNumber = r.PageNumber;
                    data.KeyWord = k;
                    var startIndex = currentIndex - index >= 0 ? currentIndex - index : 0;
                    var endIndex = currentIndex + index < r.Content.Length ?currentIndex + index : r.Content.Length ;
                    data.Content = r.Content.Substring(startIndex, endIndex - startIndex);
                    data.Content = Regex.Replace(data.Content, k, string.Format("<b style='color:red'>{0}</b>", k), RegexOptions.IgnoreCase);
                    data.Operation = "<a style='cursor:pointer' data-contentId='" + r.DocContentId + "' data-keyword='" + k + "'  onclick='viewContentDetail(this); return false'>More...</a>";
                    finalList.Add(data);
                }
            }

            foreach (var r in list)
            {
                var subList = finalList.Where(x => x.DocId == r.DocId).ToList();
                subList = subList.OrderBy(x => x.PageNumber).ToList();
                r.DocQuerySubList = subList;
            }
        }

        public string GetContentDetail(string contentId,string keyWord)
        {
            var result = string.Empty;
            string queryString = @"select  CONTENT from DBO.DOCUMENT_CONTENT  WHERE CONTENT_ID=@CONTENT_ID ";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@CONTENT_ID", contentId);
                connection.Open();
                result = command.ExecuteScalar().ToString();
            }
            result= Regex.Replace(result, keyWord, string.Format("<b style='color:red'>{0}</b>", keyWord), RegexOptions.IgnoreCase);
            return result;
        }

        public void UpdateMapColor(string cityGuid, string color)
        {
            if (color == "blue")
            {
                color = "";
            }
            var queryString = string.Format("UPDATE CITY SET COLOR= @COLOR WHERE GUID='{0}'", cityGuid);
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@COLOR", color);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<CityOrdinance> GetCityOrdinanceList(DocQueryMessage message,  out int total, string cityGuid = "")
        {

            var list = new List<CityOrdinance>();

            var queryString = @"[dbo].[GET_CITY_Ordinance]";

            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType= CommandType.StoredProcedure;
                //this is different from other two, becuase if all don't need to join document_content table
                //if (!string.IsNullOrWhiteSpace(message.KeyWord)&&!message.KeyWord.Contains("All"))
              //  {
              //      message.KeyWord = StaticSetting.GetKeyWordForFullSearch(message.KeyWord);
              //  }
               
                StaticSetting.BuildParameters(command, message);
                connection.Open();
                if (string.IsNullOrEmpty(cityGuid))
                {
                    total = Convert.ToInt32(command.ExecuteScalar());
                }
                else
                {
                    total = 0;
                    command.Parameters.AddWithValue("@CityGuid", cityGuid);
                }
                var orderBy = " CITY_NM desc";
                if (!string.IsNullOrEmpty(message.sortName))
                {
                    if(message.sortName== "Municipality")
                    {
                        orderBy = " CITY_NM";
                    }
                    else
                    {
                        orderBy = " " + message.sortName;
                    }
                    orderBy += " " + message.sortOrder;
                }
               

                command.Parameters.AddWithValue("@offset", message.offset);
                command.Parameters.AddWithValue("@limit", message.limit);
                command.Parameters.AddWithValue("@Total", 0);
                command.Parameters.AddWithValue("@OrderByField", orderBy);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new CityOrdinance();
                        data.Municipality = reader["CITY_NM"].ToString().Replace("MI", "").Replace("Charter", "");
                        data.CityGuid = reader["CITY_GUID"].ToString();
                        data.OptStatus = DBNull.Value == reader["OptStatus"] ? "" : reader["OptStatus"].ToString();
                        data.DraftDate = DBNull.Value == reader["DraftDate"] ? "" : String.IsNullOrEmpty(reader["DraftDate"].ToString()) ? "" : Convert.ToDateTime(reader["DraftDate"]).ToString("yyyy-MM-dd");
                        data.FinalDate = DBNull.Value == reader["FinalDate"] ? "" : String.IsNullOrEmpty(reader["FinalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalDate"]).ToString("yyyy-MM-dd");
                        data.Measurement = DBNull.Value == reader["Measurement"] ? "" : reader["Measurement"].ToString();

                        data.BufferSchoolFeet = DBNull.Value == reader["BufferSchoolFeet"] ? "" : reader["BufferSchoolFeet"].ToString();
                        data.BufferSchoolNote = DBNull.Value == reader["BufferSchoolNote"] ? "" : reader["BufferSchoolNote"].ToString();

                        data.BufferDaycareFeet = DBNull.Value == reader["BufferDaycareFeet"] ? "" : reader["BufferDaycareFeet"].ToString();
                        data.BufferDaycareNote = DBNull.Value == reader["BufferDaycareNote"] ? "" : reader["BufferDaycareNote"].ToString();

                        data.BufferParkFeet = DBNull.Value == reader["BufferParkFeet"] ? "" : reader["BufferParkFeet"].ToString();
                        data.BufferParkNote = DBNull.Value == reader["BufferParkNote"] ? "" : reader["BufferParkNote"].ToString();

                        data.BufferSDMFeet = DBNull.Value == reader["BufferSDMFeet"] ? "" : reader["BufferSDMFeet"].ToString();
                        data.BufferSDMNote = DBNull.Value == reader["BufferSDMNote"] ? "" : reader["BufferSDMNote"].ToString();

                        data.BufferReligiousFeet = DBNull.Value == reader["BufferReligiousFeet"] ? "" : reader["BufferReligiousFeet"].ToString();
                        data.BufferReligiousNote = DBNull.Value == reader["BufferReligiousNote"] ? "" : reader["BufferReligiousNote"].ToString();

                        data.BufferOtherFeet = DBNull.Value == reader["BufferOtherFeet"] ? "" : reader["BufferOtherFeet"].ToString();
                        data.BufferOtherNote = DBNull.Value == reader["BufferOtherNote"] ? "" : reader["BufferOtherNote"].ToString();

                        data.BufferResidentialFeet = DBNull.Value == reader["BufferResidentialFeet"] ? "" : reader["BufferResidentialFeet"].ToString();
                        data.BufferResidentialNote = DBNull.Value == reader["BufferResidentialNote"] ? "" : reader["BufferResidentialNote"].ToString();

                        data.BufferRoadFeet = DBNull.Value == reader["BufferRoadFeet"] ? "" : reader["BufferRoadFeet"].ToString();
                        data.BufferRoadNote = DBNull.Value == reader["BufferRoadNote"] ? "" : reader["BufferRoadNote"].ToString();

                        data.FacililtyGrowerClassAPermit = DBNull.Value == reader["FacililtyGrowerClassAPermit"] ? "" : reader["FacililtyGrowerClassAPermit"].ToString();  
                        data.FacililtyGrowerClassAZoningInd = DBNull.Value == reader["FacililtyGrowerClassAZoningInd"] ? "--" : reader["FacililtyGrowerClassAZoningInd"].ToString();
                        data.FacililtyGrowerClassAZoningCom = DBNull.Value == reader["FacililtyGrowerClassAZoningCom"] ? "--" : reader["FacililtyGrowerClassAZoningCom"].ToString();
                        data.FacililtyGrowerClassALimit = DBNull.Value == reader["FacililtyGrowerClassALimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtyGrowerClassALimit"].ToString()) ? "No Cap" : reader["FacililtyGrowerClassALimit"].ToString();
                        data.FacililtyGrowerClassANote = DBNull.Value == reader["FacililtyGrowerClassANote"] ? "" : reader["FacililtyGrowerClassANote"].ToString();

                        data.FacililtyGrowerClassBPermit = DBNull.Value == reader["FacililtyGrowerClassBPermit"] ? "" : reader["FacililtyGrowerClassBPermit"].ToString();
                        data.FacililtyGrowerClassBZoningInd = DBNull.Value == reader["FacililtyGrowerClassBZoningInd"] ? "--" : reader["FacililtyGrowerClassBZoningInd"].ToString();
                        data.FacililtyGrowerClassBZoningCom = DBNull.Value == reader["FacililtyGrowerClassBZoningCom"] ? "--" : reader["FacililtyGrowerClassBZoningCom"].ToString();
                        data.FacililtyGrowerClassBLimit = DBNull.Value == reader["FacililtyGrowerClassBLimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtyGrowerClassBLimit"].ToString()) ? "No Cap" : reader["FacililtyGrowerClassBLimit"].ToString();
                        data.FacililtyGrowerClassBNote = DBNull.Value == reader["FacililtyGrowerClassBNote"] ? "" : reader["FacililtyGrowerClassBNote"].ToString();

                        data.FacililtyGrowerClassCPermit = DBNull.Value == reader["FacililtyGrowerClassCPermit"] ? "" : reader["FacililtyGrowerClassCPermit"].ToString();
                        data.FacililtyGrowerClassCZoningInd = DBNull.Value == reader["FacililtyGrowerClassCZoningInd"] ? "--" : reader["FacililtyGrowerClassCZoningInd"].ToString();
                        data.FacililtyGrowerClassCZoningCom = DBNull.Value == reader["FacililtyGrowerClassCZoningCom"] ? "--" : reader["FacililtyGrowerClassCZoningCom"].ToString();
                        data.FacililtyGrowerClassCLimit = DBNull.Value == reader["FacililtyGrowerClassCLimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtyGrowerClassCLimit"].ToString()) ? "No Cap" : reader["FacililtyGrowerClassCLimit"].ToString();
                        data.FacililtyGrowerClassCNote = DBNull.Value == reader["FacililtyGrowerClassCNote"] ? "" : reader["FacililtyGrowerClassCNote"].ToString();


                        data.FacililtyProvPermit = DBNull.Value == reader["FacililtyProvPermit"] ? "" : reader["FacililtyProvPermit"].ToString();
                        data.FacililtyProvZoningInd = DBNull.Value == reader["FacililtyProvZoningInd"] ? "--" : reader["FacililtyProvZoningInd"].ToString();
                        data.FacililtyProvZoningCom = DBNull.Value == reader["FacililtyProvZoningCom"] ? "--" : reader["FacililtyProvZoningCom"].ToString();
                        data.FacililtyProvLimit = DBNull.Value == reader["FacililtyProvLimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtyProvLimit"].ToString()) ? "No Cap" : reader["FacililtyProvLimit"].ToString();
                        data.FacililtyProvNote = DBNull.Value == reader["FacililtyProvNote"] ? "" : reader["FacililtyProvNote"].ToString();

                        data.FacililtyProcPermit = DBNull.Value == reader["FacililtyProcPermit"] ? "" : reader["FacililtyProcPermit"].ToString();
                        data.FacililtyProcZoningInd = DBNull.Value == reader["FacililtyProcZoningInd"] ? "--" : reader["FacililtyProcZoningInd"].ToString();
                        data.FacililtyProcZoningCom = DBNull.Value == reader["FacililtyProcZoningCom"] ? "--" : reader["FacililtyProcZoningCom"].ToString();
                        data.FacililtyProcLimit = DBNull.Value == reader["FacililtyProcLimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtyProcLimit"].ToString()) ? "No Cap" : reader["FacililtyProcLimit"].ToString();
                        data.FacililtyProcNote = DBNull.Value == reader["FacililtyProcNote"] ? "" : reader["FacililtyProcNote"].ToString();

                        data.FacililtySCPermit = DBNull.Value == reader["FacililtySCPermit"] ? "" : reader["FacililtySCPermit"].ToString();
                        data.FacililtySCZoningInd = DBNull.Value == reader["FacililtySCZoningInd"] ? "--" : reader["FacililtySCZoningInd"].ToString();
                        data.FacililtySCZoningCom = DBNull.Value == reader["FacililtySCZoningCom"] ? "--" : reader["FacililtySCZoningCom"].ToString();
                        data.FacililtySCLimit = DBNull.Value == reader["FacililtySCLimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtySCLimit"].ToString()) ? "No Cap" : reader["FacililtySCLimit"].ToString();                     
                        data.FacililtySCNote = DBNull.Value == reader["FacililtySCNote"] ? "" : reader["FacililtySCNote"].ToString();

                        data.FacililtySTPermit = DBNull.Value == reader["FacililtySTPermit"] ? "" : reader["FacililtySTPermit"].ToString();
                        data.FacililtySTZoningInd = DBNull.Value == reader["FacililtySTZoningInd"] ? "--" : reader["FacililtySTZoningInd"].ToString();
                        data.FacililtySTZoningCom = DBNull.Value == reader["FacililtySTZoningCom"] ? "--" : reader["FacililtySTZoningCom"].ToString();
                        data.FacililtySTLimit = DBNull.Value == reader["FacililtySTLimit"] ? "No Cap" : String.IsNullOrEmpty(reader["FacililtySTLimit"].ToString()) ? "No Cap" : reader["FacililtySTLimit"].ToString();
                        data.FacililtySTNote = DBNull.Value == reader["FacililtySTNote"] ? "" : reader["FacililtySTNote"].ToString();

                        data.CityFileName= DBNull.Value == reader["CityFileName"] ? "" : reader["CityFileName"].ToString();

                        if (data.BufferSchoolFeet == "0")
                        {
                            data.BufferSchoolFeet = "";
                        }
                        if (data.BufferDaycareFeet == "0")
                        {
                            data.BufferDaycareFeet = "";
                        }
                        if (data.BufferParkFeet == "0")
                        {
                            data.BufferParkFeet = "";
                        }
                        if (data.BufferSDMFeet == "0")
                        {
                            data.BufferSDMFeet = "";
                        }
                        if (data.BufferReligiousFeet == "0")
                        {
                            data.BufferReligiousFeet = "";
                        }
                        if (data.BufferResidentialFeet == "0")
                        {
                            data.BufferResidentialFeet = "";
                        }

                        if (data.BufferRoadFeet == "0")
                        {
                            data.BufferRoadFeet = "";
                        }
                        if (data.BufferOtherFeet == "0")
                        {
                            data.BufferOtherFeet = "";
                        }
                        list.Add(data);
                    }
                }
               
            }
            return list;


        }

        public bool UpdateCityOrdinance(CityOrdinance data)
        {
            var result = true;
            try
            {
                var exist = false;
                var existStr= "select * from CITY_Ordinance where CITY_GUID='" + data.CityGuid + "'";

                using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                {
                    SqlCommand command = new SqlCommand(existStr, connection);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        exist = true;
                    }
                }
                if(!exist)
                {
                    var insertStr = "insert into dbo.[CITY_Ordinance] (USR_CRTN_ID,USR_MDFN_ID,City_Guid) VALUES(@EMAIL,@EMAIL,@GUID)";

                    using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                    {
                        SqlCommand command = new SqlCommand(insertStr, connection);
                        command.Parameters.AddWithValue("@EMAIL", data.ModifyUser);
                        command.Parameters.AddWithValue("@GUID", data.CityGuid);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                //com cap field is used to write some customize info
                if (!string.IsNullOrWhiteSpace(data.FacililtyGrowerClassAComCap))
                {
                    data.FacililtyGrowerClassALimit = data.FacililtyGrowerClassAComCap;
                }
                if (!string.IsNullOrWhiteSpace(data.FacililtyGrowerClassBComCap))
                {
                    data.FacililtyGrowerClassBLimit = data.FacililtyGrowerClassBComCap;
                }
                if (!string.IsNullOrWhiteSpace(data.FacililtyGrowerClassCComCap))
                {
                    data.FacililtyGrowerClassCLimit = data.FacililtyGrowerClassCComCap;
                }
                if (!string.IsNullOrWhiteSpace(data.FacililtyProcComCap))
                {
                    data.FacililtyProcLimit = data.FacililtyProcComCap;
                }
                if (!string.IsNullOrWhiteSpace(data.FacililtyProvComCap))
                {
                    data.FacililtyProvLimit = data.FacililtyProvComCap;
                }
                if (!string.IsNullOrWhiteSpace(data.FacililtySCComCap))
                {
                    data.FacililtySCLimit = data.FacililtySCComCap;
                }
                if (!string.IsNullOrWhiteSpace(data.FacililtySTComCap))
                {
                    data.FacililtySTLimit = data.FacililtySTComCap;
                }
                Type t = typeof(CityOrdinance);
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                var str = "UPDATE CITY_Ordinance SET ";
                foreach (System.Reflection.PropertyInfo info in properties)
                {
                    if (info.Name == "Municipality" || info.Name == "CityGuid" || info.Name == "Action" 
                        || info.Name == "FacililtySTZoning" || info.Name == "FacililtySCZoning" || info.Name == "FacililtyProcZoning" || info.Name == "FacililtyProvZoning" || info.Name == "FacililtyGrowerClassAZoning" || info.Name == "FacililtyGrowerClassBZoning" || info.Name == "FacililtyGrowerClassCZoning"
                         || info.Name == "FacililtyGrowerClassAComCap" || info.Name == "FacililtyGrowerClassBComCap" || info.Name == "FacililtyGrowerClassCComCap" || info.Name == "FacililtyProcComCap" || info.Name == "FacililtyProvComCap" || info.Name == "FacililtySCComCap" || info.Name == "FacililtySTComCap"
                        || info.Name == "OrdinanceTime" || info.Name == "CityFileDisplayName")
                    {
                        continue;
                    }
                    if(info.Name=="ModifyUser")
                    {
                        str += string.Format(" USR_MDFN_ID='{0}', ", data.ModifyUser);

                        str += string.Format(" USR_MDFN_TS='{0}', ", DateTime.Now);
                        continue;
                    }
                    if(info.Name == "CityFileName")
                    {
                        var fileName = GetObjectPropertyValue<CityOrdinance>(data, info.Name);
                        if(string.IsNullOrEmpty(fileName))
                        {
                            continue;
                        }
                    }
                    str += string.Format("{0} = '{1}', ", info.Name, GetObjectPropertyValue<CityOrdinance>(data, info.Name));
                }
                //there is one space and comma
                str = str.Substring(0, str.Length - 2);
                str += " where CITY_GUID='" + data.CityGuid + "'";
                using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                {
                    SqlCommand command = new SqlCommand(str, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                result = false;
            }
            return result;
        }

        public  string GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return string.Empty;

            object o = property.GetValue(t, null);

            if (o == null) return string.Empty;

            return o.ToString();
        }


        public List<MapMunicipalityColor> GetCartoSearchResult(string objectIds, string state)
        {

            var queryString = @"SELECT distinct objectid, color 
 FROM  DBO.CITY where objectId in (" + objectIds + ") and STATES ='" + state + "'";

            var list = new List<MapMunicipalityColor>();


            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new MapMunicipalityColor();
                        data.Color = DBNull.Value == reader["color"] ? "blue" : reader["color"].ToString();
                        data.Id = DBNull.Value == reader["objectid"] ? 0 : Convert.ToInt32(reader["objectid"]);
                        list.Add(data);
                    }
                }
                var idArr = objectIds.Split(',');
                foreach(var r in idArr)
                {
                    if(!list.Any(x=>x.Id==Convert.ToInt32(r)))
                    {
                        list.Add(new MapMunicipalityColor { Id = Convert.ToInt32(r), Color = "gray" });
                    }
                }
                return list;
            }
        }


        public Dashboard GetDashboardData(string state)
        {
            var result = new Dashboard();
            result.NumberOfCities = GetNumberOfCities(state);
            result.AvgDays = GetAvgDays(state);
            result.RecentScrapes = GetRecentScrape(state);
            result.RecentMeetings = GetRecentMeeting(state);
            result.UpcomingMeetings = GetUpcomingMeeting(state);
            result.MeetingLineGraphData = GetMeetingLineData(state);

            return result;
        }
        public int GetNumberOfCities(string state)
        {
            int result = 0;
            string queryString = @"select  count(*) from [dbo].[CITY] WHERE STATES=@STATES";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                result = Convert.ToInt32(command.ExecuteScalar());

            }
            return result;
        }

        /// <summary>
        /// get latest two month data
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int GetAvgDays(string state)
        {
            int result = 0;
            string queryString = @"SELECT AVG(DaysBetween) FROM (SELECT DISTINCT D.MEETING_DATE,D.USR_CRTN_TS, DATEDIFF(day, D.MEETING_DATE, D.USR_CRTN_TS) AS DaysBetween from DOCUMENT D INNER JOIN CITY C ON C.CITY_NM=D.CITY_NM INNER JOIN DOCUMENT_CONTENT DC ON DC.DOC_GUID=D.DOC_GUID 
where C.STATES=@STATES AND MEETING_DATE<=GETDATE() and MEETING_DATE>DATEADD(month, -2, GETDATE()) AND MEETING_DATE<D.USR_CRTN_TS) LST";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                result = Convert.ToInt32(command.ExecuteScalar());

            }
            return result;
        }

        public List<RecentScrape> GetRecentScrape(string state)
        {
            var result = new List<RecentScrape>();
            string queryString = @"select top 10 city_nm as NAME, DEPLOYE_DATE  from city WHERE STATES=@STATES order by DEPLOYE_DATE desc";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new RecentScrape();
                    data.CityName = DBNull.Value == reader["NAME"] ? "" : reader["NAME"].ToString();
                    data.DeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");
                    result.Add(data);
                }
            }
            return result;
        }

        public List<RecentMeeting> GetRecentMeeting(string state)
        {
            var result = new List<RecentMeeting>();
            string queryString = @"select distinct top 10 C.CITY_NM, DOC_TYPE , MEETING_DATE from DOCUMENT D INNER JOIN CITY C ON C.CITY_NM=D.CITY_NM where C.STATES=@STATES AND MEETING_DATE<=GETDATE() order by meeting_date desc";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new RecentMeeting();
                    data.CityName = DBNull.Value == reader["CITY_NM"] ? "" : reader["CITY_NM"].ToString();
                    data.Meeting = DBNull.Value == reader["DOC_TYPE"] ? "" : reader["DOC_TYPE"].ToString();
                    data.MeetingDate = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                    result.Add(data);
                }
            }
            return result;
        }
        public List<RecentMeeting> GetUpcomingMeeting(string state)
        {
            var result = new List<RecentMeeting>();
            string queryString = @"select distinct top 10 C.CITY_NM, DOC_TYPE , MEETING_DATE from DOCUMENT D INNER JOIN CITY C ON C.CITY_NM=D.CITY_NM where C.STATES=@STATES AND MEETING_DATE>GETDATE() order by meeting_date asc";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new RecentMeeting();
                    data.CityName = DBNull.Value == reader["CITY_NM"] ? "" : reader["CITY_NM"].ToString();
                    data.Meeting = DBNull.Value == reader["DOC_TYPE"] ? "" : reader["DOC_TYPE"].ToString();
                    data.MeetingDate = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                    result.Add(data);
                }
            }
            return result;
        }

        public List<MeetingLineGraph> GetMeetingLineData(string state)
        {
            var result = new List<MeetingLineGraph>();
            string queryString = @"select
 FORMAT(MEETING_DATE, 'yyyy-MM') AS Closing_Month
 , count(*) AMOUNT 

FROM DOCUMENT D INNER JOIN CITY C ON C.CITY_NM=D.CITY_NM
WHERE
C.STATES=@STATES
 AND MEETING_DATE <=GETDATE()
 AND MEETING_DATE>'2016'
GROUP BY FORMAT(MEETING_DATE, 'yyyy-MM')
ORDER BY Closing_Month";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MeetingLineGraph();
                    data.Period = DBNull.Value == reader["Closing_Month"] ? "" : reader["Closing_Month"].ToString();
                    data.MeetingDateAmount = DBNull.Value == reader["AMOUNT"] ? 0 : Convert.ToInt32(reader["AMOUNT"]);
                    result.Add(data);
                }
            }

            queryString = @"select
 FORMAT(USR_CRTN_TS, 'yyyy-MM') AS Closing_Month
 , count(*) AMOUNT 

FROM DOCUMENT D INNER JOIN CITY C ON C.CITY_NM=D.CITY_NM
WHERE
C.STATES=@STATES
 AND USR_CRTN_TS <=GETDATE()
 AND USR_CRTN_TS>'2016'
GROUP BY FORMAT(USR_CRTN_TS, 'yyyy-MM')
ORDER BY Closing_Month
";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("STATES", state);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var period = DBNull.Value == reader["Closing_Month"] ? "" : reader["Closing_Month"].ToString();
                    var data = result.FirstOrDefault(x => x.Period == period);
                    if (data != null)
                    {
                        data.ScrapeDateAmount = DBNull.Value == reader["AMOUNT"] ? 0 : Convert.ToInt32(reader["AMOUNT"]);
                    }
                }
            }
            return result;
        }

    }
}
