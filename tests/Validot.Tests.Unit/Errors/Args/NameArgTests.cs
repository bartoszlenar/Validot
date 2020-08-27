namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class NameArgTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var arg = new NameArg("someName");

            NameArg.Name.Should().Be("_name");

            (arg as IArg).Name.Should().Be("_name");

            arg.AllowedParameters.Count.Should().Be(1);
            arg.AllowedParameters.Should().Contain("format");
        }

        [Fact]
        public void Should_Stringify_ReturnArgName_If_NullParameters()
        {
            var arg = new NameArg("someName");

            var key = new Dictionary<string, string>()
            {
                ["invalid_key"] = "value1"
            };

            var stringified = arg.ToString(key);

            stringified.Should().Be("someName");
        }

        [Fact]
        public void Should_Stringify_ReturnArgName_If_InvalidParameterName()
        {
            var arg = new NameArg("someName");

            var key = new Dictionary<string, string>()
            {
                ["invalid_key"] = "value1"
            };

            var stringified = arg.ToString(key);

            stringified.Should().Be("someName");
        }

        [Theory]
        [InlineData("ToGetYourGEDInTimeASongAboutThe26ABCsIsOfTheEssenceButAPersonalIDCardForUser456InRoom26AContainingABC26TimesIsNotAsEasyAs123ForC3POOrR2D2Or2R2D", "To Get Your GED In Time A Song About The 26 ABCs Is Of The Essence But A Personal ID Card For User 456 In Room 26A Containing ABC 26 Times Is Not As Easy As 123 For C3PO Or R2D2 Or 2R2D")]
        [InlineData("helloThere", "Hello There")]
        [InlineData("HelloThere", "Hello There")]
        [InlineData("ILoveTheUSA", "I Love The USA")]
        [InlineData("iLoveTheUSA", "I Love The USA")]
        [InlineData("DBHostCountry", "DB Host Country")]
        [InlineData("SetSlot123ToInput456", "Set Slot 123 To Input 456")]
        [InlineData("ILoveTheUSANetworkInTheUSA", "I Love The USA Network In The USA")]
        [InlineData("Limit_IOC_Duration", "Limit IOC Duration")]
        [InlineData("This_is_a_Test_of_Network123_in_12_days", "This Is A Test Of Network 123 In 12 Days")]
        [InlineData("ASongAboutTheABCsIsFunToSing", "A Song About The ABCs Is Fun To Sing")]
        [InlineData("CFDs", "CFDs")]
        [InlineData("DBSettings", "DB Settings")]
        [InlineData("IWouldLove1Apple", "I Would Love 1 Apple")]
        [InlineData("Employee22IsCool", "Employee 22 Is Cool")]
        [InlineData("SubIDIn", "Sub ID In")]
        [InlineData("ConfigureCFDsImmediately", "Configure CFDs Immediately")]
        [InlineData("UseTakerLoginForOnBehalfOfSubIDInOrders", "Use Taker Login For On Behalf Of Sub ID In Orders")]
        public void Should_Stringify_To_TitleCase(string name, string expectedTitleCase)
        {
            var arg = new NameArg(name);

            var stringified = arg.ToString(new Dictionary<string, string>()
            {
                ["format"] = "titleCase"
            });

            stringified.Should().Be(expectedTitleCase);
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            new Action(() =>
            {
                new NameArg(null);
            }).Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("name");
        }
    }
}
