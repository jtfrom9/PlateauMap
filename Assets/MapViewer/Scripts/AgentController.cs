#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

using Cinemachine;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;

namespace Hedwig.Map3D
{
    public class AgentController: MonoBehaviour
    {
        // void OnDrawGizmos()
        // {
        //     // 矢印の色を設定
        //     Gizmos.color = Color.red;

        //     // 矢印の始点と終点を設定
        //     Vector3 start = transform.position;
        //     Vector3 end = start + transform.forward * 2.0f;

        //     // 矢印を描画
        //     Gizmos.DrawLine(start, end);
        //     Gizmos.DrawSphere(end, 0.1f);
        // }

        void OnRenderObject()
        {
            // if (lineMaterial == null)
            //     return;

            // lineMaterial.SetPass(0);
            var m = GetComponent<MeshRenderer>().material;
            m.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + transform.forward * 2);
            GL.End();
        }
    }
}
