using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Hado.Utils.ObjectPool
{
    public static class ObjectPoolUtils
    {
        public static IEnumerable<PoolObjectController> FindAllRentingPoolObjects()
        {
            // [FindObjectsOfType] cannot find SetActive(false) Objects
            var allPoolObjects = UnityEngine.Object.FindObjectsOfType<PoolObjectController>();
            return allPoolObjects.Where(p => p.IsRenting);
        }

        public static IEnumerable<PoolObjectController> FindAllRentingPoolObjects(int id)
        {
            return FindAllRentingPoolObjects().Where(c => c.Id == id);
        }

        public static IEnumerable<PoolObjectController> FindAllRentingPoolObjectsInScene(Scene scene)
        {
            // [GetRootGameObjects] cannot find SetActive(false) Objects
            return scene.GetRootGameObjects()
                        .SelectMany(go => go.GetComponentsInChildren<PoolObjectController>());
        }

        public static IEnumerable<PoolObjectController> FindAllRentingPoolObjectsInScene(Scene scene, int id)
        {
            return FindAllRentingPoolObjectsInScene(scene).Where(c => c.Id == id);
        }

        public static IEnumerable<PoolObjectController> ResolveNestedPoolObjects(PoolObjectController controller)
        {
            return controller.GetComponentsInChildren<PoolObjectController>()   // DeepFirstSearch
                             .Where(c => c.IsRenting)
                             .Reverse();
        }
    }
}