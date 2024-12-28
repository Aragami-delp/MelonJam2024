using UnityEngine;
using System.Collections.Generic; 

public class ScrapTable : MonoBehaviour
{
    [SerializeField]
    int maxCapacity = 4;
    
    List<Transform> scrap = new List<Transform>(); 
    
    public void TryAddScrap(List<Transform> scrapList) 
    {
        if (maxCapacity <= scrap.Count) 
        {
            UiManager.DisplayDamageText("Table Full!",transform.position);
            return; 
        }
        
        int endIndex = 0; 
        for (int i = 0; i < scrapList.Count; i++)
        {
            scrap.Add(scrapList[i]); 

            if (maxCapacity <= scrap.Count) 
            {
                endIndex = i; 
                break; 
            }
        }

        scrapList.RemoveRange(0,endIndex);
    }
}
