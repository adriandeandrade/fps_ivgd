using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    LevelFader levelFader;

    private void Awake()
    {
        levelFader = FindObjectOfType<LevelFader>();
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic("Title");
    }

    public void Play()
    {
        // Transition to first scene
        levelFader.FadeToLevel(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
