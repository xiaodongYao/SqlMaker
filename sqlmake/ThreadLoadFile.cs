using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace sqlmake
{
    public class ThreadLoadFile
    {
        private static int STRING_MAX_SIZE = 2048;
        public LoadFlag m_lf;
        private DataSet result;

        private int Define_Property_Type = 1;
        private int Define_Property_Row = 2;
        private int Define_Property_Col;

        private long ticks;

        public ThreadLoadFile(string name, DataSet dt, LoadFlag lf)
        {
            Console.WriteLine("LoadFromExcel start!" + name);
            ticks = DateTime.Now.Ticks;
            result = dt;
            m_lf = lf;
            CheckMsg();
        }

        private bool IsString(string clname, Dictionary<string, int> dic)
        {
            if (dic.TryGetValue(clname, out var r))
            {
                if (r == 1)
                {
                    return true;
                }

                return false;
            }

            return false;
        }


        private void WriteHeader(DataTable sheet, out List<int> lstColumns, out Dictionary<string, int> dictype, int col, BinaryWriter sw)
        {
            lstColumns = new List<int>();
            dictype = new Dictionary<string, int>();
            for (int c = Define_Property_Col; c < col; c++)
            {
                string columnname = sheet.Rows[Define_Property_Row][c].ToString().ToLower();
                if (!string.IsNullOrEmpty(columnname))
                {
                    sw.Write(columnname);
                    if (sheet.Rows[Define_Property_Type][c].ToString().ToLower() == "int")
                    {
                        sw.Write(0);
                        dictype.Add(columnname, 0);
                    }
                    else
                    {
                        sw.Write(1);
                        dictype.Add(columnname, 1);
                    }

                    lstColumns.Add(c);
                }
            }
        }


        private bool WriteData(DataTable sheet, BinaryWriter sw, Dictionary<string, int> dictype, List<int> lstColumns, int row)
        {
            int c = 0; //行数
            for (int r2 = Define_Property_Row; r2 < row; r2++)
            {
                string leftstr = sheet.Rows[r2][Define_Property_Col].ToString().ToLower();
                if (!string.IsNullOrEmpty(leftstr)) c++;
            }

            sw.Write(c);
            for (int r = Define_Property_Row + 1; r < row; r++)
            {
                string leftstr2 = sheet.Rows[r][Define_Property_Col].ToString().ToLower();
                if (string.IsNullOrEmpty(leftstr2)) continue;
                foreach (var cl in lstColumns)
                {
                    string title = sheet.Rows[Define_Property_Row][cl].ToString().ToLower();
                    if(string.IsNullOrEmpty(title)) continue;
                    string columnvalue = sheet.Rows[r][cl].ToString().ToLower();
                    if (!string.IsNullOrEmpty(columnvalue))
                    {
                        if (IsString(title, dictype))
                        {
                            sw.Write(columnvalue);
                            continue;
                        }

                        try
                        {
                            sw.Write((int) double.Parse(columnvalue));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("title:" + title);
                            Console.Write(e.ToString());
                            Console.WriteLine("value:" + columnvalue);
                            Console.WriteLine("id:" + leftstr2);
                            Console.WriteLine("table:" + sheet.TableName);
                            return false;
                        }
                    }
                    else if (IsString(title, dictype))
                    {
                        sw.Write("0");
                    }
                    else
                    {
                        sw.Write(0);
                    }
                }
            }

            return true;
        }

        public void CheckMsg()
        {
            var strdataroot = CLoader.cfgRootPath + "data";
            var sheetcount = result.Tables.Count; //sheet数量
            try
            {
                for (int i = 0; i < sheetcount; i++)
                {
                    DataTable sheet = result.Tables[i];
                    Console.WriteLine(sheet.TableName);
                    string type = sheet.Rows[Define_Property_Type][Define_Property_Col].ToString().ToLower(); //第一个字段类型
                    if (string.IsNullOrEmpty(type) || type != "int" || sheet.TableName.ToLower().Equals("doubleexp"))
                    {
                        continue;
                    }

                    string str = sheet.TableName.ToLower();
                    int row = sheet.Rows.Count;
                    int col = sheet.Columns.Count;
                    int countcol = 0; //列数
                    for (int c = Define_Property_Col; c < col; c++)
                    {
                        string columnname = sheet.Rows[Define_Property_Row][c].ToString().ToLower(); //列名
                        if (!string.IsNullOrEmpty(columnname))
                        {
                            countcol++;
                        }
                    }

                    if (countcol == 0) return;

                    string filename = $"{strdataroot}/{sheet.TableName.ToLower()}.data";
                    FileStream fsWrite = new FileStream(filename, FileMode.Create);
                    var sw = new BinaryWriter(fsWrite);
                    sw.Write(countcol);

                    List<int> lstColumns = new List<int>();
                    Dictionary<string, int> dictype = new Dictionary<string, int>();

                    WriteHeader(sheet, out lstColumns, out dictype, col, sw);
                    if (!WriteData(sheet, sw, dictype, lstColumns, row))
                    {
                        m_lf.state = 1;
                    }
                    sw.Close();
                    fsWrite.Close();
                }
            }
            catch (Exception e)
            {
                m_lf.state = 1;
                Console.WriteLine(e.ToString());
            }

            lock (m_lf)
            {
                if (m_lf.state != 1)
                {
                    m_lf.state = 2;
                }
            }
        }

        public void TheadStart()
        {
            new Thread(CheckMsg).Start();
        }
    }
}