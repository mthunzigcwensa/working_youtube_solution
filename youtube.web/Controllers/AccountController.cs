using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using youtube.Application.Common.Interfaces;
using youtube.Application.Common.Utility;
using youtube.Application.Services.Implementation;
using youtube.Application.Services.Interfaces;
using youtube.Domain.Entities;
using youtube.Domain.ViewModels;

namespace youtube.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChannelService _channelService;
        private readonly IUserService _userService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            IChannelService channelService,
            IUserService userService


            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _channelService = channelService;
            _userService = userService;
        }


        public IActionResult Login(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl
            };

            return View(loginVM);
        }


        [HttpGet]
        public JsonResult GetLoggedInUser()
        {
            try
            {
                var userData = _userService.GetLoggedInUserData(User);
                return Json(new { success = true, data = userData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Video");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User)).Wait();
            }

            RegisterVM registerVM = new()
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                RedirectUrl = returnUrl
            };

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    Name = registerVM.Name,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = true,
                    UserName = registerVM.Email,
                    ProfilePic = "",
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    
                    if (!string.IsNullOrEmpty(registerVM.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_User);
                    }

                    
                    await _channelService.CreateChannelAsync(user.Id, user.Name);

                    
                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, registerVM.Password, isPersistent: false, lockoutOnFailure: false);

                    if (signInResult.Succeeded)
                    {
                        TempData["success"] = "Account created successfully! You are now logged in.";
                    }
                    else
                    {
                        TempData["error"] = "Account created, but failed to log you in automatically. Please log in manually.";
                        return RedirectToAction("Login", "Account");
                    }

                    
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Video");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Video");
                        }
                        else
                        {
                            return LocalRedirect(registerVM.RedirectUrl);
                        }
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager
                    .PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(loginVM.Email);
                    TempData["success"] = "You have logged in successfully!";
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Video");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Video");
                        }
                        else
                        {
                            return LocalRedirect(loginVM.RedirectUrl);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(loginVM);
        }


        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult users()
        {
            var users = _unitOfWork.User.GetAll();
            return View(users);
        }



        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Adminhome() { return View(); }

        #region API Calls
        [HttpGet]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult GetAll()
        {
            IEnumerable<ApplicationUser> objUsers;
            objUsers = _unitOfWork.User.GetAll();

            return Json(new { data = objUsers });
        }
        [HttpDelete]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(string id)
        {
            try
            {
                var user = _unitOfWork.User.Get(u => u.Id == id);

                if (user != null)
                {
                    _unitOfWork.User.Remove(user);
                    _unitOfWork.Save();
                    return Json(new { success = true, message = "User deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        #endregion
    }
}
