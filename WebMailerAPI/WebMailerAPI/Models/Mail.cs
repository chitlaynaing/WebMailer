using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMailerAPI.Models
{
    public class Mail
    {
        [JsonProperty(PropertyName = "toemail")]
        public string To { get; set; }
        [JsonProperty(PropertyName = "fromemail")]
        public string From { get; set; }
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "ccemail")]
        public string CCs { get; set; }
        [JsonProperty(PropertyName = "bccemail")]
        public string BCCs { get; set; }

    }
}
