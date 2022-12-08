using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using ExcelDataReader;

namespace SqlMaker
{
    internal class CLoader
    {
        public static readonly string cfgRootPath = "../";

        public bool LoadFromExcel()
        {
            Console.WriteLine("LoadFromExcel start!");
            var ticks = DateTime.Now.Ticks;
            string[] strFiles = Directory.GetFiles($"{cfgRootPath}/excel/", "*.xlsx");
            LoadFlag[] lf = new LoadFlag[strFiles.Length];
            int c = 0;
            foreach (var strPath in strFiles)
            {
                var i = new LoadFlag();
                lf[c++] = i;
                var idxstart = strPath.LastIndexOf("/", StringComparison.Ordinal);
                var idxend = strPath.LastIndexOf(".", StringComparison.Ordinal);
                var name = strPath.Substring(idxstart + 1, idxend - idxstart - 1);
                if (name == "UILanguage" || name == "Language")
                {
                    i.state = 2;
                }
                else if (!LoadExcel(strPath, false, i))
                {
                    Console.WriteLine($"import xml : {strPath} failed");
                    return false;
                }
            }

            try
            {
                c = 0;
                while (true)
                {
                    int ct;
                    switch (CheckLoadFlag(lf, out ct))
                    {
                        case 1: return false;
                        case 0:
                            if (ct > c)
                            {
                                c = ct;
                                Console.WriteLine("loaded {0}", ct);
                            }

                            Thread.Sleep(100);
                            continue;
                        default:
                            if (ct > c)
                            {
                                c = ct;
                                Console.WriteLine("loaded {0}", ct);
                            }

                            break;
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            Console.WriteLine("LoadFromExcel end! {0}", (DateTime.Now.Ticks - ticks) / 10000000);
            return true;
        }


        private int CheckLoadFlag(LoadFlag[] vlf, out int ct)
        {
            ct = 0;
            foreach (var lf in vlf)
            {
                ct++;
                lock (lf)
                {
                    if (lf.state == 0)
                    {
                        return 0;
                    }

                    if (lf.state == 1)
                    {
                        return 1;
                    }
                }
            }

            return 2;
        }


        public static bool LoadExcel(string lpFilePath, bool excesql, LoadFlag lf)
        {
            FileStream stream = File.Open(lpFilePath, FileMode.Open, FileAccess.Read);
            ExcelReaderConfiguration cfg = new ExcelReaderConfiguration {FallbackEncoding = Encoding.UTF8};
            var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream,cfg);
            if (excelReader.Name == "Language")
            {
                return true;
            }

            DataSet result = excelReader.AsDataSet();
            ThreadLoadFile tlf = new ThreadLoadFile(excelReader.Name.ToLower(), result, lf);
            excelReader.Close();
            stream.Close();
            if (tlf.m_lf.state == 1) return false;
            return true;
        }
    }
}