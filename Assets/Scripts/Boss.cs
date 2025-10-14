using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int hp = 5;

    [Header("On Death")]
    public Door doorToOpen; // assign in Inspector

    public void TakeDamage(int d)
    {
        hp -= d;
        StartCoroutine(HitFlash());   // visual feedback (optional)
        if (hp <= 0)
        {
            if (doorToOpen) doorToOpen.Open();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            col.collider.GetComponent<PlayerHealth>()?.TakeDamage(1);
    }

    System.Collections.IEnumerator HitFlash()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr) { var c = sr.color; sr.color = Color.red; yield return new WaitForSeconds(0.08f); sr.color = c; }
    }
}
