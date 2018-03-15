using UnityEngine;
using RuntimeUnitTestToolkit;
using System.Collections;
using UniRx;

namespace Hado.Utils.ObjectPool
{
    public class ObjectPoolImplTest
    {
        IObjectPool<Transform> CreatePool(int numberOfInstances, int createCountPerFrame)
        {
            var go = new GameObject();
            var config = new ObjectPoolConfig(numberOfInstances, createCountPerFrame);
            return new ObjectPoolImpl<Transform>(go.transform, config);
        }

        public IEnumerator ClearedPoolCanReuse()
        {
            var pool = CreatePool(numberOfInstances: 1, createCountPerFrame: 1);
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
            var pool = CreatePool(numberOfInstances: 0, createCountPerFrame: 1);
            pool.Count.Is(0);
            var obj = pool.Rent();
            obj.IsNotNull();
            pool.Return(obj);
        }

        public IEnumerator ObjectCountShrinksToTheNumberOfInstances()
        {
            var numberOfInstances = 3;
            var pool = CreatePool(numberOfInstances, numberOfInstances);

            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();

            // create twice object
            var array = new Transform[numberOfInstances * 2];
            for (int i = 0; i < numberOfInstances * 2; i++)
                array[i] = pool.Rent();

            foreach (var t in array)
                pool.Return(t);

            pool.Count.Is(numberOfInstances * 2);
            pool.Shrink();
            pool.Count.Is(numberOfInstances);
        }
    }
}