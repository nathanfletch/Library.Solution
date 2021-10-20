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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
      var jsonResponse = Book.GetBooks(EnvironmentVariables.ApiKey);
      //converting c# json to actual book objects
      List<Book> bookList = JsonConvert.DeserializeObject<List<Book>>(jsonResponse["results"]["books"].ToString());
      List<Author> authorList = JsonConvert.DeserializeObject<List<Author>>(jsonResponse["results"]["books"].ToString());
      for(int i = 0; i < authorList.Count; i++)
      { 
        _db.Books.Add(bookList[i]);
        _db.Authors.Add(authorList[i]);
      }
      _db.SaveChanges();
      for(int i = 0; i < authorList.Count; i++)
      { 
        Book bookAtCurrentIndex = _db.Books.FirstOrDefault(book => book.Title == bookList[i].Title);
        Author authorAtCurrentIndex = _db.Authors.FirstOrDefault(author => author.Name == authorList[i].Name);
        // get ids, make a new authorship object
        _db.Authorship.Add( new Authorship() { BookId = bookAtCurrentIndex.BookId, AuthorId = authorAtCurrentIndex.AuthorId });
      }
      _db.SaveChanges();
      //we also want to make author objects and authorship objects and store them in the _db
      List<Book> sorted = _db.Books.ToList().OrderBy(book => book.Title).ToList();
      return View(sorted);

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