using UnityEngine;

public class EnemyPatrol : MonoBehaviour, IDamageable
{
    public float speed = 2f;
    public Transform leftPoint, rightPoint;
    public int maxHP = 3;

    Rigidbody2D rb;
    int hp;
    bool movingRight = true;

    void Awake() { rb = GetComponent<Rigidbody2D>(); hp = maxHP; }

    void Update()
    {
        var target = movingRight ? rightPoint.position.x : leftPoint.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(target - transform.position.x) * speed, 0f);

        if (movingRight && transform.position.x >= rightPoint.position.x) movingRight = false;
        else if (!movingRight && transform.position.x <= leftPoint.position.x) movingRight = true;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            var ph = col.collider.GetComponent<PlayerHealth>();
            if (ph) ph.TakeDamage(1);
        }
    }
}
