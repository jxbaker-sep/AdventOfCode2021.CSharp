using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.Serialization;
using AdventOfCode2021.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2021.CSharp;

public class Day16
{
  [Theory]
  [InlineData("D2FE28", 2021)]
  public void SanityValues(string input, long expected)
  {
    var s = ToBinary(input);
    var (packet, remainder) = ParsePacket(s);
    packet.Value.Should().Be(expected);
  }

  [Theory]
  [InlineData("38006F45291200", 9, 30)]
  [InlineData("EE00D40C823060", 14, 6)]
  public void SanityOperators(string input, long expected1, long expected2)
  {
    var s = ToBinary(input);
    var (packet, _) = ParsePacket(s);
    SumVersions(packet).Should().Be(expected1);
    SumValue(packet).Should().Be(expected2);
  }

  record Packet(long Version, long ID, long Value, List<Packet> Subpackets);

  static long SumVersions(Packet packet) => packet.Version + packet.Subpackets.Select(SumVersions).Sum();
  static long SumValue(Packet packet) => packet.Value + packet.Subpackets.Select(SumValue).Sum();

  static (Packet Packet, string Remainder) ParsePacket(string s)
  {
    long version = DecodeBinary(s, 0, 3);
    long id = DecodeBinary(s, 3, 3);

    if (id == 4)
    {
      long value = 0;
      var n = 6;
      var done = false;
      while (!done)
      {
        done = s[n] == '0';
        var hex = DecodeBinary(s, n + 1, 4);
        value = (value << 4) + hex;
        n += 5;
      }
      return (new Packet(version, id, value, []), s[n..]);
    }
    else
    {
      // operator packet
      var lengthTypeId = DecodeBinary(s, 6, 1);
      var packet = new Packet(version, id, 0, []);
      if (lengthTypeId == 0)
      {
        var tlength = (int)DecodeBinary(s, 7, 15);
        var sub = s[22..(22 + tlength)];
        while (sub.Length != 0)
        {
          var (p, r) = ParsePacket(sub);
          packet.Subpackets.Add(p);
          sub = r;
        }
        var n = 22 + tlength;
        return (packet, s[n..]);
      }
      else
      {
        var tlength = (int)DecodeBinary(s, 7, 11);
        var sub = s[18..];
        while (tlength-- > 0)
        {
          var (p, r) = ParsePacket(sub);
          packet.Subpackets.Add(p);
          sub = r;
        }
        return (packet, sub);
      }
    }
  }

  static long DecodeBinary(string s, int start, int length)
  {
    return s[start..(start + length)].Select(it => it - '0')
      .Aggregate(0, (accum, current) => accum * 2 + current);
  }

  static string ToBinary(string hex)
  {
    Dictionary<char, string> decode = new(){
      {'0', "0000"},
      {'1', "0001"},
      {'2', "0010"},
      {'3', "0011"},
      {'4', "0100"},
      {'5', "0101"},
      {'6', "0110"},
      {'7', "0111"},
      {'8', "1000"},
      {'9', "1001"},
      {'A', "1010"},
      {'B', "1011"},
      {'C', "1100"},
      {'D', "1101"},
      {'E', "1110"},
      {'F', "1111"},
    };
    return hex.Select(it => decode[it]).Join();
  }
}