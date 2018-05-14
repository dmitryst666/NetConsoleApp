using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;

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
                await Task.Factory.StartNew(() => ProcessRequest(context));
            }

            Listener.Close();
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            Console.WriteLine("Thread no {0}", Environment.CurrentManagedThreadId);
            HttpListenerRequest request = context.Request;
            //  string path = System.Net.WebUtility.UrlDecode(request.RawUrl);
            string text;
            using (var reader = new StreamReader(request.InputStream,
                                                 request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }
           // Console.WriteLine("Request: {0}", text);
            //using System.Web and Add a Reference to System.Web
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            string[] rawParams = text.Split('&');
            foreach (string param in rawParams)
            {
                string[] kvPair = param.Split('=');
                string key = kvPair[0];
                string value = System.Net.WebUtility.UrlDecode(kvPair[1]);
                postParams.Add(key, value);
            }

            //Usage
          //  Console.WriteLine("Hello " + postParams["textparam"]);


            System.Threading.Thread.Sleep(1000);
            HttpListenerResponse response = context.Response;
            
            using (Stream stream = response.OutputStream) {
                    response.StatusCode = (int)HttpStatusCode.OK;
                ////Content-Type: text/html; charset=UTF-8
                response.AddHeader(HttpResponseHeader.ContentType.ToString(), "text/html; charset=utf-8");
                string responseString = "<!DOCTYPE html><HTML lang=\"ru\"><HEAD><meta http-equiv=Content-Type content=\"text/html; charset = UTF-8\" /></HEAD><BODY>";
                responseString += "Test <br />";
                
                responseString += "<form action=\"\\\" method=\"POST\" name=\"test\">";
                responseString += "<input type=\"text\" name=\"textparam\" value=\"last entered: " + postParams["textparam"] + "\" />";
                responseString += "<input type=\"submit\" value=\"Send\">";
                responseString += "</form></BODY></HTML>";
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
