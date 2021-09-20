using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }

    [Header("Tracker")]
    [HideInInspector] public bool treasureStolen;
    [HideInInspector] public bool thiefCaught;
    [HideInInspector] public bool thiefHidden;

    [Header("UI Info")]
    public TextMeshProUGUI stolenText;
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void CommenceSteal()
    {
        stolenText.text = "No";
        treasureStolen = false;
        thiefHidden = false;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }
}
