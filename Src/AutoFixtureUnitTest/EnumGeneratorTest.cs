﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Ploeh.AutoFixtureUnitTest.Kernel;
using Ploeh.TestTypeFoundation;

namespace Ploeh.AutoFixtureUnitTest
{
    public class EnumGeneratorTest
    {
        [Fact]
        public void SutIsSpecimenBuilder()
        {
            // Fixture setup
            // Exercise system
            var sut = new EnumGenerator();
            // Verify outcome
            Assert.IsAssignableFrom<ISpecimenBuilder>(sut);
            // Teardown
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(1)]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        public void RequestNonEnumReturnsCorrectResult(object request)
        {
            // Fixture setup
            var sut = new EnumGenerator();
            // Exercise system
            var dummyContext = new DelegatingSpecimenContext();
            var result = sut.Create(request, dummyContext);
            // Verify outcome
            var expectedResult = new NoSpecimen(request);
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(typeof(TriState), 1, TriState.First)]
        [InlineData(typeof(TriState), 2, TriState.Second)]
        [InlineData(typeof(TriState), 3, TriState.Third)]
        [InlineData(typeof(TriState), 4, TriState.First)]
        [InlineData(typeof(TriState), 5, TriState.Second)]
        [InlineData(typeof(DayOfWeek), 1, DayOfWeek.Sunday)]
        [InlineData(typeof(DayOfWeek), 2, DayOfWeek.Monday)]
        [InlineData(typeof(DayOfWeek), 8, DayOfWeek.Sunday)]
        public void RequestForEnumTypeReturnsCorrectResult(Type enumType, int requestCount, object expectedResult)
        {
            // Fixture setup
            var sut = new EnumGenerator();
            // Exercise system
            var dummyContext = new DelegatingSpecimenContext();
            var result = Enumerable.Repeat<Func<object>>(() => sut.Create(enumType, dummyContext), requestCount).Select(f => f()).Last();
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(typeof(TriState), TriState.Third)]
        [InlineData(typeof(DayOfWeek), DayOfWeek.Monday)]
        [InlineData(typeof(ConsoleColor), ConsoleColor.Black)]
        public void SutCanCorrectlyInterleaveDifferentEnumTypes(Type enumType, object expectedResult)
        {
            // Fixture setup
            var sut = new EnumGenerator();
            var dummyContext = new DelegatingSpecimenContext();

            sut.Create(typeof(TriState), dummyContext);
            sut.Create(typeof(TriState), dummyContext);
            sut.Create(typeof(DayOfWeek), dummyContext);
            // Exercise system
            var result = sut.Create(enumType, dummyContext);
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }
    }
}