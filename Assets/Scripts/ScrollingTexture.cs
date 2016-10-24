using UnityEngine;
using System.Collections;

public class ScrollingTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ren = transform.GetComponent<Renderer>();

    }
    public float scrollSpeed = 0.90f;
    public float scrollSpeed2 = 0.90f;
    Renderer ren;

    void FixedUpdate()
    {
        
        var offset = Time.time * scrollSpeed;
        var offset2 = Time.time * scrollSpeed2;
       ren.material.mainTextureOffset = new Vector2(offset2, -offset);
    }
    // Update is called once per frame
    void Update () {
	
	}
}
