using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject StartGameButton;
    public GameObject SettingsButton;
    public GameObject TutorialButton;
    public GameObject NextTutButton;
    public GameObject MainMenuButton;
    public GameObject StartGame;
    public GameObject Settings;
    public GameObject Tutorial1;
    public GameObject Tutorial2;
    public GameObject GM;
    public GameObject InGameUI;
    int Tut = 1;


    // Use this for initialization
    void Start ()
    {
        //GM = GameObject.Find("Game Master");
        GM.SetActive(false);
        InGameUI.SetActive(false);
        //Screen.SetResolution(640, 1136, false);
	}
    public void reset()
    {
        InGameUI.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
    public void StartGameFunc()
    {
        GM.SetActive(true);
        GM.GetComponent<GameMaster>().Reset();
        InGameUI.SetActive(true);
        InGameUI.GetComponent<UI>().Reset();
        gameObject.SetActive(false);
    }
    public void SettingsFunc()
    {
        StartGame.SetActive(false);
        StartGameButton.SetActive(false);
        SettingsButton.SetActive(false);
        Settings.SetActive(true);
        TutorialButton.SetActive(true);
        MainMenuButton.SetActive(true);

    }
    public void TutorialFunc()
    {
        Settings.SetActive(false);
        TutorialButton.SetActive(false);
        Tutorial1.SetActive(true);
        NextTutButton.SetActive(true);
        Tut = 1;
    }
    public void TutorialNextFunc()
    {
        if(Tut == 1)
        {
            Tutorial1.SetActive(false);
            Tutorial2.SetActive(true);
            Tut++;
        }
        else
        {
            Tutorial2.SetActive(false);
            NextTutButton.SetActive(false);
            Settings.SetActive(true);
            MainMenuButton.SetActive(true);
        }

    }
    public void MainMenuFunc()
    {
        StartGame.SetActive(true);
        StartGameButton.SetActive(true);
        SettingsButton.SetActive(true);
        Settings.SetActive(false);
    }

}
