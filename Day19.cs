using System.Xml.XPath;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using Xunit.Sdk;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public partial class Day19
{
  [Theory]
  [InlineData("Day19.Sample", 79, 3621)]
  [InlineData("Day19", 419, 13210)]
  public void Part1(string file, int expected, long expected2)
  {
    var scanners = Parse(AoCLoader.LoadFile(file));
    while (scanners.Count > 1)
    {
      var i = 0;
      var j = 1;
      while (i < scanners.Count - 1)
      {
        Console.WriteLine((i, j, scanners.Count));
        var overlap2 = FindOverlap(scanners[i], scanners[j], 12);
        if (overlap2 is { } overlap)
        {
          var points = new HashSet<Point3>([.. scanners[i].Points, .. overlap.Points]);
          scanners[i].ScannerLocations.AddRange(scanners[j].ScannerLocations.Select(p => p.Apply(overlap.Translation)));
          scanners[i].Points.Clear();
          scanners[i].Points.AddRange(points);
          scanners.RemoveAt(j);
          j -= 1;
        }
        j += 1;
        if (j >= scanners.Count)
        {
          i += 1;
          j = i + 1;
        }
      }
    }

    scanners.Count.Should().Be(1);
    scanners[0].Points.Count.Should().Be(expected);
    scanners[0].ScannerLocations.Pairs().Select(it => it.First.ManhattanDistance(it.Second)).Max().Should().Be(expected2);
  }

  [Theory]
  [InlineData("Day19.Sample")]
  public void Debug(string file)
  {
    var scanners = Parse(AoCLoader.LoadFile(file));
    var overlap2 = FindOverlap(scanners[0], scanners[1], 12);

    overlap2.Should().NotBeNull();
  }

  [Fact]
  public void Sanity2()
  {
    var scanners = Parse(@"--- scanner 0 ---
1,2,0
0,-1,-2

--- scanner 1 ---
0,1,2
-1,-2,0");

    var overlap = FindOverlap(scanners[0], scanners[1], 2) ?? throw new ApplicationException();
    overlap.Points.Should().HaveCount(2);
  }

  [Fact]
  public void Sanity3()
  {
    string input = AoCLoader.LoadFile("Day19.Sample");
    var scanners = Parse(input);
    var overlap = FindOverlap(scanners[0], scanners[1], 12) ?? throw new ApplicationException();
    scanners[0].Points.Clear();
    scanners[0].Points.AddRange(new HashSet<Point3>([.. scanners[0].Points, .. overlap.Points]));
    overlap = FindOverlap(scanners[0], scanners[4], 12) ?? throw new ApplicationException();

    (scanners[0].Points.Intersect(overlap.Points)).Should().HaveCount(12);
  }

  record Point3(long X, long Y, long Z)
  {
    public long ManhattanDistance(Point3 other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
    public Point3 Orient(Orientation orient)
    {
      var result = this;
      if (orient.Roll == Roll.X) { } // do nothing
      else if (orient.Roll == Roll.Y)
      {
        result = new Point3(result.Y, -result.X, result.Z);
      }
      else if (orient.Roll == Roll.Z)
      {
        result = new Point3(result.Z, result.Y, -result.X);
      }

      for (var i = 0; i < orient.Rotations; i++) result = new Point3(result.X, -result.Z, result.Y);

      if (orient.Flipped)
      {
        result = new Point3(-result.X, -result.Y, result.Z);
      }

      return result;
    }

    public Point3 Apply(Translation t) => Orient(t.Orientation) + t.Delta;

    public static Point3 operator -(Point3 lhs, Point3 rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
    public static Point3 operator +(Point3 lhs, Point3 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
  }
  record Scanner(List<Point3> Points, List<Point3> ScannerLocations);

  enum Roll { X, Y, Z };
  record Orientation(Roll Roll, int Rotations, bool Flipped);
  record Translation(Orientation Orientation, Point3 Delta);

  // returns translated list of points of at least minOverlap overlap
  static (Translation Translation, List<Point3> Points)? FindOverlap(Scanner lhs, Scanner rhs, int minOverlap)
  {
    foreach (var roll in new[] { Roll.X, Roll.Y, Roll.Z })
    {
      foreach (var rotations in Enumerable.Range(0, 4))
      {
        foreach (var flipped in new[] { false, true })
        {
          var orientation = new Orientation(roll, rotations, flipped);
          var oriented = rhs.Points.Select(p => p.Orient(orientation)).ToList();
          foreach (var point in oriented)
          {
            foreach (var other in lhs.Points)
            {
              var delta = other - point;
              var translated = oriented.Select(p => p + delta).ToList();
              int v = lhs.Points.Intersect(translated).Count();
              if (v >= minOverlap)
              {
                Translation translation = new(orientation, delta);
                // return (new Point3(0, 0, 0).Apply(translation), translated);
                return (translation, translated);
              }
            }
          }
        }
      }
    }

    return null;
  }

  static List<Scanner> Parse(string s)
  {
    List<Scanner> result = [];
    Scanner scanner = new([], [new(0,0,0)]);
    var pointMatcher = P.Format("{},{},{}", P.Long, P.Long, P.Long)
      .Select(it => new Point3(it.First, it.Second, it.Third));
    foreach (var line in s.Lines())
    {
      if (line.StartsWith("--- scanner"))
      {
        scanner = new([], [new(0,0,0)]);
        result.Add(scanner);
      }
      else if (!string.IsNullOrEmpty(line))
      {
        scanner.Points.Add(pointMatcher.Parse(line));
      }
    }

    return result;
  }
}