using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2021.CSharp;

public class Day25
{
  [Theory]
  [InlineData("Day25.Sample", 58)]
  [InlineData("Day25", 0)]
  public void Part1(string filename, long expected)
  {
    var data = AoCLoader.LoadLines(filename).Select(it => it.ToList()).ToList();
    var maxx = data[0].Count;
    var maxy = data.Count;

    var steps = 0L;
    var moved = true;

    while (moved)
    {
      moved = false;
      steps += 1;

      foreach(var (facing, dx, dy) in new[]{('>', 1, 0), ('v', 0, 1)})
      {
        List<(int x, int y)> moves = [];
        foreach (var x in Enumerable.Range(0, maxx))
        {
          foreach (var y in Enumerable.Range(0, maxy))
          {
            if (data[y][x] == facing)
            {
              var next = data[(y + dy) % maxy][(x + dx) % maxx];
              if (next == '.') moves.Add((x, y));
            }
          }
        }
        moved = moved || moves.Count > 0;
        foreach(var (x, y) in moves)
        {
          data[y][x] = '.';
          data[(y + dy) % maxy][(x + dx) % maxx] = facing;
        }
      }
    }

    steps.Should().Be(expected);
  }

}