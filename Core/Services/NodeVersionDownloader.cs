using NodeVersionSwitcher.Core.Helpers;
using NodeVersionSwitcher.Core.Models;
using NodeVersionSwitcher.Interfaces;
using System.IO.Compression;

namespace NodeVersionSwitcher.Core.Services;

/// <summary>
/// Downloads and installs Node.js versions.
/// </summary>
internal class NodeVersionDownloader : INodeVersionDownloader
{
    private readonly string _nvmPath;
    private readonly string _platform;
    private readonly string _architecture;
    private readonly string _nodeReleasesUrl = "https://nodejs.org/download/release/";

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeVersionDownloader"/> class.
    /// </summary>
    /// <param name="nvmPath"></param>
    public NodeVersionDownloader(string nvmPath)
    {
        SystemInfoHelper.ValidatePlatformAndArchitecture();

        _nvmPath = nvmPath;
        _platform = SystemInfoHelper.GetPlatform();
        _architecture = SystemInfoHelper.GetArchitecture();
    }

    /// <summary>
    /// Gets available Node.js versions.
    /// </summary>
    /// <returns></returns>
    public async Task<List<NodeVersionInfo>> GetAvailableVersionsAsync()
    {
        var html = await HttpHelper.GetHtmlContentAsync(_nodeReleasesUrl);
        var versions = RegexHelper.GetMatches(html, "<a href=\"(v[0-9]+\\.[0-9]+\\.[0-9]+)/\">");

        return versions.Select(version => new NodeVersionInfo
        {
            Version = version,
            DownloadUrl = $"{_nodeReleasesUrl}{version}/node-{version}-{_platform}-{_architecture}.zip"
        }).ToList();
    }

    /// <summary>
    /// Downloads and installs a Node.js version.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    public async Task DownloadAndInstallVersionAsync(string version, IProgress<int> progress)
    {
        var versionInfo = new NodeVersionInfo
        {
            Version = version,
            DownloadUrl = $"{_nodeReleasesUrl}{version}/node-{version}-{_platform}-{_architecture}.zip"
        };

        var tempFile = await HttpHelper.DownloadFileAsync(versionInfo.DownloadUrl, progress);

        try
        {
            var targetDir = Path.Combine(_nvmPath, version);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            ZipFile.ExtractToDirectory(tempFile, targetDir);

            var extractedFolder = Directory.GetDirectories(targetDir)[0];
            foreach (var file in Directory.GetFiles(extractedFolder))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetDir, fileName);
                if (File.Exists(destFile)) File.Delete(destFile);
                File.Move(file, destFile);
            }
            foreach (var dir in Directory.GetDirectories(extractedFolder))
            {
                var dirName = Path.GetFileName(dir);
                var destDir = Path.Combine(targetDir, dirName);
                if (Directory.Exists(destDir)) Directory.Delete(destDir, true);
                Directory.Move(dir, destDir);
            }
            Directory.Delete(extractedFolder);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
