using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Hado.Utils.ObjectPool
{
    public class SampleCubeScript : PoolManagedBehaviour
    {
        float currentScaleY = 0f;
        IDisposable disposable;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Awake " + this);
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable " + this);
            transform.localScale = new Vector3(1, currentScaleY, 1);
            disposable = this.UpdateAsObservable()
                .TakeWhile(x => currentScaleY < 1)  // まだ開ききってないなら
                .Subscribe(_ =>
                {
                    currentScaleY += 0.1f;
                    if (currentScaleY > 1f) currentScaleY = 1f;
                    transform.localScale = new Vector3(1, currentScaleY, 1);
                }).AddTo(this);
        }

        void Start()
        {
            Debug.Log("Start " + this);
        }

        private void OnDisable()
        {
            currentScaleY = 0f;
            Debug.Log("OnDisable " + this);
            if (disposable != null)
            {
                disposable.Dispose();
                disposable = null;
            }
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy " + this);
        }

        public override string ToString()
        {
            return string.Format("[SampleCubeScript] CurrentScaleY-{0}", currentScaleY);
        }
    }
}