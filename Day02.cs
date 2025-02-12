
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day02
{
  [Theory]
  [InlineData("Day02.Sample", 150)]
  [InlineData("Day02", 1804520)]
  public void Part1(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));
    var pos = data.Aggregate(Point.Zero, (acc, next) => acc + next);
    (pos.X * pos.Y).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day02.Sample", 900)]
  [InlineData("Day02", 1971095320)]
  public void Part2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));
    var pos = Point.Zero;
    long aim = 0;
    foreach(var v in data)
    {
      if (v.Y != 0) { aim += v.Y; continue; }
      pos += new Vector(v.X * aim, v.X);
    }
    (pos.X * pos.Y).Should().Be(expected);
  }


  private static List<Vector> Convert(List<string> list)
  {
    var fwd = P.Format("forward {}", P.Long).Select(it => new Vector(0, it));
    var down = P.Format("down {}", P.Long).Select(it => new Vector(it, 0));
    var up = P.Format("up {}", P.Long).Select(it => new Vector(-it, 0));

    return (fwd | down | up).ParseMany(list);
  }
}