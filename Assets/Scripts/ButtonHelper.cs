using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ButtonHelper : MonoBehaviour
{
    public GameState gameState;

    public MenuHelper menuHelper;

    public void SetState()
    {

        if (gameState.panels.Length>0)
        {
            foreach(KeyValuePair<string, GameObject> panel in menuHelper._sceneManager.panelDB)
            {
               menuHelper._sceneManager.panelDB[panel.Key].SetActive(false);
            }

            foreach(string panel in gameState.panels)
            {
                menuHelper._sceneManager.panelDB[panel].SetActive(true);
            }

            menuHelper.SetTitle(gameState.title);
            menuHelper.SetBody(gameState.body);
            menuHelper.SetButtons(gameState);
        }
    }
}
