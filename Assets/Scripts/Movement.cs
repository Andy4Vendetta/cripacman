using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8f;
    public float speedMultiplier = 1f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 Direction { get; private set; }
    private Vector2 NextDirection { get; set; }
    private Vector3 StartingPosition { get; set; }
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        StartingPosition = transform.position;
    }
    private void Start()
    {
        ResetState();
    }
    public void ResetState()
    {
        speedMultiplier = 1f;
        Direction = initialDirection;
        NextDirection = Vector2.zero;
        transform.position = StartingPosition;
        Rigidbody.isKinematic = false;
        enabled = true;
    }
    private void Update()
    {
        if (NextDirection != Vector2.zero) {
            SetDirection(NextDirection);
        }
    }
    private void FixedUpdate()
    {
        var position = Rigidbody.position;
        var translation = Direction * (speed * speedMultiplier * Time.fixedDeltaTime);
        Rigidbody.MovePosition(position + translation);
    }
    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            Direction = direction;
            NextDirection = Vector2.zero;
        }
        else
        {
            NextDirection = direction;
        }
    }
    private bool Occupied(Vector2 direction)
    {
        var hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }
}