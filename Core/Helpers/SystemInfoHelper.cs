using System.Runtime.InteropServices;

namespace NodeVersionSwitcher.Core.Helpers;

/// <summary>
/// Helper class for getting system information.
/// </summary>
internal static class SystemInfoHelper
{
    /// <summary>
    /// Gets the current operating system platform.
    /// </summary>
    /// <returns>Platform name (win, linux, darwin, or unknown).</returns>
    internal static string GetPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "win";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "darwin";

        return "unknown";
    }

    /// <summary>
    /// Gets the current system architecture.
    /// </summary>
    /// <returns>Architecture name (x64, arm64, x86, or unknown).</returns>
    internal static string GetArchitecture()
    {
        return RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            Architecture.X86 => "x86",
            _ => "unknown"
        };
    }

    /// <summary>
    /// Validates the platform and architecture.
    /// </summary>
    internal static void ValidatePlatformAndArchitecture()
    {
        var platform = GetPlatform();
        var architecture = GetArchitecture();

        if (platform == "unknown" || architecture == "unknown")
        {
            throw new NotSupportedException($"Unsupported platform ({platform}) or architecture ({architecture}).");
        }
    }
}
