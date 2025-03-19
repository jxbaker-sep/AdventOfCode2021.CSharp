using System.ComponentModel;
using System.Net.Mail;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day14
{

  [Theory]
  [InlineData("Day14.Sample", 1588)]
  [InlineData("Day14", 3259)]
  public void Part1(string file, long expected)
  {
    var (template, rules) = Convert(AoCLoader.LoadLines(file));

    var groups = GetCounts(template, 10, rules);
    var max = groups.Values.Max();
    var min = groups.Values.Min();

    (max - min).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day14.Sample", 2188189693529)]
  [InlineData("Day14", 3459174981021)]
  public void Part2(string file, long expected)
  {
    var (template, rules) = Convert(AoCLoader.LoadLines(file));

    var groups = GetCounts(template, 40, rules);
    var max = groups.Values.Max();
    var min = groups.Values.Min();

    (max - min).Should().Be(expected);
  }

  Dictionary<(string, int), Dictionary<char, long>> Cache = [];
  Dictionary<char, long> GetCounts(string polymer, int count, Dictionary<(char, char), char> rules) {
    if (Cache.TryGetValue((polymer, count), out var found)) return found;
    if (count == 0 || polymer.Length <= 1) {
      var result2 = polymer.ToCharArray().GroupToCounts();
      Cache[(polymer, count)] = result2;
      return result2;
    }
    // if (polymer.Length == 2 && count == 1) {
    //   var result2 = polymer.ToCharArray().GroupToCounts();
    //   if (rules.TryGetValue((polymer[0], polymer[1]), out var insert2)) {
    //     result2[insert2] = result2.GetValueOrDefault(insert2) + 1;
    //   }
    //   Cache[(polymer, count)] = result2;
    //   return result2;
    // }
    Dictionary<char, long> result = []; 
    for(var i = 0; i < polymer.Length - 1; i++) {
      if (rules.TryGetValue((polymer[i], polymer[i+1]), out var insert)) {
        var d3 = GetCounts($"{polymer[i]}{insert}{polymer[i+1]}", count - 1, rules);
        Merge(result, d3);
        // remove the second character of the pair because it will be counted in the next unless it is also the last element
        if (i != polymer.Length - 2) {
          result[polymer[i+1]] -= 1;
        }
      }
      else {
        result[polymer[i]] = result.GetValueOrDefault(polymer[i]) + 1;
        // the second element of the pair will be counted in the next pair unless it is also the last element
        if (i == polymer.Length - 2) result[polymer[i+1]] = result.GetValueOrDefault(polymer[i+1]) + 1;
      }
    }

    Cache[(polymer, count)] = result;
    return result;
  }

  void Merge(Dictionary<char, long> result, Dictionary<char, long> d2) {
    foreach(var (x,y) in d2) result[x] = result.GetValueOrDefault(x) + y;
  }

  private static (string Template, Dictionary<(char, char), char> Rules) Convert(List<string> input)
  {
    return (input[0], P.Format("{}{} -> {}", P.Letter, P.Letter, P.Letter)
      .ParseMany(input[2..])
      .ToDictionary(it => (it.First, it.Second), it => it.Third));
  }
}