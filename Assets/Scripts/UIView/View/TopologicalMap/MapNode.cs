using System.Collections.Generic;
public enum NodeType
{
    Battle,
    Award,
}

public class MapNode
{
    public NodeType NodeType;
    
    public List<MapNode> children;
}
