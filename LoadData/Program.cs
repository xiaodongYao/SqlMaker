// See https://aka.ms/new-console-template for more information

using System;
using System.IO;

string rootPath = "../../../../SqlMaker/bin/Debug/";
Console.WriteLine("LoadFromData start!");
LoadCfData();
Console.WriteLine("LoadFromData end!");

void LoadCfData()
{
    string dataPath = rootPath + "data/";
    var strFiles = Directory.GetFiles(dataPath, "*.data");
    foreach (var str in strFiles)
    {
        if (!ReadCfgData(str)) return;
    }
}

bool ReadCfgData(string pathFileName)
{
    try
    {
        int index = pathFileName.LastIndexOf('.');
        int selength = (rootPath + "data/").Length;
        string tableName = pathFileName.Substring(selength, index - selength);
        using (FileStream fsWrite = new FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (BinaryReader sw = new BinaryReader(fsWrite))
            {
                var clnCount = sw.ReadInt32(); //列数
                var vType = new int[clnCount];  //类型
                var vname = new string[clnCount]; //列名

                for (int i = 0; i < clnCount; i++)
                {
                    vname[i] = sw.ReadString();
                    vType[i] = sw.ReadInt32();
                }

                int rowcount = sw.ReadInt32();

                for (int i = 0; i < rowcount; i++)
                {
                    long id = 0;
                    for (int j = 0; j < clnCount; j++)
                    {
                        if (vType[j] == 0)
                        {
                            var v = sw.ReadInt32();
                            if (vname[j].ToLower() == "id")
                            {
                                id = v;
                            }
                        }
                        else
                        {
                            string str = sw.ReadString();
                        }
                    }
                }
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        return false;
    }

    return true;
}