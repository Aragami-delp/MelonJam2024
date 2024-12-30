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
            "upg-mgnt-build",

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

            5,
            new string[] { "mgntSpeed1" },
            false,
            new Vector2(1, -1),
            20,
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

            5,
            new string[] { "mgntScrapCapacity" , "mgntAutoReload"},
            false,
            new Vector2(1, -2),
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
            new Vector2(1, -3),
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
            new string[] { "AutoHarvest" },
            false,
            new Vector2(2, -1),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                Cannon.Instance.m_autoReload = true;
            }
        ),
        new Upgrade(
            "AutoHarvest",
            "smaller magnets",
            "small magnets that Automatically harvest scrap",
            "upg-scrap-auto",

            800,
            new string[] { "AutoHarvestAmount", "AutoHarvestTime"},
            false,
            new Vector2(2, -2),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                ScrapTable.Instance.m_amountToGenerate = 1; 
            }
        ),
        new Upgrade(
            "AutoHarvestTime",
            "faster filtering",
            "better filtering for the small magnets result in faster scrap harvests ",
            "upg-scrap-auto-time",

            200,
            new string[] { },
            false,
            new Vector2(3, -2),
            10,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();
                if (!isIngame) { return; }
                ScrapTable.Instance.m_timeToGenerate --;
            }
        ),

        new Upgrade(
            "AutoHarvestAmount",
            "small but stronger ",
            "the small magnets get stronger and harvest more",
            "upg-scrap-auto-amount",

            300,
            new string[] { },
            false,
            new Vector2(2, -3),
            4,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();
                if (!isIngame) { return; }
                ScrapTable.Instance.m_amountToGenerate += 2;
            }
        ),


        //Enemy -----------------------------------
        new Upgrade(
            "EnemySlow1",
            "Repelling Field",
            "Slowes down all enemys",
            "upg-enemy-speed-1",

            15,
            new string[] { "LandMine" , "EnemyXP1"},
            false,
            new Vector2(2, 1),
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
            new string[] {"EnemyDmg" },
            false,
            new Vector2(3, 1),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                LaneSystem.Instance.m_landMinesActive = true;
            }
        ),
        new Upgrade(
            "EnemyDmg",
            "Field studies",
            "after all the time you know how to hurt them",
            "upg-enemy-health-1",

            30,
            new string[] { },
            false,
            new Vector2(4, 1),
            10,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(1.4f);
                if (!isIngame) { return; }
                Cannon.Instance.m_cannonDamage++;
            }
        ),
        new Upgrade(
            "EnemyXP1",
            "Kill counter",
            "you count more precisely maybe you missed a kill or two",
            "upg-enemy-money-1",

            20,
            new string[] {"EnemyXP2" },
            false,
            new Vector2(2, 2),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();
                if (!isIngame) { return; }
                GameManager.Instance.Multiplyer  *= 1.25f;  
            }
        ),
        new Upgrade(
            "EnemyXP2",
            "Kill Computer",
            "you need it for all your kills",
            "upg-enemy-money-2",

            50,
            new string[] { "EnemyDmg","EnemyXP3" },
            false,
            new Vector2(3, 2),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(1.5f);
                if (!isIngame) { return; }
                GameManager.Instance.Multiplyer  *= 1.35f;
            }
        ),
        new Upgrade(
            "EnemyXP3",
            "Data center",
            "it only calculates your income",
            "upg-enemy-money-3",

            100,
            new string[] {},
            false,
            new Vector2(3, 3),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(2f);
                if (!isIngame) { return; }
                GameManager.Instance.Multiplyer  *= 1.5f;
            }
        ),
        
        // Player -----------------------------------
        new Upgrade(
            "PlayerSpeed1",
            "Better shoes",
            "Increase Player Walk Speed",
            "upg-player-speed-1",

            20,
            new string[] { "PlayerCarry1", "PlayerMineSpeed1" },
            false,
            new Vector2(-1, 1),
            2,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.Speed = (int) (1.15 * PlayerController.Player.Speed);
            }
        ),
        new Upgrade(
            "PlayerCarry1",
            "Bigger Backpack",
            "Increase the number of scraps the Player can hold",
            "upg-player-capa-1",

            10,
            new string[] { "PlayerSpeed2" },
            false,
            new Vector2(-1, 2),
            5,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.MaxCarringCapacity = (int) (1.25 * PlayerController.Player.MaxCarringCapacity);
            }
        ),
        new Upgrade(
            "PlayerSpeed2",
            "Cardio training",
            "pump up the numbers",
            "upg-player-speed-2",

            15,
            new string[] {  },
            false,
            new Vector2(0, 2),
            5,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease(1.5f);
                if (!isIngame) { return; }
                PlayerController.Player.Speed = (int) (1.25 * PlayerController.Player.Speed);
            }
        ),

        new Upgrade(
            "PlayerMineSpeed1",
            "Faster Drill",
            "nothing mutch but its yours and yours alone",
            "upg-player-scrap-mine-speed-1",

            10,
            new string[] { "PlayerMineDamage1" , "PlayerMineSpeed2"},
            false,
            new Vector2(-2, 1),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.harvestCooldown = (int) (0.9f * PlayerController.Player.harvestCooldown);
            }
        ),
        new Upgrade(
            "PlayerMineSpeed2",
            "Even Faster Drill",
            "wow that thing is fast",
            "upg-player-scrap-mine-speed-2",

            100,
            new string[] {"PlayerMineSpeed3"},
            false,
            new Vector2(-3, 1),
            2,
            (self, isIngame) =>
            {
                if(self.Level >= 1 )
                {
                    self.Desc = "wow that thing is fast ... I can dual wield this";
                }

                if (!isIngame) { return; }
                PlayerController.Player.harvestCooldown = (int) (0.9f * PlayerController.Player.harvestCooldown);
            }
        ),
        new Upgrade(
            "PlayerMineSpeed3",
            "Fastest Drill",
            "its the best around",
            "upg-player-scrap-mine-speed-3",

            500,
            new string[] { },
            false,
            new Vector2(-4, 1),
            2,
            (self, isIngame) =>
            {
                if(self.Level >= 1 )
                {
                    self.Desc = "its the best around .... you can even buy 2"; 
                }
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
            new string[] { "PlayerMineDamage2"},
            false,
            new Vector2(-2, 2),
            1,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.harvestDamage = (int) (1.25 * PlayerController.Player.harvestDamage);
            }
        ),
        new Upgrade(
            "PlayerMineDamage2",
            "Even Stronger Drill",
            "thats a hard Drill",
            "upg-player-scrap-mine-dmg-2",

            50,
            new string[] {"PlayerMineDamage3" },
            false,
            new Vector2(-3, 2),
            5,
            (self, isIngame) =>
            {
                if (!isIngame) { return; }
                PlayerController.Player.harvestDamage = (int) (1.25 * PlayerController.Player.harvestDamage);
            }
        ),
        new Upgrade(
            "PlayerMineDamage3",
            "Stronger Drill",
            "you can hurt someone with that",
            "upg-player-scrap-mine-dmg-3",

            500,
            new string[] { },
            false,
            new Vector2(-4, 2),
            10,
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
            new Vector2(-1, -1),
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
            "Hier a driver that arrives every 180s",
            "upg-scrap-car",

            100,
            new string[] {"carTime","carCap" },
            false,
            new Vector2(-2, -2),
            1,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                CarUpgrade.Instance.Active = true;
            }
        ),
        new Upgrade(
            "IncreaseScrapPileCapacity1",
            "Denser Scrap Piles",
            "There are more scrap in one scrap pile",
            "upg-scrap-capacity-1",

            5,
            new string[] { "ScrapDelivery" },
            false,
            new Vector2(-2, -1),
            10,
            (self, isIngame) =>
            {
                if(self.Level == 1)
                {
                    self.Cost = 20;
                }

                self.DefaultPriceIncrease();

                if (!isIngame) { return; }
                CarUpgrade.Instance.ScrapPlaceAmount++; 

            }
        ),
        new Upgrade(
            "IncreaseScrapTableCapacity1",
            "Better Stacking Skills",
            "You can put more scrap on the table",
            "upg-scrap-table-capacity",

            15,
            new string[] {"ScrapDelivery" },
            false,
            new Vector2(-1, -2),
            10,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                ScrapTable.Instance.MaxCapacity++;
            }
        ),
        new Upgrade(
            "carTime",
            "tight schedule",
            "your car arrives faster -30s",
            "upg-scrap-car-2",

            60,
            new string[] {},
            false,
            new Vector2(-3, -2),
            10,
            (self, isIngame) =>
            {
                self.DefaultPriceIncrease();

                if (!isIngame) { return; }

                CarUpgrade.Instance.SecondCooldown -= 30f; 
            }
        ),
        new Upgrade(
            "carCap",
            "Bigger car",
            "The bigger the car the better",
            "upg-scrap-car-1",

            50,
            new string[] {},
            false,
            new Vector2(-2, -3),
            4,

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
