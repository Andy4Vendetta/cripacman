using System.Collections;
using UnityEngine;
public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;
    private void OnEnable()
    {
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        if (gameObject.activeInHierarchy) {
            StartCoroutine(ExitTransition());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            Ghost.Movement.SetDirection(-Ghost.Movement.Direction);
        }
    }
    private IEnumerator ExitTransition()
    {
        Ghost.Movement.SetDirection(Vector2.up, true);
        Ghost.Movement.Rigidbody.isKinematic = true;
        Ghost.Movement.enabled = false;
        var position = transform.position;
        const float duration1 = 0.5f;
        var elapsed = 0f;
        while (elapsed < duration1)
        {
            Ghost.SetPosition(Vector3.Lerp(position, inside.position, elapsed / duration1));
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < duration1)
        {
            Ghost.SetPosition(Vector3.Lerp(inside.position, outside.position, elapsed / duration1));
            elapsed += Time.deltaTime;
            yield return null;
        }
        Ghost.Movement.SetDirection(new Vector2(Random.value < 0.5f ? -1f : 1f, 0f), true);
        Ghost.Movement.Rigidbody.isKinematic = false;
        Ghost.Movement.enabled = true;
    }
}