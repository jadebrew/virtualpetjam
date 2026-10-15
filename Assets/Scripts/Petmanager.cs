using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

using static AudioManager.SFXTitle;
using static AudioManager.SongTitle;

public class Petmanager : MonoBehaviour
{
    public GameObject petObject;
    public float hunger;
    public TMP_Text HungerText;
    public float gametimer = 0;
    public float gametick = 60;
    public string datetime;
    public bool happy = true;

    public GameObject bagParent;
    public TMP_Text bagTitle;

    public GameObject mainMenuPanel;
    public GameObject foodMenuPanel;
    public GameObject statMenuPanel;
    public GameObject musicMenuPanel;
    public GameObject radioMenuPanel;
    public GameObject bagMenuPanel;
    public GameObject winPanel;
    public GameObject gachaMenuPanel;

    private float needMin = 0;
    private float needMax = 100;

    public GameObject templateButton;

    public GameObject lastWon;
    Vector3 gachaWinDefaultPos;
    private int lastWonIndex;
    private bool winDone;


    [SerializeField]
    public Dictionary<Item,int> inventory = new Dictionary<Item,int>();
    public Item[] banner;
    public List<Item> lootTable;

    [SerializeField]
    public Dictionary<String,float> needs = new Dictionary<String,float>();

    public Sprite gachaball;

    private AudioManager _audio;

    public int currentPull;

    void Awake()
    {
        _audio = AudioManager.Instance;
        mainMenuPanel.SetActive(true);
        foodMenuPanel.SetActive(false);
        statMenuPanel.SetActive(false);
        musicMenuPanel.SetActive(false);
        radioMenuPanel.SetActive(false);
        gachaMenuPanel.SetActive(false);
        bagMenuPanel.SetActive(false);
        winPanel.SetActive(false);

        foreach(Item item in banner)
        {
            if(!inventory.ContainsKey(item))
            {
                inventory.Add(item,0);
            }
            if(item.isConsumable && !needs.ContainsKey(item.needModifier))
            {
                needs.Add(item.needModifier,UnityEngine.Random.Range(needMax/2,needMax));
            }
            for(int i=0;i<item.rarity.amountInBanner;i++)
            {
                lootTable.Add(item);
            }
        }
        lootTable.Shuffle();
        currentPull = 0;
        gachaWinDefaultPos = lastWon.transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hunger = 80;
    }

    // Update is called once per frame
    void Update()
    {
        datetime = DateTime.Now.ToString();

        string statOuput = "";
        List<string> needsList = new List<string>(needs.Keys);
        foreach(string _need in needsList)
        {
            needs[_need] -= Time.deltaTime;
            if(needs[_need] < needMin)
            {
                needs[_need] = needMin;

            }
            statOuput += Mathf.Floor(needs[_need]) + " " + _need + "\n";
        }

        if(winDone)
        {
            Vector3 newPos = lastWon.transform.position;
            newPos = Vector3.Lerp(newPos, new Vector3(gachaWinDefaultPos.x,gachaWinDefaultPos.y+256,gachaWinDefaultPos.z), Time.deltaTime*2);
            newPos.y = Mathf.Ceil(newPos.y);
            lastWon.transform.position = newPos;
        } else {
             lastWon.transform.position = gachaWinDefaultPos;
        }

        if(currentPull>=lootTable.Count)
        {
            currentPull = 0;
            lootTable.Shuffle();
        }

        HungerText.text = statOuput;
    }

    public void Feed(GameObject consumable)
    {
        var item = consumable.GetComponent<ConsumableHelper>().item;
        if (inventory[item] > 0)
        {
            needs[item.needModifier] += item.needModValue;
            if (needs[item.needModifier]>needMax)
            { needs[item.needModifier] = needMax;}
            happy = true;
            SetPanel(0);
            inventory[item] -= 1;
            _audio.Play(item.onUseSFX);
        }
        else
        {
            _audio.Play(Cancel);
        }

    }

    public void PlaySong(AudioManager.SongTitle title)
    {
        _audio.Play(title);
        SetPanel(0);
    }

    public void Win(int value)
    {
        int win_index = currentPull;
        currentPull++;

        Item wonItem = lootTable[win_index];
        inventory[lootTable[win_index]]++;
        lastWon.GetComponent<Image>().sprite = wonItem.rarity.sprite;
        lastWonIndex = win_index;
        winDone = false;

        _audio.Play(NewItem);
        gachaMenuPanel.SetActive(false);
        winPanel.SetActive(true);
    }

    public void Reveal(int value)
    {
        if (!winDone)
        {
            lastWon.GetComponent<Image>().sprite = lootTable[lastWonIndex].sprite;
            _audio.Play(lootTable[lastWonIndex].rarity.winSFX);
            winDone = true;
        }
        else
        {
            SetPanel(4);
        }

    }

    public void SetPanel(int menuindex)
    {
        mainMenuPanel.SetActive(false);
        foodMenuPanel.SetActive(false);
        statMenuPanel.SetActive(false);
        musicMenuPanel.SetActive(false);
        radioMenuPanel.SetActive(false);
        gachaMenuPanel.SetActive(false);
        bagMenuPanel.SetActive(false);
        winPanel.SetActive(false);

        if (menuindex == -1)
        {
            mainMenuPanel.SetActive(true);
            _audio.StopSound();
            _audio.Play(Cancel);

        }
        if (menuindex == 0)
        {
            mainMenuPanel.SetActive(true);

        }
        else if (menuindex == 2)
        {
            statMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
        else if (menuindex == 3)
        {
            radioMenuPanel.SetActive(true);
            ShowBag("casette");
            _audio.Play(Select);
        }
        else if (menuindex == 4)
        {
            gachaMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
        else if (menuindex == 9)
        {
            musicMenuPanel.SetActive(true);
            radioMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
    }

    public void ShowBag(string filter)
    {
        //bag menu
        bagMenuPanel.SetActive(true);
        _audio.Play(Select);
        foreach(Transform child in bagParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(KeyValuePair<Item, int> ownedItem in inventory)
        {
            if(filter == "all" || ownedItem.Key.itemType == filter)
            {
                var buttonRef = Instantiate(templateButton, bagParent.transform, true);
                buttonRef.transform.localScale = new Vector3(1,1,1);
                buttonRef.GetComponent<Image>().sprite = ownedItem.Key.sprite;
                var consumable = buttonRef.GetComponent<ConsumableHelper>();
                consumable.Init(ownedItem.Key, filter);
                var buttonLabel = buttonRef.transform.GetChild(0).GetComponent<TMP_Text>();
                buttonLabel.text = ownedItem.Value.ToString();
                if (ownedItem.Value <= 0)
                {
                    buttonLabel.text = "";
                }
                if(ownedItem.Key.isCasette || filter == "all")
                {
                    buttonLabel.text = "";
                }
                buttonRef.GetComponent<Button>().interactable = ownedItem.Value > 0;

            }
        }
        if (filter == "all")
        {
            bagTitle.text = "collection";
        }
        else
        {
            bagTitle.text = filter;
        }
    }


}

public static class IListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
