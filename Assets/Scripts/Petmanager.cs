using UnityEngine;
using TMPro;
using System;

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

    public GameObject mainMenuPanel;
    public GameObject foodMenuPanel;
    public GameObject statMenuPanel;
    public GameObject musicMenuPanel;
    public GameObject gachaMenuPanel;

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

    public void Feed(int value)
    {
        hunger += value;
        if (hunger>100)
        { hunger = 100;}
        happy = true;
        SetPanel(0);
        _audio.Play(Eat);
    }

    public void PlaySong(AudioManager.SongTitle title)
    {
        _audio.Play(title);
        SetPanel(0);
    }

    public void Win(int value)
    {
        _audio.Play(MajorVictory);
        SetPanel(0);
    }

    public void SetPanel(int menuindex)
    {
        mainMenuPanel.SetActive(false);
        foodMenuPanel.SetActive(false);
        statMenuPanel.SetActive(false);
        musicMenuPanel.SetActive(false);
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
        else if (menuindex == 1)
        {
            foodMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
        else if (menuindex == 2)
        {
            statMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
        else if (menuindex == 3)
        {
            musicMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
        else if (menuindex == 4)
        {
            gachaMenuPanel.SetActive(true);
            _audio.Play(Select);
        }
    }
}
