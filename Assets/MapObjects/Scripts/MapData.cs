#nullable enable

using System.Collections.Generic;

using UnityEngine;

namespace Hedwig.Map3D
{
    public class MapData : MonoBehaviour
    {
        public IEnumerable<Renderer> FindRendererWithLayer(int layer)
        {
            var renderers = GameObject.FindObjectsByType<Renderer>(sortMode: FindObjectsSortMode.None);
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.layer == layer)
                    yield return renderer;
            }
        }
    }
}