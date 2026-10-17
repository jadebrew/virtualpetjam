using UnityEngine;
using System.Collections.Generic;

using static SFXTag;

public class ButtonHelper : MonoBehaviour
{
    public GameState gameState;

    public MenuHelper menuHelper;

    private AudioManager _audio;

    public Petmanager pet;


    public void SetState()
    {
        _audio = AudioManager.Instance;
        if (gameState.panels.Length>0)
        {
            foreach(KeyValuePair<string, GameObject> panel in menuHelper._sceneManager.panelDB)
            {
               menuHelper._sceneManager.panelDB[panel.Key].SetActive(false);
            }

            foreach(string panel in gameState.panels)
            {
                if(!gameState.isBag)
                {
                    menuHelper._sceneManager.panelDB[panel].SetActive(true);
                }
                if(gameState.isBag)
                {
                    pet.ShowBag(gameState.bagFilter);
                }
            }
            _audio.Play(Select);

            menuHelper.SetTitle(gameState.title);
            menuHelper.SetBody(gameState.body);
            menuHelper.SetButtons(gameState);
        }
    }
}
