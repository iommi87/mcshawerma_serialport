using System;
using System.Linq;
using McShawermaSerialPort.ActiveXConnect;
using McShawermaSerialPort.Models;

namespace McShawermaSerialPort.Helpers
{
    public class PaymentHelper
    {
        private PaymentResponseModel MakePaymentXConnect(string terminal_id, int amount, bool has_printer)
        {
            IngenicoAPI api = null;
            try
            {
                api = new IngenicoAPI { Alias = terminal_id, HasPrinter = has_printer };
                Transaction res = api.MakePaymentWithCurrency(amount, "981");
                string resp_cd_hst = (res.ErrorText.Contains('-') ? res.ErrorText.Split('-').ElementAt(0) : "").Trim();

                //System.IO.File.AppendAllText(System.IO.Path.Combine(Environment.CurrentDirectory, "log.txt"), res.ErrorText + "_" + resp_cd_hst + "_" + amount.ToString() + "_" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + Environment.NewLine);
                return new PaymentResponseModel
                {
                    StatusId = Convert.ToInt32(res.isAuthorized()),
                    StatusMessage = res.ErrorText,
                    Rrn = res.RRN,
                    AuthorizationCode = res.AuthCode,
                    Receipts = Transaction.Receipts,
                    ResponseCodeHost = resp_cd_hst
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponseModel
                {
                    StatusId = -2000,
                    StatusMessage = ex.Message,
                };
            }
            finally
            {
                if (api != null)
                    api.DisposeAPI();
            }
        }

        public PaymentResponseModel Pay(PaymentParamModel param)
        {
            try
            {
                int amount = (int)(Math.Round(param.Price, 2, MidpointRounding.AwayFromZero) * 100);
                return MakePaymentXConnect(param.TerminalAlias, amount, param.HasPrinter);
            }
            catch (Exception ex)
            {
                return new PaymentResponseModel { StatusId = -1000, StatusMessage = ex.Message };
            }
        }

        public string CloseDay()
        {
            IngenicoAPI api = null;
            try
            {
                api = new IngenicoAPI { Alias = null, HasPrinter = false };
                if(api.CloseDay())
                {
                    return Transaction.Receipts.ElementAt(0);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (api != null)
                    api.DisposeAPI();
            }
        }
    }
}