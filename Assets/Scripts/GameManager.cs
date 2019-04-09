using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject gameOverPanel;
    public Transform playerT;
    public bool beingDetected;
    public bool detected;

    bool gameOver = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
       
    }

    private void Update()
    {

    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        gameOver = true;
    }
}
