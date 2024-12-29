using UnityEngine;

public class ScrapPile : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lootPool; 

    [SerializeField]
    public int MaxHP,HP, ScrapCapacity = 2;

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
        PlayerController.Player.GiveLoot(newLoot.GetComponent<Bullet>());
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
