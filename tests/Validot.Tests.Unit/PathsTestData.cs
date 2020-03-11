namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class PathsTestData
    {
        public static IEnumerable<object[]> InvalidPaths()
        {
            yield return new[] { "segment." };
            yield return new[] { ".path.segment" };
            yield return new[] { "<.segment" };
            yield return new[] { "<.path.segment" };
            yield return new[] { "path.segment.another.segment." };
            yield return new[] { "<." };
            yield return new[] { ". " };
            yield return new[] { ".\t" };
        }

        public static IEnumerable<object[]> ValidPaths()
        {
            yield return new[] { "segment" };
            yield return new[] { "path.segment" };
            yield return new[] { "<segment" };
            yield return new[] { "<path.segment" };
            yield return new[] { "path.segment.another.segment" };
            yield return new[] { "<" };
            yield return new[] { " " };
            yield return new[] { "\t" };
        }

        public static IEnumerable<object[]> ResolveNextLevelPath_AllCases()
        {
            var cases = new[]
            {
                ResolveNextLevelPath.SimpleSegment(),
                ResolveNextLevelPath.NewSegmentContainsMoreLevelsDown(),
                ResolveNextLevelPath.UncommonCharacters(),
                ResolveNextLevelPath.NewSegmentGoesLevelUp(),
                ResolveNextLevelPath.NewSegmentIsEmpty_And_GoesLevelUp(),
                ResolveNextLevelPath.NewSegmentGoesLevelUp_And_ExceedsMinimumLevel(),
                ResolveNextLevelPath.ToSamePath()
            };

            foreach (var @case in cases)
            {
                foreach (var set in @case)
                {
                    yield return set;
                }
            }
        }

        public class ResolveNextLevelPath
        {
            public static IEnumerable<object[]> ToSamePath()
            {
                yield return new[] { "base.path", "", "base.path" };
                yield return new[] { "base", "", "base" };
                yield return new[] { "", "", "" };
            }

            public static IEnumerable<object[]> SimpleSegment()
            {
                yield return new[] { "base.path", "newSegment", "base.path.newSegment" };
                yield return new[] { "base", "newSegment", "base.newSegment" };
            }

            public static IEnumerable<object[]> NewSegmentContainsMoreLevelsDown()
            {
                yield return new[] { "base.path", "new.segment", "base.path.new.segment" };
                yield return new[] { "base.path", "new.segment.value", "base.path.new.segment.value" };
            }

            public static IEnumerable<object[]> UncommonCharacters()
            {
                yield return new[] { "base.path", "n@ew.@eg!.me-+!@!n!t", "base.path.n@ew.@eg!.me-+!@!n!t" };
                yield return new[] { "base.path", "new. .value", "base.path.new. .value" };
                yield return new[] { "base. ", "new. .value", "base. .new. .value" };
                yield return new[] { "base", "   ", "base.   " };
                yield return new[] { "base", "<   ", "   " };
                yield return new[] { "", "<   ", "   " };
                yield return new[] { "base", " <test", "base. <test" };
                yield return new[] { "base.#.item", "<", "base.#" };
            }

            public static IEnumerable<object[]> NewSegmentGoesLevelUp()
            {
                yield return new[] { "base", "<segment", "segment" };
                yield return new[] { "base.path", "<segment", "base.segment" };
                yield return new[] { "base.path", "<<segment", "segment" };
                yield return new[] { "base.path", "<new.segment", "base.new.segment" };
                yield return new[] { "base.path", "<<new.segment", "new.segment" };
            }

            public static IEnumerable<object[]> NewSegmentIsEmpty_And_GoesLevelUp()
            {
                yield return new[] { "base", "<", "" };
                yield return new[] { "base.path", "<", "base" };
                yield return new[] { "base.path", "<<", "" };
            }

            public static IEnumerable<object[]> NewSegmentGoesLevelUp_And_ExceedsMinimumLevel()
            {
                yield return new[] { "base", "<<<segment", "segment" };
                yield return new[] { "base.path", "<<<segment", "segment" };
                yield return new[] { "base.path", "<<<<<segment", "segment" };
                yield return new[] { "base.path", "<<<", "" };
                yield return new[] { "base.path", "<<<<<", "" };
                yield return new[] { "", "<", "" };
                yield return new[] { "", "<<<", "" };
            }
        }

        public static IEnumerable<object[]> GetWithIndexes_AllCases()
        {
            var cases = new[]
            {
                GetWithIndexes.CommonCases(),
                GetWithIndexes.TrickyCases(),
                GetWithIndexes.LargeIndexes()
            };

            foreach (var @case in cases)
            {
                foreach (var set in @case)
                {
                    yield return set;
                }
            }
        }

        public class GetWithIndexes
        {
            public static IEnumerable<object[]> CommonCases()
            {
                yield return new object[] { "path.#", GetIndexesStack(1), "path.#0" };
                yield return new object[] { "path.#.segment", GetIndexesStack(1), "path.#0.segment" };
                yield return new object[] { "path.#.segment.#.another.path", GetIndexesStack(2), "path.#0.segment.#1.another.path" };
                yield return new object[] { "path.#.#.segment", GetIndexesStack(2), "path.#0.#1.segment" };
                yield return new object[] { "#", GetIndexesStack(1), "#0" };
                yield return new object[] { "#.#", GetIndexesStack(2), "#0.#1" };
                yield return new object[] { "#.#.test.#.#.path.#.#", GetIndexesStack(6), "#0.#1.test.#2.#3.path.#4.#5" };
            }

            public static IEnumerable<object[]> TrickyCases()
            {
                yield return new object[] { "##.#.#", GetIndexesStack(2), "##.#0.#1" };
                yield return new object[] { "##.##.##", GetIndexesStack(2), "##.##.##" };
                yield return new object[] { "####", GetIndexesStack(4), "####" };
                yield return new object[] { "#a.#.b#.#", GetIndexesStack(2), "#a.#0.b#.#1" };
                yield return new object[] { "# . #.#.  #", GetIndexesStack(1), "# . #.#0.  #" };
                yield return new object[] { "", GetIndexesStack(2), "" };
            }

            public static IEnumerable<object[]> LargeIndexes()
            {
                Func<int, int> process = m => (1 + m) * 100;

                yield return new object[] { "path.#", GetIndexesStack(1, process), "path.#100" };
                yield return new object[] { "path.#.segment", GetIndexesStack(1, process), "path.#100.segment" };
                yield return new object[] { "path.#.segment.#.another.path", GetIndexesStack(2, process), "path.#100.segment.#200.another.path" };
                yield return new object[] { "path.#.#.segment", GetIndexesStack(2, process), "path.#100.#200.segment" };
                yield return new object[] { "#", GetIndexesStack(1, process), "#100" };
                yield return new object[] { "#.#", GetIndexesStack(2, process), "#100.#200" };
                yield return new object[] { "#.#.test.#.#.path.#.#", GetIndexesStack(6, process), "#100.#200.test.#300.#400.path.#500.#600" };
            }

            private static Stack<string> GetIndexesStack(int count, Func<int, int> process = null)
            {
                var stack = new Stack<string>();

                foreach (var i in Enumerable.Range(0, count).Select(m => process != null ? process(m) : m))
                {
                    stack.Push(i.ToString(CultureInfo.InvariantCulture));
                }

                return stack;
            }
        }

        public class GetIndexesAmount
        {
            public static IEnumerable<object[]> NoIndexes()
            {
                yield return new object[] { "" };
                yield return new object[] { "path.segment" };
                yield return new object[] { "path.segment.new" };
            }

            public static IEnumerable<object[]> InvalidIndexes()
            {
                yield return new object[] { "# " };
                yield return new object[] { " #" };
                yield return new object[] { " #1.path.##" };
                yield return new object[] { "path.#1d.#x.segm##ent" };
                yield return new object[] { "path. 1#. # .segm##ent.# " };
            }

            public static IEnumerable<object[]> WeirdIndexes()
            {
                yield return new object[] { "#1.#", 2 };
                yield return new object[] { "#.path.#1", 2 };
                yield return new object[] { "path.#1.#.segm##ent", 2 };
            }

            public static IEnumerable<object[]> ResolvedIndexes()
            {
                yield return new object[] { "#1", 1 };
                yield return new object[] { "path.#1", 1 };
                yield return new object[] { "path.#1.segment", 1 };
                yield return new object[] { "path.#1.#2.segment", 2 };
                yield return new object[] { "path.#1.#2.segment.#3", 3 };
                yield return new object[] { "path.#1.#2.#3.segment", 3 };
                yield return new object[] { "path.#1.#2.#3.segment.#4.#5.#6", 6 };
            }

            public static IEnumerable<object[]> PlaceholdersIndexes()
            {
                yield return new object[] { "#1", 1 };
                yield return new object[] { "path.#1", 1 };
                yield return new object[] { "path.#1.segment", 1 };
                yield return new object[] { "path.#1.#2.segment", 2 };
                yield return new object[] { "path.#1.#2.segment.#3", 3 };
                yield return new object[] { "path.#1.#2.#3.segment", 3 };
                yield return new object[] { "path.#1.#2.#3.segment.#4.#5.#6", 6 };
            }
        }
    }
}
