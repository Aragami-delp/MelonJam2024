using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField]
    GameObject detailsTransform;

    [SerializeField]
    GameObject upgradeButtonPrefab;

    [SerializeField]
    Transform UiCanvas;

    public static List<UpgradeNameValuePair> BoughtUpgraders = new List<UpgradeNameValuePair>();

    

    public void Awake()
    {
        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < Upgrade.Upgrades.Length; i++)
        {
            GameObject newButton = Instantiate(upgradeButtonPrefab, Upgrade.Upgrades[i].Pos * 4f, Quaternion.identity);
            newButton.transform.SetParent(UiCanvas);
            newButton.transform.localScale = Vector3.one;
            Upgrade.Upgrades[i].ConnectToButton(newButton);
            newButton.GetComponent<UpgradeButton>().SetUpgrade(Upgrade.Upgrades[i]);
            
            Sprite icon = Resources.Load<Sprite>(Upgrade.Upgrades[i].Icon);
            
            if (icon != null) 
            {
                newButton.GetComponent<Image>().sprite = icon;
            }
            else 
            {
                Debug.LogWarning($"Icon '{Upgrade.Upgrades[i].Icon}' not found in Resources");
            }
            
        }

        SceneManager.sceneLoaded += ChangedScene;
    }

    private void ChangedScene(Scene scene, LoadSceneMode mode)
    {
        UnloadUpgrades();
        LoadUpgrades(scene.name != "UpgradeScene");

    }


    
    public static void SetDetailList(UpgradeButton button) 
    {

        Instance.detailsTransform.transform.SetAsLastSibling();

        Instance.detailsTransform.SetActive(true);

        Instance.detailsTransform.transform.position = button.transform.position + Vector3.up * 4f + Vector3.back;

        TMPro.TMP_Text[] texts = Instance.detailsTransform.GetComponentsInChildren<TMPro.TMP_Text>();

        texts[0].text = button.DisplayName;
        texts[1].text = button.Desc;
        texts[2].text = button.LevelString;
        texts[3].text = button.CostString;
    }

    public static void RemoveDetailList()
    {
        Instance.detailsTransform.SetActive(false);
    }


    public static void LoadUpgrades(bool ingame)
    {
        foreach (var upgrade in BoughtUpgraders)
        {
            for (int i = 0; i < Upgrade.Upgrades.Length; i++)
            {
                if (Upgrade.Upgrades[i].IDName == upgrade.Name)
                {
                    for (int levelLoop = 0; levelLoop < upgrade.Value; levelLoop++)
                    {
                        Upgrade.Upgrades[i].UpgradeBought(ingame);
                    }
                }
            }
        }
    }

    public static void UnloadUpgrades()
    {
        foreach (var upgrade in BoughtUpgraders)
        {
            for (int i = 0; i < Upgrade.Upgrades.Length; i++)
            {
                if (Upgrade.Upgrades[i].IDName == upgrade.Name)
                {
                    Upgrade.Upgrades[i].Level = 0;
                    Upgrade.Upgrades[i].Unlocked = Upgrade.Upgrades[i].InstantUnlocked;
                }
            }
        }
    }

    public static void AddUpgrade(string name) 
    {
        for (int i = 0; i < BoughtUpgraders.Count; i++)
        {
            if(BoughtUpgraders[i].Name == name) 
            {
                UpgradeNameValuePair pair = new UpgradeNameValuePair(name , BoughtUpgraders[i].Value + 1);
                BoughtUpgraders[i] = pair;
                return;
            }
        }

        UpgradeNameValuePair newPair = new UpgradeNameValuePair(name,1);
        
        BoughtUpgraders.Add(newPair);
    }
}


public struct UpgradeNameValuePair 
{
    public string Name;
    public int Value;

    public UpgradeNameValuePair(string name, int value)
    {
        Name = name;
        Value = value;
    }
}