using UnityEngine;

public class ScrapPile : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lootPool; 

    [SerializeField]
    private int MaxHP,HP, scrapCapacity = 2;


    public int ScrapCapacity { get { return scrapCapacity; } set { scrapCapacity = value; } }

    private int scrapspent;

    private void Start()
    {
        HP = MaxHP; 
    }

    public void DealDmg(int dmg)
    {

        int remainingHP = HP - dmg;

        UiManager.DisplayDamageText(dmg.ToString(),transform.position, Color.white);

        if (remainingHP <= GetLootThreshold(scrapspent + 1)) 
        { 
            while (scrapspent < ScrapCapacity  && PlayerController.Player.CanTakeMoreLoot() && remainingHP <= GetLootThreshold(scrapspent + 1))
            {
                HP -= MaxHP / ScrapCapacity; 
                DropLoot();
                scrapspent++;
            }
        }
        else 
        {
            HP -= dmg;
        }
        


        if (HP <= 0)
        {
            Die();
        }


    }

    private void DropLoot()
    {
        GameObject newLoot = Instantiate(lootPool[Random.Range(0, lootPool.Length)]);
        PlayerController.Player.GiveLoot(newLoot.transform);
        Debug.Log("Dropped scrap");
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private int GetLootThreshold(int lootIndex)
    {
        
        return MaxHP - (MaxHP/ ScrapCapacity) * lootIndex;
    }
}
