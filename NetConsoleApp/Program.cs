using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
///    http://brain-sharper.blogspot.com/2012/03/httplistener.html
/// </summary>

namespace NetConsoleApp
{
    class Program
    {

        private static readonly HttpListener Listener = new HttpListener();

        public static void Main()
        {
            Listener.Prefixes.Add("http://localhost:8088/");
            Listener.Start();
            Listen();
            Console.WriteLine("Listening...");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async void Listen()
        {
            while (true)
            {
                var context = await Listener.GetContextAsync();
                Console.WriteLine("Client connected");
                Task.Factory.StartNew(() => ProcessRequest(context));
            }

            Listener.Close();
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            Console.WriteLine("Request: {0}, url: {1}", request.HttpMethod, request.Url.AbsolutePath);
            System.Threading.Thread.Sleep(1000);
            HttpListenerResponse response = context.Response;
            
            using (Stream stream = response.OutputStream) {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    //System.IO.Stream output = response.OutputStream;
                    stream.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    stream.Close();
            }
            Console.WriteLine("Response");
        }

    }

}
