using Microsoft.Win32;

namespace NodeVersionSwitcher.Core.Helpers;

/// <summary>
/// Provides helper methods for working with the Windows Registry.
/// </summary>
internal static class RegistryHelper
{
    /// <summary>
    /// Gets the value of a registry key.
    /// </summary>
    /// <param name="baseKey"></param>
    /// <param name="subKeyPath"></param>
    /// <param name="valueName"></param>
    /// <returns></returns>
    internal static string GetRegistryValue(RegistryKey baseKey, string subKeyPath, string valueName)
    {
        using var key = baseKey.OpenSubKey(subKeyPath);
        return key?.GetValue(valueName) as string ?? string.Empty;
    }

    /// <summary>
    /// Sets the value of a registry key.
    /// </summary>
    /// <param name="baseKey"></param>
    /// <param name="subKeyPath"></param>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    internal static void SetRegistryValue(RegistryKey baseKey, string subKeyPath, string valueName, string value)
    {
        using var key = baseKey.CreateSubKey(subKeyPath);
        key?.SetValue(valueName, value);
    }

    /// <summary>
    /// Deletes a registry value.
    /// </summary>
    /// <param name="baseKey"></param>
    /// <param name="subKeyPath"></param>
    /// <param name="valueName"></param>
    internal static void DeleteRegistryValue(RegistryKey baseKey, string subKeyPath, string valueName)
    {
        using var key = baseKey.OpenSubKey(subKeyPath, true);
        key?.DeleteValue(valueName, false);
    }
}

