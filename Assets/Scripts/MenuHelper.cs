using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Needs;
using TMPro;

public class MenuHelper : MonoBehaviour
{
    public TMP_Text _title;
    public TMP_Text _body;

    public GameObject buttonTemplate;
    public GameObject bodyTemplate;

    public GameObject bodyParent;

    public GameState gameState;
    public GameState previousState;

    public GameState startScene;

    public GameState gameScene;

    public Petmanager pet;

    public MimiSceneManager _sceneManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetTitle(string title)
    {
        _title.text = title;
    }

    // Update is called once per frame
    public void SetBody(string body)
    {
        _body.text = body;
    }

    public void Start()
    {
        SetGameState(startScene);
        previousState = startScene;
    }

    public void GoPreviousState()
    {
        //figure out something smart later
        SetGameState(gameScene);
    }

    public void SetGameState(GameState self)
    {
        previousState = gameState;
        gameState = self;

        foreach(KeyValuePair<string, GameObject> panel in _sceneManager.panelDB)
        {
            _sceneManager.panelDB[panel.Key].SetActive(false);
        }

        foreach(string panel in self.panels)
        {
            if(!self.isBag)
            {
                _sceneManager.panelDB[panel].SetActive(true);
                }
            if(self.isBag)
                {
                    pet.ShowBag(self.bagFilter);
                }
            }
        SetTitle(self.title);
        SetBody(self.body);
        SetButtons(self);
    }

    public void SetButtons(GameState self)

    {
        gameState = self;
        foreach(Transform child in bodyParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(GameState state in self.buttons)
        {
            if (state.isGame && Petmanager.Instance.IsNewGame())
                continue;
            
            var button = Instantiate(buttonTemplate, bodyParent.transform, true);
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = state.shortname;
            Vector3 scale = new Vector3(1,1,1);
            button.transform.localScale = scale;
            var buttonLogic = button.GetComponent<ButtonHelper>();
            buttonLogic.gameState = state;
            buttonLogic.menuHelper = this;
            buttonLogic.pet = pet;
            
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                //TODO this is Lovecraftian horror level bad. Lord please forgive me.
                if (state.isNewGame)
                {
                    Petmanager.Instance.StartNewGame();
                }
                buttonLogic.SetState();
            });
        }
    }
}
