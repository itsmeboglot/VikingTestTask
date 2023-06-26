using System;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;

namespace Core.UiSystem
{
    public abstract class WindowController<TView> : IWindowController 
        where TView : IWindowView
    {
        public event Action<IWindowController> OnCloseEvent;
        
        private readonly ViewPool viewPool;
        protected TView ConcreteView;
        IWindowView IWindowController.View => ConcreteView;

        public IWindowHandler WindowHandler { get; }
        public WindowType Type { get; private set; }
        public bool IsOpen { get; private set; }

        protected WindowController(WindowType type, IWindowHandler windowHandler, ViewPool viewPool)
        {
            this.viewPool = viewPool;
            WindowHandler = windowHandler;
            Type = type;
        }
        
        public void SetView(IWindowView view)
        {
            ConcreteView = (TView)view;
        }

        public void Open(object data = null)
        {
            IsOpen = true;
            OnOpen(data);
            ConcreteView.OnPop();
        }

        public void Close()
        {
            IsOpen = false;
            OnClose();
            viewPool.Push(ConcreteView as ViewPoolObject);
            
            OnCloseEvent?.Invoke(this);
        }

        protected virtual void OnOpen(object data = null) { }
        protected virtual void OnClose() { }
    }
}