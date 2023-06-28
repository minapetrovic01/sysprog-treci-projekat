using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    class Article : IObservable<string>
    {
        private ISubject<string> _subject;
        private string _accessToken;
        private readonly IScheduler _scheduler;

        public Article(string accessToken)
        {
            _accessToken = accessToken;
            _subject = new Subject<string>();
            _scheduler = new EventLoopScheduler();
        }
        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _subject.ObserveOn(_scheduler).Subscribe(observer);
        }
        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }
        public async Task GetArticles(string subreddit)
        {
            try
            {
                //var reddit = new RedditSharp.Reddit();
                //reddit.LogIn(_accessToken);
                //var sub = reddit.GetSubreddit(subreddit);
                //var post = sub.Posts.First();
                //_subject.OnNext(post.SelfText);
                using(HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _accessToken);
                    client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var response = await client.GetAsync("https://oauth.reddit.com/r/" + subreddit + "/new.json");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseContent);
                    JArray posts = (JArray)json["data"]["children"];

                    foreach (var post in posts)
                    {
                        JObject data = (JObject)post["data"];
                        if (data["name"].ToString() != "")
                        {
                            _subject.OnNext(data["name"].ToString());
                        }
                    }
                }
                _subject.OnCompleted();
            }
            catch (Exception e)
            {
                _subject.OnError(e);
            }
        }
    }
}
