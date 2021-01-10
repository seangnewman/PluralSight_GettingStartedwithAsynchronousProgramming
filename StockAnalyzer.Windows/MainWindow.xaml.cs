using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        CancellationTokenSource cancellationTokenSource = null;

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            #region Before loading stock data
            var watch = new Stopwatch();
            watch.Start();
            StockProgress.Visibility = Visibility.Visible;

            Search.Content = "Cancel";
            #endregion

            #region Cancellation
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(() =>
            {
                Notes.Text += "Cancellation requested" + Environment.NewLine;
            });
            #endregion
            #region TaskFactory attach to parent
            try
            {
                Debug.WriteLine("Starting");
                // Task.Run() uses this internally
                await Task.Factory.StartNew( () => {
                    Task.Factory.StartNew( () => {
                        Thread.Sleep(1000);
                        Debug.WriteLine("Completing 1");
                    }, TaskCreationOptions.AttachedToParent);

                    Task.Factory.StartNew(() => {
                        Thread.Sleep(1000);
                        Debug.WriteLine("Completing 2");
                    },TaskCreationOptions.AttachedToParent);

                    Task.Factory.StartNew(() => {
                        Thread.Sleep(1000);
                        Debug.WriteLine("Completing 3");
                    },TaskCreationOptions.AttachedToParent);

                    
                });
                Debug.WriteLine("Completed");
                #endregion
                
                try
                {
                    #region Load All Stocks

                    #endregion
                    #region Task.Factory unwrap to retun the object rather than wait wait
                    //var service = new StockService();


                    //var operation = Task.Factory.StartNew(async (obj) => {

                    //    var stockService = obj as StockService;
                    //    var prices = await service.GetStockPricesFor("MSFT", CancellationToken.None);
                    //    return prices.Take(5);
                    //}, service).Unwrap();

                    //var result =  await operation;
                    #endregion
                    Notes.Text = Ticker.Text;
                }
                //catch (Exception ex)
                //{
                //    Notes.Text += ex.Message + Environment.NewLine;
                //}
                //finally
                //{
                //    cancellationTokenSource = null;
                //}


                 
            
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                cancellationTokenSource = null;
            }

            #region After stock data is loaded
            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
            StockProgress.Visibility = Visibility.Hidden;
            Search.Content = "Search";
            #endregion
        }

        private async Task Run()
        {
            var result = await Task.Run(  () => "PluralSight"              
                );

            if (result == "PluralSight")
            {
                Debug.WriteLine("PluralSight");
            }
        }

        private async Task LoadStocks(IProgress<IEnumerable<StockPrice>> progress = null)
        {
            var tickers = Ticker.Text.Split(',', ' ');

            var service = new StockService();

            var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
            
            foreach (var ticker in tickers)
            {
                var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

                loadTask = loadTask.ContinueWith(stockTask => {
                    progress?.Report(stockTask.Result);
                    return stockTask.Result;
                });

                tickerLoadingTasks.Add(loadTask);
            }

            var allStocks = await Task.WhenAll(tickerLoadingTasks);

            Stocks.ItemsSource = allStocks.SelectMany(stocks => stocks);
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

        public Task<IEnumerable<StockPrice>> GetStocksFor(string ticker)
        {
            var source = new TaskCompletionSource<IEnumerable<StockPrice>>();
            
            ThreadPool.QueueUserWorkItem(_ =>
            {

                try
                {
                    var prices = new List<StockPrice>();

                    var lines = File.ReadAllLines(@"StockPrices_Small.csv");

                    foreach (var line in lines.Skip(1))
                    {
                        var segments = line.Split(',');

                        for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');
                        var price = new StockPrice
                        {
                            Ticker = segments[0],
                            TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                            Volume = Convert.ToInt32(segments[6], CultureInfo.InvariantCulture),
                            Change = Convert.ToDecimal(segments[7], CultureInfo.InvariantCulture),
                            ChangePercent = Convert.ToDecimal(segments[8], CultureInfo.InvariantCulture),
                        };
                        prices.Add(price);
                    }

                    source.SetResult(prices.Where(price => price.Ticker == ticker));
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            });

            return source.Task;
        }

        public Task WorkInNotepad()
        {
            var source = new TaskCompletionSource<object>();
            var process = new Process{
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(@"Notepad.exe") {
                RedirectStandardError = true,
                UseShellExecute = false
              }
            };
            process.Exited += (sender, e) => {
                source.SetResult(null);
            };
            return source.Task;
        }
        Random random = new Random();
        private decimal CalculateExpensiveComputation(IEnumerable<StockPrice> stocks)
        {
            Thread.Yield();

            var computedValue = 0m;

            foreach (var stock in stocks)
            {
                for (int i = 0; i < stocks.Count() - 2; i++)
                {
                    for (int a = 0; a < random.Next(50, 60); a++)
                    {
                        computedValue += stocks.ElementAt(i).Change + stocks.ElementAt(i + 1).Change;
                    }
                }
            }

            return computedValue;
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
    }
}