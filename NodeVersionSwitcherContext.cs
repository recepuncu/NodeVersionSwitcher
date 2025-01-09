using Microsoft.Win32;
using System.Diagnostics;

namespace NodeVersionSwitcher;

/// <summary>
/// Provides a context menu for switching between Node.js versions using NVM for Windows.
/// </summary>
internal class NodeVersionSwitcherContext : ApplicationContext
{
    private readonly string _nvmPath;
    private readonly NotifyIcon _trayIcon;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeVersionSwitcherContext"/> class.
    /// </summary>
    public NodeVersionSwitcherContext()
    {
        _nvmPath = GetNvmInstallationPath();

        _trayIcon = new NotifyIcon
        {
            Visible = true,
            Text = "Node Version Switcher",
            Icon = LoadTrayIcon(),
            ContextMenuStrip = CreateContextMenu()
        };
    }

    /// <summary>
    /// Determines whether the application is set to start with Windows.
    /// </summary>
    /// <returns></returns>
    private bool IsInStartup()
    {
        var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
        if (key == null) return false;

        var value = key.GetValue("NodeVersionSwitcher") as string;

        return value == Application.ExecutablePath;
    }

    /// <summary>
    /// Adds the application to the Windows startup folder.
    /// </summary>
    private void AddToStartup()
    {
        var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        key?.SetValue("NodeVersionSwitcher", Application.ExecutablePath);
    }

    /// <summary>
    /// Removes the application from the Windows startup folder.
    /// </summary>
    private void RemoveFromStartup()
    {
        var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        key?.DeleteValue("NodeVersionSwitcher", false);
    }

    /// <summary>
    /// Loads the tray icon from the application directory.
    /// </summary>
    /// <returns></returns>
    private Icon LoadTrayIcon()
    {
        var iconPath = Path.Combine(Application.StartupPath, "nodejs.ico");
        return File.Exists(iconPath) ? new Icon(iconPath) : SystemIcons.Application;
    }

    /// <summary>
    /// Creates the context menu for the tray icon.
    /// </summary>
    /// <returns></returns>
    private ContextMenuStrip CreateContextMenu()
    {
        var contextMenu = new ContextMenuStrip();
        PopulateNodeVersionsMenu(contextMenu);

        contextMenu.Opening += (s, e) =>
        {            
            contextMenu.Items.Clear();
            contextMenu.Items.Add("Node Version Switcher");
            contextMenu.Items.Add(new ToolStripSeparator());
            PopulateNodeVersionsMenu(contextMenu);
            AddStartWithWindowsMenuItem(contextMenu);
            AddExitMenuItem(contextMenu);
            contextMenu.Refresh();
        };

        return contextMenu;
    }

    /// <summary>
    /// Populates the context menu with the available Node.js versions.
    /// </summary>
    /// <param name="contextMenu"></param>
    private void PopulateNodeVersionsMenu(ContextMenuStrip contextMenu)
    {
        var nodeVersions = GetNodeVersions().ToList();

        if (!nodeVersions.Any())
        {
            contextMenu.Items.Add(new ToolStripMenuItem("No versions found") { Enabled = false });
            return;
        }

        var currentVersion = GetCurrentVersion();

        foreach (var version in nodeVersions)
        {
            var menuItem = new ToolStripMenuItem($"{version}{(version == currentVersion ? " (Current)" : string.Empty)}");
            menuItem.Click += (sender, args) => SwitchNodeVersion(version);
            contextMenu.Items.Add(menuItem);
        }

        contextMenu.Items.Add(new ToolStripSeparator());
    }

    /// <summary>
    /// Switches the Node.js version to the specified version.
    /// </summary>
    /// <param name="version"></param>
    private void SwitchNodeVersion(string version)
    {
        try
        {
            UseNodeVersion(version);
            MessageBox.Show($"Switched to Node.js {version}", "NVM Tray", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to switch Node.js version: {ex.Message}", "NVM Tray", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Adds an "Exit" menu item to the context menu.
    /// </summary>
    /// <param name="contextMenu"></param>
    private void AddExitMenuItem(ContextMenuStrip contextMenu)
    {
        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += (s, e) =>
        {
            _trayIcon.Visible = false;
            Application.Exit();
        };
        contextMenu.Items.Add(exitMenuItem);
    }

    /// <summary>
    /// Adds a "Start with Windows" menu item to the context menu.
    /// </summary>
    /// <param name="contextMenu"></param>
    private void AddStartWithWindowsMenuItem(ContextMenuStrip contextMenu)
    {
        var startWithWindowsMenuItem = new ToolStripMenuItem("Start with Windows")
        {
            Checked = IsInStartup()
        };

        startWithWindowsMenuItem.Click += (s, e) =>
        {
            if (startWithWindowsMenuItem.Checked)
            {
                RemoveFromStartup();
                startWithWindowsMenuItem.Checked = false;
            }
            else
            {
                AddToStartup();
                startWithWindowsMenuItem.Checked = true;
            }
        };

        contextMenu.Items.Add(startWithWindowsMenuItem);
        contextMenu.Items.Add(new ToolStripSeparator());
    }

    /// <summary>
    /// Gets the available Node.js versions.
    /// </summary>
    /// <returns></returns>
    private IEnumerable<string> GetNodeVersions()
    {
        return Directory.GetDirectories(_nvmPath, "v*").Select(Path.GetFileName);
    }

    /// <summary>
    /// Gets the current Node.js version.
    /// </summary>
    /// <returns></returns>
    private string GetCurrentVersion()
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
    private void UseNodeVersion(string version)
    {
        var targetPath = Path.Combine(_nvmPath, version);
        var linkPath = GetLinkPath();

        if (Directory.Exists(linkPath))
        {
            Directory.Delete(linkPath, true);
        }

        Directory.CreateSymbolicLink(linkPath, targetPath);
    }

    /// <summary>
    /// Gets the installation path of NVM for Windows.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    private string GetNvmInstallationPath()
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
            var installLocation = GetRegistryValue(Registry.LocalMachine, path, "InstallLocation");
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
    private string GetLinkPath()
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

    /// <summary>
    /// Gets a registry value from the specified key and subkey path.
    /// </summary>
    /// <param name="baseKey"></param>
    /// <param name="subKeyPath"></param>
    /// <param name="valueName"></param>
    /// <returns></returns>
    private static string GetRegistryValue(RegistryKey baseKey, string subKeyPath, string valueName)
    {
        using var key = baseKey.OpenSubKey(subKeyPath);
        return key?.GetValue(valueName) as string;
    }

    /// <summary>
    /// Disposes of the resources used by the application context.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
