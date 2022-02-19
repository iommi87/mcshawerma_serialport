using McShawermaSerialPort.CashPrintTypes;
using McShawermaSerialPort.Enums;
using McShawermaSerialPort.Models;
using System.Collections.Generic;

namespace McShawermaSerialPort.Helpers
{
    public class PrintCashHelper
    {
        public KeyValuePair<bool, string> Print(PrintCashParamModel param)
        {
            KeyValuePair<bool, string> res = new KeyValuePair<bool, string>(true, string.Empty);
            switch (param.Type)
            {
                case PrintCashDeviceType.Daisy:
                case PrintCashDeviceType.Aclas:
                    {
                        using (DaisyTech ss = new(param.ComPort))
                        {
                            bool result = ss.ExecuteCommand(param.Type, param.Total);
                            if (!result)
                                res = new KeyValuePair<bool, string>(false, ss.ErrorCOM);
                        }
                        break;
                    }

            }

            return res;
        }

        public KeyValuePair<bool, string> PrintXReport(PrintCashParamModel param)
        {
            KeyValuePair<bool, string> res = new KeyValuePair<bool, string>(true, string.Empty);
            switch (param.Type)
            {
                case PrintCashDeviceType.Daisy:
                case PrintCashDeviceType.Aclas:
                    {
                        using (DaisyTech ss = new(param.ComPort))
                        {
                            bool result = ss.Xreport();
                            if (!result)
                                res = new KeyValuePair<bool, string>(false, ss.ErrorCOM);
                        }
                        break;
                    }

            }

            return res;
        }

        public KeyValuePair<bool, string> PrintZReport(PrintCashParamModel param)
        {
            KeyValuePair<bool, string> res = new KeyValuePair<bool, string>(true, string.Empty);
            switch (param.Type)
            {
                case PrintCashDeviceType.Daisy:
                case PrintCashDeviceType.Aclas:
                    {
                        using (DaisyTech ss = new(param.ComPort))
                        {
                            bool result = ss.Zreport();
                            if (!result)
                                res = new KeyValuePair<bool, string>(false, ss.ErrorCOM);
                        }
                        break;
                    }

            }

            return res;
        }
    }
}