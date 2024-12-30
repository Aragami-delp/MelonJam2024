using UnityEngine;

public class CarUpgrade : MonoBehaviour
{
    public static CarUpgrade Instance;

    public bool Active;

    public float SecondCooldown = 180f;

    public int ScrapPlaceAmount = 0;

    [SerializeField]
    Vector2 startingPosition;

    private Animator animator;

    [SerializeField]
    private GameObject pilePrefab;

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }


    private void Start()
    {
        if (Active) 
        {
            InvokeRepeating("StartUnload",1f,SecondCooldown);
        }
    }

    public void StartUnload() 
    {
        animator.SetBool("DoAnim",true);
    }

    public void PlaceScrap() 
    {
        animator.SetBool("DoAnim", false);
        int scrapSpawned = 0;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                scrapSpawned++; 

                Vector2 position = new Vector2(startingPosition.x + x * 2, startingPosition.y + y * 2);

                Instantiate(pilePrefab, position, Quaternion.identity);

                if (scrapSpawned >= ScrapPlaceAmount) 
                {
                    return;
                }
            }
        }
    }
}
