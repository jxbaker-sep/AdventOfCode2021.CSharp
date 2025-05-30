using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day06
{
  [Theory]
  [InlineData("Day06.Sample", 80, 5934)]
  [InlineData("Day06", 80, 380612)]
  [InlineData("Day06.Sample", 256, 26984457539)]
  [InlineData("Day06", 256, 1710166656900)]
  public void Part1(string file, int iterations, long expected)
  {
    var fishes = Convert(AoCLoader.LoadLines(file).Single());
    Breed(fishes, iterations).Sum().Should().Be(expected);
  }

  private static List<long> Breed(List<long> fishes, int iterations)
  {
    return Enumerable.Range(0, iterations)
      .Aggregate(fishes, (current, _) => Enumerable.Range(0, 9).Select(index =>
      {
        var result = index switch
        {
          6 => current[index + 1] + current[0],
          < 8 => current[index + 1],
          _ => current[0]
        };
        return result;
      }).ToList());
  }

  private static List<long> Convert(string input)
  {
    var x = P.Long.Plus(",").Parse(input).GroupToCounts();
    return Enumerable.Range(0, 9).Select(index => x.GetValueOrDefault(index)).ToList();
  }
}