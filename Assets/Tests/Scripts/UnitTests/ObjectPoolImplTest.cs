using System.Collections;
using RuntimeUnitTestToolkit;
using UniRx;
using UnityEngine;

namespace Hado.Utils.ObjectPool.UnitTests
{
    public class ObjectPoolImplTest : ObjectPoolTestBase
    {
        IObjectPool<PoolManagedBehaviour> CreatePool(int numberOfInstances)
        {
            var go = new GameObject();
            PoolManagedBehaviour prefab = go.AddComponent<TestBehaviour>();
            var config = new ObjectPoolConfig(numberOfInstances, Mathf.Max(numberOfInstances, 1));
            return base.CreatePool(prefab, config);
        }

        public IEnumerator ClearedPoolCanReuse()
        {
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            obj.IsNotNull();
            pool.Return(obj);
            pool.Clear();

            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            obj = pool.Rent();
            obj.IsNotNull();
            pool.Return(obj);
            pool.Clear();
        }

        public void EmptyPoolCanRentObject()
        {
            var pool = CreatePool(numberOfInstances: 0);
            pool.Count.Is(0);
            var obj = pool.Rent();
            obj.IsNotNull();
            pool.Return(obj);
        }

        public IEnumerator ObjectCountShrinksToTheNumberOfInstances()
        {
            var numberOfInstances = 3;
            var pool = CreatePool(numberOfInstances);

            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();

            // create twice object
            var array = new PoolManagedBehaviour[numberOfInstances * 2];
            for (int i = 0; i < numberOfInstances * 2; i++)
                array[i] = pool.Rent();

            foreach (var t in array)
                pool.Return(t);

            pool.Count.Is(numberOfInstances * 2);
            pool.Shrink();
            pool.Count.Is(numberOfInstances);
        }

        public IEnumerator RentObjectIsDontDestroyOnLoad()
        {
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            TestUtils.IsInTheDontDestroyOnLoad(obj.gameObject).IsTrue();
            pool.Return(obj);
        }

        public IEnumerator RentObjectIsDontDestroyOnLoadWithoutParent()
        {
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            TestUtils.IsInTheDontDestroyOnLoad(obj.gameObject).IsTrue();
            pool.Return(obj);
        }

        public IEnumerator ObjectPoolCanShrink()
        {
            var numberOfInstances = 1;
            var pool = CreatePool(numberOfInstances: numberOfInstances);
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

        public IEnumerator ObjectPoolForceDestroy()
        {
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            pool.Rent().GetComponent<PoolObjectController>().ForceDestroy();

        }
    }
}