using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library.Models
{
    public class Book
    {
        public Book()
        {
            this.JoinEntities = new HashSet<Authorship>();
            this.Copies = new HashSet<Copy>();
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Authorship> JoinEntities { get;}
        public virtual ICollection<Copy> Copies { get;}

        public static List<Book> GetBooks(string apiKey)
        {
          var apiCallTask = ApiHelper.ApiCall(apiKey);
          var result = apiCallTask.Result;
          //take json, make it into "c# json"
          JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(result);
          //converting c# json to actual book objects
          List<Book> bookList = JsonConvert.DeserializeObject<List<Book>>(jsonResponse["results"]["books"].ToString());

          return bookList;
        }
    }
}