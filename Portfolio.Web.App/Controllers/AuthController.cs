using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Data.Context;
using Portfolio.Data.Model;
using Portfolio.Web.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portfolio.Web.App.Controllers
{
    [AllowAnonymous]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly PortfolioContext _portfolioContext;

        public AuthController(ILogger<AuthController> logger, PortfolioContext portfolioContext)
        {
            _logger = logger;
            _portfolioContext = portfolioContext;
        }


        [HttpGet]
        [Route("")]
        public ActionResult Login(string returnUrl = "/")
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (model.Login != "fred.sobel@gmail.com")
            {
                return Unauthorized();
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, model.Login),
                new Claim(ClaimTypes.Role, "admin"),
            };

            // identity for autrhenication scheme
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // user principal accoiated to autentication scheme
            var principal = new ClaimsPrincipal(identity);

            // login user by scheme 
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = model.RememberLogin }
                );

            return LocalRedirect(model.ReturnUrl);
        }


        [HttpGet]
        [Route("end")]
        public async Task<IActionResult> End()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }


        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        [Route("LoginWithGoogle")]
        public IActionResult LoginWithGoogle(string returnUrl = "/")
        {
            // Properties for google auth
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleLoginCallback"), // method to call when google auth is complete
                Items = { { "returnUrl", returnUrl } } // context information that can be sent along
            };

            // Return Challenge Result
            // Redirect to google challenge
            // Name of scheme that needs to be challenged
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }


        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        [Route("GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            // get result from google signinscheme
            var externalAuth = await HttpContext.AuthenticateAsync("ExternalAuth");

            // google claims
            var externalClaims = externalAuth.Principal.Claims.ToList();

            var key = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            // google guid / key - subject id
            var externalKey = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var externalEmail = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var externalName = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            var externalImage = externalClaims.FirstOrDefault(x => x.Type == "image").Value;

            User user = null;

            // get uset by profile 
            var profile = _portfolioContext.LoginProfiles
                .Where(x => x.ProviderKey == externalKey && x.ProviderName == "Google")
                .Include(x => x.User)
                .FirstOrDefault();

            if (profile != null)
            {
                // update profile
                profile.LoginDate = DateTime.Now;
                profile.LoginCount++;
                user = profile.User;
                _portfolioContext.SaveChanges();
            }

            if (user == null)
            {
                // get user by google id & create profile
                user = _portfolioContext.Users.Where(x => x.UserName == externalEmail).FirstOrDefault();

                // create user if needed
                if (user == null)
                {
                    // hash password
                    var passwordHasher = new PasswordHasher<string>();
                    var _passwordHash = passwordHasher.HashPassword(externalEmail, "welcome");
                    user = new Portfolio.Data.Model.User() { Email = externalEmail, UserName = externalEmail, PasswordHash = _passwordHash, };
                    _portfolioContext.Users.Add(user);
                    _portfolioContext.SaveChanges();
                }

                if (user != null)
                {
                    profile = new LoginProfile()
                    {
                        LoginCount = 1,
                        ProviderName = "Google",
                        ProviderKey = externalKey,
                        UserName = externalEmail,
                        Picture = externalImage,
                        LoginDate = DateTime.Now,
                        Created = DateTime.Now,
                    };

                    user.LoginProfiles.Add(profile);
                    _portfolioContext.SaveChanges();
                }
            }

            var claims = new List<Claim>();


            foreach (var claimn in externalClaims)
            {
                if (claimn.Type != ClaimTypes.NameIdentifier && claimn.Type != ClaimTypes.Role && claimn.Type != ClaimTypes.Email && claimn.Type != ClaimTypes.Name)
                {
                    claims.Add(new Claim(claimn.Type, claimn.Value));
                }
            }

            if (user != null)
            {
                var newClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "google"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Role, "user"),
                    //new Claim(ClaimTypes.Role, user.UserId.ToString()),
                    //new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                };
                foreach (var claimn in newClaims)
                {
                    claims.Add(new Claim(claimn.Type, claimn.Value));
                }
            }

            // create identity for cookie scheme
            var itentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(itentity);

            // delete temporary cookue used during google auth
            await HttpContext.SignOutAsync("ExternalAuth");

            // login with cookie auth
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMonths(1)
                    }
                );

            return LocalRedirect(externalAuth.Properties.Items["returnUrl"]);
        }



    }
}
