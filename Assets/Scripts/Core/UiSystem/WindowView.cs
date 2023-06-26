using System;
using UnityEngine;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;
using Zenject;

namespace Core.UiSystem
{
    public abstract class WindowView<TController> : ViewPoolObject, IWindowView
        where TController : IWindowController
    {
        private RectTransform rectTransform;
        public RectTransform Rect => rectTransform != null
            ? rectTransform
            : rectTransform = gameObject.GetComponent<RectTransform>();
        
        public (WindowType, Type) Install(DiContainer container)
        {
            container.BindInterfacesAndSelfTo<TController>().AsTransient().WithArguments(Type);
            return (Type, typeof(TController));
        }

        public void SetData(Canvas canvas)
        {
            Canvas = canvas;
        }
    }
}