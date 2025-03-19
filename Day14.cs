using System.ComponentModel;
using System.Net.Mail;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
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

    var polymer = new LinkedList<char>(template.ToCharArray());

    foreach(var _ in Enumerable.Range(0,10)) {
      var current = polymer.First!;
      while (current != null) {
        var next = current.Next;
        if (next == null) break;
        if (rules.TryGetValue((current.Value, next.Value), out var insert)) {
          polymer.AddAfter(current, insert);
        }
        current = next;
      }
    }

    var groups = polymer.GroupToCounts();
    var max = groups.Values.Max();
    var min = groups.Values.Min();

    (max - min).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day14.Sample", 2188189693529)]
  [InlineData("Day14", 0)]
  public void Part2(string file, long expected)
  {
    var (template, rules) = Convert(AoCLoader.LoadLines(file));

    var polymer = new LinkedList<char>(template.ToCharArray());

    foreach(var _ in Enumerable.Range(0,10)) {
      var current = polymer.First!;
      while (current != null) {
        var next = current.Next;
        if (next == null) break;
        if (rules.TryGetValue((current.Value, next.Value), out var insert)) {
          polymer.AddAfter(current, insert);
        }
        current = next;
      }
    }

    var groups = polymer.GroupToCounts();
    var max = groups.Values.Max();
    var min = groups.Values.Min();

    (max - min).Should().Be(expected);
  }

  private static (string Template, Dictionary<(char, char), char> Rules) Convert(List<string> input)
  {
    return (input[0], P.Format("{}{} -> {}", P.Letter, P.Letter, P.Letter)
      .ParseMany(input[2..])
      .ToDictionary(it => (it.First, it.Second), it => it.Third));
  }
}