
using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace BusinessLogic
{
    public class CSVFileHelper : IDataFileHelper
    {
        public List<DocData> OpenDoc(string filePath)
        {
            var resultList = new List<DocData>();
            string wholeText = File.ReadAllText(filePath, Encoding.UTF8);
            string[] records = wholeText.Split(new string[] { "\",\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string strLine in records)
            {

                var aryLine = strLine.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                var data = new DocData();
                data.CityName = string.IsNullOrWhiteSpace(aryLine[0]) ? "" : aryLine[0].Trim('"');
                data.DocId = string.IsNullOrWhiteSpace(aryLine[1]) ? "" : aryLine[1].Trim('"');
                data.LocalPath = string.IsNullOrWhiteSpace(aryLine[2]) ? "" : aryLine[2].Trim('"');
                data.DocType = string.IsNullOrWhiteSpace(aryLine[3]) ? "" : aryLine[3].Trim('"');
                data.DocUrl = string.IsNullOrWhiteSpace(aryLine[4]) ? "" : aryLine[4].Trim('"');
                data.CanBeRead = string.IsNullOrWhiteSpace(aryLine[5]) ? "" : aryLine[5].Trim('"');
                if (aryLine.Length > 6)
                {
                    data.IsViewed = string.IsNullOrWhiteSpace(aryLine[6]) ? "No" : aryLine[6].Trim('"');
                }
                else
                {
                    data.IsViewed = "No";
                }
                data.DocFilePath = filePath;
                resultList.Add(data);
            }

            return resultList;
        }

        public List<QueryData> OpenQuery(string filePath)
        {
            HandQueryGuid(filePath);
            var resultList = new List<QueryData>();
            string wholeText = File.ReadAllText(filePath);
            string[] records = wholeText.Split(new string[] { "\",\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string strLine in records)
            {
                if (string.IsNullOrEmpty(strLine.Trim('\r', '\n')))
                {
                    continue;
                }

                var aryLine = strLine.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                var data = new QueryData();

                data.QueryGuid = string.IsNullOrWhiteSpace(aryLine[0]) ? "" : aryLine[0].Trim('"');
                data.CityName = string.IsNullOrWhiteSpace(aryLine[1]) ? "" : aryLine[1].Trim('"');
                data.DocId = string.IsNullOrWhiteSpace(aryLine[2]) ? "" : aryLine[2].Trim('"');
                data.MeetingTitle = string.IsNullOrWhiteSpace(aryLine[3]) ? "" : aryLine[3].Trim('"');
                var dt = DateTime.MinValue;
                if (DateTime.TryParse(string.IsNullOrWhiteSpace(aryLine[4]) ? "" : aryLine[4].Trim('"'), out dt))
                {
                    data.MeetingDate = dt;
                    data.MeetingDateDisplay = data.MeetingDate.ToString("yyyy-MM-dd");
                }

                data.MeetingLocation = string.IsNullOrWhiteSpace(aryLine[5]) ? "" : aryLine[5].Trim('"');

                data.KeyWord = string.IsNullOrWhiteSpace(aryLine[6]) ? "" : aryLine[6].Trim('"');
                data.PageNumber = string.IsNullOrWhiteSpace(aryLine[7]) ? "" : aryLine[7].Trim('"');
                data.Content = string.IsNullOrWhiteSpace(aryLine[8]) ? "" : aryLine[8].Trim('"');

                data.Comment = aryLine.Length < 10 ? "" : aryLine[9].Trim('"');
                data.QueryFilePath = filePath;
                resultList.Add(data);
            }

            return resultList;
        }

        public void HandQueryGuid(string filePath)
        {
            List<String> lines = new List<String>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string strLine = "";
                string[] aryLine = null;
                while ((strLine = sr.ReadLine()) != null)
                {
                    aryLine = strLine.Split(',');
                    var guid = string.IsNullOrWhiteSpace(aryLine[0]) ? "" : aryLine[0].Trim('"');
                    if (IsGuidByParse(guid))
                    {
                        break;
                    }
                    else
                    {
                        guid = Guid.NewGuid().ToString();
                        strLine = '"' + guid + '"' + ',' + strLine;
                    }
                    lines.Add(strLine);
                }
            }
            if (lines.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }
        }

        public void UpdateQuery(DocQueryResultModel message)
        {
            List<String> lines = new List<String>();
            using (StreamReader sr = new StreamReader(message.QueryFilePath))
            {
                string strLine = "";
                string[] aryLine = null;
                while ((strLine = sr.ReadLine()) != null)
                {
                    string line = string.Empty;
                    aryLine = strLine.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                    if (aryLine[2].Trim('"').Equals(message.DocId) && aryLine[0].Trim('"').Equals(message.QueryGuid))
                    {
                        aryLine[0] = aryLine[0].TrimStart('"');
                        aryLine[9] = message.Comment;
                        line = String.Join("\",\"", aryLine);
                        line = "\"" + line + "\",";
                        lines.Add(line);
                    }
                    else
                    {
                        lines.Add(strLine);
                    }

                }
            }
            if (lines.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(message.QueryFilePath, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }
        }

        public void UpdateDocStauts(DocQueryResultModel message)
        {
            List<String> lines = new List<String>();
            using (StreamReader sr = new StreamReader(message.DocFilePath))
            {
                string strLine = "";
                string[] aryLine = null;
                while ((strLine = sr.ReadLine()) != null)
                {
                    string line = string.Empty;
                    aryLine = strLine.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                    if (aryLine[1] == message.DocId)
                    {
                        aryLine[0] = aryLine[0].TrimStart('"');
                        aryLine[5] = aryLine[5].Replace("\",", "");
                        List<string> strList = aryLine.ToList();
                        if (aryLine.Length == 6)
                        {
                            strList.Add("Yes");
                        }
                        line = String.Join("\",\"", strList);
                        line = "\"" + line + "\",";
                        lines.Add(line);
                    }
                    else
                    {
                        lines.Add(strLine);

                    }


                }
            }
            if (lines.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(message.DocFilePath, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }
        }

        public bool IsGuidByParse(string strSrc)

        {

            Guid g = Guid.Empty;

            return Guid.TryParse(strSrc, out g);

        }
        public void UpdateDocImportant(DocQueryResultModel message)
        { }
    }
}
