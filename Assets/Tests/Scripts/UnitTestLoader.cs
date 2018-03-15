using UnityEngine;
using RuntimeUnitTestToolkit;
using UniRx;
using UniRx.Triggers;

namespace Hado.Utils.ObjectPool
{
    public static class UnitTestLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            UnitTest.RegisterAllMethods<ObjectPoolConfigTest>();
            UnitTest.RegisterAllMethods<ObjectPoolImplTest>();
            UnitTest.RegisterAllMethods<ObjectPoolEventFunctionsTest>();
            UnitTest.RegisterAllMethods<DontDestroyObjectPoolTest>();
            UnitTest.RegisterAllMethods<ObjectPoolUtilsTest>();

            // avoid InvalidOperationException
            Observable.OnceApplicationQuit().Subscribe(_ =>
            {
                foreach (var c in ObjectPoolUtils.FindAllRentingPoolObjects())
                    c.OnReturn();
            });
        }
    }
}