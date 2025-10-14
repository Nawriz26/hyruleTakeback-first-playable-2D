using UnityEngine;

public class Gem : MonoBehaviour
{
    public static int Count = 0;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Count++;
            Destroy(gameObject);
        }
    }
}
