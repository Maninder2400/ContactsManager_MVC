using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
	//[AllowAnonymous]//all the action methods of this controller must be accessable if user is not authenticated
	[Route("[controller]")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager; //service layer for identity model classes(Application user) and operate on top of the inbuilt repository layer
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		[Route("[action]")]
		[Authorize("NotAuthorized")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[Route("[action]")]
		[Authorize("NotAuthorized")]
		//[AutoValidateAntiforgeryToken]//secure form XSRF requests
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			//check for validation errors
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(t => t.ErrorMessage);
				return View(registerDTO);
			}
			ApplicationUser user = new ApplicationUser()
			{
				PersonName = registerDTO.PersonName,
				Email = registerDTO.Email,
				PhoneNumber = registerDTO.Phone,
				UserName = registerDTO.Email
			};

			IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

			if (result.Succeeded)
			{
				//check status of radio button
				if(registerDTO.UserType == UserTypeOptions.Admin)
				{
					//create 'Admin' role
					if(await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
					{
						//create a admin role for the first time in Roles table
						ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.Admin.ToString() };
						await _roleManager.CreateAsync(applicationRole);
					}

					//add the new user into 'Admin' role via inserting it into UsersRoleTable (with Foriegn keys User id and role id)
					await _userManager.AddToRoleAsync(user,UserTypeOptions.Admin.ToString());
				}
				else
				{
					//create 'User' role
					if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
					{
						//create a User role for the first time in Roles table
						ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.User.ToString() };
						await _roleManager.CreateAsync(applicationRole);
					}

					//add the new user into 'User' role via inserting it into UsersRoleTable (with Foriegn keys User id and role id)
					await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
				}
				//sign IN
				await _signInManager.SignInAsync(user, isPersistent: false); //after successful insertion of the registration details of user into Identity database,and after registeration we want user to be signed in without logging in again this time,so this method will create a cokkie that user is already signedIn, will send that to browser ,and browser can store that cokkie for future reference and for every subsiquent requests after that browser will automatically attach this cokkie to request and send to browser,and the parameter isPersistent=true will tell browser that browser has to store that cokkie(persisted cokkie) for future too means if user was already signedIn and haven't logged out then user don't have to login again
				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("Register", error.Description);
				}
				return View(registerDTO);
			}
		}

		[HttpGet]
		[Route("[action]")]
		[Authorize("NotAuthorized")]
		public IActionResult Login()
		{
			return View();
		}

		[Route("[action]")]
		[HttpPost]
		[Authorize("NotAuthorized")]
		public async Task<IActionResult> Login(LoginDTO loginDTO,string? ReturnUrl)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(t => t.ErrorMessage);
				return View(loginDTO);
			}
			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);//this method will search this user email and passwordHash in the indentity database and if found then result will be success otherwise failure,and also in case of success it will create a signin cokkie,adds it to response.cokkies and send to browser,lockoutOnFailure ,when user enters password or email wrong 3times in a row it will block signin for a while (if true)

			if (result.Succeeded)
			{
				ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
				if(user != null)
				{
					if(await _userManager.IsInRoleAsync(user,UserTypeOptions.Admin.ToString())) 
					{
						return RedirectToAction("Index", "Home", new { area = "Admin" });
					}
				}
				if(!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
				{
					return LocalRedirect(ReturnUrl);
				}
				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}
			ModelState.AddModelError("Login", "Invalid Email or Password");
			return View(loginDTO);
		}

		[Route("[action]")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();//removing the identity cokkie and it adds a response header to clear cokkie
			return RedirectToAction(nameof(PersonsController.Index),"Persons");
		}

		[Route("[action]")]
		[AllowAnonymous]
		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser user = await _userManager.FindByEmailAsync(email);
			if(user == null)
			{
				return Json(true);
			}
			else
				return Json(false);
		}
	}
}
