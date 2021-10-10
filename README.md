# NineCommercialAllocator is a system that does optimal allocation of Commercials to Breaks based on their Rating.

This Project is Developed in .NetCore 5 and API exposes 2 end points, 

1. allocation/Model1 - This Allocates 9 Commercials to 3 Breaks, each Break with Maximum Capacity of 3 Commercials in a Optimum Way and Calculates the Total Rating of the Allocated Commercials.
2. allocation/Model2 - This Allocates 10 Commercials to 3 Breaks, each Break with Maximum Capacity of (Break1 - 2, Break2 - 3, Break3 - 4) Commercials respectively in a Optimum Way and Calculates the Total Rating of the Allocated Commercials. 1 Commercial will be left out without Allocation due to Maximum Capacity Limit.

External Dependencies For this API: 

1. AutoMapper 10.1.1 (Model Mapping)
2. Moq 4.16.1 (Unit Testing)
3. Newtonsoft.Json 13.0.1

The Solution is Seggregated into Following Projects.

1. WebAPI (.Net Core 5.0 WebAPI)
2. Service (Class Library Project in Framework 5.0)
3. Data (Class Library Project in Framework 5.0)
4. Common (Class Library Project in Framework 5.0)
5. Test (NUnit Test Project in Framework 5.0)
 
