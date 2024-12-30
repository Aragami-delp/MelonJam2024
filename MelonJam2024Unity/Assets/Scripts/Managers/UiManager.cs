using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [SerializeField]
    private Transform WorldCanvas, TextPrefab, TutorialBoxPrefab;

    private int tutorialTextProgress;
    GameObject lastTutorialBoy; 
    private void Start()
    {
        if (!GameManager.Instance.TutorialPlayed) 
        {
            GameManager.Instance.TutorialPlayed = true;
            PlayerInput inputs = PlayerController.Player.GetComponent<PlayerInput>();
            inputs.SwitchCurrentActionMap("Player");
            inputs.actions["Interact"].started += ForwardTutorial;
            
            LaneSystem.Instance._spawnEnemies = false;

            SpawnTutorialBox("Press E for next tutorial / interacting", new Vector2(-4.5f,7));
        }

        if(Instance != null) 
        {
            return; 
        }

        Instance = this; 
    }

    public static void DisplayDamageText(string text, Vector3 targetpos,Color color) 
    {
        Transform newText =  Instantiate(Instance.TextPrefab);
        newText.SetParent(Instance.WorldCanvas,false);
        
        TMP_Text textComponent = newText.GetComponent<TMP_Text>();
        textComponent.text = text;
        textComponent.color = color; 

        newText.position = targetpos + Vector3.up * Random.Range(-0.5f, 0.5f) + Vector3.back;

        newText.rotation = Quaternion.Euler(0,0,Random.Range(-50f, 50f)); 

        Destroy(newText.gameObject, 1f);
    }

    public static void DisplayDamageText(string text, Vector3 targetpos) 
    {
        DisplayDamageText(text, targetpos,Color.black); 
    }



    public void ForwardTutorial(InputAction.CallbackContext context) 
    {
        tutorialTextProgress++;
        Destroy(lastTutorialBoy);
        switch (tutorialTextProgress) 
        {
            case 1:
                SpawnTutorialBox("w a s d to move", new Vector2(-4.5f, 7));
                break;

            case 2:
                SpawnTutorialBox("Get Scrap by holding E", new Vector2(0.7f, 9.25f));
                break;

            case 3:
                SpawnTutorialBox("Pressing E here to drop all scrap ", new Vector2(0.7f, 9.25f));
                break;

            case 4:
                SpawnTutorialBox("use these buttons to controll the magnet", new Vector2(0.7f, 9.25f));
                break;

            case 5:
                SpawnTutorialBox("Change the lane and fire the magnet to defend yourself", new Vector2(-4f, -3f));
                break;
            case 6:
                SpawnTutorialBox("you are expected to die, learn from it", new Vector2(-4f, -3f));
                LaneSystem.Instance._spawnEnemies = true;
                break;
        }
    }

    public void SpawnTutorialBox(string text, Vector2 pos) 
    {
        lastTutorialBoy = Instantiate(TutorialBoxPrefab).gameObject;

        lastTutorialBoy.transform.SetParent(WorldCanvas, false);
        TMP_Text textComponent = lastTutorialBoy.GetComponentInChildren<TMP_Text>();
        textComponent.text = text;
        lastTutorialBoy.transform.position = pos;
    }
}
