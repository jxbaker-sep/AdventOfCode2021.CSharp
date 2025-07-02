using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.Z3;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2021.CSharp;

public class Day24
{
  [Theory]
  [InlineData("Day24", 94992992796199L)]
  public void Part1(string filename, long expected)
  {
    var data = Parse(AoCLoader.LoadLines(filename));

    Highest(data, 0, 0, 0).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day24", 11931881141161)]
  public void Part2(string filename, long expected)
  {
    var data = Parse(AoCLoader.LoadLines(filename));

    Smallest(data, 0, 0, 0).Should().Be(expected);
  }

  readonly Dictionary<(int, long), long?> Cache = [];

  long? Highest(IReadOnlyList<Coefficients> data, int index, long input_z, long previous)
  {
    if (Cache.TryGetValue((index, input_z), out var found)) return found;
    for (var proposed = 9; proposed > 0; proposed--)
    {
      var z = Operate(proposed, data[index], input_z);

      if (index == 13)
      {
        if (z == 0) {
          Cache[(index, input_z)] = previous * 10 + proposed;
          return previous * 10 + proposed;
        }
        continue;
      }

      var next = Highest(data, index + 1, z, previous * 10 + proposed);
      if (next == null) continue;
      Cache[(index, input_z)] = next;
      return next;
    }
    Cache[(index, input_z)] = null;
    return null;
  }

  long? Smallest(IReadOnlyList<Coefficients> data, int index, long input_z, long previous)
  {
    if (Cache.TryGetValue((index, input_z), out var found)) return found;
    for (var proposed = 1; proposed <= 9; proposed++)
    {
      var z = Operate(proposed, data[index], input_z);

      if (index == 13)
      {
        if (z == 0) {
          Cache[(index, input_z)] = previous * 10 + proposed;
          return previous * 10 + proposed;
        }
        continue;
      }

      var next = Smallest(data, index + 1, z, previous * 10 + proposed);
      if (next == null) continue;
      Cache[(index, input_z)] = next;
      return next;
    }
    Cache[(index, input_z)] = null;
    return null;
  }


  static long Operate(long input, Coefficients coeff, long z)
  {
    var w = input;
    var x = z % 26;
    z /= coeff.A;
    x += coeff.B;
    x = (x == w) ? 0 : 1;
    var y = x * 25 + 1;
    z *= y;
    y = (w + coeff.C) * x;
    z += y;

    return z;
  }

  public record Coefficients(long A, long B, long C);

  static List<Coefficients> Parse(List<string> input)
  {
    List<Coefficients> result = [];
    var matcher = P.Format(@"inp w
mul x 0
add x z
mod x 26
div z {}
add x {}
eql x w
eql x 0
mul y 0
add y 25
mul y x
add y 1
mul z y
mul y 0
add y w
add y {}
mul y x
add z y", P.Long, P.Long, P.Long).Select(it => new Coefficients(it.First, it.Second, it.Third));
    for (var index = 0; index < input.Count; index += 18)
    {
      var section = input[index..(index + 18)].Join("\n");
      result.Add(matcher.Parse(section));
    }
  return result;
  }
}