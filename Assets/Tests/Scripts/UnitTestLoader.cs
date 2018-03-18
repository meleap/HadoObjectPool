using RuntimeUnitTestToolkit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hado.Utils.ObjectPool
{
    public static class UnitTestLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            if (SceneManager.GetActiveScene().name != "UnitTest")
                return;

            UnitTest.RegisterAllMethods<ObjectPoolConfigTest>();
            UnitTest.RegisterAllMethods<ObjectPoolImplTest>();
            UnitTest.RegisterAllMethods<ObjectPoolEventFunctionsTest>();
            UnitTest.RegisterAllMethods<ObjectPoolUtilsTest>();
        }
    }
}