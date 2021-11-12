using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using booksClub.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace booksClub.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BooksClubContext BooksClub;
        public HomeController(ILogger<HomeController> logger, BooksClubContext context)
        {
            _logger = logger;
            BooksClub = context;
        }
        protected override void Dispose(bool disposing)
        {
            BooksClub.Dispose();
            base.Dispose(disposing);
        }

        public IActionResult AddBookToLisRead(int i)
        {
            var addedBook = BooksClub.Books.Where(x=>x.Id==i).FirstOrDefault();
            if (addedBook != null)
            {
                var userBook = BooksClub.UserBooks.Where(x => x.IdBook == i && x.UserName == User.Identity.Name).FirstOrDefault();
                if (userBook != null)
                    return Content($"{addedBook.Name} уже находится в вашем списке прочитанного");
                var addBook = new UserBook
                {
                    IdBook = i,
                    UserName = User.Identity.Name
                };
                BooksClub.UserBooks.Add(addBook);
                BooksClub.SaveChanges();
                return Content($"{addedBook.Name} успешно добавлена в список прочитанного");
            }
            return Content("Ошибка добавления книги в список прочитанного");
        }

        public IActionResult RemoveBookToLisRead(int i)
        {
            var removeBook = BooksClub.UserBooks.Where(x=>x.IdBook==i && x.UserName==User.Identity.Name).FirstOrDefault();
            if(removeBook!=null)
            {
                BooksClub.UserBooks.Remove(removeBook);
                BooksClub.SaveChanges();
                return Content("книга успешно удалена");
            }
            return Content("нечего удалять");
        }


        public IActionResult AllBooks()
        {
            return View(BooksClub);
        }
        public IActionResult UserReadBooks()
        {
            return View(BooksClub);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(User user)
        {
            if (ModelState.IsValid)
            {
                User user1 = await BooksClub.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
                if (user1 == null)
                {
                    user1 = user;
                    BooksClub.Users.Add(user1);
                    BooksClub.SaveChanges();
                }
                await Authenticate(user.Login); // аутентификация
                return RedirectToAction("AllBooks", "Home");
            }
            return RedirectToAction("Index");
        }

        private async Task Authenticate(string login)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
