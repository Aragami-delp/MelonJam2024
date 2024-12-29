using UnityEngine;

public class ScrapManager : MonoBehaviour
{
    public static ScrapManager Instance { get; private set; } 
    
    [Header("Bro idk how to name this... describes how many Scrap can exist on the x / y ")]
    [SerializeField]
    Vector2 pileDimensions;

    
    public int scrapSpawnAmount = 1;

    public int scrapCapacityMultiplier = 1;

    [SerializeField]
    private GameObject pilePrefab;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        Instance = null;
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
                pile.MaxHP *= scrapCapacityMultiplier;
                pile.ScrapCapacity *= scrapCapacityMultiplier;
                
                if (scrapSpawned >= scrapSpawnAmount)
                {
                    return;
                }

            }
        }
    }


}
