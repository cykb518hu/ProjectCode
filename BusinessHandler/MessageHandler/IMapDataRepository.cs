using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessHandler.MessageHandler
{
    public interface IMapDataRepository
    {
        List<MapFilterModel> GetAllCities();
        List<MapFilterModel> GetFilterData();
        List<MapMunicipalityColor> GetMapAreaData(DocQueryMessage message);
        List<MapMeeting> GetMainDataList(DocQueryMessage message, out int total);
        void UpdateMapColor(int cityId, string color);

        List<CityOrdinance> GetCityOrdinanceList(DocQueryMessage message, string cityGuid = "");

        bool UpdateCityOrdinance(CityOrdinance data);
    }

    public class SqlServerMapDataRepository:IMapDataRepository
    {
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
        public List<MapFilterModel> GetFilterData()
        {
            var list = new List<MapFilterModel>();
            var email = StaticSetting.GetUserEmail();
            string queryString = @"select  * from [dbo].[CITY] C INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID WHERE AC.EMAIL=@EMAIL order by DEPLOYE_DATE desc";
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

                BuildParameters(command, message);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new MapMunicipalityColor();
                        data.Color = DBNull.Value == reader["color"] ? "" : reader["color"].ToString();
                        data.Id = DBNull.Value == reader["objectid"] ? 0 : Convert.ToInt32(reader["objectid"]);
                        data.MunicipalityName = DBNull.Value == reader["city_nm"] ? "" : reader["city_nm"].ToString();
                        list.Add(data);
                    }
                }
                return list;
            }

        }


        public  List<MapMeeting> GetMainDataList(DocQueryMessage message, out int total)
        {
            var list = new List<MapMeeting>();

            string queryString = @"[dbo].[GET_DOC_QUERY]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;

                BuildParameters(command, message);

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

                        result.MeetingDateDisplay = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                        result.ScrapeDate = DBNull.Value == reader["SEARCH_DATE"] ? "" : Convert.ToDateTime(reader["SEARCH_DATE"]).ToString("yyyy-MM-dd");
                        result.CityDeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");

                        result.IsViewed = "<span class='sp_" + result.DocId + "'>" + (reader["CHECKED"].ToString().Equals("True") ? "Yes" : "No") + "</span>";
                        var important = reader["IMPORTANT"].ToString().Equals("True") ? "Yes" : "No";
                        result.MunicipalityDispaly = @"<a href='" + reader["DOC_SOURCE"].ToString() + "' target='_blank'>" + reader["CITY_NM"].ToString().Replace("MI", "") + "</a>";
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
            GetSubList(list);
            return list;
        }

        public  void GetSubList(List<MapMeeting> list)
        {
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

            queryString = @"[dbo].[GET_DOC_QUERY_SUBLIST]";
            var resultList = new List<MapMeetingKeyWord>();
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DocIdList", docIDList);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new MapMeetingKeyWord();
                        result.DocId = reader["DOC_GUID"].ToString();
                        result.QueryGuid = reader["ENTRY_GUID"].ToString();
                        result.PageNumber = reader["PAGE_NUMBER"].ToString();
                        result.KeyWord = reader["KEYWORD"].ToString();
                        result.Comment = "<span id=" + result.QueryGuid + ">" + (DBNull.Value == reader["COMMENT"] ? "" : reader["COMMENT"].ToString()) + "</span>";
                        result.Content = DBNull.Value == reader["CONTENT"] ? "" : reader["CONTENT"].ToString();
                        if (result.KeyWord.IndexOf('*') >= 0)
                        {
                            var arr = result.Content.Split(' ');
                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (Regex.IsMatch(arr[i], result.KeyWord, RegexOptions.IgnoreCase))
                                {
                                    arr[i] = string.Format("<b style='color:red'>{0}</b>", arr[i]);
                                }
                            }
                            result.Content = String.Join(" ", arr);
                        }
                        else
                        {
                            result.Content = Regex.Replace(result.Content, result.KeyWord, string.Format("<b style='color:red'>{0}</b>", result.KeyWord), RegexOptions.IgnoreCase);
                        }
                        result.Operation = @"<button type='button' class='btn btn-default glyphicon glyphicon-edit' aria-label='Left Align'  data-queryguid='" + result.QueryGuid + "' onclick='OpenDataDetail(this); return false'></button>";
                        resultList.Add(result);
                    }
                }
            }
            foreach (var r in list)
            {
                var subList = resultList.Where(x => x.DocId == r.DocId).ToList();
                int count = 0;
                subList.ForEach(x => { count += Regex.Matches(x.Content, x.KeyWord, RegexOptions.IgnoreCase).Count; });
               // r.Number = count;
                subList = subList.OrderBy(x => x.PageNumber).ToList();
                r.DocQuerySubList = subList;
            }
        }


        public void BuildParameters(SqlCommand command,DocQueryMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message.CityName) && !message.CityName.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                 command.Parameters.AddWithValue("@CityName", StaticSetting.GetArrayQuery(message.CityName));
            }
            if (!string.IsNullOrWhiteSpace(message.CountyName) && !message.CountyName.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@CountyName", StaticSetting.GetArrayQuery(message.CountyName));
            }
            if (!string.IsNullOrWhiteSpace(message.KeyWord) && !message.KeyWord.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@KeyWord", StaticSetting.GetArrayQuery(message.KeyWord));
            }
            if (!string.IsNullOrWhiteSpace(message.MeetingDate))
            {
                command.Parameters.AddWithValue("@StartMeetingDate", message.MeetingDate);
            }
            if (!string.IsNullOrWhiteSpace(message.StartMeetingDate))
            {
                command.Parameters.AddWithValue("@StartMeetingDate", message.StartMeetingDate);
            }
            if (!string.IsNullOrWhiteSpace(message.EndMeetingDate))
            {
                command.Parameters.AddWithValue("@EndMeetingDate", message.EndMeetingDate);
            }
            if (!string.IsNullOrWhiteSpace(message.DeployDate) && !message.DeployDate.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@DeployeDate", StaticSetting.GetArrayQuery(message.DeployDate));
            }
            if (!string.IsNullOrWhiteSpace(message.IsViewed) && !message.IsViewed.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var isChecked = message.IsViewed == "Yes" ? "True" : "False";
                command.Parameters.AddWithValue("@IsChecked", isChecked);
            }
            //important means removed
            if (!string.IsNullOrWhiteSpace(message.Important) && !message.Important.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var isImportant = message.Important == "Yes" ? "True" : "False";
                command.Parameters.AddWithValue("@IsImportant", isImportant);
            }

            var email = StaticSetting.GetUserEmail();
            if (!string.IsNullOrWhiteSpace(email))
            {
                command.Parameters.AddWithValue("@UserEmail", email);
            }
        }

        public void UpdateMapColor(int cityId, string color)
        {
            var queryString = "UPDATE CITY SET COLOR= @COLOR WHERE OBJECTID=" + cityId;
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@COLOR", color);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<CityOrdinance> GetCityOrdinanceList(DocQueryMessage message, string cityGuid = "")
        {

            var list = new List<CityOrdinance>();

            var queryString = @"SELECT * FROM DBO.CITY_Ordinance WHERE CITY_GUID='" + cityGuid + "'";

            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new CityOrdinance();
                        data.Municipality = "TEST";
                        data.CityGuid = reader["CITY_GUID"].ToString();
                        data.OptStatus = DBNull.Value == reader["OptStatus"] ? "" : reader["OptStatus"].ToString();
                        data.DraftDate = DBNull.Value == reader["DraftDate"] ? "" : Convert.ToDateTime(reader["DraftDate"]).ToString("yyyy-MM-dd");
                        data.FinalDate = DBNull.Value == reader["FinalDate"] ? "" : Convert.ToDateTime(reader["FinalDate"]).ToString("yyyy-MM-dd");
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

                        data.FacililtyGrPermit = DBNull.Value == reader["FacililtyGrPermit"] ? "" : reader["FacililtyGrPermit"].ToString();  
                        data.FacililtyGrZoningInd = DBNull.Value == reader["FacililtyGrZoningInd"] ? "--" : reader["FacililtyGrZoningInd"].ToString();
                        data.FacililtyGrZoningCom = DBNull.Value == reader["FacililtyGrZoningCom"] ? "--" : reader["FacililtyGrZoningCom"].ToString();
                        data.FacililtyGrLimit = DBNull.Value == reader["FacililtyGrLimit"] ? "No Limit" : reader["FacililtyGrLimit"].ToString();
                        data.FacililtyGrNote = DBNull.Value == reader["FacililtyGrNote"] ? "" : reader["FacililtyGrNote"].ToString();

                        data.FacililtyProvPermit = DBNull.Value == reader["FacililtyProvPermit"] ? "" : reader["FacililtyProvPermit"].ToString();
                        data.FacililtyProvZoningInd = DBNull.Value == reader["FacililtyProvZoningInd"] ? "--" : reader["FacililtyProvZoningInd"].ToString();
                        data.FacililtyProvZoningCom = DBNull.Value == reader["FacililtyProvZoningCom"] ? "--" : reader["FacililtyProvZoningCom"].ToString();
                        data.FacililtyProvLimit = DBNull.Value == reader["FacililtyProvLimit"] ? "No Limit" : reader["FacililtyProvLimit"].ToString();
                        data.FacililtyProvNote = DBNull.Value == reader["FacililtyProvNote"] ? "" : reader["FacililtyProvNote"].ToString();

                        data.FacililtyProcPermit = DBNull.Value == reader["FacililtyProcPermit"] ? "" : reader["FacililtyProcPermit"].ToString();
                        data.FacililtyProcZoningInd = DBNull.Value == reader["FacililtyProcZoningInd"] ? "--" : reader["FacililtyProcZoningInd"].ToString();
                        data.FacililtyProcZoningCom = DBNull.Value == reader["FacililtyProcZoningCom"] ? "--" : reader["FacililtyProcZoningCom"].ToString();
                        data.FacililtyProcLimit = DBNull.Value == reader["FacililtyProcLimit"] ? "No Limit" : reader["FacililtyProcLimit"].ToString();
                        data.FacililtyProcNote = DBNull.Value == reader["FacililtyProcNote"] ? "" : reader["FacililtyProcNote"].ToString();

                        data.FacililtySCPermit = DBNull.Value == reader["FacililtySCPermit"] ? "" : reader["FacililtySCPermit"].ToString();
                        data.FacililtySCZoningInd = DBNull.Value == reader["FacililtySCZoningInd"] ? "--" : reader["FacililtySCZoningInd"].ToString();
                        data.FacililtySCZoningCom = DBNull.Value == reader["FacililtySCZoningCom"] ? "--" : reader["FacililtySCZoningCom"].ToString();
                        data.FacililtySCLimit = DBNull.Value == reader["FacililtySCLimit"] ? "No Limit" : reader["FacililtySCLimit"].ToString();
                        data.FacililtySCNote = DBNull.Value == reader["FacililtySCNote"] ? "" : reader["FacililtySCNote"].ToString();

                        data.FacililtySTPermit = DBNull.Value == reader["FacililtySTPermit"] ? "" : reader["FacililtySTPermit"].ToString();
                        data.FacililtySTZoningInd = DBNull.Value == reader["FacililtySTZoningInd"] ? "--" : reader["FacililtySTZoningInd"].ToString();
                        data.FacililtySTZoningCom = DBNull.Value == reader["FacililtySTZoningCom"] ? "--" : reader["FacililtySTZoningCom"].ToString();
                        data.FacililtySTLimit = DBNull.Value == reader["FacililtySTLimit"] ? "No Limit" : reader["FacililtySTLimit"].ToString();
                        data.FacililtySTNote = DBNull.Value == reader["FacililtySTNote"] ? "" : reader["FacililtySTNote"].ToString();

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
                Type t = typeof(CityOrdinance);
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                var str = "UPDATE CITY_Ordinance SET ";
                foreach (System.Reflection.PropertyInfo info in properties)
                {
                    if (info.Name == "Municipality" || info.Name == "CityGuid")
                    {
                        continue;
                    }
                    if(info.Name=="ModifyUser")
                    {
                        str += string.Format(" USR_MDFN_TS='{0}', ", data.ModifyUser);
                        continue;
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
    }
}
