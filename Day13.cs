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
  }

  static List<Point> DoFold(List<Point> points, Fold fold)
  {
    if (fold.Axis == "y") {
      var max = points.Max(it => it.Y);
      (fold.Index * 2).Should().Be(max);
      return points.Select(it => {
        if (it.Y > fold.Index) {
          return new Point(max - it.Y, it.X);
        }
        return it;
      }).Distinct().ToList();
    }
    else
    {
      var max = points.Max(it => it.X);
      (fold.Index * 2).Should().Be(max);
      return points.Select(it => {
        if (it.X > fold.Index) {
          return new Point(it.Y, max - it.X);
        }
        return it;
      }).Distinct().ToList();
    }
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