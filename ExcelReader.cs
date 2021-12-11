using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LabWork2._1
{
    public class ExcelReader
    {
        private const string defaultLink = @"https://bdu.fstec.ru/files/documents/thrlist.xlsx";
        private static string filePath = Directory.GetCurrentDirectory() + @"/thrlist.xlsx";
        public static DataSet TableContentSet;
        public static List<Threat.FullThreatInfo> fullThreatInfo;
        public static List<Threat.FullThreatInfo> chfullThreatInfo;
        public static Threat[] threat;
        public static Threat[] chthreat;
        public static Threat[] newthreat;
        public static string[] headers;
        public static bool NeedsUpdate;
        public static DataSet OpenExcelTable(string newpath)
        {
            string filePath = ExcelReader.filePath;
            if (!File.Exists(filePath))
            {
                LoadFromWeb();
            }
            if (!String.IsNullOrEmpty(newpath))
            {
                filePath = newpath;
            }
            using (var stream = File.OpenRead(filePath))
            {

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    TableContentSet = reader.AsDataSet();
                }
            }
            return TableContentSet;
        }
        public static void LoadFromWeb()
        {
            File.Delete(filePath);
            new WebClient().DownloadFile(defaultLink, filePath);
        }
        public static bool SaveAs(string path)
        {
            if (path == filePath)
            {
                return false;
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Copy(filePath, path);
            return true;
        }
        public static void CheckOnStart()
        {
            if (!File.Exists(filePath))
            {
                MainWindow.MessageBoxOnStart();
            }
            else
            {
                MainWindow.MessageBoxFileFound();
            }
        }
        public static bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return true;
            }
            catch (Exception)
            {
                return false; ;
            }
        }
        public static void LoadLocal(string path)
        {
            if (path == filePath)
            {
                return;
            }
            File.Delete(filePath);
            File.Copy(path, filePath);
        }
        public static void TakeHeaders(DataSet data, ref string[] headers)
        {
            headers = data.Tables[0].Rows[1].ItemArray.Select(s => s.ToString()).ToArray();
        }
        public static Threat.ShortThreatInfo[] GetShortContent(Threat[] threat)
        {
            Threat[] data = threat;
            /*if (newthreat != null)
            {
                data = newthreat;
            } */
            if (data == null)
            {
                throw new InvalidOperationException();
            }
            Threat.ShortThreatInfo[] result = new Threat.ShortThreatInfo[data.Length];
            for (int i = 0; i < data.Length; ++i)
            {
                result[i] = new Threat.ShortThreatInfo("УБИ." + data[i].info[0].PadLeft(3, '0'), data[i].info[1]);
            }
            return result;
        }
        /*public static Threat.ShortThreatInfo[] GetChShortContent()
        {
            Threat[] data = chthreat;
            if (data == null)
            {
                throw new InvalidOperationException();
            }
            Threat.ShortThreatInfo[] result = new Threat.ShortThreatInfo[data.Length];
            for (int i = 0; i < data.Length; ++i)
            {
                result[i] = new Threat.ShortThreatInfo("УБИ." + data[i].info[0].PadLeft(3, '0'), data[i].info[1]);
            }
            return result;
        } */
        public static void GetFullContent()
        {
            Threat[] data = threat;
            if (data == null)
            {
                throw new InvalidOperationException();
            }
            List<Threat.FullThreatInfo> result = new List<Threat.FullThreatInfo>();
            List<Tuple<string, string>> resulttemp = new List<Tuple<string, string>>();
            foreach (var threat in data)
            {
                resulttemp.Clear();
                for (int i = 0; i < threat.info.Length; i++)
                {
                    resulttemp.Add(Tuple.Create(headers[i], threat.info[i]));
                }
                result.Add(new Threat.FullThreatInfo(resulttemp.ToArray()));
            }
            fullThreatInfo = result;
        }
        public static void GetChFullContent()
        {
            Threat[] data = chthreat;
            if (data == null)
            {
                throw new InvalidOperationException();
            }
            List<Threat.FullThreatInfo> result = new List<Threat.FullThreatInfo>();
            List<Tuple<string, string>> resulttemp = new List<Tuple<string, string>>();
            foreach (var threat in data)
            {
                resulttemp.Clear();
                for (int i = 0; i < threat.info.Length; i++)
                {
                    resulttemp.Add(Tuple.Create(headers[i], threat.info[i]));
                }
                result.Add(new Threat.FullThreatInfo(resulttemp.ToArray()));
            }
            chfullThreatInfo = result;
        }
        /*public static List<Threat.FullThreatInfo> GetFullContentById(int id)
        {
           
        }*/
        public static void ReadAllLines(DataSet source, ref Threat[] threat)
        {
            threat = null;
            int count = source.Tables[0].Rows.Count;
            const int index = 2;
            threat = new Threat[count - index];
            for (int i = index; i < count; ++i)
            {
                threat[i - index] = new Threat(source.Tables[0].Rows[i].ItemArray.Select(s => s.ToString()).ToArray());
                for (int j = 5; j < 8 && j < threat[i - index].info.Length; ++j)
                {
                    threat[i - index].info[j] = threat[i - index].info[j] == "1" ? "Да" : "Нет";
                }
            }
        }
        public static Threat[] GetChangedThreats(Threat[] newer, Threat[] older)
        {
            List<Threat> result = new List<Threat>();
            int newLength = Math.Min(newer.Length, older.Length);
            for (int i = 0; i < newLength; ++i)
            {
                if (!Enumerable.SequenceEqual(newer[i].info, older[i].info))
                {
                    result.Add(new Threat(newer[i].info.Length));
                    for (int j = 0; j < newer[i].info.Length; ++j)
                    {
                        if (newer[i].info[j] == older[i].info[j])
                        {
                            result.Last().info[j] = newer[i].info[j];
                        }
                        else
                        {
                            result.Last().info[j] = "БЫЛО:\n" + older[i].info[j] + "\n\nСТАЛО:\n" + newer[i].info[j];
                        }
                    }
                }
            }
            {
                int diff = newer.Length - older.Length;
                Threat[] temp;
                if (diff != 0)
                {
                    string info = "";
                    if (diff > 0)
                    {
                        temp = newer;
                        info = "#Строка добавлена при обновлении\n\n";
                    }
                    else
                    {
                        temp = older;
                        info = "#Строка была удалена при обновлении\n\n";
                    }
                    for (int i = newLength; i < temp.Length; ++i)
                    {
                        result.Add(new Threat((string[])temp[i].info.Clone()));
                        result.Last().info[1] = info + result.Last().info[1];
                    }
                }
            }
            return result.ToArray();
        }
    }
}
