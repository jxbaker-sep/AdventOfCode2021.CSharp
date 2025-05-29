using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using Xunit.Abstractions;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public partial class Day18
{
  // [Theory]
  // [InlineData("Day18.Sample", 45)]
  // [InlineData("Day18", 8911)]
  // public void Part1(string file, long expected)
  // {
    
  // }

  [Theory]
  [InlineData("[1,2]")]
  [InlineData("[[1,2],2]")]
  [InlineData("[1,[2,3]]")]
  [InlineData("[[1,2],[3,4]]")]
  public void ToStringTest(string input)
  {
    SnailfishNumber.From(input).ToString().Should().Be(input);
  }

  [Theory]
  [InlineData("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
  [InlineData("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
  [InlineData("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
  [InlineData("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]")]
  [InlineData("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
  public void ExplodeTest(string input, string expected)
  {
    SnailfishNumber.From(input).Reduce1().ToString().Should().Be(expected);
  }

  enum Kind { Open, Close, Literal };

  record SnItem(Kind Kind, int Literal);

  record SnailfishNumber(LinkedList<SnItem> Trail)
  {
    public override string ToString()
    {
      string result = "";
      SnItem? previous = null;
      foreach(var item in Trail)
      {
        if (item.Kind == Kind.Open) { if (previous != null && previous.Kind != Kind.Open) result += ","; result += "[";}
        if (item.Kind == Kind.Close) { result += "]"; }
        if (item.Kind == Kind.Literal) { if (previous != null && previous.Kind != Kind.Open) result += ",";  result += item.Literal;}
        previous = item;
      }
      return result;
    }

    public static SnailfishNumber From(string input)
    {
      LinkedList<SnItem> result = [];
      foreach (var item in input)
      {
        if (item == ',') continue;
        else if (item == '[') result.AddLast(new SnItem(Kind.Open, 0));
        else if (item == ']') result.AddLast(new SnItem(Kind.Close, 0));
        else result.AddLast(new SnItem(Kind.Literal, Convert.ToInt32($"{item}")));
      }
      return new(result);
    }

    public SnailfishNumber Add(SnailfishNumber rhs)
    {
      LinkedList<SnItem> result = [];
      result.AddLast(new SnItem(Kind.Open, 0));
      foreach(var item in Trail) result.AddLast(item);
      foreach(var item in rhs.Trail) result.AddLast(item);
      result.AddLast(new SnItem(Kind.Close, 0));
      return new(result);
    }
    
    public SnailfishNumber Reduce1()
    {
      var depth = -1;
      foreach(var item in Trail.Nodes())
      {
        if (item.Value.Kind == Kind.Close) depth -= 1;
        if (item.Value.Kind == Kind.Open) {
          depth += 1;
          if (depth >= 4)
          {
            Explode(item);
            return this;
          }
        }
      }
      return this;
    }

    private SnailfishNumber Explode(LinkedListNode<SnItem> open)
    {
      var lhs = open.Next!;
      var rhs = lhs.Next!;
      var close = rhs.Next!;
      lhs.Value.Kind.Should().Be(Kind.Literal);
      rhs.Value.Kind.Should().Be(Kind.Literal);
      close.Value.Kind.Should().Be(Kind.Close);

      LinkedListNode<SnItem>? previous = open.Previous;
      while (previous != null && previous.Value.Kind != Kind.Literal) previous = previous.Previous;

      LinkedListNode<SnItem>? next = close.Next;
      while (next != null && next.Value.Kind != Kind.Literal) next = next.Next;

      if (previous != null) previous.Value = new SnItem(Kind.Literal, previous.Value.Literal + lhs.Value.Literal);
      if (next != null) next.Value = new SnItem(Kind.Literal, next.Value.Literal + rhs.Value.Literal);

      Trail.AddAfter(lhs, new SnItem(Kind.Literal, 0));
      Trail.Remove(open);
      Trail.Remove(lhs);
      Trail.Remove(rhs);
      Trail.Remove(close);

      return this;
    }
  }

}