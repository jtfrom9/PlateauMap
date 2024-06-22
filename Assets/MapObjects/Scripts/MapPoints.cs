#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hedwig.Map3D
{
    public enum PointType
    {
        Place, Camera
    }

    public class MapPoints : MonoBehaviour
    {
        [SerializeField]
        PointType pointType;

        private Transform[] points;

        void Awake()
        {
            points = GetComponentsInChildren<PointIndicator>(includeInactive: true)
                .Select(pi => pi.transform)
                .ToArray();
        }

        public PointType Type => pointType;

        public Transform? GetPoint(int index)
        {
            if (index < 0 || points.Length <= index)
            {
                return null;
            }
            return points[index].transform;
        }
    }
}