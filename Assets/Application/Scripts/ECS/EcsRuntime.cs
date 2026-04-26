using ECS;
using Leopotam.EcsLite;
using R3;
using UnityEngine;
using VContainer;

namespace Client
{
    public sealed class EcsRuntime : MonoBehaviour
    {
        [SerializeField] private EcsEntityHierarchyPanel _entityHierarchyPanel;
        [SerializeField] private Transform _viewRoot;
        [SerializeField] private Material _defaultMaterial;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private ProjectSession _projectSession;
        private System.IDisposable _projectSubscription;

        [Inject]
        public void Construct(ProjectSession projectSession)
        {
            _projectSession = projectSession;
        }

        private void Start()
        {
            _projectSubscription = _projectSession.CurrentProject
                .Subscribe(OnProjectChanged);
        }

        private void OnProjectChanged(Project project)
        {
            DestroyEcsWorld();
            Debug.Log("[ECS] Project cleared. ECS world destroyed.");

            if (project == null)
            {
                return;
            }

            InitializeForProject(project);
        }

        private void InitializeForProject(Project project)
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, new EcsAppContext(
                project, 
                _entityHierarchyPanel,
                _viewRoot,
                _defaultMaterial));
            _systems
                .Add(new ImportElementsFromProjectSystem())
                .Add(new BuildEntityHierarchyUiSystem())
                .Add(new CreateBuildingViewsSystem())
                // register your systems here, for example:
                // .Add (new TestSystem1 ())
                // .Add (new TestSystem2 ())

                // register additional worlds here, for example:
                // .AddWorld (new EcsWorld (), "events")
#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Init();
            Debug.Log($"[ECS] Initialized for project: {project.Document.Name}");
        }

        void Update ()
        {
            // process systems here.
            _systems?.Run ();
        }



        void OnDestroy ()
        {
            _projectSubscription?.Dispose();
            _projectSubscription = null;
            DestroyEcsWorld();
        }

        private void DestroyEcsWorld()
        {
            if (_systems != null)
            {
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _systems.Destroy();
                _systems = null;
            }

            // cleanup custom worlds here.

            // cleanup default world.
            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}
