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
            new string[] { "mgntDamage" , "EnemySlow1", "PlayerSpeed1", "InitialScrapSize1"},
            true,
            new Vector2(0, 0),
            1,
            (self, isIngame) =>
            {
                if (isIngame)
                {
                    return;
                }

                if (!GameManager.Instance.TutorialPlayed)
                {
                    UpgradeUiManager.Instance.StartGame();
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
        //MAGNET -----------------------------------
        new Upgrade(
            "mgntDamage",
            "Magnet Damage",
            "Shoot Scrap faster to deal more damage",
            "upg-mgnt-dmg-1",

            1,
            new string[] { "mgntSpeed1" },
            false,
            new Vector2(0, -1),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(1.3f);

                if (!isIngame) { return; }

                Cannon.Instance.m_cannonDamage++;
            }
        ),
        new Upgrade(
            "mgntSpeed1",
            "Magnet speed",
            "Increase the move speed for the magnet",
            "upg-mgnt-speed-1",

            1,
            new string[] { "mgntScrapCapacity" , "mgntAutoReload"},
            false,
            new Vector2(0, -2),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(1.3f);

                if (!isIngame) { return; }

                Cannon.Instance._moveSpeed++;
            }
        ),
        new Upgrade(
            "mgntScrapCapacity",
            "Bigger Scrap Piles",
            "Shoot Bigger Scrap Piles to do more damage",
            "upg-mgnt-scrap-capa-1",

            1,
            new string[] { },
            false,
            new Vector2(-1, -3),
            5,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                Cannon.Instance.m_maxScrapCapacity = (int) (1.25f * Cannon.Instance.m_maxScrapCapacity);
            }
        ),
        new Upgrade(
            "mgntAutoReload",
            "Auto Reload Magnet",
            "Automatically reloads your magnet",
            "upg-mgnt-reload",

            500,
            new string[] { },
            false,
            new Vector2(1, -3),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                Cannon.Instance.m_autoReload = true;
            }
        ),
        //Enemy -----------------------------------
        new Upgrade(
            "EnemySlow1",
            "Repelling Field",
            "Slowes down all enemys",
            "upg-enemy-speed-1",

            1,
            new string[] { "LandMine" },
            false,
            new Vector2(1, 0),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();
                if (!isIngame) { return; }

                LaneSystem.Instance.m_slowUpgradeMultiplierReduction *= 0.80f;
            }
        ),
        
        new Upgrade(
            "LandMine",
            "Last Resort",
            "Places Land Mines before your base",
            "upg-lane-mine-1",

            30,
            new string[] { },
            false,
            new Vector2(2, 0),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                LaneSystem.Instance.m_landMinesActive = true;
            }
        ),
        
        // Player -----------------------------------
        new Upgrade(
            "PlayerSpeed1",
            "Better shoes",
            "Increase Player Walk Speed",
            "upg-player-speed-1",

            1,
            new string[] { "PlayerCarry1", "PlayerMineSpeed1" },
            false,
            new Vector2(0, 1),
            5,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.Speed = (int) (1.25 * PlayerController.Player.Speed);
            }
        ),
        new Upgrade(
            "PlayerCarry1",
            "Bigger Backpack",
            "Increase the number of scraps the Player can hold",
            "upg-player-capa-1",

            1,
            new string[] {  },
            false,
            new Vector2(1, 1),
            5,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.MaxCarringCapacity = (int) (1.25 * PlayerController.Player.MaxCarringCapacity);
            }
        ),
        new Upgrade(
            "PlayerMineSpeed1",
            "Faster Drill",
            "Increase the number of scraps you can get",
            "upg-player-scrap-mine-speed-1",

            1,
            new string[] { "PlayerMineDamage1" },
            false,
            new Vector2(-1, 1),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.harvestCooldown = (int) (0.9f * PlayerController.Player.harvestCooldown);
            }
        ),
        new Upgrade(
            "PlayerMineDamage1",
            "Stronger Drill",
            "Increase the number of scraps you can get",
            "upg-player-scrap-mine-dmg-1",

            30,
            new string[] { },
            false,
            new Vector2(-1, 2),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.harvestDamage = (int) (1.25 * PlayerController.Player.harvestDamage);
            }
        ),
        // Scrap -----------------------------------
        new Upgrade(
            "InitialScrapSize1",
            "More Scrap Piles",
            "Start your game with more scrap piles",
            "upg-scrap-piles-1",

            1,
            new string[] { "IncreaseScrapPileCapacity1", "IncreaseScrapTableCapacity1" },
            false,
            new Vector2(-1, 0),
            7,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                ScrapManager.Instance.scrapSpawnAmount++;
            }
        ),
        new Upgrade(
            "ScrapDelivery",
            "Scrap Delivery",
            "Gain scrap piles every 180s +1 per level",
            "upg-scrap-piles-1",

            1,
            new string[] { },
            false,
            new Vector2(-3, 0),
            7,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                ScrapManager.Instance.scrapSpawnAmount++;
            }
        ),
        new Upgrade(
            "IncreaseScrapPileCapacity1",
            "Denser Scrap Piles",
            "There are more scrap in one scrap pile",
            "upg-scrap-capacity-1",

            1,
            new string[] { "ScrapDelivery" },
            false,
            new Vector2(-2, 1),
            10,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }
                CarUpgrade.Instance.Active = true; 
                CarUpgrade.Instance.ScrapPlaceAmount++; 

            }
        ),
        new Upgrade(
            "IncreaseScrapTableCapacity1",
            "Better Stacking Skills",
            "You can put more scrap on the table",
            "upg-scrap-table-capacity",

            1,
            new string[] {"ScrapDelivery" },
            false,
            new Vector2(-2, -1),
            10,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                ScrapTable.Instance.MaxCapacity++;
            }
        )
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

    public void ClearParents() 
    {
        parents.Clear();
        activatedParents.Clear();
    }

    public void SetupParent(Upgrade parent)
    {
        if (!parents.Contains(parent)) 
        { 
            parents.Add(parent);
        }
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
