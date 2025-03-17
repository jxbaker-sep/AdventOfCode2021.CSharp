using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;

namespace AdventOfCode2021.CSharp;

public class Day09
{
  
  [Theory]
  [InlineData("Day09.Sample", 15)]
  [InlineData("Day09", 558)]
  public void Part1(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    grid.Where(it => !it.Key.CardinalNeighbors().Any(n => grid.TryGetValue(n, out var temp) && temp <= it.Value))
      .Sum(it => it.Value + 1)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day09.Sample", 1134)]
  [InlineData("Day09", 882942)]
  public void Part2(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    var lowpoints = grid.Where(it => !it.Key.CardinalNeighbors().Any(n => grid.TryGetValue(n, out var temp) && temp <= it.Value));

    List<long> basinSizes = [];
    foreach(var lp in lowpoints)
    {
      Queue<(Point, long)> open = [];
      open.Enqueue((lp.Key, lp.Value));
      HashSet<Point> closed = [];
      closed.Add(lp.Key);
      while (open.TryDequeue(out var current))
      {
        foreach(var n in current.Item1.CardinalNeighbors())
        {
          if (!grid.TryGetValue(n, out var nv)) continue;
          if (nv == 9) continue;
          if (!closed.Add(n)) continue;
          open.Enqueue((n, nv));
        }
      }
      basinSizes.Add(closed.Count);
    }

    basinSizes.OrderByDescending(it=>it).Take(3).Product().Should().Be(expected);
  }

  private static Dictionary<Point, long> Convert(List<string> input)
  {
    return input.Gridify().ToDictionary(it => it.Key, it => (long)(it.Value - '0'));
  }
} 