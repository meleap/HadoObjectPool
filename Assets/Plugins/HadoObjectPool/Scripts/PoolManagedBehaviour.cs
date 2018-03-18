using System;
using UniRx;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    [DisallowMultipleComponent]
    public class PoolManagedBehaviour : MonoBehaviour
    {
        PoolObjectController _poolObjectController;
        PoolObjectController poolObjectController
        {
            get
            {
                return _poolObjectController ? _poolObjectController : _poolObjectController = GetComponent<PoolObjectController>();
            }
        }

        protected virtual void Awake()
        {
            _poolObjectController = GetComponent<PoolObjectController>();
            if (_poolObjectController == null)
                Debug.LogWarning("PoolManagedBehaviour must be instantiated by ObjectPool.");
        }

        /// <summary>
        /// Return to the pool. Call this method instead of Destroy.
        /// </summary>
        public void Return()
        {
            poolObjectController.Return();
        }

        /// <summary>
        /// Return to the pool after <paramref name="delayTimeSeconds"/> seconds. Call this method instead of Destroy.
        /// </summary>
        /// <param name="delayTimeSeconds">Delay time seconds.</param>
        public void Return(float delayTimeSeconds)
        {
            Observable.Timer(TimeSpan.FromSeconds(delayTimeSeconds))
                      .Subscribe(_ => Return())
                      .DisposeWhenReturn(this);
        }
    }
}