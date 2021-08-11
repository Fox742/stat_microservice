using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string> { { "key", "concrete_key" }, { "eventJson", "concrete_json" } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                client.BaseAddress = new Uri("https://localhost:44364/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = client.PostAsync("statistics/add", encodedContent).Result;
                Console.WriteLine(response.ToString());
            }
            Console.WriteLine("Bye!");
            Console.ReadLine();
        }
    }
}
