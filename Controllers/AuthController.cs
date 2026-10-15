using Microsoft.AspNetCore.Mvc;

namespace apuracao.johnmarlon.com.br.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login() => Redirect("/");

        [HttpPost]
        public IActionResult Login(string? password) => Redirect("/");

        [HttpGet]
        public IActionResult Logout() => Redirect("/");
    }
}
