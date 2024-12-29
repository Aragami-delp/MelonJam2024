using System.Collections;
using UnityEngine;
using UnityEngine.Events; 

public class InteractableButton: MonoBehaviour
{
    [SerializeField]
    private bool printClickText = true;

    private SpriteRenderer _renderer;
    private Sprite _defaultSprite;
    [SerializeField] private Sprite _pressedSprite;

    [SerializeField] private float _pressedAnimationTime = 0.5f;

    public UnityEvent<GameObject> OnTrigger;

    public void InvokeButton(GameObject caller) 
    {
        if(printClickText)  UiManager.DisplayDamageText("Click!",transform.position + Vector3.up);

        OnTrigger.Invoke(caller);

        if (_renderer) { StartCoroutine(CoPressedAnimation(_pressedAnimationTime)); }
    }

    private void Start()
    {
        if (TryGetComponent(out SpriteRenderer renderer)) { _renderer = renderer; }
        else { return; }
        _defaultSprite = _renderer.sprite;
    }

    IEnumerator CoPressedAnimation(float seconds)
    {
        _renderer.sprite = _pressedSprite;
        yield return new WaitForSeconds(seconds);
        _renderer.sprite = _defaultSprite;
    }
}
