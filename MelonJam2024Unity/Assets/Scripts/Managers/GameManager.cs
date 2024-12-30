using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool TutorialPlayed; 

    public enum GAMESCENE
    {
        UPGRADE,
        INGAME
    }
    public static GameManager Instance { get; private set; }

    public int Coins;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public static bool TryPay(int cost) 
    {
        if (Instance.Coins >= cost) 
        {
            Instance.Coins -= cost;
            return true; 
        }

        return false; 
    }

    public static void LoadScene(GAMESCENE scene)
    {
        SceneManager.LoadScene(scene == GAMESCENE.UPGRADE ? 0 : 1);
    }
}
