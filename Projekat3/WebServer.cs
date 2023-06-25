using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
     class WebServer
    {
        private string _urlServer;
        private string _urlApi;
        private object _lockConsole = new object();
        private int _requestCount = 0;
        public WebServer(string urlServer, string urlApi)
        {
            _urlServer = urlServer;
            _urlApi = urlApi;
        }
        public async Task Run()
        {
            Console.WriteLine("WebServer started.");
            Thread server = new Thread(async ()=>
            {
                using (HttpListener listener = new HttpListener())
                {
                    string urlListener = _urlServer;
                    listener.Prefixes.Add(urlListener);
                    listener.Start();

                    Console.WriteLine("Server is listening on:" + urlListener);
                    while (listener.IsListening)
                    {
                        HttpListenerContext context = await listener.GetContextAsync();
                        _= ProcessRequestAsync(context);
                        //string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        //response.ContentLength64 = buffer.Length;
                        //System.IO.Stream output = response.OutputStream;
                        //output.Write(buffer, 0, buffer.Length);
                        //output.Close();
                    }
                }
            });
            server.Start();
            server.Join();
        }
        private async Task ProcessRequestAsync(HttpListenerContext con)
        {
            try
            {
                HttpListenerContext context = (HttpListenerContext)con;
                if (context == null)
                    throw new Exception("Can't parse given object to HttpListenerContext object!");
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
            }
            catch(Exception e)
            {
                WriteToConsole(e.Message);
                WriteBreakLine();
            }
           
        }
        private void logRequest(HttpListenerRequest request)
        {
            _requestCount++;
            WriteToConsole("Request number: " + _requestCount);
            WriteToConsole("Request URL: " + request.Url.ToString());
            WriteToConsole("Request HTTP method: " + request.HttpMethod);
            WriteToConsole("Request IP-adress: " + request.RemoteEndPoint.Address.ToString());
            WriteToConsole("Request User-agent: " + request.UserAgent);
            WriteToConsole("Request Content-type: " + request.ContentType);
            WriteToConsole("Request Content-length: " + request.ContentLength64);
            WriteToConsole("Request Accept-encoding: " + request.Headers["Accept-encoding"]);
            WriteToConsole("Request Accept-language: " + request.Headers["Accept-language"]);
            WriteToConsole("Request Accept: " + request.Headers["Accept"]);
            WriteToConsole("Request Host: " + request.Headers["Host"]);
            WriteToConsole("Request Connection: " + request.Headers["Connection"]);
            WriteToConsole("Request Cache-control: " + request.Headers["Cache-control"]);
            WriteToConsole("Request Cookie: " + request.Headers["Cookie"]);
            WriteToConsole("Request Referer: " + request.Headers["Referer"]);
            WriteToConsole("Request Accept-charset: " + request.Headers["Accept-charset"]);
            WriteToConsole("Request Accept-encoding: " + request.Headers["Accept-encoding"]);
            WriteBreakLine();

        }
        private void WriteToConsole(string message)
        {
            lock (_lockConsole)
            {
                Console.WriteLine(message);
            }
        }
        private void WriteBreakLine()
        {
            lock (_lockConsole)
            {
                Console.WriteLine("----------------------------------------------------");

            }
        }
    }
}
