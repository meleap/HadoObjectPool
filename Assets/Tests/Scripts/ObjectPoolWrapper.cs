using System;
using UniRx;

namespace Hado.Utils.ObjectPool
{
    public class ObjectPoolWrapper<T> : IObjectPool<T>
            where T : PoolManagedBehaviour
    {
        readonly IObjectPool<PoolObjectController> objectPool;

        public ObjectPoolWrapper(IObjectPool<PoolObjectController> objectPool)
        {
            this.objectPool = objectPool;
        }

        public ObjectPoolConfig Config { get { return objectPool.Config; } }

        public int Count { get { return objectPool.Count; } }

        public void Clear()
        {
            objectPool.Clear();
        }

        public void Dispose()
        {
            objectPool.Dispose();
        }

        public IObservable<Unit> PreactivateAsync()
        {
            return objectPool.PreactivateAsync();
        }

        public IObservable<Unit> PreloadAsync()
        {
            return objectPool.PreloadAsync();
        }

        public T Rent()
        {
            return objectPool.Rent().Behaviour as T;
        }

        public void Return(T instance)
        {
            objectPool.Return(instance.GetComponent<PoolObjectController>());
        }

        public void Shrink()
        {
            objectPool.Shrink();
        }
    }
}