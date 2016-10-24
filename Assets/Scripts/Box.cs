using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Box : MonoBehaviour
{
    public new Camera camera;
    public Transform[] BoxSlots;
    public int Slot;
    public GameMaster GM;
    public float speed;
    public int hp = 25;
    private bool move = false;
    private float healTimer;
    public float healCD;
    private int moving;
    public GameObject psMaster;
    public GameObject fullBox;
    public GameObject brokenBox;
    public GameObject CrystalText;
    public Material mat1;
    public Material mat2;
    public Color blue;
    public Color red;
    public Color grey;
    private float shaketime;

    bool isBroken = false;
    private float explotionDur;
    private float crystalTextDur;
    private bool once = true;
    Transform[] temp;
    public bool ghost = false;
    private bool movable = true;
    private bool selected = false;

    // Use this for initialization
    void Start ()
    {
        ColorUtility.TryParseHtmlString("#909090FF", out grey);
        camera = GameObject.Find("camera_main").GetComponent<Camera>();
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        BoxSlots = GM.boxPoints.ToArray();
        psMaster.SetActive(false);
        brokenBox.transform.parent.gameObject.SetActive(false);
        temp = fullBox.transform.GetComponentsInChildren<Transform>();

    }
    

    // Update is called once per frame
    void Update ()
    {
        if (moving == 2)
        {
            for (int i = 0; i < BoxSlots.Length; i++)
            {
                if (Mathf.Abs((transform.position.x - BoxSlots[i].position.x)) <= 0.75f && Mathf.Abs((transform.position.y - BoxSlots[i].position.y)) <= 0.75f)
                {
                    move = true;
                    GM.swapBox(Slot, i);
                    Slot = i;
                    moving = 0;
                    if (ghost)
                    {
                        if (i == 6)
                        {
                            fullBox.transform.parent.transform.localScale = new Vector3(21, 21, 21);
                            brokenBox.transform.parent.transform.localScale = new Vector3(21, 21, 21);
                        }
                        else
                        {
                            fullBox.transform.parent.transform.localScale = new Vector3(24, 24, 24);
                            brokenBox.transform.parent.transform.localScale = new Vector3(24, 24, 24);
                        }
                    }
                }
            }
        }
        if (once)
        {
            temp = fullBox.transform.GetComponentsInChildren<Transform>();
            
            for (int j = 0; j < temp.Length; j++)
            {
                temp[j].gameObject.layer = 8;
            }
            once = false;
        }
        if(ghost)
        {
            return;
        }
        if(selected)
        {
            Vector3 pos = Input.mousePosition;
            transform.position = camera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, camera.nearClipPlane));
            transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
        }
        if(shaketime > Time.time)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0,0,Mathf.Sin(Time.time *70)*2.7f));
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
        if (hp == 0)
        {
            GM.destroyLane(Slot);
            hp--;
            transform.GetComponent<BoxCollider>().enabled = false;
            //gameObject.SetActive(false);
            brokenBox.GetComponent<MeshRenderer>().material.SetColor("_Color", grey);
        }
        if(!isBroken && hp < 6)
        {
            isBroken = true;
            brokenBox.transform.parent.gameObject.SetActive(true);
            fullBox.transform.parent.gameObject.SetActive(false);
        }
        else if(isBroken &&hp > 5)
        {
            isBroken = false;
            brokenBox.transform.parent.gameObject.SetActive(false);
            fullBox.transform.parent.gameObject.SetActive(true);
        }
        if(crystalTextDur < Time.time)
        {
            CrystalText.SetActive(false);
        }
        if(explotionDur < Time.time)
        {
            psMaster.SetActive(false);
        }
        
        if (!Input.GetMouseButton(0) && transform.position != BoxSlots[Slot].position && moving == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, BoxSlots[Slot].position, speed * Time.deltaTime);
                
        }
        if(Slot == BoxSlots.Length-1)
        {
            if(healTimer < Time.time && hp < 11)
            {
                hp++;
                healTimer = Time.time + healCD;
            }
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if(1 == GM.abilitysInLane[Slot+12].y)
        {
            CrystalText.GetComponentsInChildren<TextMesh>(true)[0].text = "+3";
        }
        else
        {
            CrystalText.GetComponentsInChildren<TextMesh>(true)[0].text = "+1";
        }
        if(col.transform.tag == "Red" &&  transform.tag == "Red Box")
        {
            if(Slot> 2)
                CrystalText.transform.position = col.transform.position - new Vector3(0,0.2f,0);
            else
                CrystalText.transform.position = col.transform.position;
            crystalTextDur = Time.time + 1;
            CrystalText.SetActive(true);
            GM.addRedScore(Slot);
        }
        else if (col.transform.tag == "Blue" && transform.tag == "Blue Box")
        {
            CrystalText.transform.position = col.transform.position;
            crystalTextDur = Time.time + 1;
            CrystalText.SetActive(true);
            GM.addBlueScore(Slot);
        }
        else if (col.transform.tag == "Gold")
        {
            CrystalText.transform.position = col.transform.position;
            crystalTextDur = Time.time + 1;
            CrystalText.SetActive(true);
           // GM.addScore(Slot);
        }
        else
        {
            psMaster.SetActive(true);
            psMaster.transform.position = col.transform.position;
            explotionDur = Time.time + 1f;
            shaketime = 0.3f + Time.time;
            hp--;
        }
        col.gameObject.SetActive(false);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (movable)
            {
                selected = true; 
                Vector3 pos = Input.mousePosition;
                transform.position = camera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, camera.nearClipPlane));
                transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
            }
            GM.LockBoxes(Slot);
            if (GM.selectingAbility && GM.currentActive == GameMaster.Abilitys.SWITCH)
            {
                GM.activateAbility(GameMaster.Abilitys.SWITCH, Slot);
            }

        }
        if(!Input.GetMouseButton(0))
        {
            move = false;
            selected = false;
            GM.unLockBoxes(Slot);
            for (int i = 0; i < BoxSlots.Length; i++)
            {
                if (Mathf.Abs((transform.position.x - BoxSlots[i].position.x)) <= 0.75f && Mathf.Abs((transform.position.y - BoxSlots[i].position.y)) <= 0.75f)
                {
                    if (!GM.destroyedLanes.Contains(i))
                    {
                        move = true;
                        GM.swapBox(Slot, i);
                        Slot = i;
                        if(Slot == 6)
                        {
                            fullBox.transform.parent.transform.localScale = new Vector3(21, 21, 21);
                        }
                        else
                        {
                            fullBox.transform.parent.transform.localScale = new Vector3(24, 24, 24);
                        }
                    }
                }
            }
            
        }
    }
    public void newPos(int pos)
    {
        Slot = pos;
    }
    public void StartPos(int pos)
    {
        Slot = pos;
    }
    public void onTheMove(int a)
    {
        moving = a;
    }
    public void changeColor()
    {
        if (transform.tag == "Red Box")
        {
            transform.tag = "Blue Box";
            transform.GetComponentsInChildren<MeshRenderer>(true)[0].sharedMaterial = mat2;
            transform.GetComponentsInChildren<MeshRenderer>(true)[2].sharedMaterial = mat2;
            transform.GetComponentInChildren<TextMesh>(true).color = blue;
        }
        else
        {
            transform.tag = "Red Box";
            transform.GetComponentsInChildren<Renderer>(true)[0].sharedMaterial = mat1;
            transform.GetComponentsInChildren<Renderer>(true)[2].sharedMaterial = mat1;
            transform.GetComponentInChildren<TextMesh>(true).color = red;
        }
    }
    public void canMove()
    {
        movable = true;
    }
    public void cantMove()
    {
        movable = false;
    }
    public void takeDamage()
    {
        hp--;
        shaketime = 0.3f + Time.time;
    }
}
