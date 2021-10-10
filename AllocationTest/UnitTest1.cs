using NUnit.Framework;
using Data;
using Service;
using WebAPI.Controllers;
using Service.Mapper;
using Moq;
using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using Data.Model;
using System;
using System.Linq;

namespace AllocationTest
{
    public class Tests
    {
        IAllocationService _allocationService;
        AllocationController _allocationcontroller;
        Mock<ICommercialReadRepository> _commercialRepo;
        Mock<IBreakReadRepository> _breakRepo;

        [SetUp]
        public void Setup()
        {
            //Mock data repository to pass your own data and check on the Business Logic.
            _commercialRepo = new Mock<ICommercialReadRepository>();
            _breakRepo = new Mock<IBreakReadRepository>();

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BreaksProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            _allocationService = new AllocationService(_breakRepo.Object, _commercialRepo.Object, mapper);
            _allocationcontroller = new AllocationController(_allocationService);
        }

        [Test]
        public void TestInput()
        {
            // Arrange
            var model = "1111";

            //Act
            var response = _allocationcontroller.GetAllocation(model);

            //Assert                        
            Assert.NotNull(response.Result);
            Assert.IsAssignableFrom(typeof(BadRequestObjectResult), response.Result);
        }



        [Test]
        public void TestNotEnoughCommercialToAllocate()
        {
            try
            {
                //Arrange
                var model = "Model1";
                var breaks = new BreaksCollection();
                breaks.Add(new BreakData("Break1", "W25-30", 80));
                breaks.Add(new BreakData("Break1", "M18-35", 100));
                breaks.Add(new BreakData("Break1", "T18-40", 250));

                var commercials = new CommercialsCollection();
                commercials.Add(new CommercialData("Commercial1", "Automotive", "W25-30"));
                commercials.Add(new CommercialData("Commercial2", "Travel", "M18-35"));
                commercials.Add(new CommercialData("Commercial3", "Travel", "T18-40"));
                commercials.Add(new CommercialData("Commercial4", "Automotive", "M18-35"));
                commercials.Add(new CommercialData("Commercial5", "Automotive", "M18-35"));
                _breakRepo.Setup(x => x.GetAll())
                   .Returns(breaks);
                _breakRepo.Setup(x => x.GetAllocationLimit(model))
                            .Returns(new[] { 3, 3, 3 });
                _commercialRepo.Setup(x => x.GetAll(model))
                                .Returns(commercials);

                //Act
                var result = _allocationService.AllocateCommercials("Model1");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is NotEnoughCommercialsToAllocateException);
            }
        }

        [Test]
        public void TestMaxLimitIsSetForAllBreaks()
        {
            try
            {
                //Arrange
                var model = "Model1";
                var breaks = new BreaksCollection();
                breaks.Add(new BreakData("Break1", "W25-30", 80));
                breaks.Add(new BreakData("Break1", "M18-35", 100));
                breaks.Add(new BreakData("Break1", "T18-40", 250));
                breaks.Add(new BreakData("Break2", "W25-30", 100));
                breaks.Add(new BreakData("Break2", "M18-35", 150));
                breaks.Add(new BreakData("Break2", "T18-40", 350));


                var commercials = new CommercialsCollection();
                commercials.Add(new CommercialData("Commercial1", "Automotive", "W25-30"));
                commercials.Add(new CommercialData("Commercial2", "Travel", "M18-35"));
                commercials.Add(new CommercialData("Commercial3", "Travel", "T18-40"));
                commercials.Add(new CommercialData("Commercial4", "Automotive", "M18-35"));
                commercials.Add(new CommercialData("Commercial5", "Automotive", "M18-35"));
                _breakRepo.Setup(x => x.GetAll())
                   .Returns(breaks);
                _breakRepo.Setup(x => x.GetAllocationLimit(model))
                            .Returns(new[] { 3 });
                _commercialRepo.Setup(x => x.GetAll(model))
                                .Returns(commercials);

                //Act
                var result = _allocationService.AllocateCommercials("Model1");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is MaximumLimitIsNotDefinedForAllBreaks);
            }
        }

        [Test]
        public void TestIfMaximumRatingBreakIsAllocatedFirst()
        {           
                //Arrange
                var model = "Model1";
                var breaks = new BreaksCollection();
                breaks.Add(new BreakData("Break1", "W25-30", 80));
                breaks.Add(new BreakData("Break1", "M18-35", 100));
                breaks.Add(new BreakData("Break1", "T18-40", 250));
                breaks.Add(new BreakData("Break2", "W25-30", 250));
                breaks.Add(new BreakData("Break2", "M18-35", 300));
                breaks.Add(new BreakData("Break2", "T18-40", 350));

                var commercials = new CommercialsCollection();
                commercials.Add(new CommercialData("Commercial1", "Automotive", "W25-30"));
                commercials.Add(new CommercialData("Commercial2", "Travel", "M18-35"));
                commercials.Add(new CommercialData("Commercial3", "Travel", "T18-40"));
                commercials.Add(new CommercialData("Commercial4", "Automotive", "M18-35"));
                commercials.Add(new CommercialData("Commercial5", "Financial", "M18-35"));
                _breakRepo.Setup(x => x.GetAll())
                   .Returns(breaks);
                _breakRepo.Setup(x => x.GetAllocationLimit(model))
                            .Returns(new[] { 3, 2});
                _commercialRepo.Setup(x => x.GetAll(model))
                                .Returns(commercials);

                //Act
                var result = _allocationService.AllocateCommercials("Model1");

                //Assert
                var break2 = result.Breaks.Find(x => x.Name == "Break2");
                Assert.IsNotNull(break2.Commercials.Find(x => x.Demographic == Constants.Demographic.T1840));                      
        }

        // This test is to check if the process will exit, when no more Allocations are happening, Although there are un-allocated commercials.
        [Test]
        public void TestAfterOptimalAllocationProcessMustExit()
        {
            //Arrange
            var model = "Model1";
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
            commercials.Add(new CommercialData("Commercial10", "Automotive", "W25-30"));
            commercials.Add(new CommercialData("Commercial11", "Travel", "M18-35"));
            commercials.Add(new CommercialData("Commercial12", "Travel", "T18-40"));
            commercials.Add(new CommercialData("Commercial13", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial14", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial15", "Finance", "W25-30"));
            commercials.Add(new CommercialData("Commercial16", "Finance", "M18-35"));
            commercials.Add(new CommercialData("Commercial17", "Automotive", "T18-40"));
            commercials.Add(new CommercialData("Commercial18", "Travel", "W25-30"));
            commercials.Add(new CommercialData("Commercial19", "Finance", "T18-40"));
            commercials.Add(new CommercialData("Commercial20", "Finance", "T18-40"));


            _breakRepo.Setup(x => x.GetAll())
               .Returns(breaks);
            _breakRepo.Setup(x => x.GetAllocationLimit(model))
                        .Returns(new[] { 6, 6,6});
            _commercialRepo.Setup(x => x.GetAll(model))
                            .Returns(commercials);

            //Act
            var result = _allocationService.AllocateCommercials("Model1");

            //Assert
            // It has reached this point and a Allocation Process has stopped although all commercials are not yet allocated.
            Assert.IsTrue(result.Breaks.Count > 0 && commercials.Commercials.Count > 0);
        }

        // Test If Swapping of Commercials betweeen Breaks happens when Commercial Type is not Compatible with the Break.
        [Test]
        public void TestIfSwappingOfCommercialsBetweenBreakHappens()
        {
            //Arrange
            var model = "Model1";
            var breaks = new BreaksCollection();
            breaks.Add(new BreakData("Break1", "W25-30", 80));
            breaks.Add(new BreakData("Break1", "M18-35", 100));
            breaks.Add(new BreakData("Break1", "T18-40", 450));        
            breaks.Add(new BreakData("Break2", "T18-40", 350));

            var commercials = new CommercialsCollection();
            commercials.Add(new CommercialData("Commercial1", "Travel", "T18-40"));
            commercials.Add(new CommercialData("Commercial2", "Travel", "M18-35"));
            commercials.Add(new CommercialData("Commercial3", "Automotive", "W25-30"));
            commercials.Add(new CommercialData("Commercial4", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial5", "Finance", "T18-40"));
            _breakRepo.Setup(x => x.GetAll())
               .Returns(breaks);
            _breakRepo.Setup(x => x.GetAllocationLimit(model))
                        .Returns(new[] { 3, 1 });
            _commercialRepo.Setup(x => x.GetAll(model))
                            .Returns(commercials);

            //Act
            var result = _allocationService.AllocateCommercials("Model1");

            //Assert
            var break2 = result.Breaks.Find(x => x.Name == "Break2");
            Assert.IsNull(break2.Commercials.Find(x => x.Type == Constants.CommercialType.Finance));
        }

        // Test If Change of Sequence happens within the Break if incompatible types are placed next to each other.
        [Test]
        public void TestIfChangeOfSequenceOfCommercialsWithinBreakHappens()
        {
            //Arrange
            var model = "Model1";
            var breaks = new BreaksCollection();
            breaks.Add(new BreakData("Break1", "W25-30", 80));
            breaks.Add(new BreakData("Break1", "M18-35", 100));
            breaks.Add(new BreakData("Break1", "T18-40", 450));
            breaks.Add(new BreakData("Break2", "T18-40", 350));

            var commercials = new CommercialsCollection();
            commercials.Add(new CommercialData("Commercial1", "Travel", "T18-40"));
            commercials.Add(new CommercialData("Commercial2", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial3", "Automotive", "W25-30"));
            commercials.Add(new CommercialData("Commercial4", "Automotive", "M18-35"));
            commercials.Add(new CommercialData("Commercial5", "Finance", "T18-40"));
            _breakRepo.Setup(x => x.GetAll())
               .Returns(breaks);
            _breakRepo.Setup(x => x.GetAllocationLimit(model))
                        .Returns(new[] { 3, 1 });
            _commercialRepo.Setup(x => x.GetAll(model))
                            .Returns(commercials);

            //Act
            var result = _allocationService.AllocateCommercials("Model1");

            //Assert
            var break1 = result.Breaks.Find(x => x.Name == "Break1");
            var financeCommercial = break1.Commercials.Find(x => x.Type == Constants.CommercialType.Finance);
            Assert.IsNotNull(financeCommercial);
            Assert.IsTrue(break1.Commercials.IndexOf(financeCommercial) == 1);
        }
    }   
}