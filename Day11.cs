using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
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

      Queue<Point> readyToFlash = grid.Where(it => it.Value > 9).Select(it=>it.Key).ToQueue();

      HashSet<Point> flashed = [.. readyToFlash];
      while (readyToFlash.TryDequeue(out var current))
      {
        if (i <= 100) totalFlashes += 1;
        foreach(var n in current.CompassRoseNeighbors())
        {
          if (grid.TryGetValue(n, out var nv)) {
            nv += 1;
            grid[n] = nv;
            if (nv > 9 && flashed.Add(n)) readyToFlash.Enqueue(n);
          }
        }
      }
      foreach(var pt in flashed) grid[pt] = 0;
      if (flashed.Count == grid.Count) firstAllFlashed = i;
    }

    totalFlashes.Should().Be(expected);
    firstAllFlashed.Should().Be(expected2);
  }

  private static Dictionary<Point, long> Convert(List<string> input)
  {
    return input.Gridify().ToDictionary(it => it.Key, it => (long)(it.Value - '0'));
  }
} 