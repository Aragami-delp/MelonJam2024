using UnityEngine;
using UnityEngine.SceneManagement; 

[DefaultExecutionOrder(-9000)]
public class UpgradeUiManager : MonoBehaviour
{
    public static UpgradeUiManager Instance; 

    public Transform UpgradeParent;
    public Transform Details;
    public GameObject PlayGameButton; 
    

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

    public void StartGame() 
    {
        SceneManager.LoadScene("GameScene");
    }
}
