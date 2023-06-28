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
        private string _clientId = "";
        private string _clientSecret = "";
        private string _accessToken = "";
        private RedditAuth _redditAuth;

        public WebServer(string urlServer, string urlApi)
        {
            _urlServer = urlServer;
            _urlApi = urlApi;
            _redditAuth = new RedditAuth(_urlApi, _urlServer);
        }
        public async Task Run()
        {
            Console.WriteLine("WebServer started.");
            _accessToken= await _redditAuth.GetAccessToken();
            if(_accessToken == null)
                throw new Exception("Access token is null!");
            else
                Console.WriteLine("Access token here!");
            //Console.WriteLine("Access token: " + _accessToken);

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
                        _= ProcessRequestAsync(context,_requestCount++);
                    }
                }
            });
            server.Start();
            server.Join();
        }
        private async Task ProcessRequestAsync(HttpListenerContext con,int requestNumber)
        {
            try
            {
                HttpListenerContext context = (HttpListenerContext)con;
                if (context == null)
                    throw new Exception("Can't parse given object to HttpListenerContext object!");
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                logRequest(request, requestNumber);
                string validation = ValidateRequest(context);
                if (!validation.Equals("OK"))
                {
                    WriteToConsole(validation);
                    SendResponse(response, requestNumber, validation);
                    return;
                }





                //SendResponse(response, requestNumber, "OK");

            }
            catch(Exception e)
            {
                WriteToConsole(e.Message);
            }
           
        }

        private async Task SendResponse(HttpListenerResponse response, int responseId, string responseString)
        {
            try
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                if(responseString.Equals("OK"))
                {
                    response.StatusCode = 200;
                    response.StatusDescription = "OK";
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusDescription = "Bad request";
                }
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();
                logResponse(response, responseId);
            }
            catch(Exception e)
            {
                WriteToConsole(e.Message);
            }
        }
        private string ValidateRequest(HttpListenerContext context)
        {
            if (!context.Request.HttpMethod.Equals("GET"))
                return "METHOD IS NOT GET";
            return "OK";
        }
        private void logRequest(HttpListenerRequest request, int requestId)
        {
            lock(_lockConsole)
            {
                Console.WriteLine("Request number: " + requestId);
                Console.WriteLine("Request URL: " + request.Url.ToString());
                Console.WriteLine("Request HTTP method: " + request.HttpMethod);
                Console.WriteLine("Request User-agent: " + request.UserAgent);
                Console.WriteLine("Request Content-type: " + request.ContentType);
                Console.WriteLine("Request Content-length: " + request.ContentLength64);
                Console.WriteLine("Request Accept-encoding: " + request.Headers["Accept-encoding"]);
                Console.WriteLine("Request Accept-language: " + request.Headers["Accept-language"]);
                Console.WriteLine("----------------------------------------------------");
            }
        }
        private void logResponse(HttpListenerResponse response,int responseId)
        {
            lock (_lockConsole)
            {
                Console.WriteLine("Response number: " + responseId);
                Console.WriteLine("Response status code: " + response.StatusCode);
                Console.WriteLine("Response status description: " + response.StatusDescription);
                Console.WriteLine("Response Content-type: " + response.ContentType);
                Console.WriteLine("Response Content-length: " + response.ContentLength64);
                Console.WriteLine("Response Content-encoding: " + response.ContentEncoding);
                Console.WriteLine("----------------------------------------------------");
            }
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
