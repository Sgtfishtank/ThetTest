using UnityEngine;
using System.Collections;

public class Core : MonoBehaviour {
    GameMaster GM;
    public Box[] boxesscripts;
    public GameObject psMaster;
    private float explotionDur;
    // Use this for initialization
    void Start ()
    {
        GM =GameObject.Find("Game Master").GetComponent<GameMaster>();
        GameObject[] boxes =GM.boxes.ToArray();
        for (int i = 0; i < boxes.Length-1; i++)
        {
            boxesscripts[i] = boxes[i].GetComponent<Box>();
        }
        psMaster.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (explotionDur < Time.time)
        {
            psMaster.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        for (int i = 0; i < boxesscripts.Length; i++)
        {
            boxesscripts[i].takeDamage();
        }
        col.gameObject.SetActive(false);
        psMaster.SetActive(true);
        psMaster.transform.position = col.transform.position;
        explotionDur = Time.time + 1f;
    }
}   