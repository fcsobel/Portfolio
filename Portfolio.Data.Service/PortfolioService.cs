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
    public class PortfolioService
    {
        private readonly ILogger<PortfolioService> _logger;
        private readonly PortfolioContext _portfolioContext;
        private readonly IEXService _iexService;

        public PortfolioService(ILogger<PortfolioService> logger, PortfolioContext PortfolioContext, IEXService IiexService)
        {
            this._logger = logger;
            this._portfolioContext = PortfolioContext;
            this._iexService = IiexService;
        }

        // Get User With Accounts
        public async Task<User> GetUser(string userName)
        {
            return await _portfolioContext.Users.Where(x => x.UserName == userName).Include(x => x.Accounts).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get User With Accounts with latest stock data
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserAccounts(string userName)
        {
            var user = await _portfolioContext.Users.Where(x => x.UserName == userName)
                .Include(x => x.LoginProfiles)
                .Include(x => x.Accounts).ThenInclude(y => y.Investments).ThenInclude(i => i.Stock)
                .Include(x => x.Accounts).ThenInclude(y => y.Investments).ThenInclude(i => i.Stock.PriceHistory)
                .Include(x => x.Accounts).ThenInclude(y => y.Investments).ThenInclude(i => i.Stock.Dividends)
                .FirstOrDefaultAsync();

            // get all stocks for this user
            var stocks = user.Accounts.SelectMany(x => x.Investments).Select(x => x.Stock).Distinct().ToList();

            // refresh stocks
            await this.RefreshStocks(stocks);

            return user;
        }


        /// <summary>
        /// Refresh stocks as needed
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public async Task RefreshStocks(List<Stock> stocks)
        {
            try
            {
                // update each stock
                foreach (var stock in stocks)
                {
                    if (stock != null && stock.Reload)
                    {
                        await this.UpdateStock(stock);
                    }
                }
                _logger.LogInformation("{stocks} updated", stocks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }



        public async Task<bool> DeleteAccount(long accountId)
        {
            var account = await _portfolioContext.Accounts.Where(x => x.AccountId == accountId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (account != null)
            {
                _portfolioContext.Accounts.Remove(account);
                await _portfolioContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Account> AddAccount(User user, string name)
        {
            var account  = new Account() { UserId = user.UserId, Owner = user, Name = name, Url = name.ToLower() };
            _portfolioContext.Accounts.Add(account);
            await _portfolioContext.SaveChangesAsync().ConfigureAwait(false);
            return account;
        }


        public async Task<Investment> UpdateInvestment(long id, string ticker, decimal quantity)
        {
            var item = await _portfolioContext.Investments.Where(x => x.InvestmentId == id)
                .Include(x => x.Stock)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (item != null)
            {
                if (ticker != item.Stock.Ticker)
                {
                    if (!string.IsNullOrWhiteSpace(ticker))
                    {
                        var stock = await this.GetStock(ticker).ConfigureAwait(false);
                        if (stock != null)
                        {
                            item.Stock = stock;
                            item.StockId = stock.StockId;
                            _portfolioContext.SaveChanges();
                        }
                    }
                }
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                    _portfolioContext.SaveChanges();
                }
                else
                {
                    item.Quantity = 0;
                    _portfolioContext.Investments.Remove(item);
                    _portfolioContext.SaveChanges();
                }
            }

            return item;
        }

        public async Task<Investment> AddInvestment(Account account, string ticker, decimal quantuty)
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