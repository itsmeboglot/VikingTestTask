using System;
using System.Collections.Generic;
using System.Linq;
using Core.UiSystem.Data;
using Zenject;

namespace Core.UiSystem.Pool
{
    public class ViewPool
    {
        private readonly Dictionary<WindowType, List<ViewPoolObject>> objects = new ();
        private readonly Factory factory;
        
        public ViewPool(IEnumerable<ViewPoolObject> prefabs, DiContainer container)
        {
            factory = new Factory(prefabs, container);
        }
        
        public void Push(ViewPoolObject value)
        {
            value.Canvas.gameObject.SetActive(false);
            value.OnPush();
            
            if (!objects.TryGetValue(value.Type, out var list))
                list = new List<ViewPoolObject>();

            if (!list.Contains(value))
                list.Add(value);
            
            objects[value.Type] = list;
        }

        public ViewPoolObject Pop(WindowType type)
        {
            if (!objects.TryGetValue(type, out var list))
                list = new List<ViewPoolObject>();

            ViewPoolObject component;
            
            if (list.Count > 0)
            {
                component = list[0];
                list.RemoveAt(0);
                component.Canvas.gameObject.SetActive(true);
            }
            else
            {
                component = factory.Create(type);
            }
            
            return component;
        }

        private class Factory
        {
            private readonly DiContainer container;
            private readonly Dictionary<WindowType, ViewPoolObject> prefabs;

            public Factory(IEnumerable<ViewPoolObject> prefabs, DiContainer container)
            {
                this.container = container;
                this.prefabs = prefabs.ToDictionary(o => o.Type);
            }

            public ViewPoolObject Create(WindowType type)
            {
                if (!prefabs.ContainsKey(type))
                    throw new Exception($"You do not add prefab view {type}.");

                var component = container.InstantiatePrefab(prefabs[type]).GetComponent<ViewPoolObject>();
                return component;
            }
        }   
    }
}