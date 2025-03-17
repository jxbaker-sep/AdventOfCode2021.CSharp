using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;

namespace AdventOfCode2021.CSharp;

public class Day11
{
  
  [Theory]
  [InlineData("Day11.Sample", 1656)]
  [InlineData("Day11", 1749)]
  public void Part1(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    long totalFlashes = 0;

    foreach(var _ in Enumerable.Range(0, 100))
    {
      grid = grid.ToDictionary(it => it.Key, it => it.Value + 1);

      HashSet<Point> closed = [];
      while (grid.Any(it => it.Value > 9 && !closed.Contains(it.Key)))
      {
        var current = grid.First(it => it.Value > 9 && !closed.Contains(it.Key));
        closed.Add(current.Key);
        totalFlashes += 1;
        foreach(var n in current.Key.CompassRoseNeighbors())
        {
          if (grid.ContainsKey(n)) grid[n] += 1;
        }
      }
      foreach(var pt in closed) grid[pt] = 0;
    }

    totalFlashes.Should().Be(expected);
  }

  private static Dictionary<Point, long> Convert(List<string> input)
  {
    return input.Gridify().ToDictionary(it => it.Key, it => (long)(it.Value - '0'));
  }
} 