using NodeVersionSwitcher.Core.Helpers;

namespace NodeVersionSwitcher.Core.Models;

/// <summary>
/// Represents a Node.js version info.
/// </summary>
internal class NodeVersionInfo : IComparable<NodeVersionInfo>
{
    internal string Version { get; set; } = null!;
    internal string DownloadUrl { get; set; } = null!;
    internal Version ParsedVersion => RegexHelper.ParseVersion(Version);

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(NodeVersionInfo other)
    {
        return other.ParsedVersion.CompareTo(ParsedVersion);
    }
}
