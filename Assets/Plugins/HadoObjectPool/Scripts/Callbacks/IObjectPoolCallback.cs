
namespace Hado.Utils.ObjectPool.Callbacks
{
    public interface IObjectPoolCallback<T>
        where T : PoolManagedBehaviour
    {
        void OnCreateInstance(T instance);

        void OnBeforeRent(T instance);

        void OnBeforeReturn(T instance);

        void OnClear(T instance);
    }
}