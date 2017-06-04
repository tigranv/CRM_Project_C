using CRM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CRM.WebApi.Infrastructure
{
    public static class ParsingProvider
    {
        public static List<RequestContact> GetContactsFromFile(string path, string ext)
        {
            List<RequestContact> contacts = new List<RequestContact>();
            RequestContact contact = new RequestContact();

            switch (ext)
            {
                case "xlsx":
                    contacts = ReadExcelFile(path);
                        break;
                case ".csv":
                    contacts = File.ReadAllLines(path).Select(v => FromCsv(v)).ToList();
                    break;
                default:
                    return null;
            }

            return contacts;//.Where(y => y != null).ToList();
        }

        static RequestContact FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            RequestContact contactValues = new RequestContact();
            for (int i = 0; i < 5; i++)
            {
                if (values[i] == null || values[i].Length < 2) return null;
            }
            if (!EmailValidator(values[4])) return null;
            contactValues.FullName = values[0];
            contactValues.CompanyName = values[1];
            contactValues.Position = values[2];
            contactValues.Country = values[3];
            contactValues.Email = values[4];
            return contactValues;
        }
        static List<RequestContact> ReadExcelFile(string filename)
        {
            List<RequestContact> contacts = new List<RequestContact>();
            string[] strProperties = new string[5];
            RequestContact contact;
            int j = 0;

            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filename, false))
            {
                WorkbookPart workbookPart = myDoc.WorkbookPart;
                IEnumerable<Sheet> Sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = Sheets?.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)myDoc.WorkbookPart.GetPartById(relationshipId);
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                int i = 1;
                string value;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (i != 1)
                    {
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            if (c == null) continue;
                            value = c.InnerText;
                            if (c.DataType != null)
                            {
                                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                if (stringTable != null)
                                {
                                    value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                                }
                            }
                            strProperties[j] = value;
                            j = j + 1;
                        }

                       
                    }
                    j = 0;
                    i++;

                    if (strProperties.Any(p => p == null || p.Length < 2)) { contacts.Add(null); continue; }
                    if (!EmailValidator(strProperties[4])) { contacts.Add(null); continue; }
                    contact = new RequestContact();
                    contact.FullName = strProperties[0];
                    contact.CompanyName = strProperties[1];
                    contact.Position = strProperties[2];
                    contact.Country = strProperties[3];
                    contact.Email = strProperties[4];
                    contacts.Add(contact);
                }
                return contacts;
            }
        }

        static bool EmailValidator(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public class MyStreamProvider : MultipartFormDataStreamProvider
    {
        public string fileName;
        public MyStreamProvider(string uploadPath)
            : base(uploadPath)
        {

        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            fileName = headers.ContentDisposition.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".data";
            }
            return fileName.Replace("\"", string.Empty);
        }
    }
}