namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using FluentAssertions;

    using Xunit;

    public class TypeStringifierTests
    {
        [Theory]
        [InlineData(typeof(int), "Int32")]
        [InlineData(typeof(int?), "Nullable<Int32>")]
        [InlineData(typeof(IEnumerable<int>), "IEnumerable<Int32>")]
        [InlineData(typeof(Dictionary<string, int>), "Dictionary<String,Int32>")]
        [InlineData(typeof(Dictionary<Guid, ReadOnlyDictionary<string, DateTime?>>), "Dictionary<Guid,ReadOnlyDictionary<String,Nullable<DateTime>>>")]
        public void Should_Stringify_WithoutNamespaces(Type type, string expectedName)
        {
            type.GetFriendlyName().Should().Be(expectedName);
        }

        [Theory]
        [InlineData(typeof(int), "System.Int32")]
        [InlineData(typeof(int?), "System.Nullable<System.Int32>")]
        [InlineData(typeof(IEnumerable<int>), "System.Collections.Generic.IEnumerable<System.Int32>")]
        [InlineData(typeof(Dictionary<string, int>), "System.Collections.Generic.Dictionary<System.String,System.Int32>")]
        [InlineData(typeof(Dictionary<Guid, ReadOnlyDictionary<string, DateTime?>>), "System.Collections.Generic.Dictionary<System.Guid,System.Collections.ObjectModel.ReadOnlyDictionary<System.String,System.Nullable<System.DateTime>>>")]
        public void Should_Stringify_WithNamespaces(Type type, string expectedName)
        {
            type.GetFriendlyName(true).Should().Be(expectedName);
        }
    }
}
