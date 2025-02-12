
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day01
{
  [Theory]
  [InlineData("Day01.Sample", 514579)]
  [InlineData("Day01", 270144)]
  public void Part1(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();

  
  }


  private static List<long> Convert(List<string> list)
  {
    return P.Long.ParseMany(list);
  }
}