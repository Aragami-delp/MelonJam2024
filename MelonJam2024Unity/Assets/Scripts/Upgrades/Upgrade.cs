using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Upgrade
{
    public static Upgrade hp =
        new Upgrade(
            "Hp1",
            "Hp up",
            "More hp",
            "Icon",

            1,
            new string[] { "InitialScrapSize1" , "AutoReload", "Turrets1" },
            true,
            new Vector2(0, 0),
            5,
            (self,isIngame) =>
            {
                self.DefaultPriceIncrease(1.3f);

                if (!isIngame) { return; }
                
            }
        );

    public static Upgrade Turrets1 =
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
    );

    public static Upgrade InitialScrapSize =
    new Upgrade(
        "InitialScrapSize1",
        "More start scrap",
        "start your game with more scrap",
        "Icon",

        10,
        new string[] { "AutoReload" },
        false,
        new Vector2(0, 1),
        10,
        (self, isIngame) =>
        {
            self.DefaultPriceIncrease();

            if (!isIngame) { return; }

        }
    );
    public static Upgrade AutoReload =
    new Upgrade(
        "AutoReload",
        "Auto Reload",
        "Automatically reload your gun",
        "Icon",

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
    );



    public static Upgrade[] Upgrades = new Upgrade[]
    {
        hp,
        InitialScrapSize,
        AutoReload,
        Turrets1
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

    GameObject upgradeButton;
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

    public void ConnectToButton(GameObject gameObject)
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

        for (int i = 0; i < Children.Length; i++)
        {
            Children[i].SetupParent(this);
        }
    }


    public void SetupParent(Upgrade parent)
    {
        parents.Add(parent);
    }

    public void UpgradeBought(bool ingame)
    {
        Level++;

        // add as unlocked or set new level to manager
        UpgradeManager.AddUpgrade(IDName);


        if (!ingame)
        {
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
        upgradeButton.GetComponent<UpgradeButton>().UnlockChildConnectors(upgrade);
    }

    public void DefaultPriceIncrease() 
    { 
        // Exponential Growth
        Cost += (int)(BaseCost * Math.Pow((double)1.2, Level - 1));
    }
    public void DefaultPriceIncrease(float increment)
    {
        // Exponential Growth
        Cost += (int)(BaseCost * Math.Pow(increment, Level - 1));
    }
}
