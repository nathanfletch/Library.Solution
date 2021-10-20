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
  [Authorize (Roles="Librarian")]
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      //put in separate route, save to database, return this route to just showing what's in the database
      var allBooks = Book.GetBooks(EnvironmentVariables.ApiKey);
      return View(allBooks);

      // List<Book> sorted = _db.Books.ToList().OrderBy(book => book.Title).ToList();
      // return View(sorted);
    }

    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      var thisBook = _db.Books
        .Include(book => book.Copies)
        .Include(book => book.JoinEntities)
        .ThenInclude(join => join.Author)
        .FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }

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