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
using System;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Net.Sockets;

namespace Hedwig.Map3D
{
    public class MapViewer : MonoBehaviour
    {
        [SerializeField, Required]
        private NavMeshAgent agentPrefab;

        [SerializeField, Required]
        private CinemachineVirtualCamera vcam;

        [SerializeField, Required]
        private CinemachineFreeLook freeCam;

        [SerializeField]
        private Vector3 cameraOffset = Vector3.zero;

        [SerializeField]
        private string mapScene;

        [SerializeField]
        private Material material;

        private MapPoints placeMapPoints;

        async void Awake()
        {
            await SceneManager.LoadSceneAsync(mapScene, mode: LoadSceneMode.Additive).ToUniTask();

            Debug.Log(string.Join(",", SceneUtility.Scenes.Select(s => s.name)));

            placeMapPoints = SceneUtility.Scenes.SelectMany(scene => scene.FindObjectsTypeOf<MapPoints>())
                .FirstOrDefault(points => points.Type == PointType.Place);

            if (placeMapPoints == null)
            {
                Debug.LogError("Not Found MapPonits(Place)");
                return;
            }

            var mapdata = SceneUtility.Scenes.SelectMany(scene => scene.FindObjectsTypeOf<MapData>())
                .FirstOrDefault();
            if (mapdata != null)
            {
                var layer = LayerMask.NameToLayer("Obstacle0");
                foreach (var renderer in mapdata.FindRendererWithLayer(layer))
                {
                    Debug.Log($"{renderer.gameObject.name}");
                    renderer.material = material;
                }
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
            SetupAgent(agent, vcam!, freeCam!);
            SetupMouse(agent, vcam!, freeCam!);
        }

        void SetupAgent(NavMeshAgent agent, CinemachineVirtualCamera vcam, CinemachineFreeLook freeCam)
        {
            // vcam.transform.SetParent(agent.transform, worldPositionStays: false);
            vcam.transform.localPosition = Vector3.up + cameraOffset;
            vcam.Follow = agent.transform;
            vcam.LookAt = agent.transform;
            freeCam.Follow = agent.transform;
            freeCam.LookAt = agent.transform;

            var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                this.UpdateAsObservable().Subscribe(_ =>
                {
                    transposer.m_FollowOffset = cameraOffset;
                }).AddTo(this);
            }
        }

        void SetupMouse(NavMeshAgent agent, CinemachineVirtualCamera vcam,CinemachineFreeLook freeCam)
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

                // freeCam.m_YAxis.Value -= e.wheel;
                for (int i = 0; i < 3; i++)
                {
                    var radius = freeCam.m_Orbits[i].m_Radius;
                    radius -= e.wheel;
                    if (radius > 50) radius = 50;
                    if (radius < 0.1f) radius = 0.1f;
                    freeCam.m_Orbits[i].m_Radius = radius;
                }
            }).AddTo(this);

            var hratio = 180f / Screen.width;
            var vratio = -1f / Screen.height;
            var rmb = context.GetObservable(1);
            rmb.Difference().Subscribe(v =>
            {
                freeCam.m_XAxis.Value += hratio * v.x * 0.5f;
                freeCam.m_YAxis.Value += vratio * v.y * 0.5f;

            }).AddTo(this);
        }
    }
}