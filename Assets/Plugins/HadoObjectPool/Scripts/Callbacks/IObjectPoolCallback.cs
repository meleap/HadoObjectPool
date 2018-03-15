
namespace Hado.Utils.ObjectPool.Callbacks
{
    public interface IObjectPoolCallback<T>
        where T : UnityEngine.Component
    {
        void OnCreateInstance(T instance);

        void OnBeforeRent(T instance);

        void OnBeforeReturn(T instance);

        void OnClear(T instance);
    }
}