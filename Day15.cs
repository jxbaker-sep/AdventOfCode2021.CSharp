using System.ComponentModel;
using System.Net.Mail;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day15
{

  [Theory]
  [InlineData("Day15.Sample", 40)]
  [InlineData("Day15", 0)]
  public void Part1(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    FindLeastRiskyPath(grid).Should().Be(expected);
  }

  static long FindLeastRiskyPath(Grid<long> grid)
  {
    var goal = new Point(grid.Width - 1, grid.Height - 1);
    PriorityQueue<(Point Point, List<Point> Path, long Risk)> open = new(it => it.Risk + it.Point.ManhattanDistance(goal));
    open.Enqueue((Point.Zero, [Point.Zero], 0));


    while (open.TryDequeue(out var current))
    {
      foreach(var n in current.Point.CardinalNeighbors())
      {
        if (current.Path.Contains(n)) continue;
        if (!grid.TryGetValue(n, out var risk)) continue;
        if (n == goal) return current.Risk + risk;
        open.Enqueue((n, [..current.Path, n], current.Risk + risk));
      }
    }

    throw new ApplicationException();
  }

  private static Grid<long> Convert(List<string> input)
  {
    return input.Gridify(it => (long)(it - '0'));
  }
}