using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day13
{

  [Theory]
  [InlineData("Day13.Sample", 17)]
  [InlineData("Day13", 781)]
  public void Part1(string file, int expected)
  {
    var world = Convert(AoCLoader.LoadFile(file));

    DoFold(world.Points, world.Folds[0]).Count.Should().Be(expected);

    var result = world.Folds.Aggregate(world.Points, DoFold);

    var minx = result.Min(it => it.X);
    var miny = result.Min(it => it.Y);
    var maxx = result.Max(it => it.X);
    var maxy = result.Max(it => it.Y);
    for (var y = miny; y <= maxy; y++)
    {
      for (var x = minx; x <= maxx; x++)
      {
        Console.Write(result.Contains(new(y,x)) ? '#' : ' ');
      }
      Console.WriteLine();
    }
  }

  static List<Point> DoFold(List<Point> points, Fold fold)
  {
    Func<Point, long> access = it => it.Y;
    Func<Point, Point> shift = it => new(fold.Index * 2 - it.Y, it.X);

    if (fold.Axis == "x") {
      access = it => it.X;
      shift = it => new(it.Y, fold.Index * 2 - it.X);
    }

    return points.Select(it =>
    {
      if (access(it) > fold.Index)
      {
        return shift(it);
      }
      return it;
    }).Distinct().ToList();
  }

  public record Fold(string Axis, long Index);
  public record World(List<Point> Points, List<Fold> Folds);

  private static World Convert(string input)
  {
    var pps = input.Split("\n\n").Select(it => it.Split("\n").ToList()).ToList();

    var points = P.Format("{},{}", P.Long, P.Long).Select(it => new Point(it.Second, it.First)).ParseMany(pps[0]);
    var folds = P.Format("fold along {}={}", P.Enum("x", "y"), P.Long).Select(it => new Fold(it.First, it.Second)).ParseMany(pps[1]);

    return new(points, folds);
  }
}