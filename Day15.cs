using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.Serialization;
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

  [Theory]
  [InlineData("Day15.Sample", 315)]
  [InlineData("Day15", 2821)]
  public void Part2(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    var biggrid = new Grid<long>(MiscUtils.LongRange(0, grid.Height * 5).Select(_ => MiscUtils.LongRange(0, grid.Width * 5)));

    foreach (var y in Enumerable.Range(0, 5))
    {
      foreach (var x in Enumerable.Range(0, 5))
      {
        foreach (var (Key, Value) in grid.Items())
        {
          long value = Value + y + x;
          if (value > 9) value -= 9;
          biggrid[new Point(x * grid.Width + Key.X, y * grid.Height + Key.Y)] = value;
        }
      }
    }

    FindLeastRiskyPath(biggrid).Should().Be(expected);
  }

  static long FindLeastRiskyPath(Grid<long> grid)
  {
    var goal = new Point(grid.Width - 1, grid.Height - 1);
    HashSet<Point> closed = [Point.Zero];

    PriorityQueue<(Point Point, long Risk)> open = new(it => it.Risk);
    open.Enqueue((Point.Zero, 0));

    while (open.TryDequeue(out var current))
    {
      foreach (var n in current.Point.CardinalNeighbors())
      {
        if (!grid.TryGetValue(n, out var risk)) continue;
        if (!closed.Add(n)) continue;
        if (n == goal) return current.Risk + risk;
        open.Enqueue((n, current.Risk + risk));
      }
    }

    throw new ApplicationException();
  }

  private static Grid<long> Convert(List<string> input)
  {
    return input.Gridify(it => (long)(it - '0'));
  }
}