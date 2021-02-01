using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Data.Context;
using Portfolio.Data.Model;

namespace Portfolio.Data.Service
{
    public class DataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly PortfolioContext _portfolioContext;
        private readonly IEXService _iexService;

        public DataService(ILogger<DataService> logger, PortfolioContext PortfolioContext, IEXService IiexService)
        {
            this._logger = logger;
            this._portfolioContext = PortfolioContext;
            this._iexService = IiexService;
        }


        public async Task<Account> AddAccount(User user, string name)
        {
            var account  = new Account() { UserId = user.UserId, Owner = user, Name = name, Url = name.ToLower() };

            _portfolioContext.Accounts.Add(account);

            await _portfolioContext.SaveChangesAsync().ConfigureAwait(false);

            return account;
        }

        public async Task<Investment> AddInvestment(Account account,  string ticker, decimal quantuty)
        {
            if (!string.IsNullOrWhiteSpace(ticker))
            {
                var stock = await this.GetStock(ticker).ConfigureAwait(false);
                if (stock != null)
                {
                    var item = new Portfolio.Data.Model.Investment()
                    {
                        AccountId = account.AccountId,
                        Stock = stock,
                        StockId = stock.StockId,
                        Account = account,
                        Quantity = quantuty

                    };
                    _portfolioContext.Investments.Add(item);
                    _portfolioContext.SaveChanges();

                    return item;
                }
            }
            return null;
        }


        public async Task<Stock> GetStock(string symbol)
        {
            var stock = new Stock();

            stock = _portfolioContext.Stocks.Where(x => x.Ticker == symbol)
                .Include(x => x.Dividends)
                .Include(x => x.PriceHistory)
                .FirstOrDefault();

            if (stock == null)
            {
                // add stock record
                stock = new Stock() { Ticker = symbol };
                this._portfolioContext.Stocks.Add(stock);
            }

            if (stock.Reload)
            {
                stock = await UpdateStock(stock).ConfigureAwait(false);
            }

            var repsonse = await _portfolioContext.SaveChangesAsync();

            return stock;
        }


        public async Task<Stock> UpdateStock(Stock stock)
        {
            if (stock.Ticker == "$")
            {
                stock.Price = 1;
                var repsonse1 = await _portfolioContext.SaveChangesAsync().ConfigureAwait(false);
                return stock;
            }

            // load stock data from api
            if (
                    await _iexService.RefreshStock(stock) 
                    //await _polygonService.RefreshStock(stock)  ||     
                    //await _finnhubService.RefreshStock(stock) 
                )
            {
                if (stock.CacheDate == null)
                {
                    Random random = new Random();
                    int offset = random.Next(0, 60);
                    stock.CacheDate = DateTime.Now.AddMinutes(offset);
                }
                else
                {
                    stock.CacheDate = DateTime.Now;
                }
            }

            var repsonse = await _portfolioContext.SaveChangesAsync().ConfigureAwait(false);

            return stock;
        }
    }
}