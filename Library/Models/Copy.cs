using System.Collections.Generic;
using System;

namespace Library.Models
{
    public class Copy
    {
        public Copy()
        {
            this.Checkouts = new HashSet<Checkout>();
        }

        public int CopyId { get; set; }
        public int BookId { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCheckedOut {get; set;}
        public string Condition { get; set; }
        public string Format { get; set; }
        public virtual Book Book { get; set;}
        public virtual ICollection<Checkout> Checkouts { get;}
    }
}