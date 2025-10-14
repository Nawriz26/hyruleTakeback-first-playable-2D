using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI gemText;
    public PlayerHealth player;
    void Update()
    {
        if (!player) return;
        hpText.text = "HP: " + player.maxHP; // (or expose CurrentHP in PlayerHealth)
        gemText.text = "Gems: " + Gem.Count;
    }
}
