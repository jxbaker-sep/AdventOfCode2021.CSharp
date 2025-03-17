using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day07
{
  [Theory]
  [InlineData("Day07.Sample", 37)]
  [InlineData("Day07", 345197)]
  public void Part1(string file, long expected)
  {
    var fishes = Convert(AoCLoader.LoadLines(file).Single());

    var result = MiscUtils.InclusiveRange(fishes.Min(), fishes.Max())
      .Select(pos => fishes.Select(it => Math.Abs(pos - it)).Sum())
      .Min();

    result.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day07.Sample", 168)]
  [InlineData("Day07", 96361606)]
  public void Part2(string file, long expected)
  {
    var fishes = Convert(AoCLoader.LoadLines(file).Single());

    var result = MiscUtils.InclusiveRange(fishes.Min(), fishes.Max())
      .Select(pos => fishes.Select(it => MathUtils.Triangle(Math.Abs(pos - it))).Sum())
      .Min();

    result.Should().Be(expected);
  }

  private static List<long> Convert(string input)
  {
    return P.Long.Plus(",").Parse(input);
  }
}