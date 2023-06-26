using UnityEngine;

namespace Core.UiSystem
{
    public class WindowCanvasBuilder : IWindowCanvasBuilder
    {
        private readonly Canvas defaultCanvas;

        public WindowCanvasBuilder(Canvas defaultCanvas)
        {
            this.defaultCanvas = defaultCanvas;       
        }
        
        public Canvas Create()
        {
            var canvas = Object.Instantiate(defaultCanvas);
            canvas.transform.localPosition = Vector3.zero;
            canvas.transform.localScale = Vector3.one;
            canvas.worldCamera = Camera.main;
            return canvas;
        }
    }

    public interface IWindowCanvasBuilder
    {
        Canvas Create();
    }
}