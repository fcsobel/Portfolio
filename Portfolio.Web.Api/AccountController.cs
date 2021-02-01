using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Portfolio.Data.Context;
using Portfolio.Data.Service;
using Portfolio.Web.Api.Model;

namespace Portfolio.Web.Api
{
    [Authorize]
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly PortfolioContext _portfolioContext;
        private readonly DataService _dataService;

        public AccountController(ILogger<AccountController> logger, DataService dataService, PortfolioContext PortfolioContext)
        {
            _logger = logger;
            _portfolioContext = PortfolioContext;
            _dataService = dataService;
        }

        [HttpGet]
        [Route("")]
        public async Task<User> GetAccounts()
        {
                var userName = User.Identity.Name;

                var users = _portfolioContext.Users.ToList();


                var test = _portfolioContext.Database.GetDbConnection().ConnectionString;

                var user = await _portfolioContext.Users.Where(x => x.UserName == userName)
                    .Include(x=>x.LoginProfiles)
                    //.Include(x => x.Accounts).ThenInclude(y => y.Collections)
                    .Include(x => x.Accounts).ThenInclude(y => y.Investments).ThenInclude(i => i.Stock)
                    .Include(x => x.Accounts).ThenInclude(y => y.Investments).ThenInclude(i => i.Stock.PriceHistory)
                    .Include(x => x.Accounts).ThenInclude(y => y.Investments).ThenInclude(i => i.Stock.Dividends)
                    .FirstOrDefaultAsync();

                var stocks = user.Accounts.SelectMany(x => x.Investments).Select(x => x.Stock).Distinct().ToList();

                try
                {
                    // update each stock
                    foreach (var stock in stocks)
                    {
                        if (stock != null)
                        {
                            //if (stock.CacheDate != null && DateTime.Now.Subtract(stock.CacheDate.Value).Hours <= 1)
                            if (stock.Reload)
                            {
                                await this._dataService.UpdateStock(stock);
                            }
                        }
                    }
                    _logger.LogInformation("{stocks} updated", stocks.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }


                if (user != null)
                {
                    return new User(user);
                }
                else
                {
                    return null;
                }
        }


        [HttpPost]
        [Route("add")]
        public async Task<Account> AddAccount()
        {
            var userName = User.Identity.Name;

            var user = await _portfolioContext.Users.Where(x => x.UserName == userName)
                .Include(x => x.Accounts)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                // create account if necessary
                var account = new Portfolio.Data.Model.Account() { UserId = user.UserId, Owner = user, Name = "New Account", Url = "new-account" };
                _portfolioContext.Accounts.Add(account);
                _portfolioContext.SaveChanges();
                return new Account(account);
            }

            return null;
        }

        [HttpPost]
        [Route("delete")]
        public async Task<bool> DeleteAccount(Account obj)
        {
            var userName = User.Identity.Name;

            var user = await _portfolioContext.Users.Where(x => x.UserName == userName)
                .Include(x => x.Accounts)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var account = user.Accounts.Find(x => x.AccountId == obj.AccountId);

                if (account != null)
                {
                    _portfolioContext.Accounts.Remove(account);
                    _portfolioContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }


        [HttpPost]
        [Route("update")]
        public async Task<bool> UpdateTheAccount(Account obj = null)
        {
            var userName = User.Identity.Name;

            var user = await _portfolioContext.Users.Where(x => x.UserName == userName)
                .Include(x => x.Accounts)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if (obj.AccountId > 0)
                {
                    var account = user.Accounts.Find(x => x.AccountId == obj.AccountId);

                    if (account != null)
                    {
                        account.Name = obj.Name;

                        _portfolioContext.SaveChanges();

                        return true;
                    }
                }
            }

            return false;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<Investment> RefreshInvestment(Investment obj = null)
        {
            if (!string.IsNullOrWhiteSpace(obj.Ticker))
            {
                var stock = await this._dataService.GetStock(obj.Ticker).ConfigureAwait(false);
                if (stock != null)
                {
                    obj.Hydrate(stock);
                }
            }
            return obj;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<Investment> UpdateAccount(Investment obj = null)
        {
            var userName = User.Identity.Name;

            var user = _portfolioContext.Users.Where(x => x.UserName == userName)
                .Include(x => x.Accounts)
                .FirstOrDefault();

            if (user != null)
            {
                if (obj.AccountId > 0)
                {
                    var accountObj = user.Accounts.Find(x => x.AccountId == obj.AccountId);

                    if (accountObj != null)
                    {
                        //var accountObj = _PortfolioContext.Accounts.Where(x => x.Url == account).FirstOrDefault();
                        Portfolio.Data.Model.Investment item = null;

                        if (obj.InvestmentId > 0)
                        {
                            item = _portfolioContext.Investments.Where(x => x.InvestmentId == obj.InvestmentId)
                                .Include(x => x.Stock)
                                .FirstOrDefault();
                            if (item != null)
                            {
                                if (obj.Ticker != item.Stock.Ticker)
                                {
                                    if (!string.IsNullOrWhiteSpace(obj.Ticker))
                                    {
                                        var stock = await this._dataService.GetStock(obj.Ticker).ConfigureAwait(false);
                                        if (stock != null)
                                        {
                                            item.Stock = stock;
                                            item.StockId = stock.StockId;
                                            _portfolioContext.SaveChanges();
                                        }
                                    }
                                }
                                if (obj.Quantity > 0)
                                {
                                    item.Quantity = obj.Quantity;
                                    _portfolioContext.SaveChanges();
                                }
                                else
                                {
                                    item.Quantity = 0;
                                    _portfolioContext.Investments.Remove(item);
                                    _portfolioContext.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(obj.Ticker))
                            {
                                var stock = await this._dataService.GetStock(obj.Ticker).ConfigureAwait(false);
                                if (stock != null)
                                {
                                    item = new Portfolio.Data.Model.Investment()
                                    {
                                        Stock = stock,
                                        StockId = stock.StockId,
                                        Account = accountObj,
                                        Quantity = obj.Quantity

                                    };
                                    _portfolioContext.Investments.Add(item);
                                    _portfolioContext.SaveChanges();
                                }
                            }
                        }

                        return new Investment(item);
                    }
                }
            }

            return null;
        }


        [HttpGet]
        [Route("{account}")]
        public async Task<List<Investment>> GetAccount(string account, string symbol = null, string collection = null, decimal? quantity = null)
        {
            if (string.IsNullOrWhiteSpace(account)) return null;

            var userName = User.Identity.Name;
            var user = _portfolioContext.Users.Where(x => x.UserName == userName)
                .Include(x => x.Accounts)
                .FirstOrDefault();

            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(account))
                {
                    // check account
                    var model = user.Accounts.Where(x => x.Url == account).FirstOrDefault();

                    // create account if necessary
                    if (model == null)
                    {
                        model = new Portfolio.Data.Model.Account() { Owner = user, Name = account, Url = account.ToLower() };
                        _portfolioContext.Accounts.Add(model);
                        _portfolioContext.SaveChanges();
                    }

                    model = _portfolioContext.Accounts
                        .Where(x => x.Url == account)
                        .Include(x => x.Investments).ThenInclude(y => y.Stock)
                        //.Include(x => x.Collections)
                        .FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(symbol))
                    {
                        var stock = await this._dataService.GetStock(symbol);
                        if (stock != null)
                        {
                            var investment = new Portfolio.Data.Model.Investment() { Account = model, Quantity = quantity ?? 1, Stock = stock };
                            _portfolioContext.Investments.Add(investment);
                            _portfolioContext.SaveChanges();
                        }
                    }

                    var list = model.Investments.Select(x => new Investment(x)).ToList();

                    return list;
                }

            }

            return null;
        }
    }
}
