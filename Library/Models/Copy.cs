using System.Collections.Generic;
using System;

namespace Library.Models
{
    public class Copy
    {
        public Copy()
        {
            // this.JoinEntities = new HashSet<Checkouts>();
        }

        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string CheckoutDate { get; set; }
        public string DueDate { get; set; }
        public string Condition { get; set; }
        public string Format { get; set; }
        public virtual Book Book { get; set;}
        // public virtual ICollection<Checkouts> JoinEntities { get;}
    }
}