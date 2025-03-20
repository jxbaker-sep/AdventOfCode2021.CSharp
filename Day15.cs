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
  [InlineData("Day15", 487)]
  public void Part1(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    FindLeastRiskyPath(grid).Should().Be(expected);
  }

  static long FindLeastRiskyPath(Grid<long> grid)
  {
    var goal = new Point(grid.Width - 1, grid.Height - 1);
    Dictionary<Point, long> closed = [];
    closed[Point.Zero] = 0;

    Queue<(Point Point, long Risk)> open = [];
    open.Enqueue((Point.Zero, 0));

    while (open.TryDequeue(out var current))
    {
      if (closed.TryGetValue(current.Point, out var preexisting) && preexisting < current.Risk) continue;
      foreach(var n in current.Point.CardinalNeighbors()) {
        if (grid.TryGetValue(n, out var risk)) {
          if (closed.TryGetValue(n, out var existing) && existing <= current.Risk + risk) continue;
          closed[n] = current.Risk + risk;
          open.Enqueue((n, current.Risk + risk));
        }
      }
    }

    return closed[goal];
  }

  private static Grid<long> Convert(List<string> input)
  {
    return input.Gridify(it => (long)(it - '0'));
  }
}