using System;
using System.Collections.Generic;
using System.Linq;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using UnityEngine;

namespace Core.UiSystem
{
    public class WindowHandler : IWindowHandler, IDisposable
    {
        private readonly IWindowBuilder windowBuilder;
        private readonly List<IWindowController> windowCtrls = new ();
        private Action currentAction;
        private readonly Queue<Action> actions = new ();

        public WindowHandler(IWindowBuilder windowBuilder)
        {
            this.windowBuilder = windowBuilder;
        }

        public void OpenWindow(WindowType type, object openData = null)
        {
            RunAction(() =>
            {
                if (TryGetWindowController(type, out var controller))
                {
                    Debug.LogWarning($"Window is already opened!: {type}");
                    return;
                }
                
                controller = windowBuilder.Create(type);
                windowCtrls.Add(controller);
                controller.Open(openData);
                
                controller.OnCloseEvent += HandleWindowClose;

                ActionComplete();
            });
        }
        
        public void CloseWindow(WindowType type)
        {
            if (!TryGetWindowController(type, out var controller))
            {
                Debug.LogWarning("Window is not opened!");
                return;
            }
            windowCtrls.Remove(controller);
            controller.Close();
        }

        public bool IsOpen(WindowType type)
        {
            return TryGetWindowController(type, out var controller) && controller.IsOpen;
        }
        
        public void Dispose()
        {
            foreach (var windowController in windowCtrls)
            {
                windowController.Close();
                if(windowController is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        private void HandleWindowClose(IWindowController controller)
        {
            windowCtrls.Remove(controller);
            controller.OnCloseEvent -= HandleWindowClose;
        }
        
        private void RunAction(Action action)
        {
            if (currentAction == null)
            {
                currentAction = action;
                action.Invoke();
            }
            else
                actions.Enqueue(action);
        }
        
        private void ActionComplete()
        {
            currentAction = null;

            if (actions.Count > 0)
                RunAction(actions.Dequeue());
        }
        
        private bool TryGetWindowController(WindowType type, out IWindowController windowController)
        {
            windowController = windowCtrls.FirstOrDefault(x => x.Type == type);
            return windowController != null;
        }
    }
}