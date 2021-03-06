﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zipangu;

namespace NetUnitTest
{
    [TestClass]
    public class VBStringsTest
    {
        static readonly string TargetChars = EnumerableHelper.RangeChars(' ', '~')
            + EnumerableHelper.RangeChars('　', '〕').Replace("〄", "")
            + EnumerableHelper.RangeChars('ぁ', 'ゖ')
            + EnumerableHelper.RangeChars('゛', 'ヿ')
            + EnumerableHelper.RangeChars('！', '～')
            + EnumerableHelper.RangeChars('｡', 'ﾟ');

        static string ToMessage1(char c) => $"({(int)c:D5}-{(int)c:X4}) {c}";
        static string ToMessage2(char c) => $"{c} ({(int)c:D5}-{(int)c:X4})";

        static void WriteChanged_Wide(VbStrConv mode)
        {
            // 他言語の文字は "？" に変換されます。
            var changed = EnumerableHelper.RangeChars(char.MinValue, char.MaxValue)
                .Select(c => new { before = c, after = Strings.StrConv(c.ToString(), mode).Single() })
                .Where(_ => _.before != _.after)
                .Where(_ => TargetChars.Contains(_.before) || _.after != '？')
                .Select(_ => $"{ToMessage1(_.before)} > {ToMessage2(_.after)}")
                .ToArray();

            File.WriteAllLines($"VBStrings-{mode.ToString().Replace(", ", "")}.txt", changed, Encoding.UTF8);
        }

        static void WriteChanged_Narrow(VbStrConv mode)
        {
            // 他言語の文字は "?" に変換されます。
            var changed = EnumerableHelper.RangeChars(char.MinValue, char.MaxValue)
                .Select(c => new { before = c, after = Strings.StrConv(c.ToString(), mode) })
                .Where(_ => _.before.ToString() != _.after)
                .Where(_ => TargetChars.Contains(_.before) || _.after != "?")
                .Select(_ => $"{ToMessage1(_.before)} > {(_.after.Length == 1 ? ToMessage2(_.after[0]) : _.after)}")
                .ToArray();

            File.WriteAllLines($"VBStrings-{mode.ToString().Replace(", ", "")}.txt", changed, Encoding.UTF8);
        }

        static void WriteChanged_Voiced(VbStrConv mode)
        {
            var changed = EnumerableHelper.RangeChars(char.MinValue, char.MaxValue)
                .Select(c => new { before = c, after = Strings.StrConv($"{c}ﾞ", mode) })
                .Where(_ => _.after.Length == 1)
                .Select(_ => $"{ToMessage1(_.before)} > {ToMessage2(_.after[0])}")
                .ToArray();

            File.WriteAllLines($"VBStrings-{mode.ToString().Replace(", ", "")}-Voiced.txt", changed, Encoding.UTF8);
        }

        static void WriteChanged_SemiVoiced(VbStrConv mode)
        {
            var changed = EnumerableHelper.RangeChars(char.MinValue, char.MaxValue)
                .Select(c => new { before = c, after = Strings.StrConv($"{c}ﾟ", mode) })
                .Where(_ => _.after.Length == 1)
                .Select(_ => $"{ToMessage1(_.before)} > {ToMessage2(_.after[0])}")
                .ToArray();

            File.WriteAllLines($"VBStrings-{mode.ToString().Replace(", ", "")}-SemiVoiced.txt", changed, Encoding.UTF8);
        }

        [TestMethod]
        public void Wide() => WriteChanged_Wide(VbStrConv.Wide);

        [TestMethod]
        public void Narrow() => WriteChanged_Narrow(VbStrConv.Narrow);

        [TestMethod]
        public void Katakana() => WriteChanged_Narrow(VbStrConv.Katakana);

        [TestMethod]
        public void Hiragana() => WriteChanged_Narrow(VbStrConv.Hiragana);

        [TestMethod]
        public void WideKatakana() => WriteChanged_Wide(VbStrConv.Wide | VbStrConv.Katakana);

        [TestMethod]
        public void WideHiragana() => WriteChanged_Wide(VbStrConv.Wide | VbStrConv.Hiragana);

        [TestMethod]
        public void NarrowKatakana() => WriteChanged_Narrow(VbStrConv.Narrow | VbStrConv.Katakana);

        [TestMethod]
        public void NarrowHiragana() => WriteChanged_Narrow(VbStrConv.Narrow | VbStrConv.Hiragana);

        [TestMethod]
        public void WideKatakana_Voiced() => WriteChanged_Voiced(VbStrConv.Wide | VbStrConv.Katakana);

        [TestMethod]
        public void WideHiragana_Voiced() => WriteChanged_Voiced(VbStrConv.Wide | VbStrConv.Hiragana);

        [TestMethod]
        public void WideKatakana_SemiVoiced() => WriteChanged_SemiVoiced(VbStrConv.Wide | VbStrConv.Katakana);

        [TestMethod]
        public void WideHiragana_SemiVoiced() => WriteChanged_SemiVoiced(VbStrConv.Wide | VbStrConv.Hiragana);
    }
}
