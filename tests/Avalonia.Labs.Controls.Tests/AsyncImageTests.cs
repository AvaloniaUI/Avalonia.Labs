using System.Net;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Labs.Controls.Cache;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Labs.Controls.Tests;

public class AsyncImageTests
{
    [AvaloniaFact]
    public void Custom_HttpClient_From_CacheOptions_Is_Used_To_Download_Images()
    {
        var handler = new RecordingHandler();
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("X-Test-Header", "value");

        var previous = CacheOptions.Default.HttpClient;
        CacheOptions.Default.HttpClient = client;
        try
        {
            var image = new AsyncImage
            {
                IsCacheEnabled = false,
                Template = new FuncControlTemplate<AsyncImage>((_, scope) => new Panel
                {
                    Children =
                    {
                        new Image { Name = "PART_PlaceholderImage" }.RegisterInNameScope(scope),
                        new Image { Name = "PART_Image" }.RegisterInNameScope(scope)
                    }
                })
            };
            var window = new Window { Content = image };
            window.Show();

            string? failed = null;
            image.Failed += (_, e) => failed = e.ErrorException?.ToString();
            image.Source = "https://example.invalid/image.png";

            var deadline = DateTime.UtcNow.AddSeconds(10);
            while (!handler.Request.Task.IsCompleted && DateTime.UtcNow < deadline)
            {
                Dispatcher.UIThread.RunJobs();
            }

            Assert.True(handler.Request.Task.IsCompleted, $"The custom HttpClient was not used. Failed={failed}");
            var request = handler.Request.Task.Result;
            Assert.Equal(new Uri("https://example.invalid/image.png"), request.RequestUri);
            Assert.True(request.Headers.TryGetValues("X-Test-Header", out var values));
            Assert.Equal("value", Assert.Single(values));

            window.Close();
        }
        finally
        {
            CacheOptions.Default.HttpClient = previous;
        }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public TaskCompletionSource<HttpRequestMessage> Request { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request.TrySetResult(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
