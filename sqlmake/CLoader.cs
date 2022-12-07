using System;
using System.IO;

namespace sqlmake
{
    internal class CLoader
    {
        public static readonly string cfgRootPath = "../";

        public bool LoadFromExcel()
        {
            Console.WriteLine("LoadFromExcel start!");
            var ticks = DateTime.Now.Ticks;
            string[] strFiles = Directory.GetFiles($"{cfgRootPath}/excel/", "*xlsx");
            LoadFlag[] lf = new LoadFlag[strFiles.Length];
            int c = 0;
            string[] array = strFiles;
            foreach (var s in array)
            {
                
            }
            
            Console.WriteLine("LoadFromExcel end! {0}", (DateTime.Now.Ticks - ticks) / 10000000);
            return true;
        }
    }
}