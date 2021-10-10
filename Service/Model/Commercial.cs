using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Model
{
    public class Commercial
    {
        public Commercial(string name, string type, string demographic, int rating)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Type = type;
            this.Demographic = demographic;
            this.Rating = rating;
        }

        public bool IsAssigned()
        {
            return this.Break != null;
        }

        public void SetBreak(Break _break)
        {
            if (!IsAssigned())
            {
                this.Break = _break;
            }
        }

        public void SetRating(int rating)
        {
            this.Rating = rating;
        }

        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public string Type { get; protected set; }
        public string Demographic { get; protected set; }
        public int Rating { get; protected set; }
        public Break Break { get; protected set; }
    }
}
