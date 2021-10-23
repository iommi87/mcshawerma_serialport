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
                        using (DaisyTech ss = new DaisyTech(param.ComPort))
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
    }
}