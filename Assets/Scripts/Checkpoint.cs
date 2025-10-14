using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("If true, destroy this checkpoint after it’s activated once.")]
    public bool oneShot = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        var ph = col.GetComponent<PlayerHealth>();
        if (ph == null) return;

        // Save the exact checkpoint position (you can add a small offset if you want)
        ph.SetCheckpoint(transform.position);

        if (oneShot) Destroy(gameObject);
    }
}
