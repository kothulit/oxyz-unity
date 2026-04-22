using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
public class AppLifetimeScope : LifetimeScope
{
    [SerializeField] private AppStateFrameController _appStateFrameController;

    protected override void Configure(IContainerBuilder builder)
    {

        // Пока регистрируем только точку входа
        builder.Register<AppStateController>(Lifetime.Singleton);

        builder.RegisterComponent(_appStateFrameController);

        builder.RegisterEntryPoint<AppStartup>();
    }
}
 