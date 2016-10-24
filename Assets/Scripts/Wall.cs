using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    public GameObject psMaster;
    private float explotionDur;
    // Use this for initialization
    void Start ()
    {
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
    void OnCollisionEnter(Collision col)
    {
        psMaster.SetActive(true);
        psMaster.transform.position = col.transform.position;
        explotionDur = Time.time + 1f;
    }
}
