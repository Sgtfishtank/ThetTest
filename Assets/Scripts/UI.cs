using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    public Text[] textfields;
    public GameObject[] boxes;
    public new Camera camera;
    public GameObject GameOver;
    public GameObject MainMenuButton;
    public GameObject startMenu;
    GameMaster GM;
    public GameObject ContinueGameButton;
    public GameObject Settings;
    public GameObject TutorialButton;
    public GameObject Tutorial1;
    public GameObject NextTutButton;
    private int Tut;
    public GameObject Tutorial2;
    public GameObject SettingsButton;
    public GameObject[] stars;

    // Use this for initialization
    void Start () {
        //textfields = transform.GetComponentsInChildren<Text>();
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        boxes = GM.boxes.ToArray();
        GameOver.SetActive(false);
        MainMenuButton.SetActive(false);
        Settings.SetActive(false);
        Tutorial1.SetActive(false);
        Tutorial2.SetActive(false);
        TutorialButton.SetActive(false);
        NextTutButton.SetActive(false);
        ContinueGameButton.SetActive(false);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].transform.parent.gameObject.SetActive(false);
        }
    }
    public void Reset()
    {
        GameOver.SetActive(true);
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        if(textfields.Length == 0)
            textfields = transform.GetComponentsInChildren<Text>();
        boxes = GM.boxes.ToArray();
        for (int i = 0; i < 12; i++)
        {
            textfields[i].gameObject.SetActive(true);
        }
        GameOver.SetActive(false);
        MainMenuButton.SetActive(false);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].transform.parent.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(boxes.Length == 0)
        {
            
            boxes = GM.boxes.ToArray();
            return;
        }
        textfields[0].text = checkZero(boxes[0].transform.GetComponent<Box>().hp);
        textfields[0].rectTransform.position = camera.WorldToScreenPoint(boxes[0].transform.localPosition) + new Vector3(0, 7, 0);
        textfields[1].text = checkZero(boxes[1].transform.GetComponent<Box>().hp);
        textfields[1].rectTransform.position = camera.WorldToScreenPoint(boxes[1].transform.localPosition) + new Vector3(0, 7, 0);
        textfields[2].text = checkZero(boxes[2].transform.GetComponent<Box>().hp);
        textfields[2].rectTransform.position = camera.WorldToScreenPoint(boxes[2].transform.localPosition) + new Vector3(0, 7, 0);
        textfields[5].text = checkZero(boxes[3].transform.GetComponent<Box>().hp);
        textfields[5].rectTransform.position = camera.WorldToScreenPoint(boxes[3].transform.localPosition) + new Vector3(0, 7, 0);
        textfields[4].text = checkZero(boxes[4].transform.GetComponent<Box>().hp);
        textfields[4].rectTransform.position = camera.WorldToScreenPoint(boxes[4].transform.localPosition) + new Vector3(0, 7, 0);
        textfields[3].text = checkZero(boxes[5].transform.GetComponent<Box>().hp);
        textfields[3].rectTransform.position = camera.WorldToScreenPoint(boxes[5].transform.localPosition) + new Vector3(0, 7, 0);

        textfields[6].text = GM.blueScore.ToString();
        textfields[7].text = GM.redScore.ToString();

        textfields[8].text = Mathf.Clamp((GM.slowRemaning - Mathf.FloorToInt(Time.time)), 0, Mathf.Infinity).ToString();

        textfields[9].text = Mathf.Clamp((GM.wallRemaning - Mathf.FloorToInt(Time.time)), 0, Mathf.Infinity).ToString();

        textfields[10].text = Mathf.Clamp((GM.multiRemaning - Mathf.FloorToInt(Time.time)), 0, Mathf.Infinity).ToString();

        textfields[11].text = Mathf.Clamp((GM.switchRemaning - Mathf.FloorToInt(Time.time)), 0, Mathf.Infinity).ToString();

        if(GM.GameOver)
        {
            for (int i = 0; i < 12; i++)
            {
                textfields[i].gameObject.SetActive(false);
            }
            GameOver.SetActive(true);
            MainMenuButton.SetActive(true);
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].transform.parent.gameObject.SetActive(true);
            }
            
            textfields[12].text = GM.blueScore.ToString();
            textfields[13].text = GM.redScore.ToString();
            int total = (GM.redScore + GM.blueScore);
            textfields[14].text = total.ToString();
            if(total >= 200)
            {
                stars[2].SetActive(true);
            }
            if(total >= 100)
            {
                stars[1].SetActive(true);
            }
            if (total >= 50)
            {
                stars[0].SetActive(true);
            }
        }

    }
    public void mainMenu()
    {
        startMenu.SetActive(true);
        startMenu.GetComponent<MainMenu>().reset();
        gameObject.SetActive(false);
        GM.gameObject.SetActive(false);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].transform.parent.gameObject.SetActive(false);
        }
    }
    public void SettingsFunc()
    {
        Time.timeScale = 0;
        Settings.SetActive(true);
        SettingsButton.SetActive(true);
        ContinueGameButton.SetActive(true);
        TutorialButton.SetActive(true);
        for (int i = 0; i < textfields.Length; i++)
        {
            textfields[i].gameObject.SetActive(false);
        }

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
        if (Tut == 1)
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
            ContinueGameButton.SetActive(true);
        }
    }
    public void Continue()
    {
        Settings.SetActive(false);
        Tutorial1.SetActive(false);
        Tutorial2.SetActive(false);
        TutorialButton.SetActive(false);
        NextTutButton.SetActive(false);
        ContinueGameButton.SetActive(false);
        for (int i = 0; i < textfields.Length; i++)
        {
            textfields[i].gameObject.SetActive(true);
        }
        Time.timeScale = 1;
    }
    string checkZero(int value)
    {
        if (value <= 0)
            return "";
        else
            return value.ToString();
    }

}
