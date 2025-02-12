
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Xunit;
using Utils;
using P = Parser.ParserBuiltins;
using System.Collections.Generic;

namespace AdventOfCode2021.CSharp;

public class Day01
{
  [Theory]
  [InlineData("Day01.Sample", 7, 5)]
  [InlineData("Day01", 1502, 1538)]
  public void Part1(string file, int expected1, int expected2)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    data.Windows(2).Count(pair => pair[0] < pair[1]).Should().Be(expected1);

    data.Windows(3).Select(it => it.Sum()).Windows(2).Count(pair => pair[0] < pair[1]).Should().Be(expected2);
  }


  private static List<long> Convert(List<string> list)
  {
    return P.Long.ParseMany(list);
  }
}