namespace Validot.Tests.Unit
{
    using System.Collections.Generic;

    using FluentAssertions;

    public static class ValidationTestHelpers
    {
        public static void ShouldBeLikeErrorMessages(this IReadOnlyDictionary<string, IReadOnlyList<string>> a, IReadOnlyDictionary<string, IReadOnlyList<string>> b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            a.Should().NotBeSameAs(b);
            a.Should().HaveCount(b.Count);

            foreach (var bPair in b)
            {
                a.Keys.Should().Contain(bPair.Key);
                a[bPair.Key].Should().NotBeNull();
                a[bPair.Key].Should().NotBeSameAs(bPair.Value);
                a[bPair.Key].Should().HaveCount(bPair.Value.Count);

                foreach (var bValue in bPair.Value)
                {
                    a[bPair.Key].Should().Contain(bValue);
                }
            }
        }
    }
}
