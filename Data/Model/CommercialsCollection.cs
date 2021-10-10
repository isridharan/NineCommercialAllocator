using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class CommercialsCollection
    {
        public CommercialsCollection()
        {
            this.Commercials = new List<CommercialData>();
        }

        public IList<CommercialData> Commercials { get; protected set; }

        public CommercialData Get(string demographics, List<CommercialData> inCompatibleCommercials)
        {
            var commercial = this.Commercials.ToList().Except(inCompatibleCommercials).FirstOrDefault(x => x.Demographics == demographics);
            return commercial;
        }

        public void Add(CommercialData commercial)
        {
            this.Commercials.Add(commercial);
        }

        public void Remove(CommercialData commercial)
        {
            this.Commercials.Remove(commercial);
        }
    }

    public class CommercialData
    {
        public CommercialData(string name, string type, string demographics)
        {
            this.Name = name;
            this.Type = type;
            this.Demographics = demographics;
        }

        public string Name { get; protected set; }
        public string Type { get; protected set; }
        public string Demographics { get; protected set; }
    }
}
