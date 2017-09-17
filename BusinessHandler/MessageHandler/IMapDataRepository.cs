using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessHandler.MessageHandler
{
    public interface IMapDataRepository
    {
        List<MapFilterModel> GetFilterData();
        List<MapMunicipalityColor> GetMapAreaData(DocQueryMessage message);
        List<MapMeeting> GetMainDataList(DocQueryMessage message, out int total);
        void UpdateMapColor(int cityId, string color);
    }

    public class SqlServerMapDataRepository:IMapDataRepository
    {
        public List<MapFilterModel> GetFilterData()
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
    }
}
