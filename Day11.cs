using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;

namespace AdventOfCode2021.CSharp;

public class Day11
{
  
  [Theory]
  [InlineData("Day11.Sample", 1656, 195)]
  [InlineData("Day11", 1749, 285)]
  public void Part1(string file, long expected, long expected2)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    long totalFlashes = 0;
    long? firstAllFlashed = null;

    for(long i = 1; firstAllFlashed == null; i++)
    {
      grid = grid.ToDictionary(it => it.Key, it => it.Value + 1);

      HashSet<Point> closed = [];
      while (grid.Any(it => it.Value > 9 && !closed.Contains(it.Key)))
      {
        var current = grid.First(it => it.Value > 9 && !closed.Contains(it.Key));
        closed.Add(current.Key);
        if (i <= 100) totalFlashes += 1;
        foreach(var n in current.Key.CompassRoseNeighbors())
        {
          if (grid.ContainsKey(n)) grid[n] += 1;
        }
      }
      foreach(var pt in closed) grid[pt] = 0;
      if (closed.Count == grid.Count && firstAllFlashed == null) firstAllFlashed = i;
    }

    totalFlashes.Should().Be(expected);
    firstAllFlashed.Should().Be(expected2);
  }

  private static Dictionary<Point, long> Convert(List<string> input)
  {
    return input.Gridify().ToDictionary(it => it.Key, it => (long)(it.Value - '0'));
  }
} 