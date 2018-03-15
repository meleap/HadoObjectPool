using System;
using UniRx;

namespace Hado.Utils.ObjectPool
{
    public interface IObjectPool<T> : IDisposable
        where T : UnityEngine.Component
    {
        ObjectPoolConfig Config { get; }

        int Count { get; }

        T Rent();

        void Return(T instance);

        void Clear();

        void Shrink();

        IObservable<Unit> PreloadAsync();

        IObservable<Unit> PreactivateAsync();
    }
}