using System;

namespace Common
{
    public class Constants
    {
        public struct CommercialType
        {
            public const string Automotive = "Automotive";
            public const string Travel = "Travel";
            public const string Finance = "Finance";
        }

        public struct BreakName
        {
            public const string Break2 = "Break2";
        }

        public struct Demographic
        {
            public const string W2530 = "W25-30";
            public const string M1835 = "M18-35";
            public const string T1840 = "T18-40";
        }

        public struct ValidationMessage
        {
            public const string Successful = "Successful";
            public const string CommercialTypeInCompatibleWithBreak = "CommercialTypeInCompatibleWithBreak";
            public const string SuccessiveCommerialsCannotBeSameType = "SuccessiveCommerialsCannotBeSameType";
        }

        public struct ModelType
        {
            public const string Model1 = "Model1";
            public const string Model2 = "Model2";
        }

        public struct ExceptionMessage
        {
            public const string NotEnoughCommercialsToAllocate = "There are not enough number of Commercials to be allocated.";
            public const string AllocationProcessingException = "Allocation Processing has Failed";
            public const string BadRequestMessage = "Please enter Model1 / Model2 for model paramter";
            public const string MaximumLimitIsNotDefinedForAllBreaks = "Maximum limit of Commercials is not set for all Breaks";
        }
    }
}
