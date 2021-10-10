using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Common;

namespace Data
{
    public interface ICommercialReadRepository
    {
        public CommercialsCollection GetAll(string model);
    }
    public class CommercialRepository : ICommercialReadRepository
    {
        public CommercialsCollection GetAll(string modelType)
        {
            var commercials = new CommercialsCollection();
            commercials.Add(new CommercialData("Commercial1", "Automotive", "W25-30"));
            commercials.Add(new CommercialData("Commercial2", "Travel", "M18-35"));
            commercials.Add(new CommercialData("Commercial3", "Travel", "T18-40"));
            commercials.Add(new CommercialData("Commercial4", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial5", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial6", "Finance", "W25-30"));
            commercials.Add(new CommercialData("Commercial7", "Finance", "M18-35"));
            commercials.Add(new CommercialData("Commercial8", "Automotive", "T18-40"));
            commercials.Add(new CommercialData("Commercial9", "Travel", "W25-30"));

            if (modelType == Constants.ModelType.Model2)
                commercials.Add(new CommercialData("Commercial10", "Finance", "T18-40"));
            return commercials;
        }
    }
}
