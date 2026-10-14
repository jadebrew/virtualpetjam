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

    public GameObject lastWon;

    private int lastWonIndex;
    private bool winDone;

    public List<GameObject> inventory = new List<GameObject>();
    public GameObject[] banner;

    public Sprite gachaball;

    private AudioManager _audio;

    void Awake()
    {
        _audio = AudioManager.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hunger = 80;
    }

    // Update is called once per frame
    void Update()
    {
        hunger -= Time.deltaTime;


        datetime = DateTime.Now.ToString();

        if (hunger<=0) {
            hunger = 0;
            happy = false;
        }

        HungerText.text = "Hunger: " + Mathf.Floor(hunger) + "\n Happy: " + happy.ToString();
    }

    public void Feed(GameObject consumable)
    {
        hunger += consumable.GetComponent<ConsumableHelper>().itemValue;
        if (hunger>100)
        { hunger = 100;}
        happy = true;
        SetPanel(0);
        inventory.Remove(consumable);
        _audio.Play(Eat);
    }

    public void PlaySong(AudioManager.SongTitle title)
    {
        _audio.Play(title);
        SetPanel(0);
    }

    public void Win(int value)
    {
        int win_index = UnityEngine.Random.Range(0,banner.Length-1);
        inventory.Add(banner[win_index]);
        //lastWon.GetComponent<Image>().sprite = banner[win_index].GetComponent<Image>().sprite;
        lastWon.GetComponent<Image>().sprite = gachaball;
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
            lastWon.GetComponent<Image>().sprite = banner[lastWonIndex].GetComponent<Image>().sprite;
            _audio.Play(MajorVictory);
            winDone = true;
        }
        else
        {
            SetPanel(-1);
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
            radioMenuPanel.SetActive(false);
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
        foreach(GameObject obj in inventory)
        {
            if(filter == "all" || obj.GetComponent<ConsumableHelper>().filtertype == filter)
            {
                var tempItem = Instantiate(obj, bagParent.transform, true);
                tempItem.transform.localScale = new Vector3(1,1,1);
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
