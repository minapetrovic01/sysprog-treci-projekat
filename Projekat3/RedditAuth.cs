using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
     class RedditAuth
    {
        private string _urlAPI;
        public static string _clientId= "XtcZ6zRp1EJNN4HDHsTjHQ";
        private string _clientSecret= "Gx5fy3spXiNS2W_pCCe2h_LuFBok5w";
        private string _redirectUri;

        private string _accessToken;
        private string _tokenType;
        private string _scope;

        public RedditAuth(string urlAPI, string redirectUri)
        {
            _urlAPI = urlAPI;
            _redirectUri = redirectUri;
        }
        public async Task<string> GetAccessToken()
        {
            using(HttpClient client=new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    {"scope","read identity submit"}
                };
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}")));

                client.DefaultRequestHeaders.Add("User-Agent", "Projekat 3 Reddit API");
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(_urlAPI + "access_token", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);
                _accessToken = json["access_token"].ToString();
               // _refreshToken = json["refresh_token"].ToString();
                _tokenType = json["token_type"].ToString();
                _scope = json["scope"].ToString();
            }
            return _accessToken;
        }


    }
}
