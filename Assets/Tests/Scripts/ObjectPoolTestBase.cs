using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    public abstract class ObjectPoolTestBase
    {
        Transform hierarchyParent;

        public ObjectPoolTestBase()
        {
            hierarchyParent = new GameObject("parent").transform;
            hierarchyParent.OnDestroyAsObservable()
                           .SelectMany(_ => hierarchyParent.GetComponentsInChildren<PoolObjectController>(true))
                           .Subscribe(c => c.ForceDestroy());
        }

        protected IObjectPool<T> CreatePool<T>(T prefab, ObjectPoolConfig config) where T : PoolManagedBehaviour
        {
            var objectPool = new ObjectPoolImpl(IdGenerator.Generate(), prefab, config, hierarchyParent);
            return new ObjectPoolWrapper<T>(objectPool);
        }
    }
}