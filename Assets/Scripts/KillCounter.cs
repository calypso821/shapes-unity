using System;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    // UI element, ki bo prikazoval kill count
    public Text killCountText;

    // Trenutni kill count
    private int killCount = 0;

    // Metoda za dodajanje kill-a

    public void AddKill()
    {
        killCount++;
        UpdateKillCountText();
    }

    // Posodobi UI tekst
    private void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + killCount.ToString();
        }
    }
    public Text getKillCountText(){
        return killCountText;
    }
}
