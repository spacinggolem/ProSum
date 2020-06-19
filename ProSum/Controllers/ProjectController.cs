using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.ViewModels;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProSum.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _ProjectService;
        private readonly IUserService _userService;
        private readonly ISessionContainer _SessionContainer;
        private readonly IHttpContextAccessor _ContextAccessor;
        private readonly IPermissionContainer _permissionContainer;
        private readonly IClientService _ClientService;
        private readonly ILogger _Logger;
        private readonly IStepContainer _StepContainer;
        private readonly IAnnouncementContainer _AnnouncementContainer;
        private readonly IProjectFileContainer _ProjectFileContainer;

        public ProjectController(
            IProjectService projectService,
            IHttpContextAccessor contextAccessor,
            ISessionContainer sessionContainer,
            IUserService userService,
            IPermissionContainer permissionContainer,
            IClientService clientService,
            IStepContainer stepContainer,
            ILogger logger,
            IAnnouncementContainer announcementContainer,
            IProjectFileContainer projectFileContainer)
        {
            _ProjectService = projectService;
            _SessionContainer = sessionContainer;
            _ContextAccessor = contextAccessor;
            _userService = userService;
            _permissionContainer = permissionContainer;
            _ClientService = clientService;
            _StepContainer = stepContainer;
            _Logger = logger;
            _AnnouncementContainer = announcementContainer;
            _ProjectFileContainer = projectFileContainer;
        }

        // /project/
        [HttpGet]
        public IActionResult Index()
        {
            // Project overview
            List<ProjectViewModel> model = new List<ProjectViewModel>();

            List<Project> projects = _ProjectService.Get();

            foreach (Project project in projects)
            {
                model.Add(new ProjectViewModel()
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    DeadLine = project.Deadline,
                    ProjectManagers = project.ProjectManagers,
                    Steps = project.Steps.OrderBy(step => step.Step.StepNumber).ToList(),
                    Client = project.Client
                });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AddEmployee(Guid projectId)
        {
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.HasAccess(_ContextAccessor, new RolesEnum[] { RolesEnum.Manager }))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/AddEmployee?projectId={projectId}" });
            }


            ProjectAddEmployee model = new ProjectAddEmployee();
            model.ProjectId = _ProjectService.Get(projectId).Id;
            model.ProjectEmployees = _ProjectService.GetAllUsersInProject(projectId);
            List<User> allEmployees = _userService.GetAllWithRole(RolesEnum.Employee);
            allEmployees.AddRange(_userService.GetAllWithRole(RolesEnum.Manager));
            List<User> employeesNotInProject = new List<User>();

            foreach (User employee in allEmployees)
            {
                if (!model.ProjectEmployees.Any(u => u.Id == employee.Id))
                {
                    employeesNotInProject.Add(employee);
                }
            }

            model.Employees = employeesNotInProject;

            return View(model);
        }

        [HttpPost]
        public IActionResult AddEmployee(ProjectAddEmployee data)
        {
            Project project = _ProjectService.Get(data.ProjectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/AddEmployee?projectId={data.ProjectId}" });
            }

            if (ModelState.IsValid)
            {
                if (data.IsProjectLeader)
                {
                    _ProjectService.AddProjectPermission(
                    _ProjectService.Get(data.ProjectId),
                    _userService.Get(data.NewEmployeeId),
                    _permissionContainer.Permissions.FirstOrDefault(p => p.Name == "ProjectManager")
                    );
                }
                else
                {
                    _ProjectService.AddProjectPermission(
                        _ProjectService.Get(data.ProjectId),
                        _userService.Get(data.NewEmployeeId),
                        _permissionContainer.Permissions.FirstOrDefault(p => p.Name == "ProjectEmployee"));
                }


                _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, data.ProjectId, LogEntryUpdateType.UPDATED_USER_PERMISSIONS, data.NewEmployeeId);
                return RedirectToAction(nameof(ProjectController.EditEmployeePermission), new { userId = data.NewEmployeeId, projectId = data.ProjectId });

            }
            else
            {
                List<User> allEmployees = _userService.GetAllWithRole(RolesEnum.Employee);
                List<User> employeesNotInProject = new List<User>();
                data.ProjectEmployees = _ProjectService.GetAllUsersInProject(data.ProjectId);

                foreach (User employee in allEmployees)
                {
                    if (!data.ProjectEmployees.Any(u => u.Id == employee.Id))
                    {
                        employeesNotInProject.Add(employee);
                    }
                }

                data.Employees = employeesNotInProject;
                return View(data);
            }
        }

        [HttpGet]
        public IActionResult EditEmployeePermission(Guid userId, Guid projectId)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/EditEmployeePermission?userId={userId}&projectId={projectId}" });
            }

            PermissionViewModel model = new PermissionViewModel()
            {
                UserId = userId,
                ProjectId = projectId
            };

            List<Step> steps = _StepContainer.Steps.ToList();
            List<StepPermission> stepPermissions = new List<StepPermission>();
            foreach (Step step in steps)
            {
                stepPermissions.Add(new StepPermission()
                {
                    Step = step
                });
            }

            model.Permissions = stepPermissions.OrderBy(p => p.Step.StepNumber).ToList();

            User user = _userService.Get(userId);
            _ProjectService.GetProjectPermissions(user);

            model.CurrentPermissions = user.GetProjectPermission(projectId);
            model.IsProjectLeader = user.GetProjectPermission(projectId).HasPermission("ProjectManager");
            return View(model);
        }

        [HttpPost]
        public IActionResult EditEmployeePermission(PermissionViewModel data)
        {
            Project project = _ProjectService.Get(data.ProjectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/EditEmployeePermission?userId={data.UserId}&projectId={data.ProjectId}" });
            }

            User user = _userService.Get(data.UserId);
            _ProjectService.GetProjectPermissions(user);

            if (user.GetProjectPermission(project.Id) != null)
            {
                foreach (Step step in _StepContainer.Steps)
                {
                    if (user.GetProjectPermission(project.Id).HasPermission("Read" + step.Name))
                    {
                        _ProjectService.RemoveProjectPermission(
                            project,
                            _userService.Get(data.UserId),
                            _permissionContainer.Permissions.FirstOrDefault(p => p.Name == "Read" + step.Name));
                    }

                    if (user.GetProjectPermission(project.Id).HasPermission("Write" + step.Name))
                    {
                        _ProjectService.RemoveProjectPermission(
                            project,
                            _userService.Get(data.UserId),
                            _permissionContainer.Permissions.FirstOrDefault(p => p.Name == "Write" + step.Name));
                    }
                }
            }

            foreach (StepPermission stepPermission in data.Permissions)
            {
                if (stepPermission.Permission.ToString() != "None")
                {
                    _ProjectService.AddProjectPermission(
                       project,
                        _userService.Get(data.UserId),
                        _permissionContainer.Permissions.FirstOrDefault(p => p.Name == stepPermission.Permission.ToString() + stepPermission.Step.Name));
                }
            }

            if (!data.IsProjectLeader && user.GetProjectPermission(project.Id).HasPermission("ProjectManager"))
            {
                _ProjectService.RemoveProjectPermission(
                        project,
                        _userService.Get(data.UserId),
                        _permissionContainer.Permissions.FirstOrDefault(p => p.Name == "ProjectManager"));
            }
            else if (data.IsProjectLeader && !user.GetProjectPermission(project.Id).HasPermission("ProjectManager"))
            {
                _ProjectService.AddProjectPermission(
                       project,
                        _userService.Get(data.UserId),
                        _permissionContainer.Permissions.FirstOrDefault(p => p.Name == "ProjectManager"));
            }

            _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, data.ProjectId, LogEntryUpdateType.UPDATED_USER_PERMISSIONS);

            return RedirectToAction("Details", "Project", new { projectId = data.ProjectId });
        }

        // /project/details?projectId={Id}
        [HttpGet]
        public IActionResult Details(Guid projectId)
        {
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.HasAccess(_ContextAccessor, new RolesEnum[] { RolesEnum.Employee, RolesEnum.Manager }))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Details?projectId={projectId}" });
            }

            Project project = _ProjectService.Get(projectId);

            List<Permission> projectPermissions = new List<Permission>();
            if (_SessionContainer.GetSession(_ContextAccessor) != null)
            {
                ProjectPermission permissions = _SessionContainer.GetSession(_ContextAccessor).User.GetPermissions.FirstOrDefault(p => p.Project.Id == projectId);
                if (permissions != null)
                {
                    projectPermissions = permissions.Permissions.ToList();
                }
            }

            ProjectViewModel model = new ProjectViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                DeadLine = project.Deadline,
                Steps = project.Steps,
                Client = project.Client,
                UserPermissions = projectPermissions,
                ProjectEmployees = _ProjectService.GetAllUsersInProject(projectId),
                ProjectManagers = _ProjectService.GetAllProjectManagersInProject(projectId),
                Announcements = _AnnouncementContainer.Get(projectId)
            };

            // Project Detail Page
            return View(model);
        }

        // /project/create
        [HttpGet]
        public IActionResult Create()
        {
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.HasAccess(_ContextAccessor, new RolesEnum[] { RolesEnum.Manager }))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Create" });
            }


            List<Client> clients = _ClientService.Get();
            CreateProjectViewModel model = new CreateProjectViewModel()
            {
                Clients = clients
            };
            // Create Project Page
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateProjectViewModel model)
        {
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.HasAccess(_ContextAccessor, new RolesEnum[] { RolesEnum.Manager }))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Create" });
            }
            if (ModelState.IsValid)
            {

                if (_ClientService.Get(model.Client) != null)
                {
                    Project newProject = new Project(model.Title, model.DeadLine, model.Description, model.Client);


                    newProject.ProjectManagers.Add(_SessionContainer.GetSession(_ContextAccessor).User);

                    _ProjectService.Create(newProject);

                    _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, newProject.Id, LogEntryUpdateType.CREATED_PROJECT);

                    return RedirectToAction(nameof(ProjectController.Details), new { projectId = newProject.Id });
                }
                else
                {
                    ViewData["error"] = "Invalid client Id";
                    return View(model);
                }
            }
            else
            {
                if (model.DeadLine == DateTime.MinValue)
                {
                    ModelState.Remove("DeadLine");
                    ModelState.AddModelError("DeadLine", "Voer een geldige datum in.");
                }
                model.Clients = _ClientService.Get();
                return View(model);
            }


        }

        // /project/edit?projectId={Id}
        [HttpGet]
        public IActionResult Edit(Guid projectId)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Edit?projectId={projectId}" });
            }

            if (projectId == null)
            {
                return NotFound();
            }
            EditProjectViewModel model = new EditProjectViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                DeadLine = project.Deadline,
                Client = project.ClientId,
                Clients = _ClientService.Get()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditProjectViewModel model)
        {
            Project project = _ProjectService.Get(model.Id);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Edit?projectId={model.Id}" });
            }

            model.Clients = _ClientService.Get();
            if (ModelState.IsValid)
            {

                // Edit project here
                Project newProject = new Project()
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Deadline = model.DeadLine,
                    ClientId = model.Client
                };
                _ProjectService.Edit(model.Id, newProject);
                _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, newProject.Id, LogEntryUpdateType.UPDATED_PROJECT);
                return RedirectToAction(nameof(ProjectController.Details), new { projectId = newProject.Id });
            }
            else
            {
                if (model.DeadLine == DateTime.MinValue)
                {
                    ModelState.Remove("DeadLine");
                    ModelState.AddModelError("DeadLine", "Voer een geldige deadline in.");
                }
                model.Clients = _ClientService.Get();
                return View(model);
            }

        }

        [HttpPost]
        public IActionResult UpdateStatus(Guid projectId, Guid stepId, Step.Status status)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectEmployee(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            ProjectStep step = project.Steps.Find(step => step.Step.Id == stepId);
            if (step != null)
            {
                Permission permission = _permissionContainer.Permissions.FirstOrDefault(Permission => Permission.Name == $"Write{step.Step.Name}");
                bool hasPermission = _SessionContainer.GetSession(_ContextAccessor).User.HasPermission(project, permission);

                if (hasPermission)
                {
                    Step.Status oldStatus = step.Status;
                    step.Status = status;
                    _ProjectService.UpdateProjectStatus(project.Id, step);
                    _Logger.Log(_SessionContainer.GetSession(_ContextAccessor).User.Id, projectId, stepId, LogEntryUpdateType.STATUS_UPDATE, oldStatus, status);
                }
                else
                {
                    return NotFound();
                }
            }
            return Ok("Status bijgewerkt!");
        }

        [HttpPost]
        public IActionResult Delete(Guid projectId)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Delete?projectId={projectId}" });
            }

            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.HasAccess(_ContextAccessor, new RolesEnum[] { RolesEnum.Manager }) || !_SessionContainer.IsProjectManager(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Delete?projectId={projectId}" });
            }

            Permission permission = _permissionContainer.Permissions.FirstOrDefault(Permission => Permission.Name == $"ProjectManager");
            bool hasPermission = _SessionContainer.GetSession(_ContextAccessor).User.HasPermission(project, permission);

            if (hasPermission)
            {
                _ProjectService.Delete(projectId);
                return RedirectToAction("Index", "Project");
            }
            return RedirectToAction("AccessDenied", "Permission");
        }

        [HttpGet]
        public IActionResult AddAnnouncement(Guid projectId)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectEmployee(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/AddAnnouncement?projectId={projectId}" });
            }

            CreateAnnouncementViewModel model = new CreateAnnouncementViewModel()
            {
                ProjectId = projectId
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult AddProjectFile(Guid projectId)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectEmployee(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/AddAnnouncement?projectId={projectId}" });
            }

            CreateFileViewModel model = new CreateFileViewModel() { ProjectId = projectId };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddProjectFile(CreateFileViewModel model)
        {
            Project project = _ProjectService.Get(model.ProjectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectEmployee(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/AddFile?projectId={model.ProjectId}" });
            }
            if (ModelState.IsValid)
            {
                Session session = _SessionContainer.GetSession(_ContextAccessor);
                ProjectFile projectFile = new ProjectFile(session.User.Id, model.ProjectId, model.Link, model.Title, model.Department);
                
                
                if(session.User.Department == model.Department || session.User.GetProjectPermission(model.ProjectId).HasPermission("ProjectManager"))
                {
                    _ProjectFileContainer.CreateProjectFile(projectFile);

                    return RedirectToAction("Announcements", "Project", new { projectId = model.ProjectId });
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return View(model);
            }
        }


        [HttpPost]
        public IActionResult AddAnnouncement(CreateAnnouncementViewModel model)
        {
            Project project = _ProjectService.Get(model.ProjectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectEmployee(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/AddAnnouncement?projectId={model.ProjectId}" });
            }
            if (ModelState.IsValid)
            {
                Guid userId = _SessionContainer.GetSession(_ContextAccessor).User.Id;
                Announcement announcement = new Announcement(model.ProjectId, userId, model.Title, model.Message);
                _AnnouncementContainer.AddAnnouncementToDB(announcement);

                return RedirectToAction("Announcements", "Project", new { projectId = model.ProjectId });

            }
            else
            {
                return View(model);
            }

        }

        public IActionResult Announcements(Guid projectId)
        {
            Project project = _ProjectService.Get(projectId);
            if (_SessionContainer.IsLoggedIn(_ContextAccessor))
            {
                if (!_SessionContainer.IsProjectEmployee(_ContextAccessor, project))
                {
                    return RedirectToAction("AccessDenied", "Permission");
                }
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Project/Announcements?projectId={projectId}" });
            }

            List<Announcement> announcements = new List<Announcement>();
            announcements = _AnnouncementContainer.Get(projectId);

            List<ProjectFile> projectFiles = new List<ProjectFile>();
            projectFiles = _ProjectFileContainer.GetListForProject(projectId);

            if (announcements.Count > 0)
            {
                foreach (Announcement announcement in announcements)
                {
                    announcement.Author = _userService.Get(announcement.AuthorId);
                }
            }

            AnnouncementViewModel model = new AnnouncementViewModel()
            {
                ProjectId = projectId,
                Announcements = announcements.OrderByDescending(announcement => announcement.Timestamp).ToList(),
                ProjectFiles = projectFiles.Where(File => File.Department == _userService.Get(File.AccountId).Department || File.Department == DepartmentEnum.None).OrderByDescending(File => File.Department).ToList()
            };
            return View(model);
        }
    }
}