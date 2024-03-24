using UnityEngine;
public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;
    private bool eaten;
    public override void Enable(float duration1)
    {
        base.Enable(duration1);
        body.enabled = false;
        eyes.enabled = false;
        blue.enabled = true;
        white.enabled = false;
        Invoke(nameof(Flash), duration1 / 2f);
    }
    public override void Disable()
    {
        base.Disable();
        body.enabled = true;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }
    private void Eaten()
    {
        eaten = true;
        Ghost.SetPosition(Ghost.Home.inside.position);
        Ghost.Home.Enable(duration);
        body.enabled = false;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }
    private void Flash()
    {
        if (eaten) return;
        blue.enabled = false;
        white.enabled = true;
        white.GetComponent<AnimatedSprite>().Restart();
    }
    private void OnEnable()
    {
        blue.GetComponent<AnimatedSprite>().Restart();
        Ghost.Movement.speedMultiplier = 0.5f;
        eaten = false;
    }
    private void OnDisable()
    {
        Ghost.Movement.speedMultiplier = 1f;
        eaten = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var node = other.GetComponent<Node>();
        if (node == null || !enabled) return;
        var direction = Vector2.zero;
        var maxDistance = float.MinValue;
        foreach (var availableDirection in node.availableDirections)
        {
            var newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
            var distance = (Ghost.target.position - newPosition).sqrMagnitude;
            if (!(distance > maxDistance)) continue;
            direction = availableDirection;
            maxDistance = distance;
        }
        Ghost.Movement.SetDirection(direction);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Pacman")) return;
        if (enabled) {
            Eaten();
        }
    }
}