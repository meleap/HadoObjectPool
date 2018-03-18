using System;
using System.Collections;
using Hado.Utils.ObjectPool.Callbacks;
using UniRx;
using UniRx.Toolkit;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    public class DontDestroyObjectPool : ObjectPool<PoolManagedBehaviour>, IObjectPool<PoolManagedBehaviour>
    {
        public ObjectPoolConfig Config { get; private set; }

        readonly int id;
        readonly PoolManagedBehaviour prefab;
        readonly Transform hierarchyParent;
        readonly IObjectPoolCallback<PoolManagedBehaviour> callback;
        readonly bool hasParent;
        bool isLoaded;

        public DontDestroyObjectPool(int id, PoolManagedBehaviour prefab, ObjectPoolConfig config, Transform hierarchyParent = null, IObjectPoolCallback<PoolManagedBehaviour> callback = null)
        {
            this.id = id;
            this.prefab = prefab;
            this.Config = config;
            this.hierarchyParent = hierarchyParent;
            this.callback = callback ?? new NopObjectPoolCallBack<PoolManagedBehaviour>();
            hasParent = hierarchyParent != null;

            prefab.gameObject.SetActive(false); // avoid to call OnAwake OnEnable and OnDisable when CreateInstance
            UnityEngine.Object.DontDestroyOnLoad(hierarchyParent);
        }

        protected override PoolManagedBehaviour CreateInstance()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (isLoaded && Count <= 0)
                Debug.LogWarning(string.Format("create an instance bacause the pool is empty. prefabId: {0}", id));
#endif
            var instance = UnityEngine.Object.Instantiate(prefab, hierarchyParent);
            var c = instance.gameObject.AddComponent<PoolObjectController>();
            c.OnCreateInstance(id);
            callback.OnCreateInstance(instance);
            return instance;
        }

        protected override void OnBeforeRent(PoolManagedBehaviour instance)
        {
            base.OnBeforeRent(instance);
            instance.GetComponent<PoolObjectController>().OnRent();
            callback.OnBeforeRent(instance);
        }

        protected override void OnBeforeReturn(PoolManagedBehaviour instance)
        {
            var c = instance.GetComponent<PoolObjectController>();
            if (c.Id != id)
                throw new InvalidOperationException(string.Format("Id {0} is not equal to {1}", c.Id, id));

            callback.OnBeforeReturn(instance);
            c.OnReturn();

            instance.gameObject.transform.SetParent(hierarchyParent);
            if (!hasParent)
                UnityEngine.Object.DontDestroyOnLoad(instance);

            base.OnBeforeReturn(instance);
        }

        protected override void OnClear(PoolManagedBehaviour instance)
        {
            var c = instance.GetComponent<PoolObjectController>();
            c.OnBeforeDestroy();
            base.OnClear(instance);
            callback.OnClear(instance);
        }

        public IObservable<Unit> PreloadAsync()
        {
            return PreloadAsync(Config.NumberOfInstances, Config.CreateCountPerFrame).ForEachAsync(_ => isLoaded = true);
        }

        public IObservable<Unit> PreactivateAsync()
        {
            if (!Config.NeedPreactivation)
                return Observable.ReturnUnit();

            return Observable.FromCoroutine(PreactivateAsyncCoroutine);
        }

        IEnumerator PreactivateAsyncCoroutine()
        {
            var count = Count;
            var array = new PoolManagedBehaviour[count];
            for (var i = 0; i < count; i++)
                array[i] = Rent();

            yield return null;  // wait 1 frame to call MonoBehaviour.(Awake | OnEnable | Start)
            for (var i = 0; i < count; i++)
                Return(array[i]);
        }

        public void Shrink()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var shrinkNum = Config.NumberOfInstances - Count;
            if (shrinkNum > 0)
                Debug.Log(string.Format("ObjectPool id-{0}: Shrink {1} instances", id, shrinkNum));
#endif
            this.Shrink(0, Config.NumberOfInstances);
        }

        public void Clear()
        {
            Clear(callOnBeforeRent: false);
            isLoaded = false;
        }
    }
}