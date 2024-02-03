using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject overlayPanel;

    [SerializeField]
    private GameObject menuPanel;

    [SerializeField]
    private GameObject rewardsPanel;

    private void Start()
    {
        if (StaticScript.RewardsMenuIsActive)
        {
            rewardsPanel.SetActive(true);
            menuPanel.SetActive(true);
            overlayPanel.SetActive(true);
        }
        else
        {
            rewardsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void GetAnimalFromButtonPressed()
    {
        String animal = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>().text;
        Debug.Log(animal + " selected.");
        StaticScript.ARAnimal = animal;
    }

    public void SetRewardsBool(bool b)
    {
        StaticScript.RewardsMenuIsActive = b;
    }
}
