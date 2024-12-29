using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq; 

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField]
    GameObject detailsTransform;

    [SerializeField]
    GameObject upgradeButtonPrefab;


    public static List<UpgradeNameValuePair> BoughtUpgraders;

    

    public void Awake()
    {
        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        BoughtUpgraders = new List<UpgradeNameValuePair>();

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += ChangedScene;
    }


    private void ChangedScene(Scene scene, LoadSceneMode mode)
    {
        UnloadUpgrades();
        LoadUpgrades(scene.name != "UpgradeScene");
        Debug.Log("Loaded Upgrades");
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
        if (!ingame) 
        {
            Transform upgradeParent = UpgradeUiManager.Instance.UpgradeParent; 
            

            Instance.detailsTransform = UpgradeUiManager.Instance.Details.gameObject; 

            for (int i = 0; i < Upgrade.Upgrades.Length; i++)
            {
                GameObject newButton = Instantiate(Instance.upgradeButtonPrefab, Upgrade.Upgrades[i].Pos * 4f, Quaternion.identity);
                newButton.transform.SetParent(upgradeParent);
                newButton.transform.localScale = Vector3.one;

                UpgradeButton upgButton = newButton.GetComponent<UpgradeButton>();

                Upgrade.Upgrades[i].ConnectToButton(upgButton);

                upgButton.SetUpgrade(Upgrade.Upgrades[i]);
                upgButton.SetIcon(Upgrade.Upgrades[i].Icon);

            }
        }

        foreach (var upgrade in BoughtUpgraders.ToList())
        {
            if (upgrade.Name == Upgrade.BeginningUpgrade.IDName) 
            {
                Upgrade.BeginningUpgrade.UpgradeBought(ingame);
                continue;
            }

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

        foreach (var upgrade in BoughtUpgraders.ToList())
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

    public static void AddUpgrade(string name, int increment = 1)
    { 

        for (int i = 0; i < BoughtUpgraders.Count; i++)
        {
            if(BoughtUpgraders[i].Name == name) 
            {
                UpgradeNameValuePair pair = new UpgradeNameValuePair(name , BoughtUpgraders[i].Value + increment);
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