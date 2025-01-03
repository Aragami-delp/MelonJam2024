using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField]
    private bool beginningUpgrade = false; 

    private Upgrade upgrade;
    public string IDName { get { return upgrade.IDName; } }
    public string DisplayName { get { return upgrade.Name; } }
    public string Desc { get { return upgrade.Desc; } }
    public string LevelString { get { return "LVL " + upgrade.Level + " / " + upgrade.MaxLevel; } }
    public string CostString { get { return upgrade.Cost.ToString(); } }
    public int Level { get { return upgrade.Level; } }

    public void SetUpgrade(Upgrade upgrade)
    {
        this.upgrade = upgrade;
    }

    public void OnMouseHoverEnter()
    {
        UpgradeManager.SetDetailList(this);
    }

    public void OnmouseHoverExit()
    {
        UpgradeManager.RemoveDetailList();
    }
    private void Awake()
    {
        if (beginningUpgrade)
        {
            SetUpgrade(Upgrade.BeginningUpgrade);
            SetIcon(upgrade.Icon);
            upgrade.ConnectToButton(this, false);
            InitConnections();
        }
    }

    public void InitConnections()
    {
        GetComponent<Button>().enabled = upgrade.Unlocked;
        
        for (int i = 0; i < upgrade.Children.Length; i++)
        {
            GameObject childGO = new GameObject(upgrade.Children[i].IDName);
            childGO.transform.SetParent(transform);
            childGO.transform.localPosition = Vector3.zero;
            //childGO.SetActive(upgrade.Children[i].Unlocked);

            LineRenderer renderer = childGO.AddComponent<LineRenderer>();

            renderer.sortingOrder = -1;
            renderer.useWorldSpace = true;
            renderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.2f));
            renderer.positionCount = 2;
            renderer.material = new Material(Shader.Find("Sprites/Default"));

            Gradient blackGradient = new Gradient();
            blackGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.gray, 0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f) }
            );

            renderer.colorGradient = blackGradient;

            Vector3 targetPosition = (upgrade.Children[i].Pos * 4f);
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 startPosition = transform.position + direction * 0.9f;
            Vector3 endPosition = targetPosition - direction * 0.9f;
            renderer.SetPosition(0, startPosition);
            renderer.SetPosition(1, endPosition);

        }

        if (beginningUpgrade && Upgrade.BeginningUpgrade.Level == 0) 
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void BuyUpgrade() 
    {
        MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.CLICK);
        if (upgrade.Level < upgrade.MaxLevel) 
        { 
            OnUpgradeBought(true);
        }
    }

    public void OnUpgradeBought(bool UiBought)
    {
        // spent Resources
        if (GameManager.TryPay(upgrade.Cost) && upgrade.Level >= upgrade.MaxLevel)
        {
            return;
        }

        upgrade.UpgradeBought(false, true);

        if (UiBought)
        {
            UpgradeManager.SetDetailList(this);
        }

    }

    public void UnlockChildConnectors(Upgrade connection)
    { 

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.name == connection.IDName)
            {

                Gradient blackGradient = new Gradient();
                blackGradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.white, 0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f) }
                );

                child.gameObject.GetComponent<LineRenderer>().colorGradient = blackGradient;
                return;
            }
        }
    }

    public void SetIcon(string name) 
    {
        Sprite icon = Resources.Load<Sprite>(name);

        if (icon != null)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = icon;
           
        }
        else
        {
            Debug.LogWarning($"Icon '{name}' not found in Resources");
        }
    }

    public void SetImageUnlocked() 
    {
        Image[] images = transform.GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            image.color = Color.white;
        }
    }
}
