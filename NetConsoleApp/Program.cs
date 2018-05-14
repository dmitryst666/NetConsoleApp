using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Web.Script.Serialization;

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
            Listener.Prefixes.Add("http://192.168.1.168:8088/");
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

            if (text.Length > 2)
            {
                Dictionary<string, string> postParams = new Dictionary<string, string>();
                string[] rawParams = text.Split('&');
                foreach (string param in rawParams)
                {
                    string[] kvPair = param.Split('=');
                    string key = kvPair[0];
                    string value = System.Net.WebUtility.UrlDecode(kvPair[1]);
                    postParams.Add(key, value);
                }
            }

           // System.Threading.Thread.Sleep(1000);

            HttpListenerResponse response = context.Response;

            

            //ArrayList tt = new ArrayList();

            //for (int i=0; i < 10; i++ )
            //{

            //    testObject obj = new testObject();
            //    obj.name = "Test Object No. " + i;
            //    obj.account = "141000000" + i;
            //    tt.Add(obj);
            //}


            ArrayList rr = new ArrayList();

            for (int j = 0; j < 10; j++)
            {
                ClientData cld = new ClientData();
                cld.name = String.Format("Client No. {0}", j);
                cld.account = String.Format("141000000{0}", j);
                cld.address = String.Format("Address: 1st street #{0}", j);
                rr.Add(cld);
            }


            

            using (Stream stream = response.OutputStream) {

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(rr) ;

                    response.StatusCode = (int)HttpStatusCode.OK;
                ////Content-Type: text/html; charset=UTF-8
                response.AddHeader(HttpResponseHeader.ContentType.ToString(), "text/html; charset=utf-8");
                string responseString = "<!DOCTYPE html><HTML lang=\"ru\"><HEAD><script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>";
                responseString += "<meta http-equiv=Content-Type content=\"text/html; charset = UTF-8\" /></HEAD><BODY>";
                responseString += "<div id=\"accz\"></div> <div id=\"namez\"></div> <div id=\"addrz\"></div>";
                responseString += "Input form <br />";
                
                responseString += "<form action=\"\\\" method=\"POST\" name=\"test\">";
                responseString += "<input type=\"text\" name=\"textparam\" value=\"\" />";
                responseString += "<input type=\"submit\" value=\"Send\">";
                responseString += "</form>";
             //   responseString += json;
                responseString += "<script>";
                responseString += @"var obj = jQuery.parseJSON( '" + json + "' ); jQuery.each( obj, function( i, val ) {" +
                    "  $(\"#accz\").append(\"<br /> - \" + val[\"account\"]);  $(\"#namez\").append(\"<br /> - \" + val[\"name\"]);   $(\"#addrz\").append(\"<br /> - \" + val[\"address\"]); }); ";
                ///  $("#accz").append("<br /><b>AAAAAA</b>");
                responseString += "</script>";
                responseString +="</BODY></HTML>";
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

        class testObject
        {
            public string name { get; set; }
            public string account { get; set; }
        } 
       

    }

    public class ClientData
    {
        public string name { get; set; }
        public string account { get; set; }
        public string address { get; set; }
    }

}
