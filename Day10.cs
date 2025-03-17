using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day10
{

  [Theory]
  [InlineData("Day10.Sample", 26397)]
  [InlineData("Day10", 344193)]
  public void Part1(string file, long expected)
  {
    var lines = AoCLoader.LoadLines(file);
    lines.Select(Score).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("()", 0)]
  [InlineData("(()", 0)]
  [InlineData("[]", 0)]
  [InlineData("{}", 0)]
  [InlineData("<>", 0)]
  [InlineData("(]", 57)]
  [InlineData("{()()()>", 25137)]
  [InlineData("(((()))}", 1197)]
  [InlineData("<([]){()}[{}])", 3)]
  [InlineData("{([(<{}[<>[]}>{[]{[(<()>", 1197)]

  public void Sanity(string input, long expected)
  {
    Score(input).Should().Be(expected);
  }

  private long Score(string input)
  {
    Dictionary<char, long> score = new()
    {
      [')'] = 3,
      [']'] = 57,
      ['}'] = 1197,
      ['>'] = 25137,
    };
    Dictionary<char, char> match = new()
    {
      ['('] = ')',
      ['['] = ']',
      ['{'] = '}',
      ['<'] = '>',
    };
    var opener = match.Keys.ToList();
    var closer = score.Keys.ToList();
    Stack<char> stack = [];
    foreach (var c in input)
    {
      if (opener.Contains(c))
      {
        stack.Push(c);
      }
      else if (closer.Contains(c))
      {
        var x = stack.Pop();
        if (match[x] != c)
        {
          return score[c];
        }

      }
      else throw new ApplicationException();
    }
    return 0;
  }
}