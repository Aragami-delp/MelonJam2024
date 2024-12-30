using Unity.VisualScripting;
using UnityEngine;

public class SwitchSprite : MonoBehaviour
{
    public SpriteRenderer m_renderer { get; private set; }
    private Sprite _defaultSprite;
    [SerializeField] private Sprite _switchedSprite;

    
    public void Switch(bool switched = true)
    {
        if (m_renderer != null)
            m_renderer.sprite = switched ? _switchedSprite : _defaultSprite;
    }

#if UNITY_EDITOR
    [ContextMenu("Switch On")]
    public void SwitchOn()
    {
        Switch(true);
    }
    [ContextMenu("Switch Off")]
    public void SwitchOff()
    {
        Switch(false);
    }
#endif

    private void Awake()
    {
        if (TryGetComponent(out SpriteRenderer renderer)) { m_renderer = renderer; }
        else { return; }
        _defaultSprite = m_renderer.sprite;
    }
}
