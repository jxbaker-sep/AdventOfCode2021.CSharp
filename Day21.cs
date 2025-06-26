using FluentAssertions;

namespace AdventOfCode2021.CSharp;

public class Day21
{
  [Theory]
  [InlineData(4, 8, 739785)]
  [InlineData(2, 5, 0)]
  public void Part1(long p1, long p2, long expected)
  {
    var x = RunToScore(p1, 6, 1000);
    var y = RunToScore(p2, 5, 1000);

    long loser = 0;
    long rolls = 0;
    if (x.turn <= y.turn)
    {
      y = RunToTurn(p2, 5, x.turn - 1);
      loser = y.score;
      rolls = x.turn * 6 - 3;
    } else {
      x = RunToTurn(p1, 6, y.turn);
      loser = x.score;
      rolls = y.turn * 6;
    }

    (rolls * loser).Should().Be(expected);
  }

  [Theory]
  [InlineData(4, 8, 444356092776315)]
  [InlineData(2, 5, 131888061854776)]
  public void Part2(long p1, long p2, long expected)
  {
    CountWins([p1, p2], [0, 0], 0).Max().Should().Be(expected);
  }

  Dictionary<(long p1, long p2, long score1, long score2, int which), List<long>> Cache = [];
  List<long> CountWins(List<long> ps, List<long> scores, int which)
  {
    var key = (ps[0], ps[1], scores[0], scores[1], which);
    if (Cache.TryGetValue(key, out var needle)) return needle;
    List<long> wins = [0, 0];
    foreach (var i in new[] { 1, 2, 3 })
    {
      foreach (var j in new[] { 1, 2, 3 })
      {
        foreach (var k in new[] { 1, 2, 3 })
        {
          var nextPs = ps.ToList();
          nextPs[which] = (nextPs[which] + (i + j + k)) % 10;
          if (nextPs[which] == 0) nextPs[which] = 10;
          var nextScores = scores.ToList();
          nextScores[which] += nextPs[which];
          if (nextScores[which] >= 21)
          {
            wins[which] += 1;
          }
          else
          {
            var recursive = CountWins(nextPs, nextScores, (which + 1) % 2);
            wins[0] += recursive[0];
            wins[1] += recursive[1];
          }
        }
      }
    }
    Cache[key] = wins;
    return wins;
  }

  [Theory]
  [InlineData(4, 6, 25, 26)]
  [InlineData(4, 6, 999, 1000)]
  [InlineData(4, 6, 1000, 1000)]
  [InlineData(8, 5, 2, 3)]
  [InlineData(8, 5, 3, 3)]
  [InlineData(8, 5, 744, 745)]
  [InlineData(8, 5, 745, 745)]
  public void Sanity(long start, long firstIncrement, long targetScore, long expected)
  {
    RunToScore(start, firstIncrement, targetScore).score.Should().Be(expected);
  }

  public static (long score, long turn) RunToScore(long start, long firstIncrement, long targetScore)
  {
    Dictionary<(long pos, long incr), (long score, long turn)> cache = [];
    long score = 0;
    long pos = start;
    long incr = firstIncrement;
    long turn = 0;
    bool found = false;

    while (true)
    {
      if (!found && cache.TryGetValue((pos, incr), out var previous))
      {
        found = true;
        var turnDiff = turn - previous.turn;
        var scoreDiff = score - previous.score;
        var remainder = (targetScore - score) / scoreDiff;
        turn += remainder * turnDiff;
        score += remainder * scoreDiff;
        if (score >= targetScore) return (score, turn);
      }
      else 
      {
        cache[(pos, incr)] = (score, turn);
      }
      pos = (pos + incr) % 10;
      if (pos == 0) pos = 10;
      score += pos;
      incr = (incr + 18) % 10;
      turn++;
      if (score >= targetScore) return (score, turn);
    }
  }

  public static (long score, long turn) RunToTurn(long start, long firstIncrement, long targetTurns)
  {
    Dictionary<(long pos, long incr), (long score, long turn)> cache = [];
    long score = 0;
    long pos = start;
    long incr = firstIncrement;
    long turn = 0;
    bool found = false;

    while (true)
    {
      if (!found && cache.TryGetValue((pos, incr), out var previous))
      {
        found = true;
        var turnDiff = turn - previous.turn;
        var scoreDiff = score - previous.score;
        var remainder = (targetTurns - turn) / turnDiff;
        turn += remainder * turnDiff;
        score += remainder * scoreDiff;
        if (turn >= targetTurns) return (score, turn);
      }
      else 
      {
        cache[(pos, incr)] = (score, turn);
      }
      pos = (pos + incr) % 10;
      if (pos == 0) pos = 10;
      score += pos;
      incr = (incr + 18) % 10;
      turn++;
      if (turn >= targetTurns) return (score, turn);
    }
  }
}