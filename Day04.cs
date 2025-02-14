
using System.ComponentModel;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day04
{
  [Theory]
  [InlineData("Day04.Sample", 4512, 1924)]
  [InlineData("Day04", 25023, 2634)]
  public void Part1(string file, int expected, int expected2)
  {
    var world = Convert(AoCLoader.LoadFile(file));
    
    List<Board2> boards = [];

    foreach(var (b, index) in world.Boards.WithIndices())
    {
      Dictionary<int, (int, int)> v2c = [];
      foreach(var row in Enumerable.Range(0, 5))
      {
        foreach(var col in Enumerable.Range(0, 5))
        {
          v2c[b.Grid[row][col]]= (row, col);
        }
      }
      boards.Add(new(index, v2c, [5,5,5,5,5], [5,5,5,5,5], b.Grid.SelectMany(xx => xx).ToHashSet()));
    }

    var scores = WinningScores(world.Draws, boards).ToList();
    scores[0].Should().Be(expected);
    scores[^1].Should().Be(expected2);
  }

  IEnumerable<int> WinningScores(List<int> draws, List<Board2> boards)
  {
    List<int> closed = [];
    foreach (var item in draws)
    {
      foreach(var b in boards)
      {
        if (closed.Contains(b.Id)) continue;
        if (b.Values.TryGetValue(item, out var value))
        {
          b.Rows[value.Row] -= 1;
          b.Cols[value.Col] -= 1;
          b.Remaining.Remove(item);
          if (b.Rows[value.Row] == 0 || b.Cols[value.Col] == 0)
          {
            var found = b.Remaining.Sum() * item;
            closed.Add(b.Id);
            yield return found;
          }
        }
      }
    }
  }

  public record Board2(int Id, Dictionary<int, (int Row, int Col)> Values, List<int> Rows, List<int> Cols, HashSet<int> Remaining);
  public record Board(List<List<int>> Grid);
  public record World(List<int> Draws, List<Board> Boards);

  private static World Convert(string list)
  {
    var pps = list.Split("\n\n").Select(pp => pp.Split("\n").ToList()).ToList();

    var drawn = P.Int.Plus(",").Parse(pps[0].Single());

    return new(drawn, 
      pps[1..]
        .Select(it => it.Select(xx => P.Int.Trim().Plus().Parse(xx)).ToList())
        .Select(it => new Board(it))
        .ToList()
      );
  }
}