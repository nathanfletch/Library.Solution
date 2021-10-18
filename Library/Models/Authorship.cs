namespace Library.Models
{
  public class Authorship
  {
    public int AuthorshipId {get; set;}
    public int BookId {get; set;}
    public int AuthorId {get; set;}
    public virtual Book Book {get; set;}
    public virtual Author Author {get; set;}

  }
}