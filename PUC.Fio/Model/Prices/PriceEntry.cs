using System.Collections.Generic;
using Newtonsoft.Json;

namespace PUC.Fio.Model.Prices
{
    public class PriceEntry
    {
        private sealed class TickerEqualityComparer : IEqualityComparer<PriceEntry>
        {
            public bool Equals(PriceEntry x, PriceEntry y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Ticker == y.Ticker;
            }

            public int GetHashCode(PriceEntry obj)
            {
                return (obj.Ticker != null ? obj.Ticker.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<PriceEntry> TickerComparer { get; } = new TickerEqualityComparer();

        public string Ticker { get; set; }
        public float MMBuy { get; set; }
        public float MMSell { get; set; }

        [JsonProperty(PropertyName = "CI1-Average")]
        public float CI1Average { get; set; }

        [JsonProperty(PropertyName = "CI1-AskAmt")]
        public float CI1AskAmt { get; set; }

        [JsonProperty(PropertyName = "CI1-AskPrice")]
        public float CI1AskPrice { get; set; }

        [JsonProperty(PropertyName = "CI1-AskAvail")]
        public float CI1AskAvail { get; set; }

        [JsonProperty(PropertyName = "CI1-BidAmt")]
        public float CI1BidAmt { get; set; }

        [JsonProperty(PropertyName = "CI1-BidPrice")]
        public float CI1BidPrice { get; set; }

        [JsonProperty(PropertyName = "CI1-BidAvail")]
        public float CI1BidAvail { get; set; }

        [JsonProperty(PropertyName = "NI1-Average")]
        public float NI1Average { get; set; }

        [JsonProperty(PropertyName = "NI1-AskAmt")]
        public float NI1AskAmt { get; set; }

        [JsonProperty(PropertyName = "NI1-AskPrice")]
        public float NI1AskPrice { get; set; }

        [JsonProperty(PropertyName = "NI1-AskAvail")]
        public float NI1AskAvail { get; set; }

        [JsonProperty(PropertyName = "NI1-BidAmt")]
        public float NI1BidAmt { get; set; }

        [JsonProperty(PropertyName = "NI1-BidPrice")]
        public float NI1BidPrice { get; set; }

        [JsonProperty(PropertyName = "NI1-BidAvail")]
        public float NI1BidAvail { get; set; }

        [JsonProperty(PropertyName = "IC1-Average")]
        public float IC1Average { get; set; }

        [JsonProperty(PropertyName = "IC1-AskAmt")]
        public float IC1AskAmt { get; set; }

        [JsonProperty(PropertyName = "IC1-AskPrice")]
        public float IC1AskPrice { get; set; }

        [JsonProperty(PropertyName = "IC1-AskAvail")]
        public float IC1AskAvail { get; set; }

        [JsonProperty(PropertyName = "IC1-BidAmt")]
        public float IC1BidAmt { get; set; }

        [JsonProperty(PropertyName = "IC1-BidPrice")]
        public float IC1BidPrice { get; set; }

        [JsonProperty(PropertyName = "IC1-BidAvail")]
        public float IC1BidAvail { get; set; }
    }
}