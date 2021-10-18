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
  // [Authorize]
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      List<Book> sorted = _db.Books.ToList().OrderBy(book => book.Title).ToList();
      return View(sorted);
      //list all books? search? This IS the catalog
      //details: this particular book

      // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      // var currentUser = await _userManager.FindByIdAsync(userId);
      // var userBooks = _db.Books.Where(entry => entry.User.Id == currentUser.Id).ToList();

    }

    public ActionResult Details(int id)
    {
      var thisBook = _db.Books
        .Include(book => book.JoinEntities)
        .ThenInclude(join => join.Author)
        .FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }

    // public ActionResult Complete(int id)
    // {
    //   var thisBook = _db.Books.FirstOrDefault(Book => Book.BookId == id);
    //   return View(thisBook);
    // }

    // [HttpPost, ActionName("Complete")]
    // public ActionResult CompleteConfirm(int id, Book Book, bool Complete)
    // {
    //   if (Complete != true)
    //   {
    //     var thisBook = _db.Books.FirstOrDefault(Book => Book.BookId == id);
    //     thisBook.Complete = true;
    //     _db.Entry(thisBook).State = EntityState.Modified;
    //     _db.SaveChanges();
    //   }
    //   return RedirectToAction("Index");
    // }
    public ActionResult Edit(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View(thisBook);
    }

    [HttpPost]
    public ActionResult Edit(Book book, int AuthorId, string AuthorName)
    {
      if (AuthorId != 0)
      {
        _db.Authorship.Add(new Authorship() {AuthorId = AuthorId, BookId = book.BookId});
      }
       if (AuthorName != null)
      {
        Author newAuthor = new Author() { Name = AuthorName };
        _db.Authors.Add(newAuthor);
        _db.SaveChanges();
        _db.Authorship.Add(new Authorship() { AuthorId = newAuthor.AuthorId, BookId = book.BookId});
      }
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed (int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      _db.Books.Remove(thisBook);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteAuthor(int joinId)
    {
      var joinEntry = _db.Authorship.FirstOrDefault(entry => entry.AuthorshipId == joinId);
      _db.Authorship.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Create()
    {
      /*
      non-logged in user - view books & copies
      library user login - view books & copies, checking out copies
      librarian admin login - create, CRUD

      @Html.DropDownList("ID", (SelectList)ViewBag.MyList, "Please select", new { @class = "form-control" })
      */
        // List<Author> authorList = _db.Authors.ToList();
        // authorList.Add(new SelectListItem { Text = "", Value = null});

        SelectList authorList = new SelectList(_db.Authors, "AuthorId", "Name");
        ViewBag.AuthorId = authorList;
        return View();
    }

    [HttpPost]
    public ActionResult Create(Book book, int AuthorId, string AuthorName)
    {
      _db.Books.Add(book);
      _db.SaveChanges();
      if (AuthorId != 0)
      {
        _db.Authorship.Add(new Authorship() { AuthorId = AuthorId, BookId = book.BookId});
      }
      if (AuthorName != null)
      {
        Author newAuthor = new Author() { Name = AuthorName };
        _db.Authors.Add(newAuthor);
        _db.SaveChanges();
        _db.Authorship.Add(new Authorship() { AuthorId = newAuthor.AuthorId, BookId = book.BookId});
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}

/*
 var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
         var currentUser = await _userManager.FindByIdAsync(userId);
         item.User = currentUser;
         */