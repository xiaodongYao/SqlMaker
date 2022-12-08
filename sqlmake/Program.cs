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
            
            Directory.CreateDirectory(filePath);

            try
            {
                if (!loader.LoadFromExcel())
                {
                    Logger.Instance().Debug("loadCfg Return");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
                return;
            }
            Console.WriteLine("success!~");
            Console.ReadKey();
        }
    }
}