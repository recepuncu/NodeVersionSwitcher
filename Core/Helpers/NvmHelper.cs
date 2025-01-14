using Microsoft.Win32;
using System.Diagnostics;

namespace NodeVersionSwitcher.Core.Helpers;

/// <summary>
/// Provides helper methods for working with the Windows Registry.
/// </summary>
internal static class NvmHelper
{
    private static string _nvmPath = GetNvmInstallationPath();

    /// <summary>
    /// Switches the Node.js version to the specified version.
    /// </summary>
    /// <param name="version"></param>
    internal static void SwitchNodeVersion(string version)
    {
        try
        {
            UseNodeVersion(version);
            NotificationHelper.ShowInfo($"Switched to Node.js {version}", "Success");
        }
        catch (Exception ex)
        {
            NotificationHelper.ShowError($"Failed to switch Node.js version: {ex.Message}", "Error");
        }
    }

    /// <summary>
    /// Gets the available Node.js versions.
    /// </summary>
    /// <returns></returns>
    internal static IEnumerable<string> GetNodeVersions() 
        => Directory.GetDirectories(_nvmPath, "v*").Select(Path.GetFileName).Where(name => !string.IsNullOrEmpty(name))!;

    /// <summary>
    /// Gets the current Node.js version.
    /// </summary>
    /// <returns></returns>
    internal static string GetCurrentVersion()
    {
        try
        {
            var filePath = Path.Combine(GetLinkPath(), "node.exe");
            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            return !string.IsNullOrEmpty(versionInfo.FileVersion) ? $"v{versionInfo.FileVersion}" : string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error retrieving current version: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Switches the Node.js version to the specified version.
    /// </summary>
    /// <param name="version"></param>
    internal static void UseNodeVersion(string version)
    {
        var targetPath = Path.Combine(_nvmPath, version);
        var linkPath = GetLinkPath();

        SymbolicLinkHelper.CreateSymbolicLink(linkPath, targetPath);
    }

    /// <summary>
    /// Gets the installation path of NVM for Windows.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    internal static string GetNvmInstallationPath()
    {
        var nvmHome = Environment.GetEnvironmentVariable("NVM_HOME") ?? string.Empty;
        if (Directory.Exists(nvmHome)) return nvmHome;

        var registryPaths = new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\nvm",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\nvm"
        };

        foreach (var path in registryPaths)
        {
            var installLocation = RegistryHelper.GetRegistryValue(Registry.LocalMachine, path, "InstallLocation");
            if (Directory.Exists(installLocation)) return installLocation;
        }

        var defaultPaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "nvm"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\Roaming\\nvm")
        };

        return defaultPaths.FirstOrDefault(Directory.Exists) ??
               throw new DirectoryNotFoundException("NVM installation directory not found");
    }

    /// <summary>
    /// Gets the path of the symbolic link used by NVM.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal static string GetLinkPath()
    {
        var settingsFilePath = Path.Combine(_nvmPath, "settings.txt");

        try
        {
            var line = File.ReadLines(settingsFilePath).FirstOrDefault(l => l.StartsWith("path:"));
            return line?.Substring("path:".Length).Trim() ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading link path: {ex.Message}");
        }
    }
}

