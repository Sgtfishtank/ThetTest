using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
    public int lane;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Wall")
            gameObject.SetActive(false);
    }   
}
