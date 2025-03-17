using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;

namespace AdventOfCode2021.CSharp;

public class Day10
{

  [Theory]
  [InlineData("Day10.Sample", 26397)]
  [InlineData("Day10", 344193)]
  public void Part1(string file, long expected)
  {
    var lines = AoCLoader.LoadLines(file);
    lines.Select(Score1).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day10.Sample", 288957)]
  [InlineData("Day10", 3241238967)]
  public void Part2(string file, long expected)
  {
    var lines = AoCLoader.LoadLines(file);
    var x = lines.Where(it => Score1(it) == 0).Select(Score2).OrderBy(it=>it).ToList();
    x[x.Count / 2].Should().Be(expected);
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
    Score1(input).Should().Be(expected);
  }

  private long Score1(string input)
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

  private long Score2(string input)
  {
    Dictionary<char, long> score = new()
    {
      ['('] = 1,
      ['['] = 2,
      ['{'] = 3,
      ['<'] = 4,
    };
    Dictionary<char, char> match = new()
    {
      ['('] = ')',
      ['['] = ']',
      ['{'] = '}',
      ['<'] = '>',
    };
    var opener = match.Keys.ToList();
    var closer = match.Values.ToList();
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
          throw new ApplicationException("Really should match at this point.");
        }
      }
      else throw new ApplicationException($"Unknown character: {c}");
    }

    return stack.Aggregate(0L, (accum, current) => accum * 5 + score[current]);
  }
}