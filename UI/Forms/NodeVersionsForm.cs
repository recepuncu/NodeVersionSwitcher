using NodeVersionSwitcher.Core.Helpers;
using NodeVersionSwitcher.Core.Models;
using NodeVersionSwitcher.Core.Services;

namespace NodeVersionSwitcher.UI.Forms;

/// <summary>
/// Represents a form for displaying and installing Node.js versions.
/// </summary>
internal partial class NodeVersionsForm : Form
{
    private readonly NodeVersionDownloader _downloader;
    private readonly DataGridView _gridVersions;
    private readonly ProgressBar _progressBar;
    private readonly Label _statusLabel;
    private readonly TextBox _filterBox;
    private List<NodeVersionInfo> _allVersions;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeVersionsForm"/> class.
    /// </summary>
    /// <param name="nvmPath"></param>
    public NodeVersionsForm(string nvmPath)
    {
        _downloader = new NodeVersionDownloader(nvmPath);
        _allVersions = new List<NodeVersionInfo>();

        Text = "Node.js Versions";
        Size = new Size(600, 400);

        // Filter panel
        var filterPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            Padding = new Padding(5)
        };

        var filterLabel = new Label
        {
            Text = "Version Filter:",
            AutoSize = true,
            Location = new Point(10, 12)
        };

        _filterBox = new TextBox
        {
            Width = 150,
            Location = new Point(100, 9)
        };
        _filterBox.TextChanged += FilterBox_TextChanged;

        filterPanel.Controls.Add(filterLabel);
        filterPanel.Controls.Add(_filterBox);

        // Grid
        _gridVersions = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        _statusLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 20,
            Text = "Loading versions..."
        };

        _progressBar = new ProgressBar
        {
            Dock = DockStyle.Bottom,
            Height = 20,
            Visible = false
        };

        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill
        };

        mainPanel.Controls.Add(_gridVersions);
        mainPanel.Controls.Add(_statusLabel);
        mainPanel.Controls.Add(_progressBar);

        Controls.Add(mainPanel);
        Controls.Add(filterPanel);

        InitializeGrid();
        LoadVersions();
    }

    /// <summary>
    /// Handles the TextChanged event of the FilterBox control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FilterBox_TextChanged(object sender, EventArgs e)
    {
        ApplyFilter();
    }

    /// <summary>
    /// Applies the filter to the versions grid.
    /// </summary>
    private void ApplyFilter()
    {
        var filterText = _filterBox.Text.ToLower();
        var filteredVersions = _allVersions
            .Where(v => v.Version.ToLower().Contains(filterText))
            .OrderBy(v => v)
            .ToList();

        UpdateGrid(filteredVersions);
    }

    /// <summary>
    /// Updates the versions grid with the specified versions.
    /// </summary>
    /// <param name="versions"></param>
    private void UpdateGrid(List<NodeVersionInfo> versions)
    {
        _gridVersions.Rows.Clear();
        foreach (var version in versions)
        {
            _gridVersions.Rows.Add(version.Version);
        }
        _statusLabel.Text = $"{versions.Count} version(s) displayed (total: {_allVersions.Count})";
    }

    /// <summary>
    /// Initializes the versions grid.
    /// </summary>
    private void InitializeGrid()
    {
        _gridVersions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Version",
            HeaderText = "Version",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        var installButtonColumn = new DataGridViewButtonColumn
        {
            Name = "Install",
            HeaderText = "",
            Text = "Install",
            UseColumnTextForButtonValue = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 100
        };
        _gridVersions.Columns.Add(installButtonColumn);

        _gridVersions.CellClick += GridVersions_CellClick;
    }

    /// <summary>
    /// Loads the available Node.js versions.
    /// </summary>
    private async void LoadVersions()
    {
        try
        {
            _allVersions = await _downloader.GetAvailableVersionsAsync();
            _allVersions.Sort(); // Sort by version
            UpdateGrid(_allVersions);
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "Failed to load versions!";
            NotificationHelper.ShowError($"An error occurred while fetching the versions: {ex.Message}", "Error");
        }
    }

    /// <summary>
    /// Handles the CellClick event of the GridVersions control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void GridVersions_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == _gridVersions.Columns["Install"].Index && e.RowIndex >= 0)
        {
            var version = _gridVersions.Rows[e.RowIndex].Cells["Version"].Value.ToString();
            var result = NotificationHelper.ShowQuestion($"Are you sure you want to install version {version}?", "Confirmation");

            if (result == DialogResult.Yes)
            {
                await InstallVersion(version);
            }
        }
    }

    /// <summary>
    /// Installs the specified Node.js version.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    private async Task InstallVersion(string version)
    {
        _progressBar.Visible = true;
        _gridVersions.Enabled = false;
        _statusLabel.Text = $"Downloading version {version}...";

        try
        {
            var progress = new Progress<int>(value =>
            {
                _progressBar.Value = value;
                _statusLabel.Text = $"Downloading version {version}... ({value}%)";
            });

            await _downloader.DownloadAndInstallVersionAsync(version, progress);
            _statusLabel.Text = $"Version {version} installed successfully.";
            NotificationHelper.ShowInfo($"Version {version} has been installed successfully.", "Success");
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "Installation failed!";
            NotificationHelper.ShowError($"An error occurred during installation: {ex.Message}", "Error");
        }
        finally
        {
            _progressBar.Visible = false;
            _gridVersions.Enabled = true;
            _progressBar.Value = 0;
        }
    }
}
