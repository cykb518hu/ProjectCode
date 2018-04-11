﻿using BusinessHandler.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessHandler.MessageHandler
{
    public static class DocQueryDB
    {
      



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
            if (!string.IsNullOrEmpty(message.QueryGuid))
            {
                string queryString = @"UPDATE QUERY_ENTRY SET COMMENT='" + message.Comment + "'  WHERE ENTRY_GUID='" + message.QueryGuid + "'";
                using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                string queryString = @"UPDATE DOCUMENT SET COMMENT='" + message.Comment + "' , USR_MDFN_TS='" + DateTime.Now.ToString("yyyy-MM-dd") + "' WHERE DOC_GUID='" + message.DocId + "'";
                using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }




    }
}
