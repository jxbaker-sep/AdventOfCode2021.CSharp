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

    Search(MiscUtils.InclusiveRange(1, 9).Reverse().ToList(), data, 0, 0, 0).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day24", 11931881141161)]
  public void Part2(string filename, long expected)
  {
    var data = Parse(AoCLoader.LoadLines(filename));

    Search(MiscUtils.InclusiveRange(1, 9).ToList(), data, 0, 0, 0).Should().Be(expected);
  }

  readonly Dictionary<(int, long), long?> Cache = [];

  long? Search(List<long> range, IReadOnlyList<Coefficients> data, int index, long input_z, long previous)
  {
    var key = (index, input_z);
    if (Cache.TryGetValue(key, out var found)) return found;
    foreach (var proposed in range)
    {
      var z = Operate(proposed, data[index], input_z);

      if (index == 13)
      {
        if (z == 0) {
          Cache[key] = previous * 10 + proposed;
          return previous * 10 + proposed;
        }
        continue;
      }

      var next = Search(range, data, index + 1, z, previous * 10 + proposed);
      if (next == null) continue;
      Cache[key] = next;
      return next;
    }
    Cache[key] = null;
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