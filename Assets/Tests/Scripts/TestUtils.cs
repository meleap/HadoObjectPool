using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hado.Utils.ObjectPool
{
    public static class TestUtils
    {
        public static bool IsInTheDontDestroyOnLoad(GameObject go)
        {
            var scene = GetExistingScene(go);
            return !scene.HasValue; // DontDestroyOnLoadされたらSceneに属さない
        }

        public static Scene? GetExistingScene(GameObject go)
        {
            var root = go.transform.root.gameObject;
            foreach (var scene in GetAllScenes())
            {
                if (scene.GetRootGameObjects().Any(r => r == root))
                    return scene;
            }
            return null;
        }

        public static IEnumerable<Scene> GetAllScenes()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
                yield return SceneManager.GetSceneAt(i);
        }
    }
}
