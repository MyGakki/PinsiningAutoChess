using UnityEngine;

public class BaseBehaviour: MonoBehaviour, IGameBase
{
    public virtual void Init(params object[] args)
    {
    }

    public string Desc()
    {
        return string.Empty;
    }

    public override bool Equals(object other)
    {
        if (other != null && other is BaseBehaviour)
            return ((BaseBehaviour) other).GetInstanceID() == this.GetInstanceID();

        return false;
    }
}
