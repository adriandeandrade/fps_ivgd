using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPanel : MonoBehaviour
{
    LevelFader levelFader;

    private void Awake()
    {
        levelFader = FindObjectOfType<LevelFader>();
    }

    public void GoToLastLevel()
    {
        levelFader.FadeToLevel(2);
    }
}
