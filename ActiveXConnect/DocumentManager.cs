using System;
using System.Collections.Generic;
using System.IO;

namespace McShawermaSerialPort.ActiveXConnect
{
    public class DocumentManager
    {
        public static void DeleteDocument(Document doc)
        {
            string pth = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transactions\\");
            if (!Directory.Exists(pth))
                Directory.CreateDirectory(pth);
            string str = string.Concat(pth, doc.DocumentNr, ".docXml");
            if (File.Exists(str))
            {
                File.Delete(str);
            }
        }

        public static List<Document> GetAllDocuments()
        {
            List<Document> documents = new List<Document>();
            string pth = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transactions\\");
            if (!Directory.Exists(pth))
                Directory.CreateDirectory(pth);
            string[] files = Directory.GetFiles(pth, "*.docXml", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string str = files[i];
                try
                {
                    if (str.EndsWith(".docXml"))
                    {
                        documents.Add(Document.Deserialize(File.ReadAllText(str)));
                    }
                }
                catch
                {
                }
            }
            return documents;
        }

        public static void SaveDocument(Document doc)
        {
            string pth = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transactions\\");
            if (!Directory.Exists(pth))
                Directory.CreateDirectory(pth);
            File.WriteAllText(string.Concat(pth, doc.DocumentNr, ".docXml"), doc.Serialize());
        }
    }
}