// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Text;
using SqlMaker;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
CLoader loader = new CLoader();
string filePath = CLoader.cfgRootPath + "data";
if (Directory.Exists(filePath)) Directory.Delete(filePath, true);  //删除上次生成data文件
            
Directory.CreateDirectory(filePath);

try
{
    if (!loader.LoadFromExcel())
    {
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