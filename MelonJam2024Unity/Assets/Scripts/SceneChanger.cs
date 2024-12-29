using UnityEngine;
using UnityEngine.SceneManagement; 

[DefaultExecutionOrder(-9000)]
public class UpgradeUiManager : MonoBehaviour
{
    public static UpgradeUiManager Instance; 

    public Transform UpgradeParent;
    public Transform Details; 
    

    private void Awake()
    {
        Instance = this; 
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("GameScene");
    }
}
