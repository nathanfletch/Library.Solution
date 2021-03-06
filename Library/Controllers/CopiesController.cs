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
    [Authorize (Roles="Librarian")] //, HandleError
    public ActionResult Details(int id)
    {
      Copy thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      
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
    [Authorize (Roles="Librarian")]
    public ActionResult Edit(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      
      return View(thisCopy);
    }
    [Authorize (Roles="Librarian")]
    [HttpPost]
    public ActionResult Edit(Copy copy)
    {
      _db.Entry(copy).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    [Authorize (Roles="Librarian")]
    public ActionResult Delete(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(Copy => Copy.CopyId == id);
      return View(thisCopy);
    }
    [Authorize (Roles="Librarian")]
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

    [Authorize (Roles="Librarian")]
    public ActionResult Create(int BookId)
    {
      Console.WriteLine($"BookId: {BookId}");
      
      ViewBag.BookId = BookId;
      return View();
    }

    [Authorize (Roles="Librarian")]
    [HttpPost]
    public ActionResult Create(Copy copy)
    {
      _db.Copies.Add(copy);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public ActionResult Checkout(int copyId)
    {
      ViewBag.CopyId = copyId;
      return View();
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Checkout(Copy copy)
    {
      DateTime currentDateTime = DateTime.Now;
      DateTime newDueDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, (currentDateTime.Minute + 5), currentDateTime.Second, currentDateTime.Millisecond);
      
      Copy thisCopy = _db.Copies.FirstOrDefault(c => c.CopyId == copy.CopyId);
      if(!thisCopy.IsCheckedOut)
      {
        thisCopy.CheckoutDate = currentDateTime;
        thisCopy.DueDate = newDueDate;
        thisCopy.IsCheckedOut = true;
        _db.Entry(thisCopy).State = EntityState.Modified;
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        Patron currentPatron = _db.Patrons.FirstOrDefault(p => p.Name == currentUser.UserName);
        _db.Checkouts.Add(new Checkout() {PatronId = currentPatron.PatronId, CopyId = copy.CopyId});
        _db.SaveChanges();
        ViewBag.Message = $"You have successfully checked out your book. Your new due date is {thisCopy.DueDate.ToString()}.";
      }
      else
      {
        ViewBag.Message = "Sorry, it's already checked out. Try a different copy.";
      }
      return View();
    }
  }
}
