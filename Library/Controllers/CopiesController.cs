using Microsoft.AspNetCore.Mvc;
using Library.Models;
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

namespace Library.Controllers
{
  [Authorize]
  public class CopiesController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CopiesController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      //for librarian - 
      //if user, show x, if librarian, show y, if anon, show number only - book details?
      //1. skeleton (CRUD) 2. edit book details 3. patrons/checkouts 4. auth stuff
      //show current 
      List<Copy> model = _db.Copies.Include(copy => copy.Book).ToList();
      return View(model);

      // var userId = this.User.FindFirst(ClaimTypes.TitleIdentifier)?.Value;
      // var currentUser = await _userManager.FindByIdAsync(userId);
      // var userCopies = _db.Copies.Where(entry => entry.User.Id == currentUser.Id).ToList();

    }
    
    public ActionResult Details(int id)
    {
      Copy thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      // var thisCopy = _db.Copies
      //   .Include(copy => copy.JoinEntities)
      //   .ThenInclude(join => join.Book)
      //   .FirstOrDefault(copy => copy.CopyId == id);
      return View(thisCopy);
    }

    // public ActionResult Complete(int id)
    // {
    //   var thisCopy = _db.Copies.FirstOrDefault(Copy => Copy.CopyId == id);
    //   return View(thisCopy);
    // }

    // [HttpPost, ActionTitle("Complete")]
    // public ActionResult CompleteConfirm(int id, Copy Copy, bool Complete)
    // {
    //   if (Complete != true)
    //   {
    //     var thisCopy = _db.Copies.FirstOrDefault(Copy => Copy.CopyId == id);
    //     thisCopy.Complete = true;
    //     _db.Entry(thisCopy).State = EntityState.Modified;
    //     _db.SaveChanges();
    //   }
    //   return RedirectToAction("Index");
    // }
    public ActionResult Edit(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      
      return View(thisCopy);
    }

    [HttpPost]
    public ActionResult Edit(Copy copy)
    {
      _db.Entry(copy).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(Copy => Copy.CopyId == id);
      return View(thisCopy);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed (int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(Copy => Copy.CopyId == id);
      _db.Copies.Remove(thisCopy);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    // [HttpPost]
    // public ActionResult ReturnCopy(int joinId)
    // {
    //   var joinEntry = _db.Checkouts.FirstOrDefault(entry => entry.CheckoutsId == joinId);
    //   _db.Checkouts.Remove(joinEntry);
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }

    public ActionResult Create(int BookId)
    {
      Console.WriteLine($"BookId: {BookId}");
      
      ViewBag.BookId = BookId;
      return View();
    }

    [HttpPost]
    public ActionResult Create(Copy copy)
    {
      _db.Copies.Add(copy);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Checkout(int copyId)
    {
      ViewBag.CopyId = copyId;
      return View();
    }
    [HttpPost]
    public async Task<ActionResult> Checkout(Copy copy)
    {
      //get current DateTime 
      DateTime currentDateTime = DateTime.Now;

      
      // if(DateTime.Compare(currentDateTime))
      //set time to current DateTime
      Copy thisCopy = _db.Copies.FirstOrDefault(c => c.CopyId == copy.CopyId);
      thisCopy.CheckoutDate = currentDateTime;
      thisCopy.DueDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, (currentDateTime.Minute + 5), currentDateTime.Second, currentDateTime.Millisecond);
      thisCopy.IsCheckedOut = true;
      _db.Entry(thisCopy).State = EntityState.Modified;
      //display due date somewhere
      //use time to make sure 2 patrons can't checkout the same book while it's checked out
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      Patron currentPatron = _db.Patrons.FirstOrDefault(p => p.Name == currentUser.UserName);
      _db.Checkouts.Add(new Checkout() {PatronId = currentPatron.PatronId, CopyId = copy.CopyId});
      _db.SaveChanges();

      return RedirectToAction("Index");
    }
  }
}

/*
 var userId = this.User.FindFirst(ClaimTypes.TitleIdentifier)?.Value;
         var currentUser = await _userManager.FindByIdAsync(userId);
         item.User = currentUser;
         */