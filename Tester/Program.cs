using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44364/");
                var response = client.GetAsync(string.Format("statistics/add?key={0}&eventJson={1}&clientDT=","concrete_key","concrete_json")).Result;
                Console.WriteLine(response.ToString());
            }
            Console.WriteLine("Bye!");
            Console.ReadLine();
        }
    }
}
