using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace McShawermaSerialPort.Models
{
    public class PrintParamModel
    {
        [JsonPropertyName("printer_name")]
        public string PrinterName { get; set; }

        [JsonPropertyName("is_cash")]
        public bool IsCash { get; set; }

        [JsonPropertyName("order_num")]
        public string OrderNum { get; set; }

        [JsonPropertyName("items")]
        public List<ReceiptItem> Items { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }
    }

    public class ReceiptItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name_ru")]
        public string NameRu { get; set; }

        [JsonPropertyName("name_en")]
        public string NameEn { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("addons")]
        public List<ReceiptAddonItem> AddonItems { get; set; }
    }

    public class ReceiptAddonItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name_ru")]
        public string NameRu { get; set; }

        [JsonPropertyName("name_en")]
        public string NameEn { get; set; }

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }
    }
}
