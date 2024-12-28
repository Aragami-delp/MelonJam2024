using UnityEngine;
using UnityEngine.Events; 

public class InteractableButton: MonoBehaviour
{
    public UnityEvent OnTrigger;

    public void InvokeButton() 
    {
        UiManager.DisplayDamageText("Click!",transform.position + Vector3.up);
        OnTrigger.Invoke();
    }
}
