using System.Collections;
using RuntimeUnitTestToolkit;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace Hado.Utils.ObjectPool
{
    public class DontDestroyObjectPoolTest
    {
        readonly Transform hierarchyParent;

        public DontDestroyObjectPoolTest()
        {
            hierarchyParent = new GameObject("parent").transform;
            hierarchyParent.OnDestroyAsObservable()
                  .SelectMany(_ => hierarchyParent.GetComponentsInChildren<PoolObjectController>(true))
                  .Subscribe(c => c.ForceDestroy());
        }

        IObjectPool<PoolManagedBehaviour> CreatePool(int id, int numberOfInstances)
        {
            var go = new GameObject();
            var prefab = go.AddComponent<TestBehaviour>();
            var config = new ObjectPoolConfig(numberOfInstances, numberOfInstances);
            return new DontDestroyObjectPool(id, prefab, config, hierarchyParent);
        }

        public IEnumerator RentObjectIsDontDestroyOnLoad()
        {
            var pool = CreatePool(id: 1, numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            TestUtils.IsInTheDontDestroyOnLoad(obj.gameObject).IsTrue();
            pool.Return(obj);
        }

        public IEnumerator RentObjectIsDontDestroyOnLoadWithoutParent()
        {
            var pool = CreatePool(id: 1, numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            TestUtils.IsInTheDontDestroyOnLoad(obj.gameObject).IsTrue();
            pool.Return(obj);
        }

        public IEnumerator ObjectPoolCanShrink()
        {
            var numberOfInstances = 1;
            var pool = CreatePool(id: 1, numberOfInstances: numberOfInstances);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();

            // create twice objects
            var array = new PoolManagedBehaviour[numberOfInstances * 2];
            for (var i = 0; i < array.Length; i++)
                array[i] = pool.Rent();
            foreach (var o in array)
                pool.Return(o);

            pool.Shrink();
        }
    }
}