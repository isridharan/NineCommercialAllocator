using System;
using System.Collections.Generic;
using Service.Model;
using Service.ViewModel;
using Data;
using Data.Model;
using System.Linq;
using Common;
using AutoMapper;

namespace Service
{
    public interface IAllocationService
    {
        BreaksCollectionViewModel AllocateCommercials(string modelType);
    }
    public class AllocationService : IAllocationService
    {
        private readonly IBreakReadRepository _breakReadRepository;
        private readonly ICommercialReadRepository _commercialReadRepository;
        private readonly IMapper _mapper;

        public AllocationService(IBreakReadRepository breakReadRepository, ICommercialReadRepository commercialReadRepository, IMapper mapper)
        {
            _breakReadRepository = breakReadRepository;
            _commercialReadRepository = commercialReadRepository;
            _mapper = mapper;
        }

        public  BreaksCollectionViewModel AllocateCommercials(string modelType)
        {
            var breaks = processAllocation(modelType);
            var result = convertToViewModel(breaks);
            return result;
        }

        private List<Break> processAllocation(string modelType)
        {
            try
            {
                var breaksdata =  _breakReadRepository.GetAll();
                var commecialsdata = _commercialReadRepository.GetAll(modelType);
                var maxAllowedLimit = _breakReadRepository.GetAllocationLimit(modelType);
                var breaks = _initializeBreaks(maxAllowedLimit, breaksdata, commecialsdata);

                int noOfCommercialsPendingAfterLatestCycle = commecialsdata.Commercials.Count;
                int initialCountOfCommercials =  commecialsdata.Commercials.Count;

                // List that stores incompatible commercial for each break
                var incompatibleCommercials = new List<CommercialData>();

                //Check if all commercials are allocated and maximum permissible allocation is completed. If not Repeat the Allocation process.
                while (breaks.SelectMany(x => x.Commercials).Count() < maxAllowedLimit.Sum() && commecialsdata.Commercials.Count > 0)
                {
                    // This is to check no more Commercial Allocations are happening and process must Exit.
                    if (commecialsdata.Commercials.Count < noOfCommercialsPendingAfterLatestCycle)
                    {
                        noOfCommercialsPendingAfterLatestCycle = commecialsdata.Commercials.Count;
                    }
                    else if(commecialsdata.Commercials.Count == noOfCommercialsPendingAfterLatestCycle && commecialsdata.Commercials.Count != initialCountOfCommercials)
                    {
                        break;
                    }

                    foreach (BreakData breakdata in breaksdata.Breaks)
                    {
                        // default isCommercialAllocationSuccessful to false
                        var isCommercialAllocationSuccessful = default(bool);
                        incompatibleCommercials.Clear();
                        while (!isCommercialAllocationSuccessful)
                        {
                            // Get a Commercial for the current Break's demographics.
                            var commercialdata = commecialsdata.Get(breakdata.Demographics, incompatibleCommercials);
                            if (commercialdata == default)
                                break;

                            // Get the break model where commercial has to be added.
                            var _break = breaks.Find(x => x.Name == breakdata.Name);
                            if (_break.IfMaximumAllocationReached())
                                break;

                            var commercial = new Commercial(commercialdata.Name, commercialdata.Type, commercialdata.Demographics, breakdata.Rating);

                            var result = _break.AddCommercial(commercial);
                            isCommercialAllocationSuccessful = result == Constants.ValidationMessage.Successful;
                            if (isCommercialAllocationSuccessful)
                            {
                                // After successful Allocation remove the commercialdata from the collection.
                                commecialsdata.Remove(commercialdata);
                            }
                            if (!isCommercialAllocationSuccessful && (result == Constants.ValidationMessage.CommercialTypeInCompatibleWithBreak))
                            {
                                var otherAllocatedBreaksWithDemographics = breaks.Where(x => x.Commercials.ToList().Exists(y => y.Value.Demographic == breakdata.Demographics && y.Value.Type != commercial.Type)).ToList();
                                foreach (Break breakWithSameDemoGraphic in otherAllocatedBreaksWithDemographics)
                                {
                                    if (isCommercialAllocationSuccessful)
                                        break;

                                    var checkIfSwapWithOtherBreakPossible = _break.CheckIfSwapPossible(commercial, breakWithSameDemoGraphic);
                                    if (checkIfSwapWithOtherBreakPossible)
                                    {
                                        var swapBreak = breaks.Find(x => x.Name == breakWithSameDemoGraphic.Name);
                                        _break.SwapCommercialBetweenBreaks(commercial, swapBreak);
                                        isCommercialAllocationSuccessful = true;

                                        // After successful Allocation remove the commercialdata from the collection.
                                        commecialsdata.Remove(commercialdata);
                                    }
                                }
                            }
                            if (!isCommercialAllocationSuccessful && (result == Constants.ValidationMessage.SuccessiveCommerialsCannotBeSameType))
                            {
                                //check if positions can be changed such that consecutive types are not same within the Break.
                                var canSequenceBeChangedWithinBreak = _break.CanSequenceBeChangedWithinBreak(commercial);
                                if (canSequenceBeChangedWithinBreak)
                                {
                                    _break.ChangeSequenceOfCommercials(commercial);
                                    isCommercialAllocationSuccessful = true;

                                    // After successful Allocation remove the commercialdata from the collection.
                                    commecialsdata.Remove(commercialdata);
                                }
                            }
                            if (!isCommercialAllocationSuccessful)
                                incompatibleCommercials.Add(commercialdata);

                            if (isCommercialAllocationSuccessful)
                                incompatibleCommercials.Clear();
                        }
                    }
                }
                return breaks;
            }
            catch(Exception ex)
            {
                // throw the same exception to allow for stacktrace debugging               
                if (ex is NotEnoughCommercialsToAllocateException)
                    throw;
                // throw the same exception to allow for stacktrace debugging               
                else if (ex is MaximumLimitIsNotDefinedForAllBreaks)
                    throw;
                else
                    throw new AllocationProcessingException(Constants.ExceptionMessage.AllocationProcessingException);
            }            
        }

        private List<Break> _initializeBreaks(int[] maxAllowedLimit, BreaksCollection breaksCollection, CommercialsCollection commercialsCollection)
        {
            var breaks = new List<Break>();
            int index = 0;

            // Check if there are enough Commercials to be Allocated.
            var commercialsCount = commercialsCollection.Commercials.Select(x => x.Name).Distinct().Count();
            var breaksCount = breaksCollection.Breaks.Select(x => x.Name).Distinct().Count();

            if (commercialsCount < maxAllowedLimit.Sum())
                throw new NotEnoughCommercialsToAllocateException(Constants.ExceptionMessage.NotEnoughCommercialsToAllocate);

            if(maxAllowedLimit.Count() < breaksCount)
                throw new MaximumLimitIsNotDefinedForAllBreaks(Constants.ExceptionMessage.MaximumLimitIsNotDefinedForAllBreaks);

            breaksCollection.Breaks.Select(x => x.Name).OrderBy(x => x).Distinct().ToList().ForEach(x => {                
                var maxCommercialsAllowed = maxAllowedLimit[index];
                var _break = new Break(x, maxCommercialsAllowed);
                breaks.Add(_break);
                index++;
            });
            return breaks;
        }

        private BreaksCollectionViewModel convertToViewModel(List<Break> breaks)
        {
            var result = new BreaksCollectionViewModel();
            var breaksmodel = _mapper.Map<List<BreakViewModel>>(breaks);
            result.Breaks = breaksmodel;
            return result;
        }      
    }
}
