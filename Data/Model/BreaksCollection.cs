using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class BreaksCollection
    {
        public BreaksCollection()
        {
            this.Breaks = new SortedSet<BreakData>(new BreaksComparer());
        }

        public SortedSet<BreakData> Breaks { get; protected set; }

        public void Add(BreakData _break)
        {
            this.Breaks.Add(_break);
        }

        public int GetRating(string breakName, string demographic)
        {
            return this.Breaks.ToList().Find(x => x.Demographics == demographic && x.Name == breakName).Rating;
        }

        public void Remove(BreakData _break)
        {
            this.Breaks.Remove(_break);
        }
    }

    public class BreakData
    {
        public BreakData(string name, string demographics, int rating)
        {
            this.Name = name;
            this.Rating = rating;
            this.Demographics = demographics;
        }

        public string Name { get; protected set; }
        public int Rating { get; protected set; }
        public string Demographics { get; protected set; }
    }

    //create comparer
    internal class BreaksComparer : IComparer<BreakData>
    {
        public int Compare(BreakData x, BreakData y)
        {
            //first by Rating            
            int result = y.Rating.CompareTo(x.Rating);
            return result;
        }
    }
}
