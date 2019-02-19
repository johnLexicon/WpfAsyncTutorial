using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;


/// <summary>
/// Asynchronous Basics:
/// When you have an await in a method the you have to declare the method as async.
/// </summary>

namespace WpfTutorial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Playing With Async";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void ExecuteSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew(); //More precise than Datetime.

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time {elapsedMs}";
        }

        private async void ExecuteAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew(); //More precise than Datetime.

            await RunDownloadParallelASync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time {elapsedMs}";
        }

        private async Task RunDownloadASync()
        {
            List<string> websites = PrepData();

            foreach (var site in websites)
            {
                //Task.Run() is used to wrap around code that you want to run asynchronous.
                //The await keyword tells the code to run the task asynchronously but wait for the result.
                //The point to do this is to let the let the UI-thread do its other jobs while a new thread takes care of the DownloadWebsite operation. 
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }

        private async Task RunDownloadParallelASync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>(); 

            foreach (var site in websites)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            //When all tasks are complete return an array of WebsiteDataModel.
            //This means that it will take as long time as retrieving the data for the slowest site.
            var results = await Task.WhenAll(tasks);

            foreach(var result in results)
            {
                ReportWebsiteInfo(result);
            }
        }

        private void RunDownloadSync()
        {
            List<string> websites = PrepData();

            foreach(var site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        private void ReportWebsiteInfo(WebsiteDataModel model)
        {
            resultsWindow.Text += $"{model.WebsiteUrl} downloaded: {model.WebsiteData.Length} characters long. {Environment.NewLine}";
        }

        private WebsiteDataModel DownloadWebsite(string websiteUrl)
        {
            var model = new WebsiteDataModel();
            WebClient client = new WebClient();

            model.WebsiteUrl = websiteUrl;
            model.WebsiteData = client.DownloadString(websiteUrl);

            return model;
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            var model = new WebsiteDataModel();
            WebClient client = new WebClient();

            model.WebsiteUrl = websiteUrl;
            model.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

            return model;
        }

        private List<string> PrepData()
        {
            resultsWindow.Text = "";

            return new List<string>()
            {
                { "https://www.yahoo.com/" },
                { "https://www.google.com/" },
                { "https://www.microsoft.com/" },
                { "https://www.cnn.com/" },
                { "https://www.codeproject.com/" },
                { "https://www.stackoverflow.com/" },
                { "https://www.github.com/" },
                { "https://www.youtube.com/" }
            };
        }
    }
}
