using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            string newport = "5357";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{newport}/");
            listener.Start();

            Console.WriteLine($"Listening on port http://localhost:{newport}");

            while (true)
            {
                var context = listener.GetContext();

                if (context.Request.QueryString.AllKeys.Count(p => p == "echo") == 1)
                {
                    string echo = context.Request.QueryString["echo"];
                    Console.WriteLine($"Receiving echo request {echo}");
                    using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
                    {
                        writer.Write(echo);
                    }

                    context.Response.Close();
                }
                else
                {
                    Console.WriteLine("=========================");
                    Console.WriteLine($"New Webhook received!");
                    Console.WriteLine($"URL:{context.Request.Url}");
                    Console.WriteLine("=========================");
                    Console.WriteLine("HEADERS:");
                    Console.WriteLine(context.Request.Headers.ToString());
                    Console.WriteLine("=========================");
                    Console.WriteLine("BODY:");

                    using (StreamReader reader = new StreamReader(context.Request.InputStream))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }

                    context.Response.Close();
                    Console.WriteLine("");
                    Console.WriteLine("");
                }
            }
        }
    }
}
