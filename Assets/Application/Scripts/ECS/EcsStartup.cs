using ECS;
using Leopotam.EcsLite;
using UnityEngine;
using VContainer;

namespace Client
{
    public sealed class EcsStartup : MonoBehaviour
    {
        private EcsWorld _world;
        private IEcsSystems _systems;
        private bool _initialized;
        private ProjectSession _projectSession;

        [Inject]
        public void Construct(ProjectSession projectSession)
        {
            _projectSession = projectSession;
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _world = new EcsWorld();
            _systems = new EcsSystems(_world, new EcsAppContext(_projectSession));
            _systems
                .Add(new ImportElementsFromProjectSystem())
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

            _initialized = true;
            Debug.Log("[ECS] Initialized");
        }

        void Update ()
        {
            if (!_initialized)
                return;

            // process systems here.
            _systems?.Run ();
        }

        void OnDestroy ()
        {
            if (_systems != null)
            {
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _systems.Destroy ();
                _systems = null;
            }

            // cleanup custom worlds here.

            // cleanup default world.
            if (_world != null)
            {
                _world.Destroy ();
                _world = null;
            }
        }
    }
}
