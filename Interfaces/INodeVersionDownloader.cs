using NodeVersionSwitcher.Core.Models;

namespace NodeVersionSwitcher.Interfaces;

/// <summary>
/// Downloads and installs Node.js versions.
/// </summary>
internal interface INodeVersionDownloader
{
    /// <summary>
    /// Gets available Node.js versions.
    /// </summary>
    /// <returns></returns>
    Task<List<NodeVersionInfo>> GetAvailableVersionsAsync();

    /// <summary>
    /// Downloads and installs a Node.js version.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    Task DownloadAndInstallVersionAsync(string version, IProgress<int> progress);
}
