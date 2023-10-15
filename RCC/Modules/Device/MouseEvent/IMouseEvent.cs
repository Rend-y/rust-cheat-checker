namespace RCC.Modules.Device.MouseEvent;

public interface IMouseEvent<T>
{
    void CreateEvent(int posX, int posY, T eventFlags, int interval = 0);
}