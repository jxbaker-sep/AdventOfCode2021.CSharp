
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day03
{
  [Theory]
  [InlineData("Day03.Sample", 198)]
  [InlineData("Day03", 2972336)]
  public void Part1(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));
    long r1 = 0;
    var rows = grid.Max(it => it.Key.Y);
    var cols = grid.Max(it => it.Key.X) + 1;
    foreach(var col in MiscUtils.LongRange(0, cols))
    {
      var ones = grid.Where(it => it.Key.X == col).Count(it => it.Value == '1');
      r1 = (r1 << 1) + ((ones > rows / 2) ? 1 : 0);
    }
    long zed = MathUtils.LongPow(2, cols) - 1;
    (r1 * (~r1 & zed)).Should().Be(expected);
  }



  private static Dictionary<Point, char> Convert(List<string> list)
  {
    return list.Gridify();
  }
}