using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace sqlmake
{
    public enum emNoteLogger
    {
        nomal,
        script,
        data,
        sql,
        fatal,
        warning,
        the_end
    }

    internal class Logger
    {
        private static Logger m_in;

        public static Logger Instance()
        {
            if (m_in == null)
            {
                m_in = new Logger();
            }

            return m_in;
        }

        private string namesection = "default";
        public CNote[] m_vecNote = new CNote[6];
        private int LogLevel = 1;
        public void Init(string str)
        {
            namesection = str;

            if (!Directory.Exists("./log"))
            {
                Directory.CreateDirectory("./log");
            }

            string path = $"./log/{namesection}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void Debug(string msg)
        {
            if (LogLevel == 1)
            {
                Log(emNoteLogger.nomal, msg);
            }
        }

        public void Log(emNoteLogger emlogger, string log)
        {
            lock (m_vecNote[(int) emlogger].m_listString)
            {
                m_vecNote[(int) emlogger].m_listString.Add(log);
            }
        }

        public void Process()
        {
            while (true)
            {
                try
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var list = new List<string>();
                        lock (m_vecNote[i].m_listString)
                        {
                            foreach (var v2 in m_vecNote[i].m_listString)
                            {
                                list.Add(v2);
                            }

                            m_vecNote[i].m_listString.Clear();
                        }

                        if (list.Count == 0) continue;

                        var dt = DateTime.Now;
                        string str = string.Format("./log/{0}/{0}{1}-{2}-{3}-{4}.LOG", namesection, m_vecNote[i].Name, dt.Year, dt.Month, dt.Day);
                        var fileInfo = new FileInfo(str);
                        if (fileInfo.Exists && fileInfo.Length > 10485760)
                        {
                            DateTime ds = new DateTime(1970, 1, 1, 0, 0, 0);
                            string msg2 = string.Format("./log/{0}/{0}{1}.{2}.LOG", namesection, m_vecNote[i].Name, ((int) (dt - ds).TotalSeconds).ToString());
                            fileInfo.MoveTo(msg2);
                            Thread.Sleep(1000);
                        }

                        using (FileStream fsWrite = new FileStream(str, FileMode.Append))
                        {
                            using (StreamWriter sw = new StreamWriter(fsWrite))
                            {
                                foreach (var v in list)
                                {
                                    string msg = $"{dt.Year}-{dt.Month / 10}{dt.Month % 10}-{dt.Day / 10}{dt.Day % 10} {dt.Hour / 10}{dt.Hour % 10}:{dt.Minute / 10}{dt.Minute % 10}:{dt.Second / 10}{dt.Second % 10} {v}";
                                    sw.WriteLine(msg);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}