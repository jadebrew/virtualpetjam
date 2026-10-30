using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static SFXTag;
using static SongTag;
using Random = UnityEngine.Random;
using System.IO;
using System.Linq;
using Audio;
using MiniGames;
using Needs;
using Notification;
using RandomEvents;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Petmanager : MonoBehaviour
{
    public static Petmanager Instance { get; private set; }

    public GameObject petObject;

    [SerializeField]
    public Dictionary<string,Sprite> petStates = new Dictionary<string,Sprite>();

    [SerializeField]
    public Dictionary<string,Sprite> petParticles = new Dictionary<string,Sprite>();

    public GameObject[] ChillLocations;
    private float gameTickTime = 5;
    private float patience = 0;
    private float screenSaverWait = 3600;
    
    public TMP_Text HungerText;
    public TMP_Text HygieneText;
    public TMP_Text SleepText;

    public TMP_Text yen;

    public float gametimer = 0;

    public float gameTickRate = 60; //seconds that it takes to call a Tick

    private float lastInput = 0;

    public string datetime;

    public GameObject bagParent;
    public TMP_Text bagTitle;

    public GameObject homePanel;
    public GameObject mainMenuPanel;
    public GameObject radioMenuPanel;
    public GameObject bagMenuPanel;
    public GameObject winPanel;
    public GameObject gachaMenuPanel;
    public GameObject menuPanel;
    public GameObject consumePanel;
    public GameObject creditPanel;
    public GameObject screenSaver;
    public GameObject needPanel;

    private bool doscreensaver = false;
    
    public const bool DEBUG = false;

    public GameObject templateButton;

    public Item coin;

    public GameObject lastWon;
    Vector3 gachaWinDefaultPos;
    private int lastWonIndex;
    private bool winDone;

    public GameObject lastConsumed;

    //mini games
    public CatchGame catchGame;
    public MemoryGame memoryGame;
    public ReactionGame reactionGame;



    
    //xp coins
    private const int AmountOfGameTicksMultiplierTakesEffect = 5;
    public float coin_xp = 0;
    private float _multiplier = 1f;
    private int _multiplierAppliedCount = 0;
    public int expectedCoinsPerDay = 25;
    public int maxCoins = 100;

    private bool greedy = false;
    private int xpUntilNextCoin = 100;
    private bool coinflush = false;
    private float flushTimer = 0;
    private float flushDelay = 100;


    [SerializeField]
    public Dictionary<Item,int> inventory = new Dictionary<Item,int>();
    public List<Item> banner = new ();
    public List<Item> lootTable;
    
    public Sprite gachaball;

    private AudioManager _audio;
    private SFXTag assignedUseSFX;

    public int currentPull;

    private NotificationManager _notif;

    public GameObject _cursor;
    
    private UIManager _ui;

    private bool warmup = true;
    
    //TODO this is horrible and should be done with enums instead,
    //but I did it like this because it might be used by editor like this
    //and I don't want to break stuff close to the deadline

    private const int MainMenuPanelIndex = 0;
    private const int RadioMenuPanelIndex = 3;
    private const int GachaMenuPanelIndex = 4;
    private const int ScreenSaverIndex = 9;

    private bool comfortable = true;
    private bool sleeping = false;
    private bool gross = true;
    
    //TODO move to SaveManager
    private bool _isNewGame;

    private int _gameTicksCount = 0;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        SetIsNewGame();
        
        _audio = AudioManager.Instance;
        _notif = NotificationManager.Instance;
        
        _ui = UIManager.Instance;
        mainMenuPanel.SetActive(true);
        radioMenuPanel.SetActive(false);
        gachaMenuPanel.SetActive(false);
        bagMenuPanel.SetActive(false);
        winPanel.SetActive(false);
        consumePanel.SetActive(false);
        homePanel.SetActive(false);
        menuPanel.SetActive(true);

        CalculateAndSetNeedDelay();
        
        foreach (var item in Resources.LoadAll<Item>("Items"))
        {

            banner.Add(item);

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

    //TODO move
    private void FillPlaylist()
    {
        foreach (var (item, amount) in inventory)
        {
            if (item.itemType == "cassette" && amount > 0)
            {
                StereoManager.Instance.AddSongToPlaylist(item.assignedSongTitle);
            }
        }

    }


    private void FindSelectable()
    {
        var controllerMode = false;
        Selectable first = FindFirstObjectByType<Selectable>();
        if (controllerMode && first != null)
        {
            EventSystem.current.SetSelectedGameObject(first.gameObject);
            Debug.Log("Selected " + first.gameObject);
        }
    }
    
    private void CalculateAndSetNeedDelay()
    {
        const float secondsInDay = 24*60*60;
        var ticksInAday = secondsInDay/gameTickRate;

        NeedsManager.Instance.SetNeedDecay(ticksInAday);
    }

    /*private void StressTest()
    {
        for (int i = 0; i < Mathf.Floor(ticksInAday); i++)
        {

            temporaryNeed -= NeedsManager.Instance.NeedDecay;
        }

        //NeedsManager.Instance.PrintStressTestResults(gameTickRate, SecondsInDay, ticksInAday, temporaryNeed);
    }*/

    public void SetPetState(string statekey)
    {
        if (!greedy && !sleeping && comfortable && petStates.ContainsKey(statekey))
        {
            SetPetImage(petStates[statekey]);
        }
    }

    public void SetPetImage(Sprite sprite)
    {

        petObject.transform.Find("Image").GetComponent<Image>().sprite = sprite;

    }

    public void SetIcon(string iconkey)
    {
        if (petParticles.ContainsKey(iconkey))
        {
            petObject.transform.Find("Icon").gameObject.SetActive(true);
            petObject.transform.Find("Icon").GetComponent<Image>().sprite = petParticles[iconkey];
            //TODO animation or something (move up?)
        }
    }

    public void SetPetImageIfComfortable(Sprite sprite)
    {
        if (!greedy && !sleeping  && comfortable)
        {
            SetPetImage(sprite);
        }
    }
    public void ForceNeed(string statekey)
    {
        if (!greedy && !sleeping  && petStates.ContainsKey(statekey))
        {
            SetPetImage(petStates[statekey]);
        }
        comfortable = false;
    }
    public void ForceSprite(Sprite sprite)
    {
        comfortable = false;
        SetPetImage(sprite);
    }

    public void NoForce()
    {
        comfortable = true;
        gross = false;
    }

    private double TimeStamp()
    {
        var epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        var timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        return timestamp;
    }

    // Update is called once per frame
    public void Update()
    {
        patience += Time.deltaTime;
        lastInput +=  Time.deltaTime;

        if (!warmup && patience > gameTickTime)
        {
          patience = 0;
          GameTick(false);
        }

         UpdateFlush();

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

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            FindSelectable();
            lastInput = 0;
        }
        if (inventory[coin]>maxCoins)
        {
            inventory[coin] = maxCoins;
        }
        yen.text = inventory[coin] + "00";
        //_cursor.transform.position = EventSystem.current.currentSelectedGameObject.transform.position;
    }

    public void GameTick(bool catchup)
    {
        _gameTicksCount++;
        petObject.transform.Find("Icon").gameObject.SetActive(false);

        NeedsManager.Instance?.DecayNeeds();
        if(!catchup && comfortable && !greedy)
        {
            if (!gross)
            {
                SetPetState("idle");
            }
            else
            {
                SetPetState("gross");
                SetIcon("stinky");
            }
        }

        if (!catchup && _gameTicksCount % 3 == 0)
        {
            //random spice of life things
            var d6 = Mathf.Round(Random.Range(1,6));
            if (d6 == 1)
            {
                //flip orientation
                var currentX = petObject.transform.localScale.x;
                petObject.transform.localScale = new Vector3 (currentX*-1,1,1);
            } else if (d6 == 2)
            {
                //go sit
                if (!sleeping)
                {
                    SetPetState("sleeping");
                    SetIcon("zzz");
                    MoveToBed();
                    sleeping = true;
                } else {
                    sleeping = false;
                    SetPetState("sleepy");
                    SetIcon("tear");
                }

            } else if (d6 == 3)
            {

                // go sleep
                if (!gross)
                {
                    SetPetState("sit");
                }
                else
                {
                    SetPetState("gross_sit");
                    SetIcon("stinky");
                }
            }
            else
            {
                if(!catchup && comfortable && !sleeping)
                {
                    MovePetToChillSpot((int) Mathf.Round(Random.Range(0, ChillLocations.Length)));
                }

            }



        }

        if (_multiplierAppliedCount > AmountOfGameTicksMultiplierTakesEffect)//multiplier has ran out
        {
            _multiplier = 1f;
            _multiplierAppliedCount = 0;
        } else {
            if(!catchup && greedy)
            {
                NoForce();
                SetPetState("greedy");
                SetIcon("yen");
            }
        }
        if (NeedsManager.Instance && NeedsManager.Instance.IsPetHappy)
        {
            coin_xp += _multiplier;
            if(coin_xp>xpUntilNextCoin)
            {
                coin_xp -= xpUntilNextCoin;
                if (coin_xp < xpUntilNextCoin)
                {
                    greedy = false;
                }
                inventory[coin]++;
                _audio.Play(MajorVictory);
                NotificationManager.Instance.NotifyCoinReward();
            }
            _multiplierAppliedCount++;
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
            SetPanel(ScreenSaverIndex);
        } else {
            //screenSaver.SetActive(false);
        }

        Debug.Log("coin xp: " + coin_xp);
        
        if (RandomEventManager.Instance != null && _gameTicksCount % 6 == 0)//every 6 gameticks
        {
            Debug.Log("Random Event");
            RandomEventManager.Instance.RunRandomEvent();
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

            switch (item.needModifier)
            {
                case NeedType.Hunger:
                    reactionGame.Init();
                    break;
                case NeedType.Sleep:
                    memoryGame.Init();
                    break;
                case NeedType.Hygiene:
                    catchGame.Init();
                    gross = false;
                    break;
                default:
                    break;
            }

            
        }
        else
        {
            _audio.Play(Cancel);
        }
        Save();
        lastInput = 0;
        doscreensaver = false;
    }
    
    public void MovePetToRadio()
    {
        MovePetToChillSpot(1);
        ForceNeed("sleepy");
        comfortable = false;
        Debug.Log("Radio Event");
    }
    
    public void MovePetToLitterBox()
    {
        MovePetToChillSpot(3);
        ForceNeed("potty");
        comfortable = false;
        Debug.Log("Potty Event");
        gross = true;
    }

    public void MoveToBed()
    {
         MovePetToChillSpot(5);
    }
    
    private void MovePetToChillSpot(int i)
    {
        petObject.transform.position = ChillLocations[i].transform.position;
    }

    public void CurrentUseSfx()
    {
        _audio.Play(assignedUseSFX);
        lastInput = 0;
    }

    public void Pet()
    {
        lastInput = 0;
        NoForce();
        SetIcon("tear");
        if (sleeping)
        {
            sleeping = false;
            SetPetState("sleepy");
            SetIcon("tear");
            _audio.Play(Annoyed);
        }
        if (comfortable)
        {
            SetPetState("comfortable");
            _audio.Play(Annoyed);
        }
        if (!comfortable)
        {
            ForceNeed("angry");
            SetIcon("angry");
            _audio.Play(Annoyed);
        }
    }

    public void FlushCoins()
    {
        if (!coinflush)
        {
            coinflush = true;
            flushDelay = 0;
            Win();
        }

    }

    public void UpdateFlush()
    {
        if (coinflush)
        {
            flushTimer+= Time.deltaTime;
            if (flushTimer > flushDelay)
            {

                if (winDone &&  (flushTimer > flushDelay))
                {
                    Win();
                    Debug.Log("winflush");
                }
                else if(flushTimer > flushDelay)
                {
                    Reveal();
                    Debug.Log("revealflush");
                }
                flushTimer = 0;
            }
        }
    }

    public void NeedCheck()
    {
        //Debug.Log(NeedsManager.Instance.GetNeed(NeedType.Hunger).value);
    }


    public void Win()
    {
        if (inventory[coin]>0)
        {
            var winIndex = currentPull;
            currentPull++;

            Item wonItem = lootTable[winIndex];
            inventory[lootTable[winIndex]]++;
            lastWon.GetComponent<Image>().sprite = wonItem.rarity.sprite;
            lastWonIndex = winIndex;
            winDone = false;

            _audio.Play(NewItem);
            gachaMenuPanel.SetActive(false);
            winPanel.SetActive(true);
            inventory[coin]--;
            Save();
        }
        else
        {
            _audio.Play(Cancel);
            coinflush = false;
        }
        lastInput = 0;
    }

    public void Reveal()
    {
        if (!winDone)
        {
            lastWon.GetComponent<Image>().sprite = lootTable[lastWonIndex].sprite;
            _audio.Play(lootTable[lastWonIndex].rarity.winSFX);
            winDone = true;
        }
        else
        {
            SetPanel(GachaMenuPanelIndex);
        }
        lastInput = 0;
    }

    public void SetPanel(int menuIndex)
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
        needPanel.SetActive(false);
        homePanel.SetActive(false);
        
        //TODO this is horrible and should be done with enums instead,
        //but I did it like this because it might be used by editor like this
        //and I don't want to break stuff close to the deadline

        switch (menuIndex)
        {
            case <= MainMenuPanelIndex:// <= 0
                mainMenuPanel.SetActive(true);
                homePanel.SetActive(true);
                _audio.Play(Cancel);
                doscreensaver = true;
                lastInput = 0;
                break;
            case RadioMenuPanelIndex:// = 3
                radioMenuPanel.SetActive(true);
                homePanel.SetActive(false);
                ShowBag("cassette");
                _audio.Play(Select);
                break;
            case GachaMenuPanelIndex:// = 4
                gachaMenuPanel.SetActive(true);
                homePanel.SetActive(false);
                _audio.Play(Select);
                break;
            case ScreenSaverIndex:// = 9;
                screenSaver.SetActive(true);
                homePanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                break;
        }
        FindSelectable();
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
            bagTitle.text = "museum";
        }
        else if (filter == "coin")
        {
            bagTitle.text = "purse";
        }
        else
        {
            bagTitle.text = filter;
        }
        lastInput = 0;
        homePanel.SetActive(false);
        FindSelectable();
    }

    public void Spend()
    {

    }

    public void InitMenu(GameState state)
    {
        doscreensaver = false;
        lastInput = 0;
        var menu = menuPanel.GetComponent<MenuHelper>();
        homePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        menu.SetGameState(state);
        FindSelectable();
    }

    public void SetMultiplierBonus(float multiplier)
    {
        _multiplier = Mathf.Max(multiplier*xpUntilNextCoin,1.0f);
        if(_multiplier > 1.0f)
        {
            _notif.NotifyMultiplyBonus(multiplier);
            greedy = true;
            SetPetState("greedy");
        }

    }

    
    //TODO move to SaveManager
    private void Save() {
        var petJson = new JSONObject();
        Debug.Log("Attempting to save.");
        petJson.Add("name",Settings.PetName);
        var invJson = new JSONObject();
        foreach(var pair in inventory)
        {
            invJson.Add(pair.Key.title,pair.Value);
        }
        
        var needsJson = new JSONObject();
        needsJson.Add("hunger",NeedsManager.Instance.GetNeed(NeedType.Hunger).value);
        needsJson.Add("hygiene",NeedsManager.Instance.GetNeed(NeedType.Hygiene).value);
        needsJson.Add("sleep",NeedsManager.Instance.GetNeed(NeedType.Sleep).value);
        petJson.Add("needs",needsJson);

        petJson.Add("inventory",invJson);

        petJson.Add("timestamp",TimeStamp());
        
        petJson.Add("playbackMode",AudioManager.PlaybackModeToString(Settings.PlaybackMode));
        
        var path = "";
        #if UNITY_WEBGL && !UNITY_EDITOR
            path = Path.Combine("idbfs/","mimigreed");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        #else
            path = Application.persistentDataPath;
        #endif
        path = Path.Combine(path,Settings.PetName + "_save.json");


        if(!File.Exists(path))
        {
            Debug.Log("Creating new savefile at: " + path);
        }

        if (!warmup)
        {
            File.WriteAllText(path,petJson.ToString());
            Debug.Log("Save completed at: "+ path);
        } else {
            Debug.Log("Still warming up.");
        }


    }

    //TODO move to SaveManager
    private void Load(bool newGame = false)
    {
        var path = "";
        if (!newGame)
        {
            var folderPath = "";
#if UNITY_WEBGL && !UNITY_EDITOR
            folderPath = Path.Combine("idbfs/","mimigreed");;
            if (!File.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
#else
            folderPath = Application.persistentDataPath;
#endif

            folderPath = folderPath +"/";

            const string searchPattern = "*_save.json";

            path = Directory.GetFiles(folderPath, searchPattern)
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();
        }
        else  // newfile
        {
            path = Path.Combine(Application.dataPath, "Resources/json/new_game.json");
        }


        if(!string.IsNullOrEmpty(path))//savefile exists
        {
            var jsonString = File.ReadAllText(path);

            var petJson = (JSONObject)JSON.Parse(jsonString);
            
            if(petJson["name"])
            {
                Settings.PetName = petJson["name"];
            }

            var invJson = (JSONObject)petJson["inventory"];

            var loadedInventory = new Dictionary<Item,int>();

            foreach(var (key, _) in inventory)
            {
                if (invJson[key.title])
                {
                    loadedInventory.Add(key,invJson[key.title]);
                }
            }
            foreach(var (key, value) in loadedInventory)
            {
                inventory[key] = value;
            }
            
            //newGame = false;
            if(!newGame) {
                var needsJson = (JSONObject) petJson["needs"];
                if(needsJson["hunger"])
                    NeedsManager.Instance.SetNeed(NeedType.Hunger, needsJson["hunger"]);
                if(needsJson["hygiene"])
                    NeedsManager.Instance.SetNeed(NeedType.Hygiene, needsJson["hygiene"]); 
                if(needsJson["sleep"])
                    NeedsManager.Instance.SetNeed(NeedType.Sleep, needsJson["sleep"]);
                Debug.Log("Needs Loaded");
            }
            else //new game
            {
                NeedsManager.Instance.SetAllNeedsToMax();
                Debug.Log("New Needs Initialized");
            }

            //catch up system
            if(!newGame && petJson["timestamp"])
            {
                CatchUp(petJson["timestamp"]);
            }
            
            if(petJson["playbackMode"])
            {
                Settings.PlaybackMode = AudioManager.PlaybackModeFromString(petJson["playbackMode"]);
            }
            
            FillPlaylist();
            StereoManager.Instance.PlaybackMode = PlaybackMode.Shuffle;
            StereoManager.Instance.PlayNext();

            Debug.Log("Savefile loaded: " + path);
            warmup = false;

        } else {
            Debug.Log("File not found: " + path);
            warmup = false;
            Save();
        }
    }
    
    //TODO move to SaveManager
    public void StartNewGame()
    {
        DeleteAllSaves();
        Load(true);
        Save();
        _isNewGame = false;
    }
    
    //TODO move to SaveManager
    private static void DeleteAllSaves()
    {
        var path = "";
#if UNITY_WEBGL && !UNITY_EDITOR
            path = Path.Combine("idbfs/","mimigreed");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
#else
        path = Application.persistentDataPath;
#endif
        
        foreach (var filePath in Directory.GetFiles(path, "*_save.json"))
        {
            File.Delete(filePath);
        }
    }
    
    //TODO move to SaveManager
    private void SetIsNewGame()
    {
        var folderPath = "";

#if UNITY_WEBGL && !UNITY_EDITOR
            folderPath = Path.Combine("idbfs/","mimigreed");;
            if (!File.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
#else
        folderPath = Application.persistentDataPath;
#endif

        folderPath = folderPath +"/";

        const string searchPattern = "*_save.json";
        
        var path = Directory.GetFiles(folderPath, searchPattern)
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();

        _isNewGame = string.IsNullOrEmpty(path);
    }

    public bool IsNewGame()
    {
        return _isNewGame;
    }

    public void ToggleGameObject(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void CatchUp(double seconds)
    {

            int rounds = 0;
            
            int coinsBefore = inventory[coin];

            for (double t = seconds; t < TimeStamp(); t+=gameTickRate)
            {
                GameTick(true);
                rounds++;
            }
            //double timeAway = (TimeStamp() - seconds);
            
            NotificationManager.Instance.NotifyWelcomeBack(inventory[coin] - coinsBefore);
        }
}
