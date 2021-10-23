using System.Collections.Generic;

namespace McShawermaSerialPort.ActiveXConnect
{
    public class Transaction
    {
        public string OperationResult;

        public string OperationID;

        public long AmountAuthorized;

        public long AmountTips;

        public string DocumentNr;

        public string ErrorText;

        public string AuthCode;

        public string RRN;

        public string STAN;

        public string CardType;

        public bool IsVoided;

        public static List<string> Receipts = new List<string>();

        public bool isAuthorized()
        {
            return this.OperationResult != "OK" || this.AmountAuthorized == 0 ? false : true;
        }
    }
}