using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class ScrapTable : MonoBehaviour
{
    [SerializeField]
    Transform[] tables;
    
    [SerializeField]
    public int MaxCapacity = 4;
    
    public List<Bullet> Scrap = new List<Bullet>();

    public static ScrapTable Instance;

    private void Awake()
    {
        Instance = this; 
    }

    public void OnInteractedWith(GameObject caller) 
    {
        if (caller.TryGetComponent(out PlayerController player)) 
        {
            if (MaxCapacity <= Scrap.Count)
            {
                UiManager.DisplayDamageText("Table Full!", transform.position, Color.white);
                return;
            }

            MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.PLACE_SCRAP);
            TryAddScrap(player.TryGetScrap(MaxCapacity - Scrap.Count));
        }

        //TODO: Add bot handeling
        /*
        else if (caller.TryGetComponent(out Robot robot)) 
        { 
        
        }
        */
    }

    public void TryAddScrap(List<Bullet> scrapList) 
    {
        Scrap.AddRange(scrapList);

        foreach (var scrap in scrapList)
        {
            Transform newParent = tables[Random.Range(0, tables.Length)];

            // DisableFollow
            scrap.GetComponent<DelayedFollow>().enabled = false;

            scrap.transform.SetParent(newParent);
            scrap.transform.localPosition = Random.insideUnitSphere * 0.5f;
        }
    }

    public List<Bullet> TryGetScrap(int amount) 
    {
        List<Bullet> returnList = new();

        if (amount < Scrap.Count) 
        {
            returnList = Scrap.GetRange(0,amount);
            Scrap.RemoveRange(0,amount);
        }
        else 
        {
            returnList =  Scrap.GetRange(0, Scrap.Count);
            Scrap.Clear(); 
        }

        return returnList; 
    }
}
