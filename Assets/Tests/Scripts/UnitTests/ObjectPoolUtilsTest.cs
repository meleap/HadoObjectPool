using System.Collections;
using System.Linq;
using RuntimeUnitTestToolkit;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hado.Utils.ObjectPool.UnitTests
{
    public class ObjectPoolUtilsTest : ObjectPoolTestBase
    {
        IObjectPool<PoolManagedBehaviour> CreatePool(int numberOfInstances)
        {
            var go = new GameObject();
            PoolManagedBehaviour prefab = go.AddComponent<TestBehaviour>();
            var config = new ObjectPoolConfig(numberOfInstances, numberOfInstances);
            return base.CreatePool(prefab, config);
        }

        public IEnumerator AllPoolObjectCanReturnByCallingFindAllRentingPoolObjects()
        {
            var numberOfInstances = 3;
            var pool = CreatePool(numberOfInstances: numberOfInstances);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();

            pool.Count.Is(numberOfInstances);
            var obj = pool.Rent();
            pool.Count.Is(numberOfInstances - 1);

            var id = obj.GetComponent<PoolObjectController>().Id;
            var all = ObjectPoolUtils.FindAllRentingPoolObjects(id);
            foreach (var o in all)
                pool.Return(o.Behaviour);

            pool.Count.Is(numberOfInstances);
        }

        public IEnumerator NestedPoolObjectCanReturnToThePool()
        {
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();

            var parent = pool.Rent();
            var child1 = pool.Rent();
            var child2 = pool.Rent();
            var grandson1 = pool.Rent();
            var grandson2 = pool.Rent();

            child1.transform.SetParent(parent.transform);
            child2.transform.SetParent(parent.transform);
            grandson1.transform.SetParent(child1.transform);
            grandson2.transform.SetParent(child2.transform);

            var revercedDeepFirst = ObjectPoolUtils.ResolveNestedPoolObjects(parent.GetComponent<PoolObjectController>())
                                                   .Select(c => c.Behaviour);
            var enumerator = revercedDeepFirst.GetEnumerator();
            enumerator.MoveNext();
            enumerator.Current.Is(grandson2);
            enumerator.MoveNext();
            enumerator.Current.Is(child2);
            enumerator.MoveNext();
            enumerator.Current.Is(grandson1);
            enumerator.MoveNext();
            enumerator.Current.Is(child1);
            enumerator.MoveNext();
            enumerator.Current.Is(parent);
            enumerator.MoveNext().IsFalse();

            foreach (var c in revercedDeepFirst)
                pool.Return(c);
        }

        public IEnumerator FindAllRentingPoolObjects()
        {
            yield return SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Additive);
            var testScene = SceneManager.GetSceneByName("TestScene");
            SceneManager.SetActiveScene(testScene);

            var go = new GameObject();  // in TestScene
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var testSceneObj = pool.Rent();
            testSceneObj.transform.SetParent(go.transform);
            var obj = pool.Rent();
            var id = obj.GetComponent<PoolObjectController>().Id;
            var allObjects = ObjectPoolUtils.FindAllRentingPoolObjects(id)
                                            .Select(c => c.Behaviour);
            var enumerator = allObjects.GetEnumerator();
            enumerator.MoveNext().IsTrue();
            enumerator.Current.Is(obj);
            enumerator.MoveNext().IsTrue();
            enumerator.Current.Is(testSceneObj);

            foreach (var o in allObjects)
                pool.Return(o);

            yield return SceneManager.UnloadSceneAsync(testScene);
        }

        public IEnumerator FindAllRentingPoolObjectsInScene()
        {
            yield return SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Additive);
            var testScene = SceneManager.GetSceneByName("TestScene");
            SceneManager.SetActiveScene(testScene);

            var go = new GameObject();  // in TestScene
            var pool = CreatePool(numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var obj = pool.Rent();
            obj.transform.SetParent(go.transform);

            var allObjects = ObjectPoolUtils.FindAllRentingPoolObjectsInScene(testScene)
                                            .Select(c => c.Behaviour);
            var enumerator = allObjects.GetEnumerator();
            enumerator.MoveNext().IsTrue();
            enumerator.Current.Is(obj);

            foreach (var o in allObjects)
                pool.Return(o);

            yield return SceneManager.UnloadSceneAsync(testScene);
        }
    }
}