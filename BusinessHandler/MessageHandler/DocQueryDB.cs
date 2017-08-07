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
    public static class DocQueryDB
    {
        public static List<DocQueryResultModel> GetCityDate()
        {
            var list = new List<DocQueryResultModel>();
            string queryString = @"SELECT distinct D.CITY_NM , Q.MEETING_DATE, C.DEPLOYE_DATE FROM  DBO.DOCUMENT D INNER JOIN DBO.QUERY Q ON D.DOC_GUID=Q.DOC_GUID
INNER JOIN DBO.QUERY_ENTRY QE ON QE.QUERY_GUID=Q.QUERY_GUID
LEFT JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new DocQueryResultModel();
                    data.CityName = DBNull.Value == reader["CITY_NM"] ? "" : reader["CITY_NM"].ToString();
                    var dt = DateTime.MinValue;
                    if (DateTime.TryParse(DBNull.Value == reader["MEETING_DATE"] ? "" : reader["MEETING_DATE"].ToString(), out dt))
                    {
                        data.MeetingDateDisplay = dt.ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrWhiteSpace(data.CityName))
                    {
                        list.Add(data);
                    }
                    data.CityDeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");
                }
            }
            return list;
        }

        public static List<DocQueryParentModel> GetAllDataList(DocQueryMessage message, out int total)
        {
            var list = new List<DocQueryParentModel>();
            string queryString = @"[dbo].[GET_DOC_QUERY]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrWhiteSpace(message.CityName) && !message.CityName.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    command.Parameters.AddWithValue("@CityName", GetArrayQuery(message.CityName));
                }
                if (!string.IsNullOrWhiteSpace(message.KeyWord) && !message.KeyWord.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    command.Parameters.AddWithValue("@KeyWord", GetArrayQuery(message.KeyWord));
                }
                if (!string.IsNullOrWhiteSpace(message.MeetingDate))
                {
                    command.Parameters.AddWithValue("@MeetingDate", message.MeetingDate);
                }
                if (!string.IsNullOrWhiteSpace(message.DeployDate))
                {
                    command.Parameters.AddWithValue("@DeployeDate", message.DeployDate);
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
                connection.Open();

                total = Convert.ToInt32(command.ExecuteScalar());

                var orderBy = "";
                switch (message.sortName)
                {
                    case "DocType":
                        orderBy = "DOC_TYPE";
                        break;
                    case "CityNameDispaly":
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
                    case "IsViewed":
                        orderBy = "CHECKED";
                        break;
                    case "ImportantDisplay":
                        orderBy = "IMPORTANT";
                        break;
                    default:
                        orderBy = "CITY_NM";
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
                        var result = new DocQueryParentModel();
                        result.DocId = reader["DOC_GUID"].ToString();
                        result.DocUrl = @"<a href='" + reader["DOC_SOURCE"].ToString() + "' target='_blank'>Download File</a>";
                        result.DocType = reader["DOC_TYPE"].ToString();
                        result.CityNameDispaly = "<span class='showDatePicker' onclick='showDatePicker(this); return false' style='cursor: pointer'>" + reader["CITY_NM"].ToString() + "</span>";

                        result.MeetingDateDisplay = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                        result.ScrapeDate = DBNull.Value == reader["SEARCH_DATE"] ? "" : Convert.ToDateTime(reader["SEARCH_DATE"]).ToString("yyyy-MM-dd");
                        result.CityDeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");

                        result.IsViewed = "<span class='sp_" + result.DocId + "'>" + (reader["CHECKED"].ToString().Equals("True") ? "Yes" : "No") + "</span>";
                        var important = reader["IMPORTANT"].ToString().Equals("True") ? "Yes" : "No";
                        var checkStr = important.Equals("Yes") ? "checked" : "";
                        result.ImportantDisplay = @"<input type='checkbox'  onclick='RemoveData(this);'   data-file='" + important + "' data-docid='" + result.DocId + "' " + checkStr + " />";

                        list.Add(result);

                    }
                }
            }
            GetSubList(list);
            return list;
        }

        public static void GetSubList(List<DocQueryParentModel> list)
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
            string queryString = @"[dbo].[GET_DOC_QUERY_SUBLIST]";
            var resultList = new List<DocQueryResultModel>();
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
                        var result = new DocQueryResultModel();
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
                        result.Operation = @"<button type='button' class='btn btn-default glyphicon glyphicon-edit' aria-label='Left Align' data-file='' data-docid='" + result.DocId + "' data-queryguid='" + result.QueryGuid + "' onclick='OpenDataDetail(this); return false'></button>";
                        resultList.Add(result);
                    }
                }
            }
            foreach (var r in list)
            {
                var subList = resultList.Where(x => x.DocId == r.DocId).ToList();
                int count = 0;
                subList.ForEach(x => { count += Regex.Matches(x.Content, x.KeyWord, RegexOptions.IgnoreCase).Count; });
                r.Number = count;
                r.DocQuerySubList = subList;
            }
        }
        public static string GetArrayQuery(string arrayStr)
        {
            var arr = arrayStr.Split(',');
            var query = string.Empty;
            foreach (var r in arr)
            {
                if (!string.IsNullOrWhiteSpace(r))
                {
                    if (r.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                    query += "'" + r + "',";
                }
            }
            query = query.TrimEnd(',');
            return query;
        }

        public static void UpdateDocStatus(DocQueryResultModel message)
        {
            string queryString = @"UPDATE DOCUMENT SET CHECKED='True' WHERE DOC_GUID='" + message.DocId + "'";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void UpdateDocImportant(DocQueryResultModel message)
        {
            string queryString = @"UPDATE DOCUMENT SET IMPORTANT='" + message.Important + "'  WHERE DOC_GUID='" + message.DocId + "'";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void UpdateQueryComment(DocQueryResultModel message)
        {
            string queryString = @"UPDATE QUERY_ENTRY SET COMMENT='" + message.Comment + "'  WHERE ENTRY_GUID='" + message.QueryGuid + "'";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }


    }
}
