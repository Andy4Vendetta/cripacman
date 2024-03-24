using UnityEngine;
[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour
{
    protected Ghost Ghost { get; private set; }
    public float duration;
    private void Awake()
    {
        Ghost = GetComponent<Ghost>();
    }
    public void Enable()
    {
        Enable(duration);
    }
    public virtual void Enable(float duration1)
    {
        enabled = true;
        CancelInvoke();
        Invoke(nameof(Disable), duration1);
    }
    public virtual void Disable()
    {
        enabled = false;
        CancelInvoke();
    }
}