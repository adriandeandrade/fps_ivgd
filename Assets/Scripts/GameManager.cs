using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject gameOverPanel;
    public Transform playerT;
    public bool beingDetected;
    public bool detected;

    bool gameOver = false;

    LevelFader levelFader;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

        levelFader = FindObjectOfType<LevelFader>();
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {

    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        InputManager.instance.HandleMovement = false;
        gameOver = true;
    }

    public void Retry()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        levelFader.FadeToLevel(currentScene);
        Debug.Log("RETRIED");
        //SceneManager.LoadScene(currentScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
