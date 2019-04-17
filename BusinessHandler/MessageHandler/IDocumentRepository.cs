using BusinessHandler.Model;
using System.Data.SqlClient;

namespace BusinessHandler.MessageHandler
{
    public interface IDocumentRepository
    {
        void UpdateDocumentMeetingDate(string docGuid, string MeetingDateStr);
    }
    public class DocumentRepository : IDocumentRepository
    {
        public void UpdateDocumentMeetingDate(string docGuid, string MeetingDateStr)
        {
            var queryString = string.Format("UPDATE DOCUMENT SET MEETING_DATE= @MEETING_DATE WHERE DOC_GUID='{0}'", docGuid);
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@MEETING_DATE", MeetingDateStr);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
