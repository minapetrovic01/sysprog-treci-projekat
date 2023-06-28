using Reddit;
using System;
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

        public Subreddit(string title,RedditClient client)
        {
            _subject = new Subject<string>();
            _scheduler = new EventLoopScheduler();
            _title = title;
            _redditClient = client;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _subject.ObserveOn(_scheduler).Subscribe(observer);
        }
    }
}
