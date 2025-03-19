using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Utils;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day12
{
  
  [Theory]
  [InlineData("Day12.Sample.1", 10)]
  [InlineData("Day12.Sample.2", 19)]
  [InlineData("Day12.Sample.3", 226)]
  [InlineData("Day12", 3230)]
  public void Part1(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    Queue<(string Node, List<string> Path)> open = [];
    open.Enqueue(("start", []));

    long closed = 0;

    while (open.TryDequeue(out var current)) {
      foreach(var next in grid[current.Node]) {
        if (next == "end") {
          closed += 1;
          continue;
        }
        if (next == next.ToLower() && current.Path.Contains(next)) continue;
        open.Enqueue((next, [..current.Path, current.Node]));
      }
    }

    closed.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day12.Sample.1", 36)]
  [InlineData("Day12.Sample.2", 103)]
  [InlineData("Day12.Sample.3", 3509)]
  [InlineData("Day12", 83475)]
  public void Part2(string file, long expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    Queue<(string Node, List<string> Path, bool Exhausted)> open = [];
    open.Enqueue(("start", [], false));

    long closed = 0;

    while (open.TryDequeue(out var current)) {
      foreach(var next in grid[current.Node]) {
        if (next == "start") continue;
        if (next == "end") {
          closed += 1;
          continue;
        }
        var pathContainsNext = next == next.ToLower() && current.Path.Contains(next);
        if (current.Exhausted && pathContainsNext) continue;
        open.Enqueue((next, [..current.Path, current.Node], current.Exhausted || pathContainsNext));
      }
    }

    closed.Should().Be(expected);
  }

  private static Dictionary<string, List<string>> Convert(List<string> input)
  {
    Dictionary<string, List<string>> result = [];
    foreach(var line in input) {
      var (first, second) = P.Format("{}-{}", P.Word, P.Word).Parse(line);
      result[first] = result.GetValueOrDefault(first) ?? [];
      result[first].Add(second);
      result[second] = result.GetValueOrDefault(second) ?? [];
      result[second].Add(first);
    }
    return result;
  }
} 