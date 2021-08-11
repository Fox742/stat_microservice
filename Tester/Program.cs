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

                client.BaseAddress = new Uri("http://localhost:5001/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = client.PostAsync("statistics/add", encodedContent).Result;
                Console.WriteLine(response.ToString());

                Console.WriteLine("------------------------------");

                var parameters2 = new Dictionary<string, string> { { "key", "key" }, { "eventJson", "json" }, { "clientDT", new DateTime(1999, 5, 6, 14, 38, 12, 234).ToString() } };
                var encodedContent2 = new FormUrlEncodedContent(parameters2);

                var response2 = client.PostAsync("statistics/add", encodedContent2).Result;
                Console.WriteLine(response2.ToString());

                var parameters3 = new Dictionary<string, string> { { "key", "ключ" }, { "eventJson", "json2" } };
                var encodedContent3 = new FormUrlEncodedContent(parameters3);

                var response3 = client.PostAsync("statistics/add", encodedContent3).Result;
                Console.WriteLine(response3.ToString());
            }
            Console.WriteLine("Bye!");
            Console.ReadLine();
        }
    }
}
