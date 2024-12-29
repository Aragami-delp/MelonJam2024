using UnityEngine;
using UnityEngine.Events; 

public class InteractableButton: MonoBehaviour
{
    [SerializeField]
    private bool printClickText = true;
    
    public UnityEvent<GameObject> OnTrigger;

    public void InvokeButton(GameObject caller) 
    {
        if(printClickText)  UiManager.DisplayDamageText("Click!",transform.position + Vector3.up);

        OnTrigger.Invoke(caller);
    }
}
