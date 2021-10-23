using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace McShawermaSerialPort.ActiveXConnect
{
    public class Document
    {
        public string DocumentNr;
        public List<Transaction> Transactions = new List<Transaction>();
        public States State;

        public void ChangeStateToClosed()
        {
            States state = this.State;
            if (state == States.ToBeReversed)
            {
                this.State = States.Reversed;
            }
            else if (state == States.ToBeConfirmed)
            {
                this.State = States.Confirmed;
                return;
            }
        }

        public static Document Deserialize(string data)
        {
            return (Document)(new XmlSerializer(typeof(Document))).Deserialize(new StringReader(data));
        }

        public void GenerateNewUniqueDocumentNr()
        {
            Guid guid = Guid.NewGuid();
            this.DocumentNr = guid.ToString("N").Substring(0, 20);
        }

        public bool IsClosed()
        {
            switch (this.State)
            {
                case States.ToBeReversed:
                case States.ToBeConfirmed:
                    {
                        return false;
                    }
                case States.Reversed:
                case States.Confirmed:
                    {
                        return true;
                    }
            }
            throw new Exception("Unknown state");
        }

        public string Serialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Document));
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, this);
            stringWriter.Flush();
            return stringWriter.ToString();
        }

        public enum States
        {
            ToBeReversed,
            Reversed,
            ToBeConfirmed,
            Confirmed
        }
    }
}