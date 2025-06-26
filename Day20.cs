using System.Xml.XPath;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using Xunit.Sdk;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day20
{
  [Theory]
  [InlineData("Day20.Sample", 35)]
  [InlineData("Day20", 5489)]
  public void Part1(string file, int expected)
  {
    var world = Parse(AoCLoader.LoadFile(file));
    world.Enhance(false).Enhance(true).Image.Count(it => it.Value == Pixel.Bright).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day20.Sample", 3351)]
  [InlineData("Day20", 19066)]
  public void Part2(string file, int expected)
  {
    var world = Parse(AoCLoader.LoadFile(file));
    for (var index = 0; index < 50; index ++)
      world = world.Enhance(index % 2 == 1);
    world.Image.Count(it => it.Value == Pixel.Bright).Should().Be(expected);
  }

  public enum Pixel { Dark, Bright };

  public record World(List<Pixel> Enhancement, Dictionary<Point, Pixel> Image)
  {
    public World Enhance(bool isOn)
    {
      var ps = Image.Keys.SelectMany(p => p.CompassRoseNeighbors()).ToHashSet();
      Dictionary<Point, Pixel> next = [];
      foreach (var p in ps)
      {
        next[p] = Enhancement[GetIndex(p, isOn)];
      }
      return new(Enhancement, next);
    }

    private int GetIndex(Point p, bool isOn)
    {
      return
        (1 << 8) * (GetVal(p + new Vector(-1, -1), isOn)) +
        (1 << 7) * (GetVal(p + new Vector(0, -1), isOn)) +
        (1 << 6) * (GetVal(p + new Vector(1, -1), isOn)) +
        (1 << 5) * (GetVal(p + new Vector(-1, 0), isOn)) +
        (1 << 4) * (GetVal(p + new Vector(0, 0), isOn)) +
        (1 << 3) * (GetVal(p + new Vector(1, 0), isOn)) +
        (1 << 2) * (GetVal(p + new Vector(-1, 1), isOn)) +
        (1 << 1) * (GetVal(p + new Vector(0, 1), isOn)) +
        (1 << 0) * (GetVal(p + new Vector(1, 1), isOn));
    }

    private int GetVal(Point p, bool isOn)
    {
      if (Image.TryGetValue(p, out var it)) return it == Pixel.Bright ? 1 : 0;
      if (Enhancement[0] == Pixel.Bright && isOn) return 1;
      return 0;
    }
  }

  static World Parse(string s)
  {
    var pps = s.Split("\n\n").Select(it => it.Split("\n")).ToList();
    var p1 = pps[0];

    var bright = '#';

    var enhancement = p1[0].Select(it => it == bright ? Pixel.Bright : Pixel.Dark).ToList();

    var image = pps[1].WithIndices().SelectMany(row => row.Value.WithIndices().Select(col => (col: col.Index, row: row.Index, c: col.Value)))
      .ToDictionary(it => new Point(it.col, it.row), it => it.c == bright ? Pixel.Bright : Pixel.Dark);

    return new(enhancement, image);
  }
}