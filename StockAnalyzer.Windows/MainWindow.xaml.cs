using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using StockAnalyzer.Windows.Services;

namespace StockAnalyzer.Windows
{
    public partial class MainWindow : Window
    {

        CancellationTokenSource cancellationTokenSource = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
        //    StockProgress.IsIndeterminate = true;
        //    #endregion
        //    try
        //    {
        //        await GetStocks();
        //    }
        //    catch (Exception ex )
        //    {

        //        Notes.Text += ex.Message;
        //    }
           
        //    #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.Visibility = Visibility.Hidden;
        //    #endregion
        //}


        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
        //    StockProgress.IsIndeterminate = true;
        //    #endregion

        //    //Queue work passed to run on a different  thread pool
        //    await Task.Run(() =>
        //    {

        //        // This will execute on a separate thread
        //        var lines = File.ReadAllLines(@"StockPrices_Small.csv");

        //        var data = new List<StockPrice>();

        //        foreach (var line in lines.Skip(1))
        //        {
        //            var segments = line.Split(',');

        //            for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');
        //            var price = new StockPrice
        //            {
        //                Ticker = segments[0],
        //                TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
        //                Volume = Convert.ToInt32(segments[6], CultureInfo.InvariantCulture),
        //                Change = Convert.ToDecimal(segments[7], CultureInfo.InvariantCulture),
        //                ChangePercent = Convert.ToDecimal(segments[8], CultureInfo.InvariantCulture),
        //            };
        //            data.Add(price);
        //        }

        //        // UI objects on the main thread, use the Dispatcher to acess main thread
        //        Dispatcher.Invoke(() =>
        //        {
        //            /// Make sure not to introduce a blocker to the UI thread
        //            Stocks.ItemsSource = data.Where(price => price.Ticker == Ticker.Text);
        //        });
                

        //    });


        //    // Using the await keyboard ensures that continuation does not occur until previous code has completed
        //    #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.Visibility = Visibility.Hidden;
        //    #endregion
        //}

        //private  void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion

        //    if (cancellationTokenSource !=null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();
        //    cancellationTokenSource.Token.Register( () => { 
        //       Notes.Text = "Cancellation Requested";
        //    });
        //    //Queue work passed to run on a different  thread pool
        //    //var loadLinesTask =  Task.Run(() =>
        //    //{

        //    //    // This will execute on a separate thread
        //    //    var lines = File.ReadAllLines(@"StockPrices_Small.csv");
        //    //    return lines;

        //    // });
        //    var loadLinesTask = SearchForStocks(cancellationTokenSource.Token);
        //    var processStocksTask = loadLinesTask.ContinueWith(t => {

        //        var lines = t.Result;
        //        var data = new List<StockPrice>();

        //        foreach (var line in lines.Skip(1))
        //        {
        //            var segments = line.Split(',');

        //            for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');
        //            var price = new StockPrice
        //            {
        //                Ticker = segments[0],
        //                TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
        //                Volume = Convert.ToInt32(segments[6], CultureInfo.InvariantCulture),
        //                Change = Convert.ToDecimal(segments[7], CultureInfo.InvariantCulture),
        //                ChangePercent = Convert.ToDecimal(segments[8], CultureInfo.InvariantCulture),
        //            };
        //            data.Add(price);
        //        }

        //        // UI objects on the main thread, use the Dispatcher to acess main thread
        //        Dispatcher.Invoke(() =>
        //        {
        //                    /// Make sure not to introduce a blocker to the UI thread
        //                    Stocks.ItemsSource = data.Where(price => price.Ticker == Ticker.Text);
        //        });

        //    }, cancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);

        //    loadLinesTask.ContinueWith(t =>
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            Notes.Text = t.Exception.InnerException.Message;
        //        });
        //    }, TaskContinuationOptions.OnlyOnFaulted);
        //    // This should only complete after the previous continuation
        //    processStocksTask.ContinueWith(_ => {

        //    Dispatcher.Invoke(() => { 
        //                 #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.Visibility = Visibility.Hidden;
        //        Search.Content = "Search";
        //            #endregion
        //        });
        //    });

        //   // cancellationTokenSource = null;
        //}

        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion

        //    if (cancellationTokenSource != null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();
        //    cancellationTokenSource.Token.Register(() => {
        //        Notes.Text = "Cancellation Requested";
        //    });

        //    try
        //    {
        //        var service = new StockService();
        //        var data = await service.GetStockPricesFor(Ticker.Text, cancellationTokenSource.Token);

        //        Stocks.ItemsSource = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        Notes.Text += ex.Message + Environment.NewLine;
                 
        //    }

        //    // This should only complete after the previous continuation
            

        //        Dispatcher.Invoke(() => {
        //            #region After stock data is loaded
        //            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //            StockProgress.Visibility = Visibility.Hidden;
        //            Search.Content = "Search";
        //            #endregion
        //        });
        

        //    // cancellationTokenSource = null;
        //}

        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion
        //    #region Cancellation
        //    if (cancellationTokenSource != null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();
        //    cancellationTokenSource.Token.Register(() => {
        //        Notes.Text = "Cancellation Requested";
        //    });
        //#endregion
        //    try
        //    {
        //        var tickers = Ticker.Text.Split(',', ' ');
        //        var service = new StockService();
        //        //ConcurrentBag is threadsafe
        //        var stocks = new ConcurrentBag<StockPrice>();
        //        var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
                
        //        foreach (var ticker in tickers)
        //        {

                   
        //            var loadTask =  service.GetStockPricesFor(ticker, cancellationTokenSource.Token)
        //                .ContinueWith(t => {
        //                    foreach (var stock in t.Result.Take(5))
        //                    {
        //                        stocks.Add(stock);
        //                    }
        //                    Dispatcher.Invoke( ()=> {
        //                        Stocks.ItemsSource = stocks.ToArray();
        //                    });
        //                    return t.Result;
        //                })  ;
        //            tickerLoadingTasks.Add(loadTask);
                    
        //        }

        //        var timeoutTask = Task.Delay(250);
        //        var allStocksLoadingTask =  Task.WhenAll(tickerLoadingTasks);

        //        //var completedTask = await  Task.WhenAny(timeoutTask, allStocksLoadingTask);

        //        //if(completedTask == timeoutTask)
        //        //{
        //        //    cancellationTokenSource.Cancel();
        //        //    cancellationTokenSource = null;
        //        //    throw new Exception("Timeout!");
        //        //}


        //        //Stocks.ItemsSource = allStocksLoadingTask.Result.SelectMany(stocks => stocks);
        //        await allStocksLoadingTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        Notes.Text += ex.Message + Environment.NewLine;

        //    }

        //    // This should only complete after the previous continuation


        //    Dispatcher.Invoke(() => {
        //        #region After stock data is loaded
        //        StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //        StockProgress.Visibility = Visibility.Hidden;
        //        Search.Content = "Search";
        //       // Notes.Text = String.Empty;
        //        #endregion
        //    });


        //    // cancellationTokenSource = null;
        //}

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var result = await GetStockFor(Ticker.Text);
            Notes.Text += $"Stocks loaded!{Environment.NewLine}";
        }

        private async Task<IEnumerable<StockPrice>> GetStockFor(string ticker)
        {
            var service = new StockService();

            var stocks = await service.GetStockPricesFor(ticker, CancellationToken.None)
                //Configure await allows you to continue on the thread without heading back to the main thread
                .ConfigureAwait(false);
            

            return stocks.Take(5);
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

        private Task<List<string>> SearchForStocks(CancellationToken cancellationToken)
        {
            var loadLinesTask = Task.Run(async () =>
            {
                var lines = new List<string>();

                using (var stream = new StreamReader(File.OpenRead(@"StockPrices_small.csv")))
                {
                    string line;
                    while ((line = await stream.ReadLineAsync()) != null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return lines;
                        }
                          
                          lines.Add(line);
                    }
                }

                return lines;
            }, cancellationToken);

            return loadLinesTask;
        }

        

    }
}
