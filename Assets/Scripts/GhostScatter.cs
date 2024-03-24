using UnityEngine;
public class GhostScatter : GhostBehavior
{
    private void OnDisable()
    {
        Ghost.Chase.Enable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var node = other.GetComponent<Node>();
        if (node == null || !enabled || Ghost.Frightened.enabled) return;
        var index = Random.Range(0, node.availableDirections.Count);
        if (node.availableDirections.Count > 1 && node.availableDirections[index] == -Ghost.Movement.Direction)
        {
            index++;
            if (index >= node.availableDirections.Count) {
                index = 0;
            }
        }
        Ghost.Movement.SetDirection(node.availableDirections[index]);
    }
}