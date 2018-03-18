
namespace Hado.Utils.ObjectPool.Callbacks
{
    public class NopObjectPoolCallBack<T> : IObjectPoolCallback<T>
        where T : PoolManagedBehaviour
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