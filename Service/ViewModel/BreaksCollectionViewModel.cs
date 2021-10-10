using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Service.ViewModel
{   
    public class BreaksCollectionViewModel
    {
        public BreaksCollectionViewModel()
        {
            this.Breaks = new List<BreakViewModel>();
        }

        [JsonProperty("breaks")]
        public List<BreakViewModel> Breaks { get; set; }
        [JsonProperty("rating")]
        public int Rating
        {
            get
            {
               return this.Breaks.SelectMany(x=>x.Commercials).Sum(x => x.Rating);
            }
        }
    }

    public class BreakViewModel
    {
        public BreakViewModel()
        {
            this.Commercials = new List<CommercialViewModel>();
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("commercials")]
        public List<CommercialViewModel> Commercials { get; set; }
        public int Rating
        {
            get
            {
                return this.Commercials.Sum(x => x.Rating);
            }
        }
    }

    public class CommercialViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("demographics")]
        public string Demographic { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
}
