using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Door : MonoBehaviour
{
    public float fadeTo = 0.25f;
    public void Open()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, fadeTo);
    }
}
