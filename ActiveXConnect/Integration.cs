using System;
using System.Collections.Generic;
using System.Diagnostics;
using ActiveXConnect64Lib;

namespace McShawermaSerialPort.ActiveXConnect
{
    public class Integration
    {
        private ActiveXConnect64API _api;

        public string Lang { get; set; } = "GE";

        public string OperID { get; set; } = "999";

        public string OperName { get; set; } = "John";

        public bool HasPrinter { get; set; } = false;

        public string Alias { get; set; }

        public bool ApiCloseDay()
        {
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                return false;
            }
            if (!this.UnlockDevice(this.OperID, this.OperName, "", 0, ""))
                return false;
            aPI.CloseDay(this.OperID, this.OperName);
            if (!this.ProcessEventsUntil(aPI, "OnCloseDayResult", 130))
                return false;
            string operationResult = aPI.OperationResult;
            return operationResult == "OK";
        }

        public bool ApiPrintTotals()
        {
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                return false;
            }
            if (!this.UnlockDevice(this.OperID, this.OperName, "", 0, ""))
                return false;
            aPI.PrintTotals(this.OperID, this.OperName);
            if (!this.ProcessEventsUntil(aPI, "OnPrintTotalsResult", 130))
                return false;
            string operationResult = aPI.OperationResult;
            return operationResult == "OK";
        }

        public long AuthorizeWithCurrency(long amount, string documentNr, string currencyCode, out Transaction transaction)
        {
            transaction = new Transaction();
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                transaction.ErrorText = "Failed to load API library";
                return 0;
            }
            while (this.ProcessEventsUntil(aPI, "OnCard", 30))
            {
                if (this.ContainsFlag(aPI.Flags, "AllowAuthorize"))
                {
                    string str = null;
                    if (this.ContainsFlag(aPI.Flags, "ReqLastFourDigits"))
                    {
                        Console.WriteLine("Please enter last 4 digits of the card");
                        str = Console.ReadLine();
                    }
                    aPI.AuthorizeWithCurrency(amount, 0, documentNr, str, currencyCode);
                    if (!this.ProcessEventsUntil(aPI, "OnAuthorizeResult", 100))
                    {
                        transaction.ErrorText = "Timeout waiting for OnAuthorizeResult";
                        return 0;
                    }
                    string operationResult = aPI.OperationResult;
                    transaction.OperationID = aPI.OperationID;
                    transaction.AmountAuthorized = aPI.AmountAuthorized;
                    transaction.AmountTips = aPI.AmountTips;
                    transaction.OperationResult = operationResult;
                    transaction.AuthCode = aPI.AuthCode;
                    transaction.RRN = aPI.RRN;
                    transaction.CardType = aPI.CardType;
                    transaction.DocumentNr = aPI.DocumentNr;
                    transaction.ErrorText = aPI.Text;
                    transaction.STAN = aPI.STAN;
                    return transaction.AmountAuthorized;
                }
            }
            transaction.ErrorText = "Timeout waiting for card to be inserted into device";
            return 0;
        }

        public long CreditWithCurrency(long amount, string documentNr, string currencyCode, out Transaction transaction)
        {
            transaction = new Transaction();
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                transaction.ErrorText = "Failed to load API library";
                return 0;
            }
            while (this.ProcessEventsUntil(aPI, "OnCard", 30))
            {
                if (this.ContainsFlag(aPI.Flags, "AllowAuthorize"))
                {
                    string str = null;
                    aPI.CreditWithCurrency(amount, documentNr, str, null, null, null, currencyCode);
                    if (!this.ProcessEventsUntil(aPI, "OnCreditResult", 100))
                    {
                        transaction.ErrorText = "Timeout waiting for OnAuthorizeResult";
                        return 0;
                    }
                    string operationResult = aPI.OperationResult;
                    transaction.OperationID = aPI.OperationID;
                    transaction.AmountAuthorized = aPI.AmountAuthorized;
                    transaction.AmountTips = aPI.AmountTips;
                    transaction.OperationResult = operationResult;
                    transaction.AuthCode = aPI.AuthCode;
                    transaction.RRN = aPI.RRN;
                    transaction.CardType = aPI.CardType;
                    transaction.DocumentNr = aPI.DocumentNr;
                    transaction.ErrorText = aPI.Text;
                    transaction.STAN = aPI.STAN;
                    return transaction.AmountAuthorized;
                }
            }
            transaction.ErrorText = "Timeout waiting for card to be inserted into device";
            return 0;
        }

        public bool CloseDocument(Document document)
        {
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                return false;
            }
            List<string> strs = new List<string>();
            if (document.Transactions != null)
            {
                foreach (Transaction transaction in document.Transactions)
                {
                    strs.Add(transaction.OperationID);
                }
            }
            aPI.DocClosed(document.DocumentNr, strs.ToArray());
            if (!this.ProcessEventsUntil(aPI, "OnDocClosedResult", 15))
                return false;
            string operationResult = aPI.OperationResult;
            return operationResult == "OK";
        }

        private bool ContainsFlag(Array array, string flag)
        {
            bool flag1;
            if (array == null)
            {
                return false;
            }
            var enumerator = array.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if ((string)enumerator.Current != flag)
                    {
                        continue;
                    }
                    flag1 = true;
                    return flag1;
                }
                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }


        public void DisposeAPI()
        {
            if (this._api == null)
            {
                return;
            }
            this._api.Dispose();
        }

        public ActiveXConnect64API GetAPI()
        {
            if (this._api != null)
            {
                return this._api;
            }
            this._api = new ActiveXConnect64API();
            List<string> _flags = new List<string> { "OnSelectEventSupported", "ReceiptControlSymbolsNotSupported", "OnDisplayTextEventSupported", "OnMessageBoxEventSupported" };
            if (HasPrinter)
                _flags.Add("OnPrintEventAutoForward");
            if (!string.IsNullOrWhiteSpace(Alias))
                this._api.InitializeWithAlias(_flags.ToArray(), Alias);
            else
                this._api.Initialize(_flags.ToArray());
            if (this._api.OperationResult == "OK")
            {
                return this._api;
            }
            this._api.Dispose();
            this._api = null;
            return null;
        }

        public bool LockDevice(string text)
        {
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                return false;
            }
            aPI.LockDevice(text);
            if (this.ProcessEventsUntil(aPI, "OnLockDeviceResult", 15) && !(aPI.OperationResult != "OK"))
            {
                return true;
            }
            return false;
        }

        private bool ProcessEventsUntil(ActiveXConnect64API api, string expectedEventType, int timeoutInSeconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            api.PollEvent(200);
            string eventType = api.EventType;
            while (eventType != expectedEventType)
            {
                if (eventType == "OnPrint")
                {
                    api.GetNextReceipt();
                    while (!string.IsNullOrEmpty(api.ReceiptText))
                    {
                        Transaction.Receipts.Add(api.ReceiptText);
                        api.GetNextReceipt();
                    }
                }
                else if (eventType == "OnDisplayText")
                {

                }
                else if (eventType != "OnKBD")
                {
                    if (!string.IsNullOrEmpty(eventType))
                    {

                    }
                }
                else if (expectedEventType == "OnCard" && api.KBDKey == "FR")
                {
                    return false;
                }
                if (stopwatch.ElapsedMilliseconds > timeoutInSeconds * 1000)
                {
                    return false;
                }
                api.PollEvent((int)(timeoutInSeconds * 1000 - stopwatch.ElapsedMilliseconds));
                eventType = api.EventType;
            }
            return true;
        }

        public bool UnlockDevice(string operatorID, string operatorName, string operation, long amount, string text)
        {
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
                return false;
            aPI.UnlockDevice(text, this.Lang, operatorID, operatorName, amount, "1.0", operation, 0);
            if (this.ProcessEventsUntil(aPI, "OnUnlockDeviceResult", 15) && !(aPI.OperationResult != "OK"))
                return true;
            return false;
        }

        public bool VoidTransaction(string operationID, out string errorText)
        {
            errorText = null;
            ActiveXConnect64API aPI = this.GetAPI();
            if (aPI == null)
            {
                errorText = "Failed to load API library...";
                return false;
            }
            aPI.Void(operationID);
            if (!this.ProcessEventsUntil(aPI, "OnVoidResult", 100))
            {
                errorText = "Timeout waiting for OnVoidResult";
                return false;
            }
            if (aPI.OperationResult == "OK")
            {
                return true;
            }
            errorText = aPI.Text;
            return false;
        }
    }
}