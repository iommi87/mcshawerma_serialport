using System.Collections.Generic;
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

        [JsonPropertyName("raw_data")]
        public string RawData { get; set; }

        [JsonPropertyName("receipts")]
        public List<string> Receipts { get; set; }
    }
}