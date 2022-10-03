using System.Collections.Generic;

public interface RoomConnectionInterface
{
    public void resetVisibility();
    public void Updatevisibility();
    public void SetConnections(List<bool> pConnectionState);
    public void SetTPCollidersActive(bool pActive);
    public void SetHighlighted(bool pHighlighted);
}