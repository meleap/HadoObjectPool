using System;

namespace Hado.Utils.ObjectPool
{
    public static class PoolManagedBehaviourExtensions
    {
        /// <summary>Add disposable to controller. Return value is self disposable.</summary>
        /// see UniRx.DisposableExtensions.AddTo
        public static T DisposeWhenReturn<T>(this T disposable, PoolManagedBehaviour poolManagedBehaviour)
        where T : IDisposable
        {
#pragma warning disable RECS0017
            if (disposable == null) throw new ArgumentNullException("disposable");
#pragma warning restore RECS0017

            if (poolManagedBehaviour == null) throw new ArgumentNullException("poolManagedBehaviour");
            var poolObjectController = poolManagedBehaviour.GetComponent<PoolObjectController>();
            if (poolObjectController == null) throw new ArgumentNullException("poolObjectController");
            poolObjectController.AddDisposable(disposable);
            return disposable;
        }
    }
}