
namespace Hado.Utils.ObjectPool.Callbacks
{
    public class NopObjectPoolCallBack<T> : IObjectPoolCallback<T>
        where T : UnityEngine.Component
    {
        public void OnBeforeRent(T instance)
        {
        }

        public void OnBeforeReturn(T instance)
        {
        }

        public void OnClear(T instance)
        {
        }

        public void OnCreateInstance(T instance)
        {
        }
    }
}