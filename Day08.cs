using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day08
{
  const int Length1 = 2;
  const int Length4 = 4;
  const int Length7 = 3;
  const int Length8 = 7;
  [Theory]
  [InlineData("Day08.Sample", 26)]
  [InlineData("Day08", 303)]
  public void Part1(string file, long expected)
  {
    var items = Convert(AoCLoader.LoadLines(file));

    var result = items.SelectMany(it => it.Output).LongCount(it => new[]{Length1, Length4, Length7, Length8 }.Contains(it.Length));

    result.Should().Be(expected);
  }

  public record Item(List<string> Patterns, List<string> Output);

  private static List<Item> Convert(List<string> input)
  {
    return P.Format("{}|{}", P.Word.Trim().Plus(), P.Word.Trim().Plus())
      .Select(it => new Item(it.First, it.Second))
      .ParseMany(input);
  }
} 