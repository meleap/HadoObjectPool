using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    [RequireComponent(typeof(PoolManagedBehaviour))]
    [DisallowMultipleComponent]
    public sealed class PoolObjectController : MonoBehaviour
    {
        public bool IsRenting { get; private set; }

        public int Id { get; private set; }

        public PoolManagedBehaviour Behaviour { get; private set; }

        bool canDestroy = false;

        List<IDisposable> disposables = new List<IDisposable>();

        public void OnCreateInstance(int id)
        {
            Id = id;
            Behaviour = GetComponent<PoolManagedBehaviour>();
        }

        public void OnRent()
        {
            IsRenting = true;
        }

        public void OnReturn()
        {
            IsRenting = false;
        }

        public void OnBeforeDestroy()
        {
            canDestroy = true;
        }

        public void Return()
        {
            ObjectPoolManager.Instance.Return(this);
        }

        public void ForceDestroy()
        {
            OnBeforeDestroy();
            Destroy(gameObject);
        }

        public void AddDisposable(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        void ClearDisposables()
        {
            foreach (var d in disposables)
                d.Dispose();
            disposables.Clear();
        }

        void OnDisable()
        {
            ClearDisposables();
            if (IsRenting)
                throw new InvalidOperationException("Renting GameObject must not be disalbed");
        }

        void OnDestroy()
        {
            if (!canDestroy)
                throw new InvalidOperationException("PoolObject must not be destroyed");
        }
    }
}