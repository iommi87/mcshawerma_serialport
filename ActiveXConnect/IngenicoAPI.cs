using System;
using System.Collections.Generic;

namespace McShawermaSerialPort.ActiveXConnect
{
    public class IngenicoAPI : Integration
    {
        private bool AttemptToCloseUnclosedDocuments()
        {
            bool flag;
            List<Document>.Enumerator enumerator = DocumentManager.GetAllDocuments().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Document current = enumerator.Current;
                    if (current.IsClosed())
                    {
                        continue;
                    }
                    if (!base.CloseDocument(current))
                    {
                        flag = false;
                        return flag;
                    }
                    else
                    {
                        current.ChangeStateToClosed();
                        DocumentManager.SaveDocument(current);
                    }
                }
                return true;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
        }

        public bool CloseDay()
        {
            bool flag;
            Transaction.Receipts.Clear();
            try
            {
                if (this.AttemptToCloseUnclosedDocuments())
                {
                    flag = base.ApiCloseDay();
                }
                else
                {
                    flag = false;
                }
            }
            finally
            {
                base.LockDevice("Welcome");
            }
            return flag;
        }

        public Transaction MakeCreditWithCurrency(long amountToPay, string currencyCode)
        {
            Transaction transaction;
            Transaction transaction1;
            Transaction.Receipts.Clear();
            Document document = new Document();
            document.GenerateNewUniqueDocumentNr();
            DocumentManager.SaveDocument(document);
            if (!base.UnlockDevice(base.OperID, base.OperName, "CREDIT", amountToPay, "Please insert or swipe card"))
                return null;
            try
            {
                long num = base.CreditWithCurrency(amountToPay, document.DocumentNr, currencyCode, out transaction);
                if (num == 0)
                    transaction1 = transaction;
                else if (num != amountToPay)
                    transaction1 = transaction;
                else
                {
                    document.Transactions.Add(transaction);
                    DocumentManager.SaveDocument(document);
                    if (base.CloseDocument(document))
                    {
                        document.ChangeStateToClosed();
                        DocumentManager.SaveDocument(document);
                    }
                    transaction1 = transaction;
                }
            }
            finally
            {
                base.LockDevice("Welcome");
            }
            return transaction1;
        }

        public Transaction MakePaymentWithCurrency(long amountToPay, string currencyCode)
        {
            Transaction transaction;
            Transaction transaction1;
            Transaction.Receipts.Clear();
            Document document = new Document();
            document.GenerateNewUniqueDocumentNr();
            DocumentManager.SaveDocument(document);
            if (!base.UnlockDevice(base.OperID, base.OperName, "AUTHORIZE", amountToPay, "Please insert or swipe card"))
                return null;
            try
            {
                long num = base.AuthorizeWithCurrency(amountToPay, document.DocumentNr, currencyCode, out transaction);
                if (num == 0)
                    transaction1 = transaction;
                else if (num != amountToPay)
                    transaction1 = transaction;
                else
                {
                    document.Transactions.Add(transaction);
                    DocumentManager.SaveDocument(document);
                    if (base.CloseDocument(document))
                    {
                        document.ChangeStateToClosed();
                        DocumentManager.SaveDocument(document);
                    }
                    transaction1 = transaction;
                }
            }
            finally
            {
                base.LockDevice("Welcome");
            }
            return transaction1;
        }

        public bool PerformVoid(Document document, Transaction transaction)
        {
            string str;
            bool flag;
            Transaction.Receipts.Clear();
            if (!base.UnlockDevice(base.OperID, base.OperName, "VOID", 0, ""))
                return false;
            try
            {
                if (!base.VoidTransaction(transaction.OperationID, out str))
                    flag = false;
                else
                {
                    transaction.IsVoided = true;
                    DocumentManager.SaveDocument(document);
                    flag = true;
                }
            }
            finally
            {
                base.LockDevice("Welcome");
            }
            return flag;
        }

        public bool PrintTotals()
        {
            bool flag;
            Transaction.Receipts.Clear();
            try
            {
                if (this.AttemptToCloseUnclosedDocuments())
                    flag = base.ApiPrintTotals();
                else
                    flag = false;
            }
            finally
            {
                base.LockDevice("Welcome");
            }
            return flag;
        }
    }
}