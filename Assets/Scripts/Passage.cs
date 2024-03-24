using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Passage : MonoBehaviour
{
    public Transform connection;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var position = connection.position;
        var transform1 = other.transform;
        position.z = transform1.position.z;
        transform1.position = position;
    }
}