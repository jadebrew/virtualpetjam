using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MenuHelper : MonoBehaviour
{
    public TMP_Text _title;
    public TMP_Text _body;

    public GameObject buttonTemplate;
    public GameObject bodyTemplate;

    public GameObject bodyParent;

    public GameState assignedState;

    public SceneManager _sceneManager;

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

    public void SetButtons(GameState self)
    {
        foreach(Transform child in bodyParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(GameState state in self.buttons)
        {
            var button = Instantiate(buttonTemplate, bodyParent.transform, true);
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = state.title;
            Vector3 scale = new Vector3(1,1,1);
            button.transform.localScale = scale;
            var buttonLogic = button.GetComponent<ButtonHelper>();
            buttonLogic.gameState = state;
            buttonLogic.menuHelper = this;
            button.GetComponent<Button>().onClick.AddListener(buttonLogic.SetState);
        }
    }
}
