using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 3;
    int hp;
    Vector3 lastCheckpoint;

    void Start()
    {
        hp = maxHP;
        lastCheckpoint = transform.position;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Respawn();
    }

    public void SetCheckpoint(Vector3 pos) => lastCheckpoint = pos;

    void Respawn()
    {
        hp = maxHP;
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;
        transform.position = lastCheckpoint;
    }
}
