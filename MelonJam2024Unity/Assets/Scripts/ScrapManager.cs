using UnityEngine;

public class ScrapManager : MonoBehaviour
{
    [Header("Bro idk how to name this... describes how many Scrap can exist on the x / y ")]
    [SerializeField]
    Vector2 pileDimensions;

    [SerializeField]
    private int scrapSpawnAmount; 

    public static int ScrapSpawnAmount = 1;

    [SerializeField]
    private GameObject pilePrefab;

    private void Awake()
    {
        ScrapSpawnAmount = scrapSpawnAmount; 
    }

    private void Start()
    {
        int scrapSpawned = 0;

        // offsets need to be x -2 or y -2 

        for (int y = 0; y < pileDimensions.y; y++)
        {
            for (int x = 0; x < pileDimensions.x; x++)
            {
                scrapSpawned++;

                GameObject newPile = Instantiate(pilePrefab, transform);

                newPile.transform.localPosition = new Vector2(x * 2, y * -2 ); 

                ScrapPile pile = newPile.GetComponent<ScrapPile>();

                if (scrapSpawned >= ScrapSpawnAmount)
                {
                    return;
                }

            }
        }
    }


}
