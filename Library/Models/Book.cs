using System.Collections.Generic;
using System;

namespace Library.Models
{
    public class Book
    {
        public Book()
        {
            this.JoinEntities = new HashSet<Authorship>();
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Authorship> JoinEntities { get;}
    }
}