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

using InputObservable;

namespace Hedwig.Map3D
{
    public class MapViewer : MonoBehaviour
    {
        [SerializeField, Required]
        private NavMeshAgent agentPrefab;

        [SerializeField, Required]
        private CinemachineVirtualCamera vcam;

        [SerializeField]
        private Vector3 cameraOffset = Vector3.zero;

        private MapPoints placeMapPoints;

        void Awake()
        {
            placeMapPoints = SceneUtility.Scenes.SelectMany(scene => scene.FindObectsTypeOf<MapPoints>(includeInactive: true))
                .FirstOrDefault(ps => ps.Type == PointType.Place);

            if (placeMapPoints == null)
            {
                Debug.LogError("Not Found MapPonits(Place)");
            }
        }

        void Start()
        {
            if (placeMapPoints == null)
                return;
            var point = placeMapPoints.GetPoint(0);
            if (point == null)
            {
                Debug.LogError("no start points");
                return;
            }

            var agent = Instantiate(agentPrefab, point.position, Quaternion.identity).GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("no agent");
                return;
            }
            SetupAgent(agent, vcam!);
            SetupMouse(agent);
        }

        void SetupAgent(NavMeshAgent agent, CinemachineVirtualCamera vcam)
        {
            vcam.transform.SetParent(agent.transform, worldPositionStays: false);
            // vcam.transform.localPosition = Vector3.up * cameraHeight;
            vcam.Follow = agent.transform;
            vcam.LookAt = agent.transform;
            var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                this.UpdateAsObservable().Subscribe(_ =>
                {
                    transposer.m_FollowOffset = cameraOffset;
                }).AddTo(this);
            }
        }

        void SetupMouse(NavMeshAgent agent)
        {
            var context = this.DefaultInputContext();
            var lmb = context.GetObservable(0);
            lmb.OnBegin.Subscribe(e =>
            {
                var ray = Camera.main.ScreenPointToRay(e.position);
                if (Physics.Raycast(ray, out var hit))
                {
                    Debug.Log($"hit: Point={hit.point}, Normal={hit.normal}",
                        context: hit.collider.gameObject);
                    agent.SetDestination(hit.point);
                }
            }).AddTo(this);

            var mouse = new MouseInputContext(this, null);
            mouse.Wheel.Subscribe(e =>
            {
                Debug.Log(e.wheel);
                cameraOffset -= new Vector3(0, e.wheel, 0);
            }).AddTo(this);
        }
    }
}