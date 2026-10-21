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
using SimpleJSON;
using TMPro;

public class Petmanager : MonoBehaviour
{

    public GameObject petObject;
    public GameObject[] ChillLocations;
    private float gameTickTime = 5;
    private float patience = 0;
    
    public TMP_Text HungerText;
    public TMP_Text HygieneText;
    public TMP_Text SleepText;

    public float gametimer = 0;
    public float gametick = 60;
    public string datetime;
    public bool happy = true;

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

    private float needMin = 0;
    private float needMax = 100;

    private float needDecay = 1;

    public GameObject templateButton;

    public Item coin;

    public GameObject lastWon;
    Vector3 gachaWinDefaultPos;
    private int lastWonIndex;
    private bool winDone;

    public GameObject lastConsumed;

    public CatchGame catchGame;


    public string petname = "Mimi";

    [SerializeField]
    public Dictionary<Item,int> inventory = new Dictionary<Item,int>();
    public List<Item> banner = new ();
    public List<Item> lootTable;


    [SerializeField] private Dictionary<NeedType, Need> needs;

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
        
        foreach (var need in Resources.LoadAll<Need>("Needs"))
        {
            needs[need.type] = need;
        }
        
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

        Load();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameTick(true);
    }

    double TimeStamp()
    {
        var epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        var timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        return timestamp;
    }

    // Update is called once per frame
    void Update()
    {
        datetime = DateTime.Now.ToString();

        patience += Time.deltaTime;
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

        //HungerText.text = Mathf.Round(needs[Hunger].value).ToString();
        //HygieneText.text = Mathf.Round(needs[Hygiene].value).ToString();
        //SleepText.text = Mathf.Round(needs[Sleep].value).ToString();
        
        _notif.Tick();
    }

    public void GameTick(bool catchup)
    {
        happy = true;
        foreach(var pair in needs)
        {
            var need = pair.Value;
            need.value -= needDecay;
            if(need.value < needMin)
            {
                need.value = needMin;
                happy = false;
            }
        }
        if(!catchup)
        {
            petObject.transform.position = ChillLocations[(int)Mathf.Round(Random.Range(0,ChillLocations.Length))].transform.position;
        }

        if (happy)
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
    }

    public void Feed(GameObject consumable)
    {
        var item = consumable.GetComponent<ConsumableHelper>().item;
        
        if (inventory[item] > 0)
        {
            var need = needs[item.needModifier];
            
            need.value += item.needModValue;
            if (need.value > needMax)
            {
                need.value = needMax;
            }
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
    }

    public void currentUseSFX()
    {
        _audio.Play(assignedUseSFX);
    }

    public void Pet()
    {
        patience += 1;
        _audio.Play(Annoyed);
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
        radioMenuPanel.SetActive(false);
        gachaMenuPanel.SetActive(false);
        bagMenuPanel.SetActive(false);
        menuPanel.SetActive(false);
        winPanel.SetActive(false);
        consumePanel.SetActive(false);
        creditPanel.SetActive(false);

        if (menuindex == -1)
        {
            mainMenuPanel.SetActive(true);
            _audio.Play(Cancel);

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
    }

    public void InitMenu(GameState state)
    {
        var menu = menuPanel.GetComponent<MenuHelper>();
        menu.SetGameState(state);
    }

    void Save() {
        JSONObject petJson = new JSONObject();
        petJson.Add("Name",petname);

        JSONObject invJson = new JSONObject();

        foreach(var pair in inventory)
        {
            invJson.Add(pair.Key.title,pair.Value);
        }

        petJson.Add("inventory",invJson);

        petJson.Add("timestamp",TimeStamp());

        string path = Application.persistentDataPath + "/mimi_save.json";
        File.WriteAllText(path,petJson.ToString());
        Debug.Log("Game Saved to " + path);
    }

    void Load()
    {
        string path = Application.persistentDataPath + "/mimi_save.json";
        if(File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);

            JSONObject petJson = (JSONObject)JSON.Parse(jsonString);

            petname = petJson["name"];

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
                var lastPlay = petJson["timestamp"];

                for (double t = lastPlay; t < TimeStamp(); t+=gameTickTime)
                {
                    GameTick(true);
                }
                var TimeAway = (TimeStamp() - lastPlay);
                Debug.Log("Welcome back! You've been away for " + Mathf.Floor((float)TimeAway) + " Seconds.");
                // while you were away Notification ()
                Save();
            }
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
