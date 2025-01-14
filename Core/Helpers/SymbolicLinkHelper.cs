namespace NodeVersionSwitcher.Core.Helpers;

/// <summary>
/// Provides helper methods for working with symbolic links.
/// </summary>
internal static class SymbolicLinkHelper
{
    /// <summary>
    /// Creates a symbolic link at the specified path that points to the target path.
    /// </summary>
    /// <param name="linkPath"></param>
    /// <param name="targetPath"></param>
    internal static void CreateSymbolicLink(string linkPath, string targetPath)
    {
        if (Directory.Exists(linkPath))
        {
            Directory.Delete(linkPath, true);
        }

        Directory.CreateSymbolicLink(linkPath, targetPath);
    }
}

