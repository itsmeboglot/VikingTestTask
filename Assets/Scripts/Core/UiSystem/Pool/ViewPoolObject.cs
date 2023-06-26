using Core.UiSystem.Data;
using UnityEngine;

namespace Core.UiSystem.Pool
{
    public class ViewPoolObject : MonoBehaviour
    {
        [field: SerializeField] public WindowType Type { get; private set; }
        
        public Canvas Canvas { get; protected set; }
        
        public virtual void OnPop() {}
        public virtual void OnPush() {}
    }
}