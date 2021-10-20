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
  public class AuthorsController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorsController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      IEnumerable<Author> sorted = _db.Authors.OrderBy(author => author.Name);
      return View(sorted);
    }

    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      var thisAuthor = _db.Authors
        .Include(Author => Author.JoinEntities)
        .ThenInclude(join => join.Book)
        .FirstOrDefault(author => author.AuthorId == id);
      return View(thisAuthor);
    }

    public ActionResult Edit(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(author => author.AuthorId == id);
      ViewBag.BookId = new SelectList(_db.Books, "BookId", "Title");
      return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult Edit(Author author, int BookId, string BookTitle)
    {
      if (BookId != 0)
      {
        _db.Authorship.Add(new Authorship() {BookId = BookId, AuthorId = author.AuthorId});
      }
      if (BookTitle != null)
      {
        Book newBook = new Book() { Title = BookTitle };
        _db.Books.Add(newBook);
        _db.SaveChanges();
        _db.Authorship.Add(new Authorship() { BookId = newBook.BookId, AuthorId = author.AuthorId});
      }
      _db.Entry(author).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(author => author.AuthorId == id);
      return View(thisAuthor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed (int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(author => author.AuthorId == id);
      _db.Authors.Remove(thisAuthor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteBook(int joinId)
    {
      var joinEntry = _db.Authorship.FirstOrDefault(entry => entry.AuthorshipId == joinId);
      _db.Authorship.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Create()
    {
      ViewBag.BookId = new SelectList(_db.Books, "BookId", "Title");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Author author, int BookId, string BookTitle)
    {
      _db.Authors.Add(author);
      _db.SaveChanges();
      if (BookId != 0)
      {
        _db.Authorship.Add(new Authorship() { BookId = BookId, AuthorId = author.AuthorId});
      }
      if (BookTitle != null)
      {
        Book newBook = new Book() { Title = BookTitle };
        _db.Books.Add(newBook);
        _db.SaveChanges();
        _db.Authorship.Add(new Authorship() { BookId = newBook.BookId, AuthorId = author.AuthorId});
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}
