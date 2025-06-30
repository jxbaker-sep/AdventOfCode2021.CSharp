using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.Z3;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2021.CSharp;

public class Day23
{
  const char Empty = '.';
  const long ACost = 1;
  const long BCost = 10;
  const long CCost = 100;
  const long DCost = 1000;

  [Theory]
  [InlineData("BACDBCDA", 12521)]
  // [InlineData("BCBADADC", 10607)]
  public void Part1(string amphipods, long expected)
  {
    Dictionary<Point, char> world = [];
    world[new(0, 0)] = Empty;
    world[new(1, 0)] = Empty;
    world[new(2, 0)] = Empty;
    world[new(3, 0)] = Empty;
    world[new(4, 0)] = Empty;
    world[new(5, 0)] = Empty;
    world[new(6, 0)] = Empty;
    world[new(7, 0)] = Empty;
    world[new(8, 0)] = Empty;
    world[new(9, 0)] = Empty;
    world[new(10, 0)] = Empty;

    world[new(2, 1)] = amphipods[0];
    world[new(2, 2)] = amphipods[1];

    world[new(4, 1)] = amphipods[2];
    world[new(4, 2)] = amphipods[3];

    world[new(6, 1)] = amphipods[4];
    world[new(6, 2)] = amphipods[5];

    world[new(8, 1)] = amphipods[6];
    world[new(8, 2)] = amphipods[7];

    Go(world, 2).Should().Be(expected);
  }


  long Go(Dictionary<Point, char> start, long depth)
  {
    List<Point> spaces = start.Keys.ToList();
    List<Point> taboo = [new(2, 0), new(4, 0), new(6, 0), new(8, 0)];
    Dictionary<char, (long space, long cost)> lookup = new() {
      { 'A', (2, ACost) },
      { 'B', (4, BCost) },
      { 'C', (6, CCost) },
      { 'D', (8, DCost) }
    };

    bool AtGoal(Dictionary<Point, char> world)
    {
      foreach(var (amphipod, (space, _)) in lookup)
      {
        for (int i = 1; i <= depth; i++)
        {
          if (world[new(space, i)] != amphipod) return false;
        }
      }
      return true;
    }

    long Estimation(Dictionary<Point, char> world)
    {
      long estimate = 0;
      foreach (var (amphipod, (space, cost)) in lookup)
      {
        var amphipods = world.Where(it => it.Value == amphipod).Select(item => item.Key).ToList();
        var deepestOutOfPlace = depth;
        for (; deepestOutOfPlace > 0; deepestOutOfPlace--) { 
          if (new[]{Empty, amphipod}.Contains(world[new(space, deepestOutOfPlace)])) break;
        }
        foreach(var it in amphipods) 
        {
          if (it.X != space)
          {
            estimate += cost * (it.Y + new Point(it.X, 0).ManhattanDistance(new(space, 1)));
          }
          else if (it.Y < deepestOutOfPlace)
          {
            estimate += 2 * cost * (it.Y + 1);
          }
        }
      }
      return estimate;
    }

    PriorityQueue<(Dictionary<Point, char> World, long Energy)> open = new(it => it.Energy + Estimation(it.World));

    open.Enqueue((start, 0));

    while (open.TryDequeue(out var current))
    {
      // Console.WriteLine($"{current.Energy}, {current.Energy + Estimation(current.World)}");
      foreach (var (amphipod, (space, energyCostPerStep)) in lookup)
      {
        var deepestOutOfPlace = depth;
        for (; deepestOutOfPlace > 0; deepestOutOfPlace--) { 
          if (!new[]{Empty, amphipod}.Contains(current.World[new(space, deepestOutOfPlace)])) break;
        }
        foreach (var currentPosition in current.World.Where(it => it.Value == amphipod).Select(it => it.Key))
        {
          // amphipods in completed home spaces do not move
          if (currentPosition.X == space && currentPosition.Y > deepestOutOfPlace) continue;
          var openSpaces = FindAllOpenSpaces(current.World, currentPosition).ToList();
          // don't bother opening any other space if I can immediately go home
          var homespaces = openSpaces.Where(it => it.Point.X == space && it.Point.Y > 0).ToList();
          if (homespaces.Count > 0 && deepestOutOfPlace == 0)
          {
            var nextPosition = homespaces.MaxBy(it => it.Point.Y);
            var nextWorld = current.World.Clone();
            nextWorld[currentPosition] = '.';
            nextWorld[nextPosition.Point] = amphipod;
            var nextEnergy = current.Energy + energyCostPerStep * nextPosition.Distance;
            if (AtGoal(nextWorld)) return nextEnergy;
            open.Enqueue((nextWorld, nextEnergy));
            continue;
          }
          foreach (var (nextPosition, steps) in openSpaces)
          {
            // an amphipod in the hallway can only move into a room
            if (currentPosition.Y == 0 && nextPosition.Y == 0) continue;
            // amphipods never stop immediately outside any room
            if (taboo.Contains(nextPosition)) continue;
            // Can't go into any room (checked if we can go home above)
            if (nextPosition.Y > 0) continue;

            var nextWorld = current.World.Clone();
            nextWorld[currentPosition] = '.';
            nextWorld[nextPosition] = amphipod;
            var nextEnergy = current.Energy + energyCostPerStep * steps;
            if (AtGoal(nextWorld)) return nextEnergy;
            open.Enqueue((nextWorld, nextEnergy));
          }
        }
      }
    }
    throw new ApplicationException("Did not find solution.");
  }

  public static IEnumerable<(Point Point, long Distance)> FindAllOpenSpaces(Dictionary<Point, char> world, Point start)
  {
    HashSet<Point> closed = [];
    Queue<(Point Point, long Distance)> open = [];
    open.Enqueue((start, 0));
    while (open.TryDequeue(out var current))
    {
      foreach (var neighbor in current.Point.CardinalNeighbors())
      {
        if (closed.Contains(neighbor)) continue;
        if (!world.TryGetValue(neighbor, out var neighborSpace)) continue;
        if (neighborSpace == Empty)
        {
          closed.Add(neighbor);
          open.Enqueue((neighbor, current.Distance + 1));
          yield return (neighbor, current.Distance + 1);
        }
      }
    }
  }
}