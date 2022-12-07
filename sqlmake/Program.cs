using System;
using System.IO;

namespace sqlmake
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CLoader loader = new CLoader();
            string filePath = CLoader.cfgRootPath + "data";
            if (Directory.Exists(filePath)) Directory.Delete(filePath, true);  //删除上次生成data文件

            Logger.Instance().Init("sqlmake"); //log init 
            new ThreadLogFile();
            Directory.CreateDirectory(filePath);

            try
            {
                if (!loader.LoadFromExcel())
                {
                    Logger.Instance().Debug("loadCfg Return");
                }
            }
            catch (Exception e)
            {
                Logger.Instance().Init(e.ToString());
                Console.WriteLine(e.ToString());
                return;
            }
            Console.WriteLine("success!~");
            Console.ReadKey();
        }
    }
}