using Core.UiSystem.Data;

namespace Core.UiSystem.Interfaces
{
    public interface IWindowHandler
    {
        bool IsOpen(WindowType type);
        void OpenWindow(WindowType type, object openData = null);
        void CloseWindow(WindowType type);
    }
}