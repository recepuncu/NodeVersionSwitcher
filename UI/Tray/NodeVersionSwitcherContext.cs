using Microsoft.Win32;
using NodeVersionSwitcher.Core.Helpers;
using NodeVersionSwitcher.UI.Forms;
using System.Diagnostics;

namespace NodeVersionSwitcher.UI.Tray;

/// <summary>
/// Provides a context menu for switching between Node.js versions using NVM for Windows.
/// </summary>
internal class NodeVersionSwitcherContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeVersionSwitcherContext"/> class.
    /// </summary>
    public NodeVersionSwitcherContext()
    {
        _trayIcon = new NotifyIcon
        {
            Visible = true,
            Text = "Node Version Switcher",
            Icon = LoadTrayIcon(),
            ContextMenuStrip = CreateContextMenu()
        };
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
    /// Shows the Node.js versions form.
    /// </summary>
    private void ShowVersionsForm()
    {
        var form = new NodeVersionsForm(NvmHelper.GetNvmInstallationPath());
        form.Show();
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
            AddNodeVersionsMenuItem(contextMenu);
            PopulateNodeVersionsMenu(contextMenu);
            AddStartWithWindowsMenuItem(contextMenu);
            AddExitMenuItem(contextMenu);
            contextMenu.Refresh();
        };

        return contextMenu;
    }

    /// <summary>
    /// Adds a "Install Node.js Versions" menu item to the context menu.
    /// </summary>
    /// <param name="contextMenu"></param>
    private void AddNodeVersionsMenuItem(ContextMenuStrip contextMenu)
    {
        var versionsItem = new ToolStripMenuItem("Install Node.js Versions");
        versionsItem.Click += (s, e) => ShowVersionsForm();
        contextMenu.Items.Insert(0, versionsItem);
        contextMenu.Items.Add(new ToolStripSeparator());
    }

    /// <summary>
    /// Populates the context menu with the available Node.js versions.
    /// </summary>
    /// <param name="contextMenu"></param>
    private void PopulateNodeVersionsMenu(ContextMenuStrip contextMenu)
    {
        var nodeVersions = NvmHelper.GetNodeVersions().ToList();

        if (!nodeVersions.Any())
        {
            contextMenu.Items.Add(new ToolStripMenuItem("No versions found") { Enabled = false });
            return;
        }

        var currentVersion = NvmHelper.GetCurrentVersion();

        foreach (var version in nodeVersions)
        {
            var menuItem = new ToolStripMenuItem($"{version}{(version == currentVersion ? " (Current)" : string.Empty)}");
            menuItem.Click += (sender, args) => NvmHelper.SwitchNodeVersion(version);
            contextMenu.Items.Add(menuItem);
        }

        contextMenu.Items.Add(new ToolStripSeparator());
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
            Checked = StartupHelper.IsInStartup()
        };

        startWithWindowsMenuItem.Click += (s, e) =>
        {
            if (startWithWindowsMenuItem.Checked)
            {
                StartupHelper.RemoveFromStartup();
                startWithWindowsMenuItem.Checked = false;
            }
            else
            {
                StartupHelper.AddToStartup();
                startWithWindowsMenuItem.Checked = true;
            }
        };

        contextMenu.Items.Add(startWithWindowsMenuItem);
        contextMenu.Items.Add(new ToolStripSeparator());
    }    

    /// <summary>
    /// Disposes of the resources used by the application context.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}
