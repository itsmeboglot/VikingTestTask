using System.Collections.Generic;
using Core.UiSystem.Data;
using Core.UiSystem.DI;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;
using UnityEngine;

namespace Core.UiSystem
{
    public class WindowBuilder : IWindowBuilder
    {
        private readonly IWindowControllerFactory controllerFactory;
        private readonly IWindowCanvasBuilder canvasBuilder;
        private readonly ViewPool pool;
        private readonly Dictionary<WindowType, IWindowController> controllerPool = new ();

        public WindowBuilder(IWindowControllerFactory controllerFactory, 
            IWindowCanvasBuilder canvasBuilder, ViewPool pool)
        {
            this.controllerFactory = controllerFactory;
            this.canvasBuilder = canvasBuilder;
            this.pool = pool;
        }

        public IWindowController Create(WindowType type)
        {
            var isExisted = controllerPool.ContainsKey(type);
            var controller = isExisted ? controllerPool[type] : controllerFactory.GetWindowController(type);

            var view = (IWindowView) pool.Pop(controller.Type);

            if (!isExisted)
            {
                var canvas = canvasBuilder.Create();
                canvas.gameObject.name = view.Type.ToString();
                var canvasTransform = canvas.GetComponent<RectTransform>(); 
                
                view.Rect.SetParent(canvasTransform, false);
                view.SetData(canvas);
                controller.SetView(view);
                
                controllerPool.Add(type, controller);
            }

            return controller;
        }
    }

    public interface IWindowBuilder
    {
        IWindowController Create(WindowType type);
    }
}