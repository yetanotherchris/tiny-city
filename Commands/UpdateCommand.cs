using Spectre.Console;
using Spectre.Console.Cli;

namespace TinyCity.Commands
{
    public class UpdateCommandSettings : CommandSettings
    {
    }

    public class UpdateCommand : Command<UpdateCommandSettings>
    {
        public override int Execute(CommandContext context, UpdateCommandSettings settings)
        {
            string fileUrl = "";

            switch (Environment.OSVersion.Platform)
            {   
                case PlatformID.Win32NT:
                    fileUrl = "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity.exe";
                    break;
                case PlatformID.Unix:
                    fileUrl = "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(fileUrl))
            {
                AnsiConsole.MarkupLine("[red]Unsupported OS.[/]");
                return 1;
            }

            // Download the file
            string downloadFilename = $"{Environment.ProcessPath}.new";
            var task = Task.Run(async () =>
            {
                AnsiConsole.MarkupLine($"[green]Url: '{fileUrl}'.[/]");
                await Download(downloadFilename, fileUrl);
            });
            task.Wait(TimeSpan.FromMinutes(5));

            // Backup the current process
            string backupFilename = $"{Environment.ProcessPath}.bak";
            if (Path.Exists(backupFilename))
            {
                File.Delete(backupFilename);
            }
            var processFileInfo = new FileInfo(Environment.ProcessPath);
            processFileInfo.MoveTo(backupFilename);

            // Rename the downloaded file to the current process filename
            string newFilename = downloadFilename.Replace(".new", "");
            var downloadedFileInfo = new FileInfo(downloadFilename);
            downloadedFileInfo.MoveTo(newFilename);

            // ...tinycity.bak removal is done on startup

            return 0;
        }

        private async Task Download(string localFilename, string fileUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using (FileStream fileStream = new FileStream(localFilename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await CopyWithProgressAsync(response.Content, fileStream);
                }

                AnsiConsole.MarkupLine("File downloaded successfully!");
            }
        }

        static async Task CopyWithProgressAsync(HttpContent content, Stream destination)
        {
            long totalBytes = content.Headers.ContentLength ?? -1;
            long totalBytesCopied = 0;
            byte[] buffer = new byte[8092];
            int bytesRead;

            var progressBar = AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                        {
                            new TaskDescriptionColumn(),
                            new ProgressBarColumn(),    
                            new PercentageColumn(),     
                            new RemainingTimeColumn(),  
                            new SpinnerColumn(),        
                            new TransferSpeedColumn(),  
                        });

            await progressBar.StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"[green]Downloading {totalBytes} bytes:[/]");
                task.MaxValue(totalBytes);
                task.StartTask();

                while (!ctx.IsFinished)
                {
                    using (Stream sourceStream = await content.ReadAsStreamAsync())
                    {
                        while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await destination.WriteAsync(buffer, 0, bytesRead);
                            totalBytesCopied += bytesRead;

                            if (bytesRead > 0)
                            {
                                task.Increment(bytesRead);
                            }
                        }
                    }
                }
            });
        }
    }
}
