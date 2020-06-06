using System;

namespace ObservableBlazor
{
    internal class LambdaObserver<T> : IObserver<T>
    {
        private readonly Action<T> onNext;

        public LambdaObserver(Action<T> onNext)
        {
            this.onNext = onNext;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            onNext(value);
        }
    }
}