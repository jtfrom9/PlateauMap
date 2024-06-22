#nullable enable

using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

using Hedwig.Map3D;
using System.Collections;
using System.Collections.Generic;

namespace Hedwig.Map3D
{
    public class MapViewer : MonoBehaviour
    {
        [SerializeField]
        private NavMeshAgent agentPrefab;

        private MapPoints placeMapPoints;

        void Awake()
        {
            placeMapPoints = SceneUtility.Scenes.SelectMany(scene => scene.FindObectsTypeOf<MapPoints>(includeInactive: true))
                .FirstOrDefault(ps => ps.Type == PointType.Place);

            if (placeMapPoints == null) {
                Debug.LogError("Not Found MapPonits(Place)");
            }
        }

        void Start()
        {
            if (placeMapPoints == null)
                return;
            var point = placeMapPoints.GetPoint(0);
            if (point != null)
            {
                Instantiate(agentPrefab, point.position, Quaternion.identity);
            }
        }
    }
}