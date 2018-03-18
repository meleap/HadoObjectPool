using System.Collections;
using System.Linq;
using RuntimeUnitTestToolkit;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hado.Utils.ObjectPool
{
    public class ObjectPoolUtilsTest
    {
        readonly Transform hierarchyParent;

        public ObjectPoolUtilsTest()
        {
            hierarchyParent = new GameObject("parent").transform;
            hierarchyParent.OnDestroyAsObservable()
                  .SelectMany(_ => hierarchyParent.GetComponentsInChildren<PoolObjectController>(true))
                  .Subscribe(c => c.ForceDestroy());
        }

        IObjectPool<PoolManagedBehaviour> CreatePool(int id, int numberOfInstances)
        {
            var go = new GameObject();
            var prefab = go.AddComponent<TestBehaviour>();
            var config = new ObjectPoolConfig(numberOfInstances, numberOfInstances);
            return new DontDestroyObjectPool(id, prefab, config, hierarchyParent);
        }

        public IEnumerator AllPoolObjectCanReturnByCallingFindAllRentingPoolObjects()
        {
            var numberOfInstances = 3;
            var pool = CreatePool(id: 1, numberOfInstances: numberOfInstances);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();

            pool.Count.Is(numberOfInstances);
            pool.Rent();
            pool.Count.Is(numberOfInstances - 1);

            var all = ObjectPoolUtils.FindAllRentingPoolObjects().Select(c => c.Behaviour);
            foreach (var o in all)
                pool.Return(o);

            pool.Count.Is(numberOfInstances);
        }

        public IEnumerator NestedPoolObjectCanReturnToThePool()
        {
            var pool = CreatePool(id: 1, numberOfInstances: 1);
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
            var pool = CreatePool(id: 1, numberOfInstances: 1);
            yield return pool.PreloadAsync().ToYieldInstruction();
            yield return pool.PreactivateAsync().ToYieldInstruction();
            var testSceneObj = pool.Rent();
            testSceneObj.transform.SetParent(go.transform);
            var dontDestroyOnLoadObj = pool.Rent();

            var allObjects = ObjectPoolUtils.FindAllRentingPoolObjects()
                                            .Select(c => c.Behaviour);
            var enumerator = allObjects.GetEnumerator();
            enumerator.MoveNext().IsTrue();
            enumerator.Current.Is(dontDestroyOnLoadObj);
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
            var pool = CreatePool(id: 1, numberOfInstances: 1);
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