using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library.Models
{
  public class Author
  {
    public Author()
    {
        this.JoinEntities = new HashSet<Authorship>();
    }

    public int AuthorId { get; set; }

    [JsonProperty(PropertyName = "author")]
    public string Name { get; set; }
    public virtual ICollection<Authorship> JoinEntities { get; set; }
  }
}