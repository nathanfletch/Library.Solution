using System.Threading.Tasks;
using RestSharp;

namespace Library.Models
{
  class ApiHelper
  {
    public static async Task<string> ApiCall(string apiKey)
    {
      //https://api.nytimes.com/svc/books/v3   /lists/current/hardcover-fiction.json?api-key=
      RestClient client = new RestClient("https://api.nytimes.com/svc/books/v3");
      RestRequest request = new RestRequest($"lists/current/hardcover-fiction.json?api-key={apiKey}", Method.GET);
      var response = await client.ExecuteTaskAsync(request);
      return response.Content;
    }
  }
}