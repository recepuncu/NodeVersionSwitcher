using Microsoft.Win32;

namespace NodeVersionSwitcher.Core.Helpers;

internal static class StartupHelper
{
    private const string StartupRegistryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string ApplicationName = "NodeVersionSwitcher";

    /// <summary>
    /// Determines whether the application is set to start with Windows.
    /// </summary>
    /// <returns></returns>
    internal static bool IsInStartup()
    {
        var value = RegistryHelper.GetRegistryValue(Registry.CurrentUser, StartupRegistryPath, ApplicationName);

        return value == Application.ExecutablePath;
    }

    /// <summary>
    /// Adds the application to the Windows startup folder.
    /// </summary>
    internal static void AddToStartup()
        => RegistryHelper.SetRegistryValue(Registry.CurrentUser, StartupRegistryPath, ApplicationName, Application.ExecutablePath);

    /// <summary>
    /// Removes the application from the Windows startup folder.
    /// </summary>
    internal static void RemoveFromStartup()
        => RegistryHelper.DeleteRegistryValue(Registry.CurrentUser, StartupRegistryPath, ApplicationName);
}
