using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c3o.Core;
using IEXSharp;
using IEXSharp.Model.CoreData.InvestorsExchangeData.Response;
using IEXSharp.Model.CoreData.ReferenceData.Response;
using IEXSharp.Model.CoreData.StockFundamentals.Response;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;
using IEXSharp.Model.CoreData.StockProfiles.Response;
using IEXSharp.Model.CoreData.StockResearch.Response;
using IEXSharp.Model.Shared.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Portfolio.Data.Model;

namespace Portfolio.Data.Service
{
    public class ApiSettings {
        public string Id { get; set; }
        public string Secret { get; set; }
    }

    public class IEXService 
    {
        private IEXCloudClient _client { get; set; }
        private readonly ILogger<IEXService> _logger;
        public IConfiguration Configuration { get; }

        public IEXService(ILogger<IEXService> logger, IConfiguration configuration) //  IEXCloudClient client
        {
            this.Configuration = configuration;
            this._logger = logger;
            this._client = new IEXCloudClient(configuration["IEXCloud:ApiKey"], configuration["IEXCloud:Secret"], signRequest: false, useSandBox: false);
        }

        //public async Task<bool> GetSymbols()
        //{
            
        //}

        public async Task<bool> RefreshStockLite(List<Stock> list)
        {
            var symbols = list.Select(x => x.Ticker).Take(10);

            var data = await this.LastAsync(symbols);

            foreach (var item in data)
            {
                var stock = list.Find(x => x.Ticker == item.symbol);

                if (stock != null)
                {
                    stock.Price = item.price;
                }
            }

            return true;
        }

        public async Task<bool> RefreshStock(Stock stock)
        {
            try
            {
                var company = await this.CompanyAsync(stock.Ticker);
                if (company != null)
                {
                    stock.Name = company.companyName;
                }

                // get price target
                var target = await this.PriceTargetAsync(stock.Ticker);
                if (target != null)
                {
                    stock.PriceTargetHigh = target.priceTargetHigh;
                    stock.PriceTargetLow = target.priceTargetLow;
                    stock.PriceTargetAverage = target.priceTargetAverage;
                    stock.PriceTargetAnalysts = target.numberOfAnalysts;
                    stock.PriceTargetUpdated = target.updatedDate;
                }

                //// get dividend percent
                stock.Dividend = await this.DividendYield(stock.Ticker).ConfigureAwait(false);

                if (stock.Dividend > 0)
                {
                    // get basic dividends
                    var divs = await this.DividendsBasic(stock.Ticker);
                    if (divs != null && divs.Any())
                    {
                        foreach (var div in divs)
                        {
                            var date = div.paymentDate.ParseDate();
                            if (date.HasValue)
                            {
                                var match = stock.Dividends.Find(x => x.Date == date);
                                if (match == null)
                                {
                                    stock.Dividends.Add(new StockDividend()
                                    {
                                        Date = date.Value,
                                        Dividend = div.amount ?? 0
                                    });
                                }
                            }
                        }
                    }
                }

                // get quote for price
                var quote = await this.QuoteAsync(stock.Ticker).ConfigureAwait(false);
                if (quote != null)
                {
                    stock.Price = quote.latestPrice ?? 0;

                    var date = quote.latestTime.ParseDate();

                    if (date.HasValue)
                    {
                        var match = stock.PriceHistory.Find(x=>x.Date == date.Value);
                        if (match == null)
                        {
                            stock.PriceHistory.Add(new StockPrice()
                            {
                                Date = date.Value,
                                High = quote.high ?? 0,
                                Low = quote.low ?? 0,
                                Close = quote.close ?? 0,
                                Open = quote.open ?? 0

                            });
                        }
                    }
                }

                bool paid = false;
                if (paid)
                {
                    stock.Price = await this.Price(stock.Ticker).ConfigureAwait(false);

                    // get quote for price
                    var dquote = await this.DelayedQuoteAsync(stock.Ticker).ConfigureAwait(false);
                    if (dquote != null)
                    {
                        stock.Price = dquote.delayedPrice;

                        if (!stock.PriceHistory.Any())
                        {
                            stock.PriceHistory.Add(new StockPrice()
                            {
                                Date = DateTime.Now,
                                High = dquote.high,
                                Low = dquote.low,
                            });
                        }
                    }

                }

                //// get dividend
                //var dividendYield = await this.KeyStatsAsync(stock.Ticker, "dividendYield").ConfigureAwait(false);
                //if (dividendYield != null)
                //{
                //    stock.Dividend = dividendYield.ParseDecimal(0);
                //}

                //var metric = await this.KeyStatsAsync(stock.Ticker).ConfigureAwait(false);
                //if (metric != null)
                //{
                //    stock.Dividend = metric.dividendYield ?? 0;
                //    //stock.DividendPerShareAnnual = metric.DividendPerShareAnnual;
                //    //stock.DividendGrowthRate5Y = metric.DividendGrowthRate5Y;
                //    //stock.DividendPerShare5Y = metric.DividendPerShare5Y;
                //    //stock.DividendsPerShareTTM = metric.DividendsPerShareTTM;
                //    //stock.DividendYield5Y = metric.DividendYield5Y;
                //    //stock.DividendYieldIndicatedAnnual = metric.DividendYieldIndicatedAnnual;
                //    stock.High_52Week = metric.week52high;
                //    stock.Low_52Week = metric.week52low;
                //    //stock.AverageTradingVolume10Day = metric._10DayAverageTradingVolume;
                //    //stock.PriceReturnDaily13Week = metric._13WeekPriceReturnDaily;
                //    //stock.PriceReturnDaily26Week = metric._26WeekPriceReturnDaily;
                //}

                _logger.LogInformation("{stock} updated", stock.Ticker);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<decimal> Price(string symbol)
        {
            var response = await _client.StockPrices.PriceAsync(symbol);
            return response.Data;
        }

        public async Task<PriceTargetResponse> PriceTargetAsync(string symbol)
        {
            var response = await _client.StockResearch.PriceTargetAsync(symbol);
            return response.Data;
        }

        public async Task<CompanyResponse> CompanyAsync(string symbol)
        {
            var response = await _client.StockProfiles.CompanyAsync(symbol);
            return response.Data;
        }

        public async Task<IEnumerable<SymbolResponse>> SymbolsAsync(string symbol)
        {
            var response = await _client.ReferenceData.SymbolsAsync();
            return response.Data;
        }

        public async Task<IEnumerable<SymbolCryptoResponse>> SymbolCryptoAsync(string symbol)
        {
            var response = await _client.ReferenceData.SymbolCryptoAsync();
            return response.Data;
        }

        public async Task<IEnumerable<SymbolMutualFundResponse>> SymbolsMutualFundAsync(string symbol)
        {
            var response = await _client.ReferenceData.SymbolsMutualFundAsync();
            return response.Data;
        }


        public async Task<IEnumerable<TagResponse>> TagsAsync(string symbol)
        {
            var response = await _client.ReferenceData.TagsAsync();
            return response.Data;
        }

        public async Task<IEnumerable<LastResponse>> LastAsync(string symbol)
        {
            var response = await _client.InvestorsExchangeDataService.LastAsync(new List<string> { symbol });
            return response.Data;
        }

        public async Task<IEnumerable<LastResponse>> LastAsync(IEnumerable<string> symbols)
        {
            var response = await _client.InvestorsExchangeDataService.LastAsync(symbols);
            return response.Data;
        }


        /// <summary>
        /// Provides basic dividend data for US equities, ETFs, and Mutual Funds for the last 5 years. 
        /// For 13+ years of history and comprehensive data, use the Advanced Dividends endpoint.
        /// - Weighting: 10 per symbol per period returned
        /// - Timing: End of day
        /// - Schedule: Updated at 9am UTC every day
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DividendBasicResponse>> DividendsBasic(string symbol)
        {
            var response = await _client.StockFundamentals.DividendsBasicAsync(symbol, IEXSharp.Model.CoreData.StockFundamentals.Request.DividendRange.OneYear).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<decimal> DividendYield(string symbol)
        {
            var response = await _client.StockResearch.KeyStatsStatAsync(symbol, "dividendYield");
            return response.Data.ParseDecimal(0) * 100;
        }

        public async Task<Quote> QuoteAsync(string symbol)
        {
            var response = await _client.StockPrices.QuoteAsync(symbol);
            return response.Data;
        }

        public async Task<DelayedQuoteResponse> DelayedQuoteAsync(string symbol)
        {
            var response = await _client.StockPrices.DelayedQuoteAsync(symbol);
            return response.Data;
        }

        public async Task<KeyStatsResponse> KeyStatsAsync(string symbol)
        {
            var response = await _client.StockResearch.KeyStatsAsync(symbol);
            return response.Data;
        }

        public async Task<string> KeyStatsAsync(string symbol, string stat)
        {
            var response = await _client.StockResearch.KeyStatsStatAsync(symbol, stat);
            return response.Data;
        }
    }
}