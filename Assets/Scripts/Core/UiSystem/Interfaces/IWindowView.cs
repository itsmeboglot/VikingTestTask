using System;
using Core.UiSystem.Data;
using UnityEngine;
using Zenject;

namespace Core.UiSystem.Interfaces
{
    public interface IWindowView
    {
        RectTransform Rect { get; }
        WindowType Type { get; }
        (WindowType, Type) Install(DiContainer container);
        void SetData(Canvas canvas);
        void OnPop();
        void OnPush();
    }
}