using System.Collections.Generic;
using System.Linq;

namespace Hado.Utils.ObjectPool.Callbacks
{
    public class CompositeCallback<T> : IObjectPoolCallback<T>
        where T : UnityEngine.Component
    {
        readonly IObjectPoolCallback<T>[] callbacks;

        public CompositeCallback(IEnumerable<IObjectPoolCallback<T>> callbacks)
        {
            this.callbacks = callbacks.ToArray();
        }

        public void OnBeforeRent(T instance)
        {
            foreach (var c in callbacks)
                OnBeforeRent(instance);
        }

        public void OnBeforeReturn(T instance)
        {
            foreach (var c in callbacks)
                OnBeforeReturn(instance);
        }

        public void OnClear(T instance)
        {
            foreach (var c in callbacks)
                OnClear(instance);
        }

        public void OnCreateInstance(T instance)
        {
            foreach (var c in callbacks)
                OnCreateInstance(instance);
        }
    }
}