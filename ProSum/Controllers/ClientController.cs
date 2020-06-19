using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.ViewModels;
using ProSum.Services.Interfaces;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProSum.Controllers
{
    public class ClientController : Controller
    {
        private readonly ISessionContainer _SessionContainer;
        private readonly IClientService _ClientService;
        private readonly ILogger _Logger;
        private readonly IHttpContextAccessor _ContextAccessor;

        public ClientController(IClientService clientService, ILogger logger, ISessionContainer sessionContainer, IHttpContextAccessor http)
        {
            _ClientService = clientService;
            _Logger = logger;
            _SessionContainer = sessionContainer;
            _ContextAccessor = http;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateClientViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client(
                    model.Name,
                    model.Email,
                    model.Company,
                    model.PhoneNumber);

                Client retrieved = _ClientService.GetByEmail(model.Email);
                if (retrieved == null)
                {
                    _ClientService.Create(client);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Detail", "Client", new { clientId = client.Id });
                    }

                }
                else
                {
                    ModelState.AddModelError("Email", "E-mailadres is al in gebruik.");
                    return View(model);
                }

            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(Guid clientid)
        {
            if (clientid == null || clientid == Guid.Empty)
            {
                return NotFound();
            }

            Client client = _ClientService.Get(clientid);
            EditClientViewModel model = new EditClientViewModel()
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Company = client.Company,
                Phonenumber = client.PhoneNumber
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(EditClientViewModel model)
        {
            Client client = new Client()
            {
                Name = model.Name,
                Email = model.Email,
                Company = model.Company,
                PhoneNumber = model.Phonenumber
            };
            _ClientService.Edit(model.Id, client);

            _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, LogEntryUpdateType.UPDATED_CLIENT, client.Id);

            return RedirectToAction("Index", "Client");
        }
        [HttpGet]
        public IActionResult Detail(Guid clientid)
        {
            if (clientid == null)
            {
                return NotFound();
            }
            Client client = _ClientService.Get(clientid);

            return View(client);

        }
    }
}
