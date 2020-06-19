using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.Helpers;
using ProSum.Models.ViewModels;
using ProSum.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProSum.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService usersql;
        private readonly ISessionContainer sessionContainer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectService _projectService;

        public UserController(IUserService usercontext, ISessionContainer sessionContainer, IHttpContextAccessor httpContextAccessor, IProjectService projectService)
        {
            usersql = usercontext;
            this.sessionContainer = sessionContainer;
            _httpContextAccessor = httpContextAccessor;
            _projectService = projectService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            Session session = sessionContainer.GetSession(_httpContextAccessor);
            if (session != null)
            {
                if (returnUrl != null)
                {
                    if (session.HasAccess(RolesEnum.Admin))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Project");
                    }
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                User submit = new User()
                {
                    Email = viewModel.Email,
                    Password = viewModel.Password
                };


                User retrieved = usersql.Login(submit);

                if (retrieved != null)
                {
                    if (PasswordHasher.ValidatePassword(submit.Password, retrieved.Password))
                    {
                        _projectService.GetProjectPermissions(retrieved);
                        Session session = sessionContainer.CreateSession(retrieved, _httpContextAccessor);
                        if (returnUrl == null)
                        {
                            if (session.HasAccess(RolesEnum.Admin))
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            return RedirectToAction("Index", "Project");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        return View(viewModel);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Onjuiste combinatie");
                    return View(viewModel);
                }


            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpPost]
        public IActionResult LogOut()
        {
            sessionContainer.DeleteSession(_httpContextAccessor);
            return RedirectToAction("Login", "User");
        }
    }
}
