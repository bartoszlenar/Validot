namespace Validot.Tests.Unit.Rules
{
    using System.Collections.Generic;
    using System.Linq;

    public static class RulesHelper
    {
        public static IEnumerable<object[]> GetTestDataCombined(params IEnumerable<object[]>[] sets)
        {
            return sets.SelectMany(s => s);
        }
    }
}
