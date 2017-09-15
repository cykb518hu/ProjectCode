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
        bool UpdateMeetingNotes(List<MeetingNote> notes);
        List<MapMeetingNote> GetAllDataList(DocQueryMessage message, out int total);
        List<MapMeetingNote> GetMapPopUpInfo(int cityId);
    }

    public class SqlServerMeetingNote : IMeetingNote
    {

        public List<MeetingNote> GetMeetingNotes(string docGuid, string note)
        {
            var list = new List<MeetingNote>();
            string queryString = "";
            if (string.IsNullOrEmpty(note))
            {
                queryString = @"select  * from [dbo].[MeetingNote] where doc_guid in('" + docGuid + "') order by usr_mdfn_ts desc";
            }
            else
            {
                queryString = @"select  * from [dbo].[MeetingNote] where doc_guid in('" + docGuid + "') and notes like '%" + note + "%' order by usr_mdfn_ts desc";

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
                    data.NoteEdit = @"<a href='#' data-guid='" + data.Guid + "' data-docId='" + data.DocGuid + "' onclick='editMeetingNotes(this); return false' style='white-space:pre'>" + reader["Notes"].ToString() + "</a>";
                    data.CreateDate = DBNull.Value == reader["USR_CRTN_TS"] ? "" : Convert.ToDateTime(reader["USR_CRTN_TS"]).ToString("yyyy-MM-dd");
                    data.ModifyDate = DBNull.Value == reader["USR_MDFN_TS"] ? "" : Convert.ToDateTime(reader["USR_MDFN_TS"]).ToString("yyyy-MM-dd");           
                    data.Operation += @"<button type='button' class='btn btn-default glyphicon glyphicon-remove' title='Remove Notes'   data-guid='" + data.Guid + "' data-docId='" + data.DocGuid + "'  onclick='removeMeetingNotes(this); return false'></button>";

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
                if (!string.IsNullOrWhiteSpace(message.Note))
                {
                    command.Parameters.AddWithValue("@Notes", message.Note);
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
                        orderBy = "DEPLOYE_DATE desc, CITY_NM ";
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
                    r.NoteList = noteList.Where(x => x.DocGuid == r.DocGuid).ToList();
                }
            }

        }

        public bool UpdateMeetingNotes(List<MeetingNote> notes)
        {
            if (notes.Any())
            {
                foreach (var r in notes)
                {

                    string queryString = string.Empty;
                    switch (r.Status)
                    {
                        case "Added":
                            queryString = "INSERT INTO MeetingNote([GUID],[Doc_Guid],Notes) values('" + r.Guid + "','" + r.DocGuid + "',@note)";
                            break;
                        case "Deleted":
                            queryString = "DELETE FROM MeetingNote WHERE [GUID]='" + r.Guid + "'";
                            break;
                        case "Modified":
                            queryString = "UPDATE MeetingNote SET Notes= @note , USR_MDFN_TS='" + DateTime.Now + "' WHERE [Guid]='" + r.Guid + "'";
                            break;
                    }
                    using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);
                        if (r.Status == "Added"|| r.Status == "Modified")
                        {
                            command.Parameters.AddWithValue("@note", r.Note);
                        }
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }


        public List<MapMeetingNote> GetMapPopUpInfo(int cityId)
        {
            var list = new List<MapMeetingNote>();
            var queryString = @"SELECT C.LONG_NM,c.color, Q.MEETING_DATE,D.DOC_TYPE, D.DOC_GUID FROM DBO.CITY C INNER JOIN DBO.DOCUMENT D ON C.CITY_NM=D.CITY_NM
INNER JOIN DBO.QUERY Q ON Q.DOC_GUID=D.DOC_GUID
WHERE C.objectid = " + cityId + "  order by Q.MEETING_DATE desc";

            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var result = new MapMeetingNote();
                    result.CityName= reader["LONG_NM"].ToString();
                    result.Color = DBNull.Value == reader["color"] ? "" : reader["color"].ToString();
                    result.DocGuid = reader["DOC_GUID"].ToString();
                    result.DocType = reader["DOC_TYPE"].ToString();
                    result.MeetingDate = DBNull.Value == reader["MEETING_DATE"] ? "" : Convert.ToDateTime(reader["MEETING_DATE"]).ToString("yyyy-MM-dd");
                    list.Add(result);
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
            return list;
        }
    }
}
