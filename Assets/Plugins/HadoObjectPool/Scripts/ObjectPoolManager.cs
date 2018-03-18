using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hado.Utils.Rx;
using UniRx;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    public class ObjectPoolManager
    {
        public static readonly ObjectPoolManager Instance = new ObjectPoolManager();

        ObjectPoolManager() { }

        Dictionary<int, IObjectPool<PoolObjectController>> poolDictionary = new Dictionary<int, IObjectPool<PoolObjectController>>();

        // require PoolObjectController
        public void RegisterPool(int id, ObjectPoolImpl objectPool)
        {
            poolDictionary.Add(id, objectPool);
        }

        public IObservable<Unit> PreloadAsync(IProgress<float> progress = null)
        {
            return Observable.FromCoroutine(() => PreloadCoroutine(progress ?? ProgressUtil.Nop));
        }

        public IObservable<Unit> PreloadAsyncParallel()
        {
            var loadings = poolDictionary.Values.Select(p => p.PreloadAsync());
            return Observable.WhenAll(loadings);
        }

        IEnumerator PreloadCoroutine(IProgress<float> progress)
        {
            if (poolDictionary.Count <= 0)
            {
                progress.Report(1f);
                yield break;
            }

            var totalCount = poolDictionary.Values.Sum(p => p.Config.NumberOfInstances);
            var currentCount = 0;
            foreach (var p in poolDictionary.Values)
            {
                var preload = p.PreloadAsync().ToYieldInstruction();
                while (!preload.IsDone)
                {
                    progress.Report((float)(currentCount + p.Count) / totalCount);
                    yield return null;
                }
                if (preload.HasError)
                    throw preload.Error;
                currentCount += p.Count;
                progress.Report((float)currentCount / totalCount);
            }
        }

        /// <summary>
        /// Side effect: Set the volume to 0 for 2 frames.
        /// </summary>
        public IObservable<Unit> PreactivateAsync()
        {
            var volume = AudioListener.volume;
            AudioListener.volume = 0;
            var preactivates = poolDictionary.Values.Select(p => p.PreactivateAsync());
            return Observable.WhenAll(preactivates)
                             .ForEachAsync(_ => AudioListener.volume = volume);
        }

        public PoolManagedBehaviour Rent(int id)
        {
            return poolDictionary[id].Rent().Behaviour;
        }

        public void Return(PoolManagedBehaviour behaviour)
        {
            Return(behaviour.GetComponent<PoolObjectController>());
        }

        public void Return(PoolObjectController controller)
        {
            foreach (var c in ObjectPoolUtils.ResolveNestedPoolObjects(controller))
                poolDictionary[c.Id].Return(c);
        }

        public void AllReturn()
        {
            foreach (var c in ObjectPoolUtils.FindAllRentingPoolObjects())
                Return(c);
        }

        public void Shrink()
        {
            foreach (var p in poolDictionary.Values)
                p.Shrink();
        }

        public void Clear()
        {
            foreach (var p in poolDictionary.Values)
                p.Dispose();
            poolDictionary.Clear();
        }
    }
}