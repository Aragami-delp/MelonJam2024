using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Collections;

public class ScrapTable : MonoBehaviour
{
    [SerializeField]
    Transform[] tables;

    [SerializeField]
    public int MaxCapacity = 4;

    public List<Bullet> Scrap = new List<Bullet>();

    public static ScrapTable Instance;

    #region ScrapGenerator

    [SerializeField] private SwitchSprite _generatorSprite;
    [SerializeField] private Bullet _toGenerateScrap;
    [ReadOnly, SerializeField] private float _timeToGenerate = 20f;
    [ReadOnly, SerializeField] private int _amountToGenerate = 0;
    public float m_timeToGenerate
    {
        get { return _timeToGenerate; }
        set
        {
            _timeToGenerate = Mathf.Clamp(value, .5f, 120f);
        }
    }

    public int m_amountToGenerate
    {
        get { return _amountToGenerate; }
        set
        {
            _amountToGenerate = Mathf.Max(value, 0);
            _generatorSprite?.Switch(_amountToGenerate > 0);
        }
    }

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(AddNewScrapRepeatedly(m_timeToGenerate));
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

    private IEnumerator AddNewScrapRepeatedly(float interval)
    {
        while (true) // until scene change
        {
            yield return new WaitForSeconds(interval);
            TryAddNewScrap(m_amountToGenerate);
        }
    }

    public void TryAddNewScrap(int amount)
    {
        List<Bullet> newScrap = new();
        Mathf.Clamp(amount, 0, MaxCapacity - Scrap.Count);
        for (int i = 0; i < amount; i++)
        {
            newScrap.Add(Instantiate(_toGenerateScrap));
        }
        if (newScrap.Count > 0)
            TryAddScrap(newScrap);
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
            returnList = Scrap.GetRange(0, amount);
            Scrap.RemoveRange(0, amount);
        }
        else
        {
            returnList =  Scrap.GetRange(0, Scrap.Count);
            Scrap.Clear();
        }

        return returnList;
    }
}
