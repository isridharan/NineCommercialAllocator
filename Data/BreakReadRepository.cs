using System;
using Data.Model;
using Common;
using System.Threading.Tasks;

namespace Data
{
    public interface IBreakReadRepository
    {
        public BreaksCollection GetAll();
        int[] GetAllocationLimit(string modelType);
    }
    public class BreakRepository : IBreakReadRepository
    {
        public BreaksCollection GetAll()
        {
            var breaks = new BreaksCollection();
            breaks.Add(new BreakData("Break1", "W25-30", 80));
            breaks.Add(new BreakData("Break1", "M18-35", 100));
            breaks.Add(new BreakData("Break1", "T18-40", 250));
            breaks.Add(new BreakData("Break2", "W25-30", 50));
            breaks.Add(new BreakData("Break2", "M18-35", 120));
            breaks.Add(new BreakData("Break2", "T18-40", 200));
            breaks.Add(new BreakData("Break3", "W25-30", 350));
            breaks.Add(new BreakData("Break3", "M18-35", 150));
            breaks.Add(new BreakData("Break3", "T18-40", 500));
            return breaks;
        }

        public int[] GetAllocationLimit(string modelType)
        {
            int[] maxAllowedLimit;
            if (modelType == Constants.ModelType.Model2)
            {
                maxAllowedLimit = new[] { 2, 3, 4 };
                return maxAllowedLimit;
            }
            maxAllowedLimit = new[] { 3, 3, 3 };
            return maxAllowedLimit;
        }
    }
}
