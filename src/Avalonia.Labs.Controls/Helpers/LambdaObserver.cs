using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Labs.Controls.Helpers
{
    internal class LambdaObserver<T> : IObserver<T>
    {
        private readonly Action<T> _listener;
        public LambdaObserver(Action<T> listener) => _listener = listener;
        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(T value) => _listener(value);
    }
}
