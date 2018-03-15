using System.Collections;
using RuntimeUnitTestToolkit;
using UniRx;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    public class DontDestroyObjectPoolTest
    {
        IObjectPool<PoolManagedBehaviour> CreatePool(int id, int numberOfInstances, int createCountPerFrame, Transform parent)
        {
            var go = new GameObject();
            var prefab = go.AddComponent<TestBehaviour>();
            var config = new ObjectPoolConfig(numberOfInstances, createCountPerFrame);
            return new DontDestroyObjectPool(id, prefab, config, parent);
        }

        public IEnumerator RentObjectIsDontDestroyOnLoad()
        {
            var parent = new GameObject();
            var pool = CreatePool(id: 1, numberOfInstances: 1, createCountPerFrame: 1, parent: parent.transform);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            TestUtils.IsInTheDontDestroyOnLoad(obj.gameObject).IsTrue();
            pool.Return(obj);
        }

        public IEnumerator RentObjectIsDontDestroyOnLoadWithoutParent()
        {
            var pool = CreatePool(id: 1, numberOfInstances: 1, createCountPerFrame: 1, parent: null);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            TestUtils.IsInTheDontDestroyOnLoad(obj.gameObject).IsTrue();
            pool.Return(obj);
        }

        public IEnumerator ObjectPoolCanShrink()
        {
            var numberOfInstances = 1;
            var pool = CreatePool(id: 1, numberOfInstances: numberOfInstances, createCountPerFrame: 1, parent: null);
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