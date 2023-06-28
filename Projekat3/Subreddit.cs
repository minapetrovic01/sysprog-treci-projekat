using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Listings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    class Subreddit : IObservable<string>, IObserver<string>
    {
        private readonly IScheduler _scheduler;
        private ISubject<string> _subject;
        private RedditClient _redditClient;
        public string _title;
        private object _lock = new object();
        private bool _postsGeathered=false;
        private ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();
        private ConcurrentBag<string> _comments = new ConcurrentBag<string>();

        public Subreddit(string title,RedditClient client, object locky)
        {
            _subject = new Subject<string>();
            _scheduler = new EventLoopScheduler();
            _title = title;
            _redditClient = client;
            _lock = locky;
        }

        public void OnCompleted()
        {
            lock(_lock)
            {
                Console.WriteLine($"--------------{_title} SUBREDDIT----------------------\n");
            }
            Task.WaitAll(_tasks.ToArray());
            DoTopicModeling();
        }

        private void DoTopicModeling()
        {
            Console.WriteLine($"Topic modeling for {_title} subreddit\n");
            foreach(var comment in _comments)
            {
                Console.WriteLine(comment);
                _subject.OnNext(comment);

            }
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public async void OnNext(string value)
        {
            _postsGeathered = false;
             var t=  GetPosts(value);
            _tasks.Add(t);
            await t;
            if(_postsGeathered)
            {
                //lock(_lock)
                //{
                //    Console.WriteLine($"{_title} posts geathered.\n");
                //}
            }
            else
            {
                lock(_lock)
                {
                    Console.WriteLine($"{_title} posts not geathered.\n");
                }
            }
        }

        private async Task GetPosts(string value)
        {
            try
            {
                await Task.Delay(20);
                Post post = _redditClient.Subreddit(_title).Post(value).About();
                //lock(_lock)
                //{
                //    Console.WriteLine($"Post title: {post.Title}\n");
                //    Console.WriteLine($"Number of comments: {post.Listing.NumComments}");
                //}
                GetComments(post);
                _postsGeathered = true;
                
            }
            catch(Exception e)
            {
                lock(_lock)
                {
                    Console.WriteLine(e.Message+"when getting posts");
                }
                _postsGeathered = false;
            }
        }
        private void GetComments(Post post)
        {
            try
            {
                var comments = post.Comments.GetNew(limit:100);
                foreach(var comment in comments)
                {
                    lock(_lock)
                    {
                        Console.WriteLine($"Comment: {comment.Body}");
                        _comments.Add(comment.Body);
                    }
                }
            }
            catch(Exception e)
            {
                lock(_lock)
                {
                    Console.WriteLine(e.Message+"when getting comments");
                }
            }
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _subject.ObserveOn(_scheduler).Subscribe(observer);
        }
    }
}
