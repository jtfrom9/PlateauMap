#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Hedwig.Map3D
{
    public static class SceneUtility
    {
        public static IEnumerable<Scene> Scenes
        {
            get
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    yield return SceneManager.GetSceneAt(i);
                }
            }
        }
    }

    public static class SceneExtension
    {
        public static T[] FindObectsTypeOf<T>(this Scene scene, bool includeInactive)
        {
            var results = new List<T>();
            var gos = scene.GetRootGameObjects();
            foreach(var go in gos)
            {
                if(go.TryGetComponent<T>(out var component))
                {
                    results.Add(component);
                }
            }
            return results.ToArray();
        }
    }
}