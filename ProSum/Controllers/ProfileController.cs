using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.ViewModels;
using ProSum.Services.Interfaces;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProSum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ISessionContainer _sessionContainer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public ProfileController(IProjectService projectService, IUserService userService, ISessionContainer sessionContainer, IHttpContextAccessor httpContextAccessor, ILogger logger)
        {
            _projectService = projectService;
            _userService = userService;
            _sessionContainer = sessionContainer;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

        }
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index(Guid userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            else
            {

                User retrieved = _userService.Get(userId);

                if (retrieved == null)
                {
                    return NotFound();
                }
                else
                {

                    List<LogEntryViewModel> model = new List<LogEntryViewModel>();
                    foreach (LogEntry log in _logger.GetUserLog(userId))
                    {
                        LogEntryViewModel entry = new LogEntryViewModel();
                        entry.Author = _userService.Get(log.UserId);
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
                                entry.UpdatedUser = _userService.Get(log.UpdatedUserId);
                                break;

                            case LogEntryUpdateType.ADD_EMPLOYEE:
                                entry.Project = _projectService.Get(log.ProjectId);
                                entry.UpdatedUser = _userService.Get(log.UpdatedUserId);
                                break;

                            case LogEntryUpdateType.CREATED_PROJECT:
                            case LogEntryUpdateType.UPDATED_PROJECT:
                            case LogEntryUpdateType.DELETED_PROJECT:
                                entry.Project = _projectService.Get(log.ProjectId);
                                break;

                            case LogEntryUpdateType.STATUS_UPDATE:
                                entry.Project = _projectService.Get(log.ProjectId);
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

                    ProfileViewModel viewModel = new ProfileViewModel()
                    {
                        Id = retrieved.Id,
                        Firstname = retrieved.FirstName,
                        Lastname = retrieved.LastName,
                        Email = retrieved.Email,
                        Phonenumber = retrieved.PhoneNumber,
                        ProjectCount = _projectService.GetProjectCount(userId),
                        logEntries = model,
                        Department = retrieved.Department.ToString(),
                        Role = retrieved.Role.Name,
                        Editable = _sessionContainer.GetSession(_httpContextAccessor).User.Id == userId

                    };
                    return View(viewModel);
                }

            }
        }
        [HttpGet]
        public IActionResult Edit(Guid userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            else if (_sessionContainer.GetSession(_httpContextAccessor).User.Id == userId || _sessionContainer.GetSession(_httpContextAccessor).User.Role.Name == "Admin")
            {
                User user = _userService.Get(userId);

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    EditProfileViewModel viewModel = new EditProfileViewModel();
                    viewModel.Id = user.Id;
                    viewModel.Firstname = user.FirstName;
                    viewModel.Lastname = user.LastName;
                    viewModel.Email = user.Email;
                    viewModel.Phonenumber = user.PhoneNumber;
                    return View(viewModel);
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult Edit(EditProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User userToUpdate = new User();
                userToUpdate.Id = viewModel.Id;
                userToUpdate.FirstName = viewModel.Firstname;
                userToUpdate.LastName = viewModel.Lastname;
                userToUpdate.Email = viewModel.Email;
                userToUpdate.PhoneNumber = viewModel.Phonenumber;
                _userService.Update(userToUpdate);

                return RedirectToAction(nameof(Index), new { userId = viewModel.Id });
            }
            else
            {
                return View(viewModel);
            }

        }
    }
}
