using System;
using System.Reactive;
using System.Threading.Tasks;

namespace WikiCrawler
{
    public interface ICrawler : IDisposable
    {
        void Subscribe(Action<string> onNext, Action onComplete);
        Task StartCrawling(string baseUri, string startUri);
    }
}