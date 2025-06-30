using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2021.CSharp;

public class Day22
{
  [Theory]
  [InlineData("Day22.Sample.1", 39)]
  [InlineData("Day22.Sample.2", 590784)]
  [InlineData("Day22", 648023)]
  public void Part1(string file, long expected)
  {
    var data = Parse(AoCLoader.LoadLines(file));

    var regions = data.Aggregate(new List<Region>(), (acc, it) => Operate(it, acc));

    var olap = new Segment(-50, 50);
    regions = regions.Where(r => r.X.Overlap(olap) != null && r.Y.Overlap(olap) != null && r.Z.Overlap(olap) != null)
      .Select(r => r with {X = r.X.Overlap(olap)!, Y = r.Y.Overlap(olap)!, Z = r.Z.Overlap(olap)!})
      .ToList();

    regions.Sum(r => r.Size).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day22.Sample.3", 2758514936282235)]
  [InlineData("Day22", 1285677377848549)]
  public void Part2(string file, long expected)
  {
    var data = Parse(AoCLoader.LoadLines(file));

    var regions = data.Aggregate(new List<Region>(), (acc, it) => Operate(it, acc));

    regions.Sum(r => r.Size).Should().Be(expected);
  }

  private static List<Region> Operate(Instruction instruction, List<Region> regions)
  {
    var result = regions.SelectMany(it => it.Slice(instruction.Region)).ToList();
    if (instruction.Selector == Selector.On)
    {
      result.Add(instruction.Region);
    }
    return result;
  }

  private record Region(Segment X, Segment Y, Segment Z)
  {
    public long Size => X.Size * Y.Size * Z.Size;

    // Returns a list of regions that overlap "this" that do not overlap "other".
    public List<Region> Slice(Region other)
    {
      var overlapx = X.Overlap(other.X);
      var overlapy = Y.Overlap(other.Y);
      var overlapz = Z.Overlap(other.Z);
      if (overlapx == null || overlapy == null || overlapz == null)
      {
        return [this];
      }
      var slicex = X.Slice(other.X);
      var slicey = Y.Slice(other.Y);
      var slicez = Z.Slice(other.Z);
      List<Region> result = [];
      // Each X with full range of others
      foreach (var sx in slicex)
      {
        result.Add(this with { X = sx });
      }
      // Each Y with overlap X and full Z
      foreach (var sy in slicey)
      {
        result.Add(this with { X = overlapx, Y = sy });
      }
      // each Z with overlap X and Y
      foreach (var sz in slicez)
      {
        result.Add(this with { X = overlapx, Y = overlapy, Z = sz });
      }

      return result;
    }
  }

  private enum Selector { On, Off };

  private record Segment(long First, long Last)
  {
    public long Size => Last - First + 1;

    public Segment? Overlap(Segment other)
    {
      if (First > other.Last) return null;
      if (Last < other.First) return null;
      var first = Math.Max(First, other.First);
      var second = Math.Min(Last, other.Last);
      return new(first, second);
    }

    // returns a list of Segments that overlap "this" that do not overlap "other"
    public List<Segment> Slice(Segment other)
    {
      var overlap = Overlap(other);
      if (overlap == null)
      {
        return [this];
      }
      List<Segment> result = [];
      if (overlap.First > First)
      {
        result.Add(new(First, overlap.First - 1));
      }
      if (overlap.Last < Last)
      {
        result.Add(new(overlap.Last + 1, Last));
      }
      return result;
    }
  }

  private record Instruction(Selector Selector, Region Region);

  private static List<Instruction> Parse(List<string> lines)
  {
    var pairp = P.Format("{}={}..{}", P.Any, P.Long, P.Long).Select(it => new Segment(it.Second, it.Third));
    return P.Format("{} {},{},{}",
      P.Enum("on", "off"),
      pairp, pairp, pairp)
    .Select(it => new Instruction(it.First == "on" ? Selector.On : Selector.Off, new(it.Second, it.Third, it.Fourth)))
    .ParseMany(lines);
  }
}