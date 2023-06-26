using System;
using Core.UiSystem.Data;

namespace Core.UiSystem.Interfaces
{
    public interface IWindowController
    {
        event Action<IWindowController> OnCloseEvent;
        IWindowHandler WindowHandler { get; }
        WindowType Type { get; }
        IWindowView View { get; }
        bool IsOpen { get; }
        void Open(object data = null);
        void Close();
        void SetView(IWindowView view);
    }
}