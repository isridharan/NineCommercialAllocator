using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Service.Model
{
    public class Break
    {
        public Break(string name, int maxCommercialsAllowed)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Commercials = new SortedList<int, Commercial>();
            this.MaxCommercialsAllowed = maxCommercialsAllowed;
        }

        public Guid Id { get; protected set; }
        public int MaxCommercialsAllowed { get; protected set; }
        public string Name { get; protected set; }
        public SortedList<int, Commercial> Commercials { get; protected set; }
        public int Rating { get { return this.Commercials.Sum(x => x.Value.Rating); } }

        public string IsAdditionValid(Commercial commercial)
        {
            // Check if previous commercial is same type 
            // Check if break 2 is not added to Finance Commercial

            if (!CheckIfCommercialTypeIsAllowed(commercial))
                return Constants.ValidationMessage.CommercialTypeInCompatibleWithBreak;

            if (CheckIfPreviousIsSameType(commercial))
                return Constants.ValidationMessage.SuccessiveCommerialsCannotBeSameType;

            return Constants.ValidationMessage.Successful;
        }


        public bool CanSequenceBeChangedWithinBreak(Commercial commercial)
        {
            var count = this.Commercials.Count + 1;
            var noOfCommercialsOfSameType = this.Commercials.Values.Where(x => x.Type == commercial.Type).Count() + 1;
            // For 3 items in a list, maximum permissible duplicate is 2.
            var maxDuplicateTolerance = (count % 2 != 0) ? (count + 1) / 2 : count / 2;
            return (noOfCommercialsOfSameType <= maxDuplicateTolerance);
        }


        public void ChangeSequenceOfCommercials(Commercial commercial)
        {
            // change positions where consecutive commercial types wont be same.
            for (int key = this.Commercials.Count - 1; key >= 0; key--)
            {
                if ((key == 0 && this.Commercials[key].Type != commercial.Type) || (this.Commercials[key - 1].Type != commercial.Type && this.Commercials[key].Type != commercial.Type))
                {
                    AddCommercialAtPosition(key, commercial);
                    break;
                }
            }
        }

        public void AddCommercialAtPosition(int key, Commercial commercial)
        {
            var data = this.Commercials.Skip(key).Take(this.Commercials.Count - key).ToList();
            data.ForEach(x =>
            {
                this.Commercials.Remove(x.Key);
            });
            //Move All Commercials Down
            data.ForEach(x =>
            {
                this.Commercials.Add(x.Key + 1, x.Value);
            });
            // Add commerical in the position of key
            this.Commercials.Add(key, commercial);
        }

        public bool IfMaximumAllocationReached()
        {
            return this.Commercials.Count == this.MaxCommercialsAllowed;
        }

        private bool CheckIfPreviousIsSameType(Commercial commercial)
        {
            if (this.Commercials.Count == 0) return false;
            return this.Commercials.Values[(this.Commercials.Count) - 1].Type == commercial.Type;
        }

        private bool CheckIfCommercialTypeIsAllowed(Commercial commercial)
        {
            if (this.Name == Constants.BreakName.Break2 && commercial.Type == Constants.CommercialType.Finance)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfSwapPossible(Commercial commercial, Break _break)
        {
            var commercialTobeSwapped = _break.GetCommercial(commercial.Demographic);
            var isAssignableToThisBreak = this.IsAdditionValid(commercialTobeSwapped);
            var isAssignableToPassedBreak = _break.IsAdditionValid(commercial);
            return (isAssignableToThisBreak == Constants.ValidationMessage.Successful && isAssignableToPassedBreak == Constants.ValidationMessage.Successful);
        }

        public void SwapCommercialBetweenBreaks(Commercial commercial, Break swapBreak)
        {
            var swapCommercial = swapBreak.GetCommercial(commercial.Demographic);
            var rating = swapCommercial.Rating;
            swapCommercial.SetRating(commercial.Rating);
            commercial.SetRating(rating);
            var index = swapBreak.RemoveCommercial(swapCommercial);
            swapBreak.AddCommercial(commercial, index);
            this.AddCommercial(swapCommercial);
        }

        public int GetRating(string demoGraphics)
        {
            var commercialTobeSwapped = this.GetCommercial(demoGraphics);
            return commercialTobeSwapped.Rating;
        }


        public Commercial GetCommercial(string demographic)
        {
            return this.Commercials.ToList().Find(x => x.Value.Demographic == demographic).Value;
        }

        public Commercial GetCommercial(string demographic, string type)
        {
            return this.Commercials.ToList().Find(x => x.Value.Demographic == demographic && x.Value.Type == type).Value;
        }


        public bool IsCommercialExists(string demographic)
        {
            return this.Commercials.ToList().Exists(x => x.Value.Name == demographic);
        }

        public Commercial GetCommercial(Commercial commercial)
        {
            return this.Commercials.ToList().Find(x => x.Value.Name == commercial.Demographic).Value;
        }

        public string AddCommercial(Commercial commercial)
        {
            return _addCommerical(commercial, this.Commercials.Count);
        }

        public string AddCommercial(Commercial commercial, int index)
        {
            return _addCommerical(commercial, index);
        }

        private string _addCommerical(Commercial commercial, int index)
        {
            var isValid = IsAdditionValid(commercial);
            if (isValid == Constants.ValidationMessage.Successful)
            {
                commercial.SetBreak(this);
                this.Commercials.Add(index, commercial);
                return Constants.ValidationMessage.Successful;
            }
            return isValid;
        }

        public int RemoveCommercial(Commercial commercial)
        {
            var index = this.Commercials.IndexOfValue(commercial);
            this.Commercials.Remove(index);
            return index;
        }
    }
}
