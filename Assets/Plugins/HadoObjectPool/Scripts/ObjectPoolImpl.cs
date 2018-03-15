using System.Collections;
using Hado.Utils.ObjectPool.Callbacks;
using UniRx;
using UniRx.Toolkit;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    public class ObjectPoolImpl<T> : ObjectPool<T>, IObjectPool<T>
        where T : UnityEngine.Component
    {
        public ObjectPoolConfig Config { get; private set; }

        readonly T prefab;
        readonly Transform hierarchyParent;
        readonly IObjectPoolCallback<T> callback;

        public ObjectPoolImpl(T prefab, ObjectPoolConfig config, Transform hierarchyParent = null, IObjectPoolCallback<T> callback = null)
        {
            this.prefab = prefab;
            this.Config = config;
            this.hierarchyParent = hierarchyParent;
            this.callback = callback ?? new NopObjectPoolCallBack<T>();
            prefab.gameObject.SetActive(false); // avoid to call OnAwake OnEnable and OnDisable when CreateInstance
        }

        protected override T CreateInstance()
        {
            var instance = UnityEngine.Object.Instantiate(prefab, hierarchyParent);
            callback.OnCreateInstance(instance);
            return instance;
        }

        protected override void OnBeforeRent(T instance)
        {
            base.OnBeforeRent(instance);
            callback.OnBeforeRent(instance);
        }

        protected override void OnBeforeReturn(T instance)
        {
            instance.gameObject.transform.SetParent(hierarchyParent);
            callback.OnBeforeReturn(instance);
            base.OnBeforeReturn(instance);
        }

        protected override void OnClear(T instance)
        {
            base.OnClear(instance);
            callback.OnClear(instance);
        }

        public IObservable<Unit> PreloadAsync()
        {
            return PreloadAsync(Config.NumberOfInstances, Config.CreateCountPerFrame);
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
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = Rent();

            yield return null;  // wait 1 frame to call MonoBehaviour.(Awake | OnEnable | Start)
            for (var i = 0; i < count; i++)
                Return(array[i]);
        }

        public void Shrink()
        {
            this.Shrink(0, Config.NumberOfInstances);
        }

        public void Clear()
        {
            Clear(callOnBeforeRent: false);
        }
    }
}