using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.MessageHandler
{
    public interface IMeetingNote
    {
        List<MeetingNote> GetMeetingNotes(string docGuid, string note);
        void UpdateMeetingNotes(List<MeetingNote> notes);
        List<MapMeetingNote> GetAllDataList(DocQueryMessage message, out int total);
        MapMeetingCity GetMapPopUpInfo(string cityGuid);
        int GetMeetingRelatedNotesAmount(string guid);
        List<MeetingCalendar> GetMeetingCalendar(DocQueryMessage message);
        List<MeetingTypeTime> GetMeetingType(string guid);
    }

    public class SqlServerMeetingNote : IMeetingNote
    {

        public List<MeetingNote> GetMeetingNotes(string docGuid, string note)
        {
            var list = new List<MeetingNote>();
            string queryString = "";
            if (string.IsNullOrEmpty(note))
            {
                queryString = @"select  * from [dbo].[MeetingNote] where doc_guid in('" + docGuid + "') order by usr_crtn_ts asc";
            }
            else
            {
                queryString = @"select  * from [dbo].[MeetingNote] where doc_guid in('" + docGuid + "') and notes like '%" + note + "%' order by usr_crtn_ts desc";

            }
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MeetingNote();
                    data.Guid = DBNull.Value == reader["Guid"] ? "" : reader["Guid"].ToString();
                    data.DocGuid = DBNull.Value == reader["Doc_Guid"] ? "" : reader["Doc_Guid"].ToString();
                    data.Note = reader["Notes"].ToString();
                    data.NoteEdit = @"<a href='#' data-guid='" + data.Guid + "' data-docId='" + data.DocGuid + "' onclick='editMeetingNotes(this); return false' style='white-space:pre-wrap'>" + reader["Notes"].ToString() + "</a>";
                    data.CreateDate = DBNull.Value == reader["USR_CRTN_TS"] ? "" : Convert.ToDateTime(reader["USR_CRTN_TS"]).ToString("yyyy-MM-dd");
                    data.ModifyDate = DBNull.Value == reader["USR_MDFN_TS"] ? "" : Convert.ToDateTime(reader["USR_MDFN_TS"]).ToString("yyyy-MM-dd");
                    data.ModifyUser= DBNull.Value == reader["USR_MDFN_ID"] ? "" : reader["USR_MDFN_ID"].ToString();
                    data.Operation += @"<button type='button' class='btn btn-default glyphicon glyphicon-remove' title='Remove Notes'   data-guid='" + data.Guid + "' data-docId='" + data.DocGuid + "'  onclick='removeMeetingNotes(this); return false'></button>";
                    data.Tags = DBNull.Value == reader["Tags"] ? "" : reader["Tags"].ToString();
                    list.Add(data);
                }
            }
            return list;
        }
      

        public List<MapMeetingNote> GetAllDataList(DocQueryMessage message, out int total)
        {
            var list = new List<MapMeetingNote>();

            string queryString = @"[dbo].[GET_DOC_MeetingNote]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrWhiteSpace(message.CityName) && !message.CityName.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
                {
                    command.Parameters.AddWithValue("@CityName", StaticSetting.GetArrayQuery(message.CityName));
                }
                if (!string.IsNullOrWhiteSpace(message.CountyName) && !message.CountyName.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
                {
                    command.Parameters.AddWithValue("@CountyName", StaticSetting.GetArrayQuery(message.CountyName));
                }
                //no keyword in screen now
                if (!string.IsNullOrWhiteSpace(message.KeyWord) && !message.KeyWord.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
                {
                    command.Parameters.AddWithValue("@KeyWord", StaticSetting.GetArrayQuery(message.KeyWord));
                }
                if (!string.IsNullOrWhiteSpace(message.StartMeetingDate))
                {
                    command.Parameters.AddWithValue("@StartNoteDate", message.StartMeetingDate);
                }
                if (!string.IsNullOrWhiteSpace(message.EndMeetingDate))
                {
                    command.Parameters.AddWithValue("@EndNoteDate", message.EndMeetingDate);
                }
                if (!string.IsNullOrWhiteSpace(message.DeployDate) && !message.DeployDate.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
                {
                    command.Parameters.AddWithValue("@DeployeDate", StaticSetting.GetArrayQuery(message.DeployDate));
                }
                if (!string.IsNullOrWhiteSpace(message.Note))
                {
                    command.Parameters.AddWithValue("@Notes", message.Note);
                }
                var email = StaticSetting.GetUserEmail();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    command.Parameters.AddWithValue("@UserEmail", email);
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
                        orderBy = "noteorder,MEETING_DATE desc, CITY_NM ";
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
                        var result = new MapMeetingNote();
                        result.DocGuid = reader["DOC_GUID"].ToString();
                        result.DocType = reader["DOC_TYPE"].ToString();
                        result.CityName = reader["CITY_NM"].ToString();

                        result.MeetingDate = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                        result.ScrapeDate = DBNull.Value == reader["SEARCH_DATE"] ? "" : Convert.ToDateTime(reader["SEARCH_DATE"]).ToString("yyyy-MM-dd");
                        result.DeployDate = DBNull.Value == reader["DEPLOYE_DATE"] ? "" : Convert.ToDateTime(reader["DEPLOYE_DATE"]).ToString("yyyy-MM-dd");

                        result.Operation += @"<button type='button' class='btn btn-default glyphicon glyphicon-plus' title='Maintain Notes'   data-docid='" + result.DocGuid + "'  onclick='addMeetingNotes(this); return false'></button>";

                        list.Add(result);
                    }
                }
            }
            GetSubList(list, message.Note);
            return list;
        }

        public void GetSubList(List<MapMeetingNote> list, string note)
        {
            var guidStr = "";
            foreach (var r in list)
            {
                guidStr += "'" + r.DocGuid + "',";

            }
            if (!string.IsNullOrEmpty(guidStr))
            {
                guidStr = guidStr.TrimStart('\'');
                guidStr = guidStr.TrimEnd(',');
                guidStr = guidStr.TrimEnd('\'');
                var noteList = GetMeetingNotes(guidStr, note);
                foreach(var r in list)
                {
                    var subList= noteList.Where(x => x.DocGuid == r.DocGuid).ToList();
                    if (subList.Any() && !string.IsNullOrEmpty(r.Operation))
                    {
                        r.Operation = r.Operation.Replace("btn-default", "btn-success");

                    }
                    r.NoteList = subList;

                }
            }

        }

        public void UpdateMeetingNotes(List<MeetingNote> notes)
        {
            if (notes.Any())
            {
                foreach (var r in notes)
                {
                    if (string.IsNullOrWhiteSpace(r.Note))
                    {
                        r.Status = "Deleted";
                    }
                    string queryString = string.Empty;
                    switch (r.Status)
                    {
                        case "Added":
                            queryString = "INSERT INTO MeetingNote([GUID],[Doc_Guid],Notes,Tags, USR_CRTN_ID,USR_MDFN_ID,FutureDate) values(@GUID,'" + r.DocGuid + "',@note,@Tags, @modifyUser,@modifyUser,@FutureDate)";
                            break;
                        case "Deleted":
                            queryString = "DELETE FROM MeetingNote WHERE [GUID]=@GUID";
                            break;
                        case "Modified":
                            queryString = "UPDATE MeetingNote SET Notes= @note , USR_MDFN_TS= '" + DateTime.Now + "' ,Tags=@Tags, USR_MDFN_ID=@modifyUser, FutureDate=@FutureDate WHERE [Guid]=@GUID";
                            break;
                    }
                    using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@GUID", r.Guid);
                        if (r.Status == "Added" || r.Status == "Modified")
                        {
                            var futureDate = "";
                            if (!string.IsNullOrWhiteSpace(r.Note))
                            {
                                var startIndex = r.Note.IndexOf('(');
                                var endIndex = r.Note.IndexOf(')');
                                if (endIndex > startIndex && startIndex > -1)
                                {
                                    futureDate = r.Note.Substring(startIndex + 1, endIndex - startIndex - 1);
                                }
                            }
                            command.Parameters.AddWithValue("@note", r.Note);
                            command.Parameters.AddWithValue("@modifyUser", r.ModifyUser);
                            command.Parameters.AddWithValue("@Tags", string.IsNullOrWhiteSpace(r.Tags) ? "" : r.Tags);
                            command.Parameters.AddWithValue("@FutureDate", futureDate);
                        }
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public MapMeetingCity GetMapPopUpInfo(string cityGuid)
        {
            var result = new MapMeetingCity();
            var list = new List<MapMeetingNote>();
            var queryString = @"SELECT C.LONG_NM,c.color,c.CITY_NM,  Q.MEETING_DATE,D.DOC_TYPE, D.DOC_GUID,D.IMPORTANT FROM DBO.CITY C INNER JOIN DBO.DOCUMENT D ON C.CITY_NM=D.CITY_NM
INNER JOIN DBO.QUERY Q ON Q.DOC_GUID=D.DOC_GUID
WHERE C.guid = '" + cityGuid + "'  order by Q.MEETING_DATE desc";

            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new MapMeetingNote();
                    result.CityName= reader["CITY_NM"].ToString();
                    result.Color = DBNull.Value == reader["color"] ? "" : reader["color"].ToString();
                    result.CityLongName= reader["LONG_NM"].ToString();
                    data.DocGuid = reader["DOC_GUID"].ToString();
                    data.DocType = reader["DOC_TYPE"].ToString();
                    data.MeetingDate = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                    data.Removed = DBNull.Value == reader["IMPORTANT"] ? "" : reader["IMPORTANT"].ToString();
                    //important column now is means removed , so true is removed, false is not removed
                    data.Removed = data.Removed == "False" ? "" : "[Removed]";
                    list.Add(data);
                }
            }

            GetSubList(list, "");
            foreach(var r in list)
            {
                if(r.NoteList.Any())
                {
                    foreach(var n in r.NoteList)
                    {
                        n.NoteEdit = "";
                        n.Operation = "";
                        n.CreateDate = "";
                    }
                }
            }
            result.MeetingList = list;
            return result;
        }


        public int GetMeetingRelatedNotesAmount(string guid)
        {
            var amount = 0;
            var queryString = @"SELECT COUNT(*) AMOUNT FROM MeetingNote WHERE DOC_GUID=@GUid
UNION
SELECT COUNT(*) AMOUNT FROM DOCUMENT D INNER JOIN 
QUERY Q ON D.DOC_GUID=Q.DOC_GUID
INNER JOIN DBO.QUERY_ENTRY QE ON QE.QUERY_GUID=Q.QUERY_GUID
WHERE D.DOC_GUID=@GUid
AND QE.COMMENT IS NOT NULL AND QE.COMMENT<>''";

            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@GUid", guid);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    amount += Convert.ToInt32(reader["AMOUNT"]);
                }
            }
            return amount;
        }

        public List<MeetingCalendar> GetMeetingCalendar(DocQueryMessage message)
        {
            var list = new List<MeetingCalendar>();

            string queryString = @"[dbo].[GET_MeetingCalendar_Modify]";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.StoredProcedure;
                //this is different from other two, becuase if all don't need to join document_content table
                if (!string.IsNullOrWhiteSpace(message.KeyWord) && !message.KeyWord.Contains("All"))
                {
                    message.KeyWord = StaticSetting.GetKeyWordForFullSearch(message.KeyWord);
                }
                StaticSetting.BuildParameters(command, message);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new MeetingCalendar();
                        result.DocGuid = reader["DOC_GUID"].ToString();
                        result.DocType = reader["DOC_TYPE"].ToString();
                        result.CityName = reader["CITY_NM"].ToString();
                        result.FutureDate = DBNull.Value == reader["FutureDate"] ? "" : reader["FutureDate"].ToString();
                        result.Note = DBNull.Value == reader["Notes"] ? "" : reader["Notes"].ToString();
                        result.Note = result.CityName + "-" + result.DocType + "-" + result.Note;
                        list.Add(result);

                    }
                }
            }
            return list;
        }

        public List<MeetingTypeTime> GetMeetingType(string guid)
        {
            var list = new List<MeetingTypeTime>();
            var queryString = @"select D.DOC_TYPE , max(Q.MEETING_DATE) MEETING_DATE , max(Q.USR_CRTN_TS) USR_CRTN_TS from DOCUMENT D
INNER JOIN QUERY Q ON D.DOC_GUID=Q.DOC_GUID INNER JOIN CITY C ON C.CITY_NM=D.CITY_NM WHERE C.GUID=@GUID group by D.DOC_TYPE";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@GUID", guid);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new MeetingTypeTime();
                        data.MeetingType = DBNull.Value == reader["DOC_TYPE"] ? "" : reader["DOC_TYPE"].ToString();
                        data.LastMeeting = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                        data.LastScrape = DBNull.Value == reader["USR_CRTN_TS"] ? "" : Convert.ToDateTime(reader["USR_CRTN_TS"]).ToString("yyyy-MM-dd");
                        list.Add(data);
                    }
                }
                return list;
            }
        }
    }
}
