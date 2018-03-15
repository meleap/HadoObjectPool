using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Hado.Utils.ObjectPool.Examples
{
    public class HadoObjectPoolSample : MonoBehaviour
    {
        [SerializeField]
        Transform parent;

        [SerializeField]
        int numberOfInstances;

        [SerializeField]
        bool needPreactivation;

        [SerializeField]
        PoolManagedBehaviour prefab;

        int id = 1;

        Queue<PoolManagedBehaviour> queue = new Queue<PoolManagedBehaviour>();

        IEnumerator Start()
        {
            SetUpKeyController();

            var config = new ObjectPoolConfig(
                numberOfInstances: numberOfInstances,
                createCountPerFrame: 1,
                needPreactivation: needPreactivation
            );
            var objectPool = new DontDestroyObjectPool(id, prefab, config);
            ObjectPoolManager.Instance.RegisterPool(id, objectPool);

            yield return ObjectPoolManager.Instance.PreloadAsync().ToYieldInstruction();
            yield return ObjectPoolManager.Instance.PreactivateAsync().ToYieldInstruction();
            Debug.Log("OnLoaded");
        }

        void SetUpKeyController()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Return))
                .Subscribe(_ => RentEnqueue())
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => DequeueReturn())
                .AddTo(this);
        }

        void RentEnqueue()
        {
            var obj = ObjectPoolManager.Instance.Rent(id);
            queue.Enqueue(obj);
            obj.transform.SetParent(parent);
        }

        void DequeueReturn()
        {
            if (queue.Count <= 0)
                return;
            var obj = queue.Dequeue();
            ObjectPoolManager.Instance.Return(obj);
        }
    }
}