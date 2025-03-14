using System.Numerics;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day08
{
  const int Length1 = 2;
  const int Length4 = 4;
  const int Length7 = 3;
  const int Length8 = 7;
  
  [Theory]
  [InlineData("Day08.Sample", 26)]
  [InlineData("Day08", 303)]
  public void Part1(string file, long expected)
  {
    var items = Convert(AoCLoader.LoadLines(file));

    var result = items.SelectMany(it => it.Output).LongCount(it => new[]{Length1, Length4, Length7, Length8 }.Contains(it.Length));

    result.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day08.Sample", 61229)]
  [InlineData("Day08", 961734)]
  public void Part2(string file, long expected)
  {
    var items = Convert(AoCLoader.LoadLines(file));

    var result = items.Select(it => {
      var map = CreateMap(it.Patterns);
      return System.Convert.ToInt64(it.Output.Select(it2 => map[it2.OrderBy(it=>it).Join()]).Join());
    }).Sum();

    result.Should().Be(expected);
  }

  public Dictionary<string, long> CreateMap(List<string> patterns) {
    var p1 = patterns.Single(it => it.Length == Length1).ToCharArray();
    var p4 = patterns.Single(it => it.Length == Length4).ToCharArray();
    var p7 = patterns.Single(it => it.Length == Length7).ToCharArray();
    var p8 = patterns.Single(it => it.Length == Length8).ToCharArray();    

    // Length 5: 2, 3, 5
    // Length 6: 0, 6, 9

    var p3 = patterns.Single(it => it.Length == 5 && it.Intersect(p7).Count() == 3).ToCharArray();
    
    var a = p7.Except(p1).Single();
    // segment f is used in every pattern but 1.
    char f = 'a';
    List<char> p2 = [];
    for (f = 'a'; f <= 'g'; f++) {
      if (patterns.Count(p => p.Contains(f)) == 9) {
        p2 = [.. patterns.Single(it => !it.Contains(f))];
        break;
      }
    }

    var p5 = patterns.Single(it => it.Length == 5 && it.Intersect(p2).Count() != 5 && it.Intersect(p3).Count() != 5).ToCharArray();

    var c = p1.Except([f]).Single();

    var p6 = patterns.Single(it => it.Length == 6 && it.Intersect(p1).Count() == 1).ToCharArray();

    var be = p8.Except(p3).ToList();

    var _09 = patterns.Where(it => it.Length == 6 && it.Union(p6).Count() != 6).ToList();

    var p9 = _09.Single(it => it.Intersect(be).Count() != 2);
    var p0 = _09.Except([p9]).Single();

    return new Dictionary<string, long>{
      {p0.OrderBy(it=>it).Join(), 0},
      {p1.OrderBy(it=>it).Join(), 1},
      {p2.OrderBy(it=>it).Join(), 2},
      {p3.OrderBy(it=>it).Join(), 3},
      {p4.OrderBy(it=>it).Join(), 4},
      {p5.OrderBy(it=>it).Join(), 5},
      {p6.OrderBy(it=>it).Join(), 6},
      {p7.OrderBy(it=>it).Join(), 7},
      {p8.OrderBy(it=>it).Join(), 8},
      {p9.OrderBy(it=>it).Join(), 9},
    };
  }

  public record Item(List<string> Patterns, List<string> Output);

  private static List<Item> Convert(List<string> input)
  {
    return P.Format("{}|{}", P.Word.Trim().Plus(), P.Word.Trim().Plus())
      .Select(it => new Item(it.First, it.Second))
      .ParseMany(input);
  }
} 