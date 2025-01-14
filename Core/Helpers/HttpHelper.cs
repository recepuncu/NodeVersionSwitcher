namespace NodeVersionSwitcher.Core.Helpers;

internal static class HttpHelper
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<string> GetHtmlContentAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DownloadFileAsync(string url, IProgress<int>? progress = null)
    {
        var tempFile = Path.GetTempFileName();

        using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var buffer = new byte[8192];
                long totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    if (totalBytes != -1 && progress != null)
                    {
                        var progressPercentage = (int)(totalBytesRead * 100 / totalBytes);
                        progress.Report(progressPercentage);
                    }
                }
            }
        }

        return tempFile;
    }
}
