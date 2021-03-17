using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.Data.Context;
using Portfolio.Web.App.Models;

namespace Portfolio.Web.App.Controllers
{
    [Authorize(Roles = "site.admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        private PortfolioContext _context { get; set; }

        public AdminController(ILogger<AdminController> logger, PortfolioContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}