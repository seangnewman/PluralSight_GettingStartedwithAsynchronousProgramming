using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            #region Before loading stock data
            var watch = new Stopwatch();
            watch.Start();
            StockProgress.Visibility = Visibility.Visible;
            StockProgress.IsIndeterminate = true;
            #endregion
            try
            {
                await GetStocks();
            }
            catch (Exception ex )
            {

                Notes.Text += ex.Message;
            }
           
            #region After stock data is loaded
            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
            StockProgress.Visibility = Visibility.Hidden;
            #endregion
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // By marking as async we introduce capability to use 
        public async Task GetStocks()
        {
            using (var client = new HttpClient())
            {
                //Get the http response message
                // Pause execution until the result is available
                // await introduces continuation that allows us to get back to main thread
                var response = await client.GetAsync($"http://localhost:61363/api/stocks/{Ticker.Text}");
                try
                {
                    // Catch an exception if the response is not successful 
                    response.EnsureSuccessStatusCode();
                    // We need to await until the async ReadAsString has completed
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

                    Stocks.ItemsSource = data;
                }
                catch (Exception ex)
                {
                    // If an exception occurs, we are back to the main UI thread that allows us to record messages
                    Notes.Text += ex.Message;
                }
            }  // end using

        }
    }
}
