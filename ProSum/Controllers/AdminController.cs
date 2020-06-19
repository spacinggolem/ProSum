using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.Helpers;
using ProSum.Models.ViewModels;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProSum.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoleContainer _RoleContainer;
        private readonly IAdminService _AdminService;
        private readonly ISessionContainer _SessionContainer;
        private readonly IHttpContextAccessor _ContextAccessor;
        private readonly RolesEnum[] Roles = { RolesEnum.Admin };
        private readonly ILogger _Logger;
        private readonly IUserService _UserService;
        private readonly IProjectService _ProjectService;
        private readonly IStepContainer _StepContainer;
        private readonly IPermissionContainer _PermissionContainer;

        public AdminController(
            IRoleContainer roleContainer,
            IAdminService adminService,
            ISessionContainer sessionContainer,
            IHttpContextAccessor contextAccessor,
            ILogger logger,
            IUserService userService,
            IProjectService projectService,
            IStepContainer stepContainer,
            IPermissionContainer permissionContainer
            )
        {
            _RoleContainer = roleContainer;
            _AdminService = adminService;
            _SessionContainer = sessionContainer;
            _ContextAccessor = contextAccessor;
            _Logger = logger;
            _UserService = userService;
            _ProjectService = projectService;
            _StepContainer = stepContainer;
            _PermissionContainer = permissionContainer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/Index" });
            }

            return View();
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/CreateUser" });
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(CreateUserViewModel model, string returnUrl = null)
        {


            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/CreateUser" });
            }
            else if (ModelState.IsValid)
            {
                Role role = _RoleContainer.Roles.Where(role => role.Name == model.Role.ToString()).FirstOrDefault();

                User newUser = new User(
                    model.Firstname,
                    model.Lastname,
                    model.Username,
                    PasswordHasher.HashPassword(model.Password),
                    model.Email,
                    model.PhoneNumber,
                    role,
                    model.Department
                );

                User retrieved = _UserService.GetByEmail(newUser.Email);
                if (retrieved == null)
                {
                    _AdminService.CreateUser(newUser);
                    _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, LogEntryUpdateType.ADD_EMPLOYEE, newUser.Id);
                    return RedirectToAction("Index", "Admin");

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
        public IActionResult Logs()
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/Logs" });
            }

            List<LogEntry> logs = _Logger.GetLogs();
            List<LogEntryViewModel> model = new List<LogEntryViewModel>();
            foreach (LogEntry log in logs)
            {
                LogEntryViewModel entry = new LogEntryViewModel();
                entry.Author = _UserService.Get(log.UserId);
                entry.UpdateType = log.UpdateType;
                entry.TimeStamp = log.TimeStamp;
                switch (entry.UpdateType)
                {
                    case LogEntryUpdateType.CREATED_CLIENT:
                    case LogEntryUpdateType.UPDATED_CLIENT:
                    case LogEntryUpdateType.DELETED_CLIENT:
                    case LogEntryUpdateType.CREATED_USER:
                    case LogEntryUpdateType.UPDATED_USER:
                    case LogEntryUpdateType.DELETED_USER:
                        entry.UpdatedUser = _UserService.Get(log.UpdatedUserId);
                        break;

                    case LogEntryUpdateType.ADD_EMPLOYEE:
                        entry.Project = _ProjectService.Get(log.ProjectId);
                        entry.UpdatedUser = _UserService.Get(log.UpdatedUserId);
                        break;

                    case LogEntryUpdateType.CREATED_PROJECT:
                    case LogEntryUpdateType.UPDATED_PROJECT:
                    case LogEntryUpdateType.DELETED_PROJECT:
                        entry.Project = _ProjectService.Get(log.ProjectId);
                        break;

                    case LogEntryUpdateType.STATUS_UPDATE:
                        entry.Project = _ProjectService.Get(log.ProjectId);
                        entry.Step = log.Step;
                        entry.NewStatus = log.NewStepStatus;
                        entry.OldStatus = log.OldStepStatus;
                        break;
                }
                // If the project doens't exist anymore, don't show it
                if (entry.Project != null)
                {
                    model.Add(entry);
                }
            }
            model = model.OrderByDescending(Log => Log.TimeStamp).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateStep()
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/CreateStep" });
            }

            CreateStepViewModel model = new CreateStepViewModel();
            model.Steps = _StepContainer.Steps.OrderBy(Step => Step.StepNumber).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateStep([FromForm] List<Step> steps)
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/CreateStep", steps });
            }

            List<Step> currentSteps = _StepContainer.Steps.OrderBy(Step => Step.StepNumber).ToList();

            List<Step> newSteps = steps.FindAll(Step => Step.Id == Guid.Empty);
            // A step has been added
            if (newSteps.Count > 0)
            {
                foreach (Step newStep in newSteps)
                {
                    newStep.Id = Guid.NewGuid();
                    _PermissionContainer.CreatePermission("Read" + newStep.Name);
                    _PermissionContainer.CreatePermission("Write" + newStep.Name);
                    _StepContainer.CreateStep(newStep);
                    currentSteps.Add(newStep);
                }
            }

            List<Step> deletedSteps = currentSteps.FindAll(Step => steps.Find(x => x.Id == Step.Id) == null);
            // A step has been deleted
            if (deletedSteps.Count > 0)
            {
                foreach (Step deletedStep in deletedSteps)
                {
                    _StepContainer.DeleteStep(deletedStep);
                    currentSteps.Remove(currentSteps.Find(Step => Step.Id == deletedStep.Id));

                    Permission writePermission = _PermissionContainer.Permissions.FirstOrDefault(Permission => Permission.Name == $"Write{deletedStep.Name}");
                    Permission readPermission = _PermissionContainer.Permissions.FirstOrDefault(Permission => Permission.Name == $"Read{deletedStep.Name}");

                    _PermissionContainer.DeletePermission(writePermission);
                    _PermissionContainer.DeletePermission(readPermission);

                }
            }

            // Update step numbers
            List<Step> changedStepNumbers = steps.FindAll(Step => Step.StepNumber != currentSteps.Find(x => x.Id == Step.Id).StepNumber);
            if (changedStepNumbers.Count > 0)
            {
                foreach (Step changedStep in changedStepNumbers)
                {
                    _StepContainer.UpdateStepNumber(changedStep);
                }
            }

            // Step names has been changed
            List<Step> changedStepNames = steps.FindAll(Step => Step.Name != currentSteps.Find(x => x.Id == Step.Id).Name);
            if(changedStepNames.Count > 0)
            {
                foreach (Step changedStep in changedStepNames)
                {
                    _PermissionContainer.UpdatePermissionName(changedStep.Name, currentSteps.Find(Step => Step.Id == changedStep.Id).Name);
                    _StepContainer.UpdateStepName(changedStep);
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public IActionResult Users()
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = "/Admin/Users" });
            }

            List<User> users = _UserService.GetAll();
            return View(users);
        }

        [HttpGet]
        public IActionResult EditUser(Guid userId)
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Admin/EditUser", userId });
            }

            User user = _UserService.Get(userId);
            EditUserViewModel model = new EditUserViewModel()
            {
                Username = user.Name,
                userId = user.Id,
                Department = user.Department,
                Email = user.Email,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Password = "not edited",
                Role = (RolesEnum)Enum.Parse(typeof(RolesEnum), user.Role.Name),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(EditUserViewModel model)
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Admin/EditUser", model });
            }

            if (ModelState.IsValid)
            {
                User user = _UserService.Get(model.userId);
                if (user != null)
                {
                    if (model.Password != "not edited")
                    {
                        model.Password = PasswordHasher.HashPassword(model.Password);
                    }

                    user.Update(model.Firstname, model.Lastname, model.Username, model.Password, model.Email, model.PhoneNumber, _RoleContainer.Roles.FirstOrDefault(r => r.Name == model.Role.ToString()), model.Department);
                    _UserService.Update(user);
                }
            }
            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public IActionResult DeleteUser(Guid userId)
        {
            if (!_SessionContainer.HasAccess(_ContextAccessor, Roles))
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Admin/EditUser", userId });
            }

            User user = _UserService.Get(userId);
            if(user != null)
            {
                if(_SessionContainer.GetSession(_ContextAccessor) != null && user.Id != _SessionContainer.GetSession(_ContextAccessor).Id)
                _UserService.Delete(user);
            }
            return RedirectToAction("Users", "Admin");
        }
    }
}