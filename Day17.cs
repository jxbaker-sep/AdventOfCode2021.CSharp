using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day17
{
  [Theory]
  [InlineData("Day17.Sample", 45)]
  [InlineData("Day17", 8911)]
  public void Part1(string file, long expected)
  {
    var targetArea = Parse(AoCLoader.LoadLines(file).Single());

    // times when X intersects target area.
    HashSet<long> xts = [];
    long infiniT = long.MaxValue;

    for (var startdx = targetArea.X2; startdx > 0; startdx--)
    {
      long x = 0;
      var t = 1;
      for (var dx = startdx; dx > 0 && x <= targetArea.X2; dx--, t++)
      {
        x += dx;
        if (targetArea.X1 <= x && x <= targetArea.X2) {
          xts.Add(t);
          if (dx == 1) infiniT = Math.Min(infiniT, t);
        }
      }
    }

    long maxY = 0;
    bool found = false;
    for (var startdy = Math.Abs(targetArea.Y2); startdy > 0 && !found ; startdy--)
    {
      long y = 0;
      var t = startdy * 2 + 1; // when velocity crosses 0 again
      long dy = -(startdy+1); // falling
      while (y >= targetArea.Y2 && !found)
      {
        y += dy;
        if (targetArea.Y1 >= y && y >= targetArea.Y2) {
          if (xts.Contains(t) || t >= infiniT) {
            maxY = Math.Max(MathUtils.Triangle(startdy), maxY);
            found = true;
            break;
          }
        }
        dy -= 1;
        t += 1;
      }
    }

    maxY.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day17.Sample", 112)]
  [InlineData("Day17", 4748)]
  public void Part2(string file, long expected)
  {
    var targetArea = Parse(AoCLoader.LoadLines(file).Single());

    // times when X intersects target area.
    Dictionary<long, List<long>> t2xs = [];
    Dictionary<long, long> infinities = [];

    for (var startdx = targetArea.X2; startdx > 0; startdx--)
    {
      long x = 0;
      var t = 1;
      for (var dx = startdx; dx > 0 && x <= targetArea.X2; dx--, t++)
      {
        x += dx;
        if (targetArea.X1 <= x && x <= targetArea.X2) {
          t2xs[t] = [.. (t2xs.GetValueOrDefault(t) ?? []), startdx];
          if (dx == 1) {
            infinities.Should().NotContainKey(t + 1);
            infinities[t + 1] = startdx;
          }
        }
      }
    }

    HashSet<(long,long)> count = [];

    for (var startdy = targetArea.Y2; startdy <= -targetArea.Y2 ; startdy++)
    {
      long y = 0;
      long t = 1; 
      long dy = startdy;
      if (startdy > 0) {
        dy = -(startdy + 1); // falling
        t = startdy * 2 + 2; // when velocity crosses 0 again
      }
      while (y >= targetArea.Y2)
      {
        y += dy;
        if (targetArea.Y1 >= y && y >= targetArea.Y2) {
          foreach (var x in t2xs.GetValueOrDefault(t) ?? []) {
            count.Add((startdy, x));
          }
          foreach (var x in infinities.Where(tx => tx.Key <= t))
          {
            count.Add((startdy, x.Value));
          }
        }
        dy -= 1;
        t += 1;
      }
    }

    count.Count.Should().Be((int)expected);
  }

  public record TargetArea(long X1, long X2, long Y1, long Y2);

  private static TargetArea Parse(string line)
  {
    return P.Format("target area: x={}..{}, y={}..{}", P.Long, P.Long, P.Long, P.Long)
      .Select(it => new TargetArea(it.First, it.Second, Math.Max(it.Third, it.Fourth), Math.Min(it.Third, it.Fourth)))
      .Parse(line);
  }
}