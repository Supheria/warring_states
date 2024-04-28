using System.Text;
using System.Text.RegularExpressions;

namespace DlaTest;

public class RegexTest
{
    private void test()
    {


        Console.WriteLine("Hello, World!");

        var test = @"[^\s]+固定为[^\s]这个";
        //var test = @"将.+固定为[^\s]";

        //var pattern = @"([^[.]*(\[\^[^]]+\])(.*)";
        var pattern = @"([^[\.]*)(\[\^[^]]+\]|\.)(.*)";

        var str1 = ReplacePartInString(@"([^：]+)：?([+-])([\d\.%]+)", new[] { @"\[\^[^]]+\]", @"(?<!\\)\." },
            s => s.Substring(s.Length - 1, 1) is "." ? @"[^\s]" : s.Insert(s.Length - 1, @"\s"));

        var str2 = ReplacePartInString(@"将.+固定为[^\s]", new[] { @"\[\^[^]]+\]", @"\." },
            s => s.Substring(s.Length - 1, 1) is "." ? @"[^\s]" : s.Insert(s.Length - 1, @"\s"));
        Console.ReadKey();

        //if (GetMatch(test, pattern, out var match))
        //{
        //    var sb = new StringBuilder();
        //    string post;
        //    do
        //    {
        //        var prev = match.Groups[1].Value;
        //        if (prev.Substring(prev.Length - 1, 1) is ".")
        //        {
        //            sb.Append(prev[..1]);
        //            sb.Append(@"[^\s]");
        //        }
        //        else
        //            sb.Append(prev.Insert(prev.Length - 1, @"\s"));
        //        post = match.Groups[2].Value;
        //    } while (GetMatch(post, pattern, out match));

        //    sb.Append(post);
        //}

        static string ReplacePartInString(string input, string[] partPatterns, Func<string, string> replace)
        {
            var ignores = new HashSet<string>();
            foreach (var part in partPatterns)
            {
                var ignore = part;
                // remove the condition limit part of "look behind", some like (?<=pattern)
                if (GetMatch(ignore, @"\(\?.+\)(.+)", out var m))
                    ignore = m.Groups[1].Value;
                // add together the escape mark at start
                ignore = GetMatch(ignore, @"\\.+", out m) ? m.Groups[0].Value[..2] : part[..1];
                ignores.Add(ignore);
            }
            var pattern = $"([^{new StringBuilder().AppendJoin("", ignores)}]*)({new StringBuilder().AppendJoin('|', partPatterns)})(.*)";
            StringBuilder sb;
            if (!GetMatch(input, pattern, out var match))
                sb = new(input);
            else
            {
                sb = new();
                string post;
                do
                {
                    sb.Append(match.Groups[1].Value);
                    sb.Append(replace(match.Groups[2].Value));
                    post = match.Groups[3].Value;
                } while (GetMatch(post, pattern, out match));
                sb.Append(post);
            }
            return sb.ToString();
        }
        //var post = match.Groups[2].Value;

        //var a = match.Groups;

        static bool GetMatch(string input, string pattern, out Match match)
        {
            match = Regex.Match(input, pattern);
            return match.Success;
        }
    }
}