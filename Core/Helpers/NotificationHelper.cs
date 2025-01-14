
namespace NodeVersionSwitcher.Core.Helpers;

/// <summary>
/// Helper class to show notifications to the user.
/// </summary>
internal static class NotificationHelper
{
    /// <summary>
    /// Shows an information message to the user.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    internal static void ShowInfo(string message, string title = "Node Version Switcher")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Shows an error message to the user.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    internal static void ShowError(string message, string title = "Node Version Switcher")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Shows a warning message to the user.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    internal static DialogResult ShowQuestion(string message, string title = "Node Version Switcher")
    {
        return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }
}

