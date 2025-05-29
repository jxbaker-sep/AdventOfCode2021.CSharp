using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Parser;

namespace AdventOfCode2021.CSharp;

public partial class Day18
{
  [Theory]
  [InlineData("Day18.Sample.5", 4140)]
  [InlineData("Day18", 0)]
  public void Part1(string file, long expected)
  {
    AoCLoader.LoadLines(file).Select(it => SnailfishNumber.From(it))
      .Aggregate((a, b) =>
      {
        var result = a.Add(b);
        return result;
      })
      .Magnitude().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day18.Sample.1", "[[[[1,1],[2,2]],[3,3]],[4,4]]")]
  [InlineData("Day18.Sample.2", "[[[[3,0],[5,3]],[4,4]],[5,5]]")]
  [InlineData("Day18.Sample.3", "[[[[5,0],[7,4]],[5,5]],[6,6]]")]
  [InlineData("Day18.Sample.4", "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]")]
  public void Samples(string file, string expected)
  {
    AoCLoader.LoadLines(file).Select(it => SnailfishNumber.From(it))
      .Aggregate((a, b) =>
      {
        var result = a.Add(b);
        return result;
      })
      .ToString().Should().Be(expected);
  }

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

  [Fact]
  public void Sanity()
  {
    SnailfishNumber.From("[[[[4,3],4],4],[7,[[8,4],9]]]").Add(SnailfishNumber.From("[1,1]"))
      .Reduce().ToString().Should().Be("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");
  }

  [Fact]
  public void Sanity2()
  {
    SnailfishNumber.From("[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]").Add(SnailfishNumber.From("[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]"))
      .Reduce().ToString().Should().Be("[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]");
  }

  [Theory]
  [InlineData("[9,1]", 29)]
  [InlineData("[1,9]", 21)]
  [InlineData("[[9,1],[1,9]]", 129)]
  [InlineData("[[1,2],[[3,4],5]]", 143)]
  [InlineData("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384)]
  [InlineData("[[[[1,1],[2,2]],[3,3]],[4,4]]", 445)]
  [InlineData("[[[[3,0],[5,3]],[4,4]],[5,5]]", 791)]
  [InlineData("[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137)]
  [InlineData("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488)]
  public void MagnitudeTests(string input, long expected)
  {
    SnailfishNumber.From(input).Magnitude().Should().Be(expected);
  }

  enum Kind { Open, Close, Literal };

  record SnItem(Kind Kind, long Literal);

  record SnailfishNumber(LinkedList<SnItem> Trail)
  {
    public long Magnitude()
    {
      LinkedList<SnItem> copy = [];
      foreach(var item in Trail) copy.AddLast(new SnItem(item.Kind, item.Literal));
      
      while (copy.Count > 1)
      {
        var current = copy.First;
        while (current != null)
        {
          var next = current.Next;
          if (next != null && current.Value.Kind == Kind.Literal && next.Value.Kind == Kind.Literal)
          {
            var open = current.Previous!;
            var close = next.Next!;
            copy.AddAfter(current, new SnItem(Kind.Literal, current.Value.Literal * 3 + next.Value.Literal * 2));
            copy.Remove(open);
            copy.Remove(current);
            current = close.Next;
            copy.Remove(next);
            copy.Remove(close);
          } else {
            current = next;
          }
        }
      }

      return copy.First!.Value.Literal;
    }

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
      return new SnailfishNumber(result).Reduce();
    }

    public SnailfishNumber Reduce()
    {
      var original = ToString();
      while (true)
      {
        Reduce1();
        if (original == ToString()) break;
        original = ToString();
      }
      return this;
    }
    
    public SnailfishNumber Reduce1()
    {
      var depth = -1;
      foreach(var item in Trail.Nodes())
      {
        if (item.Value.Kind == Kind.Close) depth -= 1;
        else if (item.Value.Kind == Kind.Open) {
          depth += 1;
          if (depth >= 4) return Explode(item);
        }
      }
      foreach(var item in Trail.Nodes())
      {
        if (item.Value.Kind == Kind.Literal && item.Value.Literal >= 10) return Split(item);
      }
      return this;
    }

    private SnailfishNumber Explode(LinkedListNode<SnItem> open)
    {
      var lhs = open.Next!;
      var rhs = lhs.Next!;
      var close = rhs.Next!;
      open.Value.Kind.Should().Be(Kind.Open);
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

    private SnailfishNumber Split(LinkedListNode<SnItem> open)
    {
      Trail.AddBefore(open, new SnItem(Kind.Open, 0));
      Trail.AddBefore(open, new SnItem(Kind.Literal, open.Value.Literal / 2));
      Trail.AddBefore(open, new SnItem(Kind.Literal, open.Value.Literal / 2 + (open.Value.Literal % 2)));
      Trail.AddBefore(open, new SnItem(Kind.Close, 0));
      Trail.Remove(open);
      return this;
    }
  }

}