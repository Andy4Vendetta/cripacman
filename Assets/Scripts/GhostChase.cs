using UnityEngine;
public class GhostChase : GhostBehavior
{
    private void OnDisable()
    {
        Ghost.Scatter.Enable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var node = other.GetComponent<Node>();
        if (node == null || !enabled || Ghost.Frightened.enabled) return;
        var direction = Vector2.zero;
        var minDistance = float.MaxValue;
        foreach (var availableDirection in node.availableDirections)
        {
            var newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
            var distance = (Ghost.target.position - newPosition).sqrMagnitude;
            if (!(distance < minDistance)) continue;
            direction = availableDirection;
            minDistance = distance;
        }
        Ghost.Movement.SetDirection(direction);
    }
}