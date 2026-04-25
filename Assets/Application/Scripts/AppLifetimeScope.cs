using Client;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
public class AppLifetimeScope : LifetimeScope
{
    [SerializeField] private AppStateFrameController _appStateFrameController;
    [SerializeField] private ProjectFolderPicker _projectFolderPicker;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<AppStateController>(Lifetime.Singleton);
        builder.Register<ProjectSession>(Lifetime.Singleton);

        builder.Register<IProjectLoader, ProjectLoader>(Lifetime.Singleton);

        builder.RegisterComponent(_appStateFrameController);
        builder.RegisterComponent(_projectFolderPicker);

        builder.RegisterComponentInHierarchy<EcsStartup>();

        builder.RegisterEntryPoint<AppStartup>();
    }
}
 