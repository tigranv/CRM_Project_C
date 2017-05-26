using CRM.EntityFrameWorkLib;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public class ParsingProvider
    {
        public List<Contact> GetContactsFromFile(byte[] baytarray)
        {
            string path = @"C:\Users\Lusine\Desktop\TestExcel.xlsx";
            List<Contact> contacts = new List<Contact>();
            Contact contact = new Contact();

            try
            {
                File.WriteAllBytes(path, baytarray);
                string sheetName = "Sheet1";
                var excelFile = new ExcelQueryFactory(path);

                // ADD COLUMN MAPPINGS:
                //excelFile.AddMapping("FullName", "FullName");
                //excelFile.AddMapping("CompanyName", "CompanyName");
                //excelFile.AddMapping("Position", "Position");
                //excelFile.AddMapping("Country", "Country");
                //excelFile.AddMapping("Email", "Email");

                List<Row> contactList = (from item in excelFile.Worksheet<Row>(sheetName) select item).ToList();

                foreach (Row item in contactList)
                {
                    contact.FullName = item["FullName"];
                    contact.CompanyName = item["Company"];
                    contact.Position = item["Position"];
                    contact.Country = item["Country"];
                    contact.Email = item["Email"];
                    contact.GuID = Guid.NewGuid();
                    contact.DateInserted = DateTime.Now;
                    contacts.Add(contact);
                }
            }
            catch (Exception)
            {
                try
                {
                    path = path.Remove(path.Length - 4, 4) + "csv";
                    File.WriteAllBytes(path, baytarray);
                    List<Contact> values = File.ReadAllLines(path)
                                 .Skip(1)
                                 .Select(v => FromCsv(v))
                                 .ToList();
                }
                catch (Exception)
                {

                    return null;
                }
               
            }
            return contacts;
        }

        public static Contact FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            Contact contactValues = new Contact();
            contactValues.FullName = values[0];
            contactValues.CompanyName = values[1];
            contactValues.Position = values[2];
            contactValues.Country = values[3];
            contactValues.Email = values[4];
            return contactValues;
        }
    }
}