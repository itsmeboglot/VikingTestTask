using System;
using System.Collections.Generic;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;
using UnityEngine;
using Zenject;

namespace Core.UiSystem.DI
{
    public class UiInstaller : MonoInstaller, IWindowControllerFactory
    {
        [SerializeField] private Canvas defaultCanvas;
        [SerializeField] private ViewPoolObject[] windowPrefabs;
        
        private readonly Dictionary<WindowType, Type> controllerTypes = new ();
        private ViewPool pool;

        public override void InstallBindings()
        {
            Container.Bind<IWindowControllerFactory>().FromInstance(this);
            Container.BindInterfacesTo<WindowBuilder>().AsSingle();
            Container.BindInterfacesTo<WindowHandler>().AsSingle();
            Container.BindInterfacesTo<WindowCanvasBuilder>().AsSingle().WithArguments(defaultCanvas);
            Container.Bind<ViewPool>().AsSingle().WithArguments(windowPrefabs);
            
            foreach (var view in windowPrefabs)
            {
                var windowView = (IWindowView) view;
                var (windowType, controllerType) = windowView.Install(Container);
                controllerTypes.Add(windowType, controllerType);
            }
        }
        
        public IWindowController GetWindowController(WindowType type)
        {
            if (!controllerTypes.ContainsKey(type))
                Debug.LogError($"You have not bind controller: {type}");

            return (IWindowController) Container.Resolve(controllerTypes[type]);
        }
    }

    public interface IWindowControllerFactory
    {
        IWindowController GetWindowController(WindowType type);
    }
}
