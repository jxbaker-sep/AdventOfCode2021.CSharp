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
  [InlineData("Day16", 963, 1549026292886)]
  public void Part1(string file, long expected, long expected2)
  {
    var input = AoCLoader.LoadLines(file).Single();

    var (packet, _) = ParsePacket(ToBinary(input));
    SumVersions(packet).Should().Be(expected);
    Value(packet).Should().Be(expected2);
  }

  [Theory]
  [InlineData("D2FE28", 2021)]
  public void SanityLiteral(string input, long expected)
  {
    var s = ToBinary(input);
    var (packet, _) = ParsePacket(s);
    packet.Literal.Should().Be(expected);
  }

  [Theory]
  [InlineData("38006F45291200", 9)]
  [InlineData("EE00D40C823060", 14)]
  [InlineData("8A004A801A8002F478", 16)]
  [InlineData("620080001611562C8802118E34", 12)]
  [InlineData("C0015000016115A2E0802F182340", 23)]
  [InlineData("A0016C880162017C3686B18A3D4780", 31)]
  public void SanityOperators(string input, long expected)
  {
    var s = ToBinary(input);
    var (packet, _) = ParsePacket(s);
    SumVersions(packet).Should().Be(expected);
  }

  [Theory]
  [InlineData("C200B40A82", 3)]
  [InlineData("04005AC33890", 54)]
  [InlineData("880086C3E88112", 7)]
  [InlineData("CE00C43D881120", 9)]
  [InlineData("D8005AC2A8F0", 1)]
  [InlineData("F600BC2D8F", 0)]
  [InlineData("9C005AC2F8F0", 0)]
  [InlineData("9C0141080250320F1802104A08", 1)]
  public void SanityValues(string input, long expected)
  {
    var s = ToBinary(input);
    var (packet, _) = ParsePacket(s);
    Value(packet).Should().Be(expected);
  }

  record Packet(long Version, long ID, long Literal, List<Packet> Subpackets);

  static long SumVersions(Packet packet) => packet.Version + packet.Subpackets.Select(SumVersions).Sum();

  static long Value(Packet packet)
  {
    return packet.ID switch {
      0 => packet.Subpackets.Select(Value).Sum(),
      1 => packet.Subpackets.Select(Value).Product(),
      2 => packet.Subpackets.Select(Value).Min(),
      3 => packet.Subpackets.Select(Value).Max(),
      4 => packet.Literal,
      5 => Value(packet.Subpackets[0]) > Value(packet.Subpackets[1]) ? 1 : 0,
      6 => Value(packet.Subpackets[0]) < Value(packet.Subpackets[1]) ? 1 : 0,
      7 => Value(packet.Subpackets[0]) == Value(packet.Subpackets[1]) ? 1 : 0,
      _ => throw new ApplicationException()
    };
  }

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