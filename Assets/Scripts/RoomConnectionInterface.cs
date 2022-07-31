using System.Collections.Generic;

public interface RoomConnectionInterface
{
    public void resetVisibility();
    public void Updatevisibility();
    public void SetConnections(List<bool> pConnectionState);
}