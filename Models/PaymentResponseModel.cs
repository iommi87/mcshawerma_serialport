using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace McShawermaSerialPort.Models
{
    public class PaymentResponseModel
    {
        [JsonPropertyName("status_id")]
        public int StatusId { get; set; }

        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; }

        [JsonPropertyName("response_code_host")]
        public string ResponseCodeHost { get; set; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonPropertyName("rfn")]
        public string Rrn { get; set; }


        [JsonPropertyName("receipts")]
        public List<string> Receipts { get; set; }

        [JsonPropertyName("raw_data")]
        public string RawData
        {
            get
            {
                if (Receipts != null && Receipts.Any())
                    return Receipts.ElementAt(0);
                else
                {
                    if (string.IsNullOrEmpty(RawData))
                        return null;
                    string[] answer_data = RawData.Split(new string[] { "0xDF^^", "0xDA^^", "0x4F^^", "0x95^^", "0xDD^^", "0xDE^^" }, StringSplitOptions.RemoveEmptyEntries);
                    string result = "";
                    if (RawData.Contains("0xDF^^"))
                        result = answer_data.ElementAt(0).Trim('~');
                    return result;
                }
            }
        }
    }
}