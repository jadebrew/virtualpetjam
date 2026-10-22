using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using static NeedType;
using static SFXTag;
using static SongTag;
using Random = UnityEngine.Random;
using System.IO;
using System.Linq;
using Needs;
using Notification;
using SimpleJSON;
using TMPro;

public class Petmanager : MonoBehaviour
{

    public GameObject petObject;
    public GameObject[] ChillLocations;
    private float gameTickTime = 5;
    private float patience = 0;
    public float screenSaverWait = 3600;
    
    public TMP_Text HungerText;
    public TMP_Text HygieneText;
    public TMP_Text SleepText;

    public float gametimer = 0;

    public float gameTickRate = 45; //seconds that it takes to call a Tick

    private float lastInput = 0;

    public string datetime;

    public GameObject bagParent;
    public TMP_Text bagTitle;

    public GameObject mainMenuPanel;
    public GameObject radioMenuPanel;
    public GameObject bagMenuPanel;
    public GameObject winPanel;
    public GameObject gachaMenuPanel;
    public GameObject menuPanel;
    public GameObject consumePanel;
    public GameObject creditPanel;
    public GameObject screenSaver;

    private bool doscreensaver = false;
    
    public const bool DEBUG = false;

    //TODO figure out formula later!!! (thanks honey <333)

    public GameObject templateButton;

    public Item coin;

    public GameObject lastWon;
    Vector3 gachaWinDefaultPos;
    private int lastWonIndex;
    private bool winDone;

    public GameObject lastConsumed;

    public CatchGame catchGame;


    [SerializeField]
    public Dictionary<Item,int> inventory = new Dictionary<Item,int>();
    public List<Item> banner = new ();
    public List<Item> lootTable;
    
    public Sprite gachaball;

    private AudioManager _audio;
    private SFXTag assignedUseSFX;

    public int currentPull;

    private NotificationManager _notif;
    
    private UIManager _ui;
    
    void Awake()
    {
        _audio = AudioManager.Instance;
        _notif = NotificationManager.Instance;
        
        _ui = UIManager.Instance;
        mainMenuPanel.SetActive(true);
        radioMenuPanel.SetActive(false);
        gachaMenuPanel.SetActive(false);
        bagMenuPanel.SetActive(false);
        winPanel.SetActive(false);
        consumePanel.SetActive(false);
        menuPanel.SetActive(true);

        CalculateAndSetNeedDelay();
        
        foreach (var item in Resources.LoadAll<Item>("Items"))
        {
            if (item.type != ItemType.Coin)
            {
                banner.Add(item);
            }
        }

        foreach(var item in banner)
        {
            if(!inventory.ContainsKey(item))
            {
                inventory.Add(item,0);
            }
            for(int i=0;i<item.rarity.amountInBanner;i++)
            {
                lootTable.Add(item);
            }
        }
        lootTable.Shuffle();
        currentPull = 0;
        gachaWinDefaultPos = lastWon.transform.localPosition;
        _audio.Play(JapanInverted);

        Debug.Log("Game Initialized.");
        /*if (DEBUG)
        {
            StressTest();
        }*/
        
        Load();
    }
    
    private void CalculateAndSetNeedDelay()
    {
        float secondsInDay = 24*60*60;
        float ticksInAday = secondsInDay/gameTickRate;
        NeedsManager.Instance.SetNeedDecay(ticksInAday);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // GameTick(true);


    }

    /*private void StressTest()
    {
        for (int i = 0; i < Mathf.Floor(ticksInAday); i++)
        {

            temporaryNeed -= NeedsManager.Instance.NeedDecay;
        }

        //NeedsManager.Instance.PrintStressTestResults(gameTickRate, SecondsInDay, ticksInAday, temporaryNeed);
    }*/

    double TimeStamp()
    {
        var epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        var timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        return timestamp;
    }

    // Update is called once per frame
    void Update()
    {
        patience += Time.deltaTime;
        lastInput +=  Time.deltaTime;

        if (patience > gameTickTime)
        {
          patience = 0;
          GameTick(false);
        }

        if(winDone)
        {
            Vector3 newPos = lastWon.transform.localPosition;
            newPos = Vector3.Lerp(newPos, new Vector3(gachaWinDefaultPos.x,gachaWinDefaultPos.y+34,gachaWinDefaultPos.z), Time.deltaTime*2);
            newPos.y = Mathf.Ceil(newPos.y);
            lastWon.transform.localPosition = newPos;
        } else {
             lastWon.transform.localPosition = gachaWinDefaultPos;
        }

        if(currentPull>=lootTable.Count)
        {
            currentPull = 0;
            lootTable.Shuffle();
        }
    }

    public void GameTick(bool catchup)
    {
        NeedsManager.Instance?.DecayNeeds();
        
        if(!catchup)
        {
            petObject.transform.position = ChillLocations[(int)Mathf.Round(Random.Range(0,ChillLocations.Length))].transform.position;
        }

        if (NeedsManager.Instance && NeedsManager.Instance.IsPetHappy)
        {
            inventory[coin]++;
        }
        else
        {
            if(!catchup)
            {
               _audio.Play(Annoyed);
            }
        }
        
        if (NotificationUIManager.Instance && !NotificationUIManager.Instance.notificationMessage.activeSelf && doscreensaver && lastInput > screenSaverWait)
        {
            SetPanel(9);
        }
    }

    public void Feed(GameObject consumable)
    {
        var item = consumable.GetComponent<ConsumableHelper>().item;
        
        if (inventory[item] > 0)
        {
            NeedsManager.Instance.UpdateNeedAfterItem(item);
            
            SetPanel(-1);
            inventory[item] -= 1;
            _audio.Play(item.onUseSFX);
            assignedUseSFX = item.onUseSFX;
            lastConsumed.GetComponent<Image>().sprite = item.sprite;
            catchGame.Init(assignedUseSFX);
        }
        else
        {
            _audio.Play(Cancel);
        }
        Save();
        lastInput = 0;
        doscreensaver = false;
    }

    public void currentUseSFX()
    {
        _audio.Play(assignedUseSFX);
        lastInput = 0;
    }

    public void Pet()
    {
        patience += 1;
        _audio.Play(Annoyed);
        lastInput = 0;
        Save();
    }


    public void Win(int value)
    {
        if (inventory[coin]>0)
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
            inventory[coin]--;
        }
        else
        {
            _audio.Play(Cancel);
        }
        Save();
        lastInput = 0;
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
        lastInput = 0;
    }

    public void SetPanel(int menuindex)
    {
        lastInput = 0;
        mainMenuPanel.SetActive(false);
        radioMenuPanel.SetActive(false);
        gachaMenuPanel.SetActive(false);
        bagMenuPanel.SetActive(false);
        menuPanel.SetActive(false);
        winPanel.SetActive(false);
        consumePanel.SetActive(false);
        creditPanel.SetActive(false);
        screenSaver.SetActive(false);
        doscreensaver = false;

        if (menuindex == -1)
        {
            mainMenuPanel.SetActive(true);
            _audio.Play(Cancel);
            doscreensaver = true;
            lastInput = 0;

        }
        else if (menuindex == 3)
        {
            radioMenuPanel.SetActive(true);
            ShowBag("cassette");
            _audio.Play(Select);
        }
        else if (menuindex == 4)
        {
            gachaMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
        else if (menuindex == 9)
        {
            screenSaver.SetActive(true);
        }
        Save();
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
        
        
        //sort by how many you own
        //this makes it so f.e. when you open the stereo the songs you own are on top
        var sortedInventory =
            from entry
            in inventory
            orderby entry.Value
            descending
            select entry;
        
        foreach(KeyValuePair<Item, int> ownedItem in sortedInventory)
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
                if((ownedItem.Key.IsCassette() || filter == "all" ) && ownedItem.Key.itemType != "coin")
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
        lastInput = 0;
    }

    public void InitMenu(GameState state)
    {
        var menu = menuPanel.GetComponent<MenuHelper>();
        menu.SetGameState(state);
    }

    void Save() {
        JSONObject petJson = new JSONObject();
        Debug.Log("Attempting to save.");
        petJson.Add("Name",Settings.PetName);
        JSONObject invJson = new JSONObject();
        foreach(var pair in inventory)
        {
            invJson.Add(pair.Key.title,pair.Value);
        }

        petJson.Add("inventory",invJson);

        petJson.Add("timestamp",TimeStamp());
        string path = Application.persistentDataPath + "/mimi_save.json";
        if(!File.Exists(path))
        {
            Debug.Log("Creating new savefile at: " + path);
        }
        File.WriteAllText(path,petJson.ToString());
        Debug.Log("Save complete.");

    }

    void Load()
    {
        string path = Application.persistentDataPath + "/mimi_save.json";
        if(File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);

            JSONObject petJson = (JSONObject)JSON.Parse(jsonString);

            Settings.PetName = petJson["name"];

            JSONObject invJson = (JSONObject)petJson["inventory"];

            Dictionary<Item,int> loadedInventory = new Dictionary<Item,int>();

            foreach(KeyValuePair<Item, int> ownedItem in inventory)
            {
                if (invJson[ownedItem.Key.title])
                {
                    loadedInventory.Add(ownedItem.Key,invJson[ownedItem.Key.title]);
                }
            }
            foreach(KeyValuePair<Item, int> ownedItem in loadedInventory)
            {
                inventory[ownedItem.Key] = ownedItem.Value;
            }
            //catch up system
            if(petJson["timestamp"])
            {
                CatchUp(petJson["timestamp"]);
            }

        } else {
            Debug.Log("File not found: " + path);
            Save();
        }
    }

    void CatchUp(double seconds)
    {

            int rounds = 0;

            for (double t = seconds; t < TimeStamp(); t+=gameTickRate)
            {
                GameTick(true);
                rounds++;
            }
            double timeAway = (TimeStamp() - seconds);
            Debug.Log("Welcome back! You've been away for " + Mathf.Floor((float)timeAway) + " Seconds.");
            Debug.Log("Game caught up for " + rounds.ToString() + " Ticks.");
            //TODO  while you were away Notification ()
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
