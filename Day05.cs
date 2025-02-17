
using System.ComponentModel;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day05
{
  [Theory]
  [InlineData("Day05.Sample", 5)]
  [InlineData("Day05", 6189)]
  public void Part1(string file, int expected)
  {
    var world = Convert(AoCLoader.LoadLines(file));
    
    world.Where(it => it.Item1.X == it.Item2.X || it.Item1.Y == it.Item2.Y)
      .SelectMany(Open)
      .GroupToDictionary(it => it, it => it)
      .Count(it => it.Value.Count > 1)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 12)]
  [InlineData("Day05", 19164)]
  public void Part2(string file, int expected)
  {
    var world = Convert(AoCLoader.LoadLines(file));
    
    world
      .SelectMany(Open)
      .GroupToDictionary(it => it, it => it)
      .Count(it => it.Value.Count > 1)
      .Should().Be(expected);
  }

  private IEnumerable<object> Open((Point, Point) points)
  {
    var vector = new Vector(Math.Sign(points.Item2.Y - points.Item1.Y), Math.Sign(points.Item2.X - points.Item1.X));
    var current = points.Item1;
    while (true)
    {
      yield return current;
      if (current == points.Item2) break;
      current += vector;
    }
  }

  private static List<(Point, Point)> Convert(List<string> list)
  {
    return P.Format("{},{} -> {},{}", P.Long, P.Long, P.Long, P.Long)
      .Select(it => (new Point(it.Second, it.First), new Point(it.Fourth, it.Third)))
      .ParseMany(list);
  }
}