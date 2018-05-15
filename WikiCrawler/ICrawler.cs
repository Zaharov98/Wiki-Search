using System;
using System.Reactive;
using System.Threading.Tasks;

namespace WikiCrawler.HttpCrawler
{
    public interface ICrawler : IDisposable
    {
        void Subscribe(Action<string> onNext, Action onComplete);
        Task StartCrawling(string startUri);
    }
}