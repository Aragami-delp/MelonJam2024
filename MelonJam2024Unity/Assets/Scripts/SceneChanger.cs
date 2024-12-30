using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 

[DefaultExecutionOrder(-9000)]
public class UpgradeUiManager : MonoBehaviour
{
    public static UpgradeUiManager Instance; 

    public Transform UpgradeParent;
    public Transform Details;
    public GameObject PlayGameButton;
    public TMP_Text Money;
    public TMP_Text Score;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!GameManager.Instance.TutorialPlayed)
        {
            PlayGameButton.SetActive(false);
        }
    }

    private void Update()
    {
        Money.SetText("Coins: " + GameManager.Instance.Coins);
        Score.SetText("Score: " + GameManager.Instance.Score);
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("GameScene");
    }
}
