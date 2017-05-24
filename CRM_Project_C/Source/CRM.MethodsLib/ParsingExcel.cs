using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.MethodsLib
{
    //class ParsingExcel
    ////{
    ////    public void ExcelUpload(byte[] array, string path)
    ////    {
    ////        File.WriteAllBytes(path, array);


    ////        ExcelPackage ep = new ExcelPackage(new FileInfo(path));
    ////        ExcelWorksheet ws = ep.Workbook.Worksheets["Sheet1"];

    ////        List<string> domains = new List<string>();
    ////        for (int rw = 1; rw <= ws.Dimension.End.Row; rw++)
    ////        {
    ////            for (int cl = 1; cl < ws.Dimension.End.Column; cl++)
    ////            {
    ////                if (ws.Cells[rw, cl].Value != null)
    ////                {
    ////                    domains.Add(ws.Cells[rw, cl].Value.ToString());
    ////                    Console.Write(ws.Cells[rw, cl].Value + ", ");
    ////                }
    ////            }
    ////            Console.WriteLine();
    ////        }           
    ////    }
    //}
}
