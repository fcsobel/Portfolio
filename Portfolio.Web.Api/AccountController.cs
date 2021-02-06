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
        private readonly PortfolioService _portfolioService;

        public AccountController(ILogger<AccountController> logger, PortfolioService portfolioService, PortfolioContext PortfolioContext)
        {
            _logger = logger;
            _portfolioContext = PortfolioContext;
            _portfolioService = portfolioService;
        }

        [HttpGet]
        [Route("")]
        public async Task<User> GetAccounts()
        {
            var userName = User.Identity.Name;

            var user = await _portfolioService.GetUserAccounts(userName);

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

            var user = await _portfolioService.GetUser(userName);

            if (user != null)
            { 
                var account = await _portfolioService.AddAccount(user, "New Account");

                return new Account(account);
            }

            return null;
        }


        [HttpPost]
        [Route("delete")]
        public async Task<bool> DeleteAccount(Account obj)
        {
            var userName = User.Identity.Name;

            var user = await _portfolioService.GetUser(userName);

            if (user != null)
            {
                // check for account
                var account = user.Accounts.Find(x => x.AccountId == obj.AccountId);

                if (account != null)
                {
                    await _portfolioService.DeleteAccount(account.AccountId);
                    
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

            var user = await _portfolioService.GetUser(userName);

            if (user != null)
            {
                if (obj.AccountId > 0)
                {
                    // check for account
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
                var stock = await this._portfolioService.GetStock(obj.Ticker).ConfigureAwait(false);
                if (stock != null)
                {
                    obj.Hydrate(stock);
                }
            }
            return obj;
        }
        

        [HttpPost]
        [Route("")]
        public async Task<Investment> UpdateInvestment(Investment obj = null)
        {
            var userName = User.Identity.Name;

            var user = await _portfolioService.GetUser(userName);

            if (user != null)
            {
                if (obj.AccountId > 0)
                {
                    var accountObj = user.Accounts.Find(x => x.AccountId == obj.AccountId);

                    if (accountObj != null)
                    {
                        Portfolio.Data.Model.Investment item = null;

                        if (obj.InvestmentId > 0)
                        {
                            item = await _portfolioService.UpdateInvestment(obj.InvestmentId, obj.Ticker, obj.Quantity);
                        }
                        else
                        {
                            item = await _portfolioService.AddInvestment(accountObj, obj.Ticker, obj.Quantity);
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
            
            var user = await _portfolioService.GetUser(userName);

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
                        var stock = await this._portfolioService.GetStock(symbol);
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