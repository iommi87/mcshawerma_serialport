using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.IO.Ports;
using McShawermaSerialPort.Enums;

namespace McShawermaSerialPort.CashPrintTypes
{
    public class DaisyTech : IDisposable
    {
        private SerialPort m_ComPort;
        private bool _disposed = false;
        enum CommandType
        {
            StartOfNonFiscalReceipt = 0x26,
            PrintOfNonFiscalText = 0x2A,
            EndOfNonFiscalReceipt = 0x27,
            StartOfFiscalReceipt = 0x30,
            TotalSum = 0x35,
            PrintOfFiscalText = 0x36,
            CancelReceipt = 0x82,
            DailyFinancialReport = 0x45,
            EndOfFiscalReceipt = 0x38,
            BriefReportFromFmByDate = 0x61,
            DiagnosticInformation = 0x5A,
            ServiceIssuedSums = 0x46,
            SaleAndDisplay = 0x34,
        }

        byte m_seq = 32;
        private readonly Queue<byte> m_queue;
       

        public string ErrorCOM { get; private set; }

        public DaisyTech(string portname)
        {
            try
            {
                m_ComPort = new SerialPort(portname, 9600, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
            }
            catch(Exception ex)
            {
                ErrorCOM = ex.Message;
            }
            m_queue = new Queue<byte>();
        }

        public bool PortOpen()
        {
            try
            {
                if (m_ComPort.IsOpen)
                    m_ComPort.Close();
                m_ComPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }
        }

        public bool ClosePort()
        {
            try
            {
                if (m_ComPort.IsOpen)
                    m_ComPort.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }
        }

        private bool ReadByte()
        {
            int b = m_ComPort.ReadByte();
            m_queue.Enqueue((byte)b);
            return b != 0x03;
        }

        private bool SendData(byte[] data)
        {
            byte[] lastStatusBytes = null;
            for (var r = 0; r < 3; r++)
            {
                try
                {
                    m_ComPort.Write(data, 0, data.Length);
                    List<byte> list = new List<byte>();
                    while (ReadByte())
                    {
                        var b = m_queue.Dequeue();
                        if (b == 22)
                            continue;
                        if (b == 21)
                            throw new Exception("(NAK) Invalid packet checksum or form of messsage.");// try again
                        list.Add(b);
                    }
                    list.Add(m_queue.Dequeue());
                    lastStatusBytes = list.Skip(list.Count - 12).Take(6).ToArray();
                    break;
                }
                catch (Exception ex)
                {
                    if (r >= 2)
                    {
                        ErrorCOM = ex.Message;
                        return false;
                    }
                    m_queue.Clear();
                }
            }
            return AnalyzeStatuses(lastStatusBytes);
        }


        private bool AnalyzeStatuses(byte[] st)
        {
            ErrorCOM = null;
            List<string> _st = new List<string>();
            for (byte b = 0; b < 6; b++)
            {
                BitArray ba = new BitArray(new byte[] { st[b] });
                switch (b)
                {
                    case 0:
                        {
                            if (ba[0])
                                _st.Add("Syntactic error");
                            if (ba[1])
                                _st.Add("Invalid command");
                            if (ba[2])
                                _st.Add("Date and time are not set");
                            if (ba[4])
                                _st.Add("Printing mechanism error");
                            if (ba[5])
                                _st.Add("General error");
                            break;
                        }
                    case 1:
                        {
                            if (ba[0])
                                _st.Add("Overflow of sum fields");
                            if (ba[1])
                                _st.Add("Prohibited command in current mode");
                            //if (ba[5])
                            //    _st.Add("Error in cutter");
                            if (ba[6])
                                _st.Add("Wrong password");
                            break;
                        }
                    case 2:
                        {
                            if (ba[0])
                                _st.Add("No paper");
                            if (ba[1])
                                _st.Add("Not enough paper");
                            if (ba[2])
                                _st.Add("No paper (control tape)");
                            if (ba[4])
                                _st.Add("Insufficient paper  in EJT");
                            break;
                        }
                    case 4:
                        {
                            if (ba[0])
                                _st.Add("Record error in fiscal memory");
                            if (ba[2])
                                _st.Add("Invalid record in fiscal memory");
                            //if (ba[3])
                            //    _st.Add("Room for less than 50 records in fiscal memory");
                            if (ba[4])
                                _st.Add("Fiscal memory full");
                            if (ba[4])
                                _st.Add("General error");
                            break;
                        }
                    case 5:
                        {
                            if (ba[0])
                                _st.Add("Fiscal memmory overflowed");
                            //if (ba[3])
                            //    _st.Add("Fiscal device in use");
                            break;
                        }
                }
            }
            if (_st.Any())
            {
                ErrorCOM = string.Join(", ", _st.ToArray());
                return false;
            }

            return true;
        }

        private string GetEncodedString(string source_string)
        {
            StringBuilder res = new StringBuilder();
            foreach (char char_code in source_string)
                res.Append((char)((char_code >= 4304 && char_code <= 4336) ? (char_code - 4096) : char_code));
            return res.ToString();
        }

        private byte[] CalculateBCC(byte LEN, byte SEQ, byte CMD, byte[] DATA, byte POST)
        {
            int res = LEN + SEQ + CMD + DATA.Sum(a => a) + POST;
            string hex_string = string.Join("", res.ToString("X4").Select(a => string.Concat("3", a)).ToArray());
            return Enumerable.Range(0, hex_string.Length)
                              .Where(x => x % 2 == 0)
                              .Select(x => Convert.ToByte(hex_string.Substring(x, 2), 16))
                              .ToArray();
        }


        private byte[] GenerateCommand(CommandType type, string data)
        {
            m_seq = (byte)(m_seq < 255 ? m_seq + 1 : 32);
            List<byte> result = new List<byte>();
            byte[] DATA = Encoding.GetEncoding("ISO-8859-1").GetBytes(data);
            byte LEN = (byte)(0x24 + DATA.Length);
            byte CMD = (byte)type;
            byte POST = 05;
            byte[] BCC = CalculateBCC(LEN, m_seq, CMD, DATA, POST);
            result.Add(01);//SOH
            result.Add(LEN);//LEN
            result.Add(m_seq);//SEQ
            result.Add(CMD);//CMD
            result.AddRange(DATA);//DATA
            result.Add(POST);// POST
            result.AddRange(BCC);// BCC
            result.Add(03);//ETC
            Console.WriteLine(type.ToString());
            return result.ToArray();
        }

        public bool ExecuteCommand(PrintCashDeviceType type, decimal total)
        {
            if(m_ComPort == null)
            {
                return false;
            }

            ErrorCOM = string.Empty;
            string data_string = null;
            try
            {
                if (!PortOpen())
                    return false;
                SendData(GenerateCommand(CommandType.EndOfFiscalReceipt, string.Empty));
                if (!SendData(GenerateCommand(CommandType.StartOfFiscalReceipt, "1,1")))
                    return false;

                if (type == PrintCashDeviceType.Daisy)
                    data_string = GetEncodedString("") + '\t' + "B" + Math.Round(total, 2, MidpointRounding.AwayFromZero).ToString().Replace(',', '.');
                else if (type == PrintCashDeviceType.Aclas)
                    data_string = GetEncodedString("განყ 1") + '\t' + "1" + '\t' + Math.Round(total, 2, MidpointRounding.AwayFromZero).ToString().Replace(',', '.');
                if (!SendData(GenerateCommand(CommandType.SaleAndDisplay, data_string)))
                {
                    string ss = ErrorCOM;
                    SendData(GenerateCommand(CommandType.CancelReceipt, string.Empty));
                    ErrorCOM = ss;
                    return false;
                }

                Dictionary<EcrPaymentType, decimal> extra_pays = new Dictionary<EcrPaymentType, decimal> { { EcrPaymentType.Cash, total } }.Where(a => a.Value > 0).OrderByDescending(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
                var single_item = extra_pays.Where(a => a.Value >= total).FirstOrDefault();
                if (!single_item.Equals(default(KeyValuePair<EcrPaymentType, decimal>)))
                {
                    extra_pays.Clear();
                    extra_pays.Add(single_item.Key, single_item.Value);
                }
                foreach (var p in extra_pays)
                {
                    if (!SendData(GenerateCommand(CommandType.TotalSum, string.Empty + p.Value.ToString().Replace(',', '.'))))
                        return false;
                }

                if (!SendData(GenerateCommand(CommandType.EndOfFiscalReceipt, string.Empty)))
                {
                    string ss = ErrorCOM;
                    SendData(GenerateCommand(CommandType.CancelReceipt, string.Empty));
                    ErrorCOM = ss;
                    return false;
                }
                if (!ClosePort())
                    return false;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }

            return true;
        }



        public bool Xreport()
        {
            ErrorCOM = string.Empty;
            try
            {
                if (!PortOpen())
                    return false;
                if (!SendData(GenerateCommand(CommandType.DailyFinancialReport, GetEncodedString("2"))))
                    return false;
                if (!ClosePort())
                    return false;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }
            return true;
        }

        public bool Zreport()
        {
            ErrorCOM = string.Empty;
            try
            {
                if (!PortOpen())
                    return false;
                if (!SendData(GenerateCommand(CommandType.DailyFinancialReport, GetEncodedString("0"))))
                    return false;
                if (!ClosePort())
                    return false;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }
            return true;
        }

        public bool PrintSimpleText(string text)
        {
            ErrorCOM = string.Empty;
            try
            {
                if (!PortOpen())
                    return false;
                SendData(GenerateCommand(CommandType.EndOfNonFiscalReceipt, string.Empty));
                if (!SendData(GenerateCommand(CommandType.StartOfNonFiscalReceipt, string.Empty)))
                    return false;
                string[] lines = text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                foreach (string comment in lines)
                {
                    if (!SendData(GenerateCommand(CommandType.PrintOfNonFiscalText, GetEncodedString(comment))))
                        return false;
                }
                if (!SendData(GenerateCommand(CommandType.EndOfNonFiscalReceipt, string.Empty)))
                    return false;
                if (!ClosePort())
                    return false;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }
            return true;
        }

        public bool PrintServiceIOSum(decimal sum_price, string comment1, string comment2)
        {
            ErrorCOM = string.Empty;
            try
            {
                if (!PortOpen())
                    return false;
                if (!SendData(GenerateCommand(CommandType.ServiceIssuedSums, Math.Round(sum_price, 2, MidpointRounding.AwayFromZero).ToString().Replace(',', '.') + "," + GetEncodedString(comment1) + '\n' + GetEncodedString(comment2))))
                    return false;
                if (!ClosePort())
                    return false;
            }
            catch (Exception ex)
            {
                ErrorCOM = ex.Message;
                return false;
            }

            return true;


        }





        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
                return;
            if (disposing)
            {
                if (m_ComPort == null)
                    return;
                if (m_ComPort.IsOpen)
                    m_ComPort.Close();
                m_ComPort.Dispose();
                m_ComPort = null;
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DaisyTech()
        {
            Dispose(false);
        }
    }
}