using System.Text.RegularExpressions;

namespace NodeVersionSwitcher.Core.Helpers;

internal static class RegexHelper
{
    public static List<string> GetMatches(string input, string pattern)
    {
        var matches = Regex.Matches(input, pattern);
        return matches.Select(m => m.Groups[1].Value).ToList();
    }

    public static Version ParseVersion(string versionString)
    {
        var match = Regex.Match(versionString, @"v?(\d+)\.(\d+)\.(\d+)");
        if (match.Success)
        {
            return new Version(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value)
            );
        }
        return new Version(0, 0, 0);
    }
}
