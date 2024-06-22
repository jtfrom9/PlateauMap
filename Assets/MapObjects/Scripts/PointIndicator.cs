using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hedwig.Map3D
{
    public class PointIndicator : MonoBehaviour
    {
        public Color gizmoColor = Color.yellow;
        public float gizmoSize = 1.0f;

        // シーンビューで常にギズモを表示
        void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
    }
}
