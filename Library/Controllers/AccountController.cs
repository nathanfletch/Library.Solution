using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Library.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
  public class AccountController : Controller
  {
    private readonly LibraryContext _db;
    private RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController (RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, LibraryContext db)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _roleManager = roleManager;
      _db = db;
    }

    public async Task<IActionResult> Index()
    {
      if(_roleManager.Roles.ToList().Count == 0)
      {
        IdentityResult librarianResult = await _roleManager.CreateAsync(new IdentityRole("Librarian"));
        IdentityResult visitorResult = await _roleManager.CreateAsync(new IdentityRole("Visitor"));
      }
      return View();
    }
    public IActionResult Register()
    {
      List<IdentityRole> roles = _roleManager.Roles.ToList();
      // ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      ViewBag.Id = new SelectList(_roleManager.Roles, "Id", "Name");
      //selectlist with roles
      return View();
    }
    [HttpPost]
    public async Task<ActionResult> Register (RegisterViewModel model, string Id)
    {
      //option to choose "librarian" in the view form
      //if librarian, var user = new Librian();
      //else 
      var user = new ApplicationUser { UserName = model.Email };
      IdentityResult userCreateResult = await _userManager.CreateAsync(user, model.Password);
      if (userCreateResult.Succeeded)
      {
        //find the role
        /*
          var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          var currentUser = await _userManager.FindByIdAsync(userId);
          item.User = currentUser;
         */
        var selectedRole = await _roleManager.FindByIdAsync(Id);
        IdentityResult roleAddResult = await _userManager.AddToRoleAsync(user, selectedRole.Name);
        if(roleAddResult.Succeeded)
        {
          Console.WriteLine($"Added role! :D");
        }
        else
        {
          Console.WriteLine($"Failed to add role!");
        }
        //create new
        _db.Patrons.Add( new Patron() { Name = user.UserName} );
        _db.SaveChanges();
        return RedirectToAction("Index");
      }
      else
      {
        return View();
      }
    }

    public ActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
      Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }
      else
      {
        return View();
      }
    }

    [HttpPost]
    public async Task<ActionResult> LogOff()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index");
    }
  }
}