using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Upgrade
{
    public static Upgrade BeginningUpgrade = 
        new Upgrade(
            "NewBeginning",
            "New Beginning",
            "Lets build a Cannon",
            "Icon",

            0,
            new string[] { "InitialScrapSize1", "Turrets1", "mgntSpeed", "EnemySlow1", "LandMine1" },
            true,
            new Vector2(0, 0),
            1,
            (self, isIngame) =>
            {
                if (isIngame)
                {
                    return;
                }
                
                UpgradeUiManager.Instance.UpgradeParent.gameObject.SetActive(true);

                for (int i = 0; i < self.upgradeButton.transform.childCount; i++)
                {
                    self.upgradeButton.transform.GetChild(i).gameObject.SetActive(true);
                }

            }
        );

    public static Upgrade[] Upgrades = new Upgrade[]
    {
        new Upgrade(
            "EnemySlow1",
            "Repelling Field",
            "Slowes down all enemys",
            "upg-enemy-speed-1",

            30,
            new string[] { },
            false,
            new Vector2(2, 2),
            5,
                (self, isIngame) =>
                {
                    self.DefaultPriceIncrease();
                    if (!isIngame) { return; }
                    //Enemy.m_speedMultiplyer -= 0.05f; 
                }
        ),

        new Upgrade(
            "AutoReload",
            "Auto Reload",
            "Automatically reload your gun",
            "upg-mgnt-reload",

            500,
            new string[] { },
            false,
            new Vector2(1, 2),
            1,
                (self, isIngame) =>
                {
                    if (!isIngame) { return; }
                    Cannon.Instance.m_autoReload = true;
                }
        ),

        new Upgrade(
            "InitialScrapSize1",
            "More start scrap",
            "start your game with more scrap",
            "upg-scrap-piles-1",

            10,
            new string[] { "AutoReload" },
            false,
            new Vector2(0, 1),
            7,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                ScrapManager.ScrapSpawnAmount++;
            }
        ),

        new Upgrade(
            "Turrets1",
            "Auto Turrets",
            "Add a auto turret to a lane",
            "Icon",

            1,
            new string[] { },
            false,
            new Vector2(-1, 0),
            5,
            (self, isIngame) =>
            {

                if (!isIngame)
                {
                    self.DefaultPriceIncrease(1.5f);
                    return;
                }

            }
        ),
        new Upgrade(
            "mgntSpeed",
            "Magnet speed ",
            "More speed more fun",
            "upg-mgnt-speed-1",

            1,
            new string[] {},
            false,
            new Vector2(4, 0),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(1.3f);

                if (!isIngame) { return; }

                Cannon.Instance._moveSpeed++;
            }
        ),
        new Upgrade(
            "LandMine1",
            "Last Resort",
            "Desc",
            "upg-lane-mine-1",

            30,
            new string[] { },
            true,
            new Vector2(0, -1),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                LaneSystem.Instance.m_landMinesActive = true;
            }
        ),
    };

    public string IDName;
    public string Name;
    public string Desc;
    public string Icon;
    public bool Unlocked;
    public int Cost;
    public int BaseCost;
    public Vector2 Pos;
    public int Level;
    public int MaxLevel;
    public readonly bool InstantUnlocked;
    private string[] promissedChildren;
    public Upgrade[] Children { get; set; }
    List<Upgrade> parents = new List<Upgrade>();
    List<Upgrade> activatedParents = new List<Upgrade>();
    Action<Upgrade,bool> Apply;

    UpgradeButton upgradeButton;
    public Upgrade(string idName, string name, string desc, string icon, int cost, string[] children, bool instantUnlock, Vector2 pos, int maxLevel, Action<Upgrade,bool> apply)
    {
        promissedChildren = children;
        Children = new Upgrade[children.Length];
        Unlocked = instantUnlock;
        InstantUnlocked = instantUnlock;
        this.IDName = idName;
        this.Name = name;
        this.Desc = desc;
        this.Icon = icon;
        this.Pos = pos;
        this.MaxLevel = maxLevel;
        this.BaseCost = cost;
        this.Cost = BaseCost;
        Apply = apply;
    }

    public void ConnectToButton(UpgradeButton gameObject, bool callChildren = true)
    {
        upgradeButton = gameObject;

        // init children 
        for (int i = 0; i < promissedChildren.Length; i++)
        {
            for (int upgradeIndex = 0; upgradeIndex < Upgrades.Length; upgradeIndex++)
            {
                if (Upgrades[upgradeIndex].IDName == promissedChildren[i])
                {
                    Children[i] = Upgrades[upgradeIndex];
                }
            }
        }
        
        if (!callChildren) 
        {
            return;
        }

        for (int i = 0; i < Children.Length; i++)
        {
            Children[i].SetupParent(this);
        }
    }


    public void SetupParent(Upgrade parent)
    {
        parents.Add(parent);
    }

    public void UpgradeBought(bool ingame, bool buttonBought = false)
    {
        Level++;

        // add as unlocked or set new level to manager

        if (buttonBought) 
        { 
            UpgradeManager.AddUpgrade(IDName);
        }

        if (!ingame)
        {
            
            upgradeButton.SetImageUnlocked();
            ResolveUnlockes();
        }

        Apply.Invoke(this,ingame);
    }

    public void ResolveUnlockes()
    {
        for (int i = 0; i < Children.Length; i++)
        {
            Children[i].ParentUnlocked(this);
        }
    }

    public void ParentUnlocked(Upgrade parent)
    {
        
        parents.Remove(parent);

        if (!activatedParents.Contains(parent))
        {
            activatedParents.Add(parent);
        }

        if (parents.Count <= 0 && upgradeButton != null)
        {   

            Unlocked = true;
            upgradeButton.GetComponent<Button>().enabled = true;

            for (int i = 0; i < activatedParents.Count; i++)
            {
                activatedParents[i].ActivateConnection(this);
            }
        }
    }

    public void ActivateConnection(Upgrade upgrade)
    {
        upgradeButton.UnlockChildConnectors(upgrade);
    }

    public void DefaultPriceIncrease(float increment = 1.2f)
    {
        // Exponential Growth
        Cost += (int)(BaseCost * Math.Pow(increment, Level - 1));
    }
}
