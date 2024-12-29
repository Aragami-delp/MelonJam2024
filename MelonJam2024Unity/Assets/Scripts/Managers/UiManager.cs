using UnityEngine;
using TMPro; 

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [SerializeField]
    private Transform WorldCanvas, TextPrefab; 
    

    private void Start()
    {
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
}
