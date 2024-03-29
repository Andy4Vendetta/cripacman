using UnityEngine;
[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(Movement))]
public class Ghost : MonoBehaviour
{
    public Movement Movement { get; private set; }
    public GhostHome Home { get; private set; }
    public GhostScatter Scatter { get; private set; }
    public GhostChase Chase { get; private set; }
    public GhostFrightened Frightened { get; private set; }
    public GhostBehavior initialBehavior;
    public Transform target;
    public int points = 200;
    private void Awake()
    {
        Movement = GetComponent<Movement>();
        Home = GetComponent<GhostHome>();
        Scatter = GetComponent<GhostScatter>();
        Chase = GetComponent<GhostChase>();
        Frightened = GetComponent<GhostFrightened>();
    }
    private void Start()
    {
        ResetState();
    }
    public void ResetState()
    {
        gameObject.SetActive(true);
        Movement.ResetState();
        Frightened.Disable();
        Chase.Disable();
        Scatter.Enable();
        if (Home != initialBehavior) {
            Home.Disable();
        }
        if (initialBehavior != null) {
            initialBehavior.Enable();
        }
    }
    public void SetPosition(Vector3 position)
    {
        // Keep the z-position the same since it determines draw depth
        var transform1 = transform;
        position.z = transform1.position.z;
        transform1.position = position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Pacman")) return;
        if (Frightened.enabled) {
            GameManager.Instance.GhostEaten(this);
        } else {
            GameManager.Instance.PacmanEaten();
        }
    }
}