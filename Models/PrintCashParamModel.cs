
using McShawermaSerialPort.Enums;
using System.Text.Json.Serialization;

namespace McShawermaSerialPort.Models
{
    public class PrintCashParamModel
    {
        [JsonPropertyName("type")]
        public PrintCashDeviceType Type { get; set; }

        [JsonPropertyName("com_port")]
        public string ComPort { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }
    }
}
