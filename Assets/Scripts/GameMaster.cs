using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameMaster : MonoBehaviour
{
    public List<GameObject> boxes;
    public List<GameObject> cores;
    public List<GameObject> balls;
    public List<Transform> boxPoints;
    public List<Transform> BallSpawnPoints;
    public ScrollingTexture[] lanetextscript;
    public MeshRenderer[] lanerenders;
    public Vector2[] abilitysInLane;
    public List<int> destroyedLanes;
    public GameObject blueBox;
    public GameObject redBox;
    public GameObject ghostBox;
    public GameObject blueBall;
    public GameObject redBall;
    public GameObject WallObj;
    public GameObject SlowObj;
    public GameObject healEffect;
    public GameObject MultiObj;
    public GameObject SwitchObj;
    public int blueScore;
    public int redScore;

    public float spawnRate;
    private float lastSpawn;
    public bool abilityActive;
    public float SlowDuration;
    public float WallDuration;
    public float MultiDuration;
    public float SwitchDuration;
    public float slowCD;
    public float wallCD;
    public float multiCD;
    public float switchCD;
    public float slowTime;
    public float wallTime;
    public float multiTime;
    public float switchTime;
    public Abilitys currentActive;
    public float slowSpeed;
    public float normalspeed;
    public int multiplier;
    public bool selectingAbility = false;
    public Color defaultcolor;
    public Color slowcolor;
    public Color wallcolor;
    public Color multicolor;
    public Color switchcolor;
    public Color healemissive;
    public Color blue;
    public Color red;
    public Color purple;


    public int slowRemaning;
    public int wallRemaning;
    public int switchRemaning;
    public int multiRemaning;
    private AbilityButton[] abb;
    public GameObject coreObj;
    private GameObject healslot;
    public bool GameOver = false;
    private float speedUpTime;
    private float spawnRateTime;

    public enum Abilitys
    {
        SLOW,WALL,MULTIPLIER,SWITCH,NONE
    };

    void Start()
    {
        ColorUtility.TryParseHtmlString("#00000000", out defaultcolor);
        ColorUtility.TryParseHtmlString("#121E3300", out slowcolor);
        ColorUtility.TryParseHtmlString("#330F0F00", out wallcolor);
        ColorUtility.TryParseHtmlString("#300B3300", out multicolor);
        ColorUtility.TryParseHtmlString("#33310E00", out switchcolor);
        ColorUtility.TryParseHtmlString("#002D0000", out healemissive);
        healEffect.SetActive(false);
        
        SlowObj = Instantiate(SlowObj, Vector3.zero, Quaternion.identity) as GameObject;
        SlowObj.SetActive(false);
        WallObj = Instantiate(WallObj, Vector3.zero, Quaternion.identity) as GameObject;
        WallObj.SetActive(false);
        MultiObj = Instantiate(MultiObj, Vector3.zero, Quaternion.identity) as GameObject;
        MultiObj.SetActive(false);
        SwitchObj = Instantiate(SwitchObj, Vector3.zero, Quaternion.identity) as GameObject;
        SwitchObj.SetActive(false);

        speedUpTime = Time.time + 10;

        healslot = GameObject.Find("mesh_heal_slot_new");
        clear();
        initalize();
    }
    public void Reset()
    {
        clear();
        initalize();
        blueScore = 0;
        redScore = 0;
        slowTime = 0;
        wallTime = 0;
        multiTime = 0;
        switchTime = 0;
        normalspeed = 0.75f;
        destroyedLanes.Clear();
        GameOver = false;
        speedUpTime = Time.time + 10;
        for (int i = 0; i < lanetextscript.Length; i++)
        {
            if (lanetextscript[i].scrollSpeed != 0)
            {
                lanetextscript[i].scrollSpeed = -0.144f;
                lanetextscript[i].scrollSpeed2 = -0.15f;
            }
        }
    }
    void clear()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            DestroyImmediate(boxes[i]);
        }
        for (int i = 0; i < balls.Count; i++)
        {
            DestroyImmediate(balls[i]);
        }
        for (int i = 0; i < cores.Count; i++)
        {
            DestroyImmediate(cores[i]);
        }
        boxes.Clear();
        balls.Clear();
        cores.Clear();

    }
    void initalize()
    {
        Transform parent = GameObject.Find("Ball Objects").transform;
        lanetextscript = transform.GetComponentsInChildren<Transform>().Where(x => x.tag == "Lane").Select(x => x.transform.GetComponent<ScrollingTexture>()).ToArray();
        lanerenders = transform.GetComponentsInChildren<Transform>().Where(x => x.tag == "Lane").Select(x => x.transform.GetComponent<MeshRenderer>()).ToArray();
        abb = transform.GetComponentsInChildren<AbilityButton>();
        
        for (int i = 0; i < boxPoints.Count; i++)
        {
            if (i < 3)
                boxes.Add(Instantiate(blueBox, boxPoints[i].position, Quaternion.identity) as GameObject);
            else if (i < 6 && i > 2)
                boxes.Add(Instantiate(redBox, boxPoints[i].position, Quaternion.identity) as GameObject);
            else
            {
                boxes.Add(Instantiate(ghostBox, boxPoints[i].position, Quaternion.identity) as GameObject);
            }
            if (i != 6)
                boxes[i].transform.GetComponent<Box>().StartPos(i);
        }
        for (int i = 0; i < boxPoints.Count - 1; i++)
        {
            cores.Add(Instantiate(coreObj, boxPoints[i].position + new Vector3(0, 0, 0.1f), Quaternion.identity) as GameObject);
        }
        for (int i = 0; i < 20; i++)
        {
            if (i < 10)
            {
                balls.Add(Instantiate(blueBall, Vector3.zero, Quaternion.identity) as GameObject);
            }
            else if (i < 20 && i > 9)
            {
                balls.Add(Instantiate(redBall, Vector3.zero, Quaternion.identity) as GameObject);

            }
            balls[i].SetActive(false);
            balls[i].transform.parent = parent;
        }
    }
    void Update()
    {
        if(GameOver)
        {
            return;
        }
        if (destroyedLanes.Count == 5)
        {
            for (int i = 0; i < lanetextscript.Length; i++)
            {
                lanetextscript[i].scrollSpeed = 0;
                lanetextscript[i].scrollSpeed2 = 0;
            }
            for(int i = 0; i < balls.Count;i++)
            {
                balls[i].SetActive(false);
            }
            GameOver = true;
            return;
        }

        if (Time.time > lastSpawn)
        {
            for (int i = 0; i < abilitysInLane.Length; i++)
            {
                if(abilitysInLane[i].y == 1  && abilitysInLane[i].x < Time.time)
                {
                    if(i < 6)
                    {
                        slowReset(i);
                    }
                    else if(i <12 && i>5)
                    {
                        wallReset(i-6);
                    }
                    else if (i < 18 && i > 11)
                    {
                        multiReset(i-12);
                    }
                }
            }

            if (UnityEngine.Random.Range(0, 1) == 0)//pattern
            {
                int spawnpos = getLane();
                int color = UnityEngine.Random.Range(0, 2);
                if (abilityActive && (abilitysInLane[spawnpos].y == 1 || abilitysInLane[spawnpos+12].y == 1))
                {
                    abilitySpawning(spawnpos, color);
                }
                else
                {
                    spawnNormal(spawnpos, normalspeed, color, false);
                }
            }
            lastSpawn = Time.time + spawnRate;
        }
        
        if(speedUpTime < Time.time)
        {
            speedUp();
            speedUpTime = Time.time + 24;
            for (int i = 0; i < lanetextscript.Length; i++)
            {
                lanetextscript[i].scrollSpeed -= 0.0096f;
                lanetextscript[i].scrollSpeed2 -= 0.01f;
            }
        }
        if (spawnRateTime < Time.time)
        {
            spawnrateUp();
            spawnRateTime = Time.time + 30f;
        }

        if (slowTime < Time.time)
        {
            abb[0].isCD(false);

        }
        if (wallTime < Time.time)
        {
            abb[1].isCD(false);
        }
        if (multiTime < Time.time)
        {
            abb[2].isCD(false);
        }
        if (switchTime < Time.time)
        {
            abb[3].isCD(false);
        }
        if ((switchTime - Time.time - switchCD) < -0.7f)
        {
            SwitchObj.SetActive(false);
        }

        if (boxes[6].tag == "Blue Box" || boxes[6].tag == "Red Box")
        {
            healEffect.SetActive(true);
            if(healslot.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor") != healemissive)
                healslot.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", healemissive);
        }
        else  if(healEffect.activeSelf)
        {
            healEffect.SetActive(false);
            if (healslot.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor") == healemissive)
                healslot.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", defaultcolor);
        }
        

    }
    void speedUp()
    {
        if(normalspeed < 1.96f)
            normalspeed += 0.05f;
    }
    void spawnrateUp()
    {
        if (spawnRate > 0.54f)
            spawnRate -= 0.05f;
    }
    public void unLockBoxes(int slot)
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].GetComponent<Box>().canMove();
        }
    }
    public void LockBoxes(int slot)
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            if(i != slot)
                boxes[i].GetComponent<Box>().cantMove();
        }
    }
    int getLane()
    {
        int temp = UnityEngine.Random.Range(0,6);
        if(destroyedLanes.Contains(temp))
        {
            temp = getLane();
        } 
        return temp;
    }
    public void destroyLane(int slot)
    {
        destroyedLanes.Add(slot);
        lanetextscript[slot].scrollSpeed = 0;
        lanetextscript[slot].scrollSpeed2 = 0;
        cores[slot].SetActive(false);
        for (int i = 0; i <balls.Count; i++)
        {
            if(balls[i].transform.GetComponent<Ball>().lane == slot)
            {
                balls[i].SetActive(false);
            }
        }
    }
    void slowReset(int lane)
    {

        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].activeSelf && balls[i].transform.GetComponent<Ball>().lane == lane)
            {
                balls[i].GetComponent<Rigidbody>().velocity = new Vector3(0, balls[i].GetComponent<Rigidbody>().velocity.y * 2, 0);
            }
        }
        lanetextscript[lane].scrollSpeed *= 2;
        lanetextscript[lane].scrollSpeed2 *= 2;
        lanerenders[lane].material.SetColor("_EmissionColor", defaultcolor);
        slowTime = Time.time + slowCD;
        slowRemaning = Mathf.FloorToInt(slowTime);
        abb[0].isCD(true);
        abb[0].active = false;
        abb[0].setColor();
        abilitysInLane[lane].y = 0;
        SlowObj.SetActive(false);
    }
    void wallReset(int lane)
    {
        WallObj.SetActive(false);
        lanerenders[lane].material.SetColor("_EmissionColor", defaultcolor);
        abilitysInLane[lane + 6].y = 0;
        wallTime = Time.time + wallCD;
        wallRemaning= Mathf.FloorToInt(wallTime);
        abb[1].active = false;
        abb[1].isCD(true);
        abb[1].setColor();
    }
    void multiReset(int lane)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].activeSelf && balls[i].transform.GetComponent<Ball>().lane == lane)
            {
                if (balls[i].tag == "Red")
                    balls[i].GetComponentInChildren<ParticleSystem>().startColor = red;
                else
                    balls[i].GetComponentInChildren<ParticleSystem>().startColor = blue;
            }
        }
        MultiObj.SetActive(false);
        multiTime = Time.time + multiCD;
        multiRemaning = Mathf.FloorToInt(multiTime);
        lanerenders[lane].material.SetColor("_EmissionColor", defaultcolor);
        abilitysInLane[lane + 12].y = 0;
        abb[2].isCD(true);
        abb[2].active = false;
        abb[2].setColor();
    }
    public void highlightLanes(Abilitys ability)
    {
        currentActive = ability;
        selectingAbility = true;
        switch (currentActive)
        {
            case Abilitys.SLOW:
                abb[0].active = true;
                abb[0].setColor();
                break;
            case Abilitys.WALL:
                abb[1].active = true;
                abb[1].setColor();
                break;
            case Abilitys.MULTIPLIER:
                abb[2].active = true;
                abb[2].setColor();
                break;
            case Abilitys.SWITCH:
                abb[3].active = true;
                abb[3].setColor();
                break;
            default:
                break;
        }
    }
    public void unlightLanes()
    {
        currentActive = Abilitys.NONE;
        selectingAbility = false;
        switch (currentActive)
        {
            case Abilitys.SLOW:
                abb[0].active = false;
                abb[0].setColor();
                break;
            case Abilitys.WALL:
                abb[1].active = false;
                abb[1].setColor();
                break;
            case Abilitys.MULTIPLIER:
                abb[2].active = false;
                abb[2].setColor();
                break;
            case Abilitys.SWITCH:
                abb[3].active = false;
                abb[3].setColor();
                break;
            default:
                break;
        }

    }
    public void setLane(int lane)
    {
        if (selectingAbility)
        {
            activateAbility(currentActive, lane);
        }
    }
    void spawnNormal(int lane, float speed, int color, bool multi)
    {
        int ball = 0;
        if (color == 0)
        {
            ball = findInactiveBlue();
        }
        else if(color == 1)
        {
            ball = findInactiveRed();
        }
        
        balls[ball].transform.position = BallSpawnPoints[lane].position;
        balls[ball].transform.rotation = UnityEngine.Random.rotation;
        balls[ball].transform.GetComponent<Ball>().lane = lane;
        if (balls[ball].tag == "Red")
            balls[ball].GetComponentInChildren<ParticleSystem>().startColor = red;
        else
            balls[ball].GetComponentInChildren<ParticleSystem>().startColor = blue;
        if (lane < 3)
            balls[ball].GetComponent<Rigidbody>().velocity = new Vector3(0, -speed, 0);
        else
            balls[ball].GetComponent<Rigidbody>().velocity = new Vector3(0, speed, 0);
        balls[ball].SetActive(true);
    }
    public void abilitySpawning(int lane, int color)
    {
        if(abilitysInLane[lane+12].y == 1)
            spawnNormal(lane, normalspeed, color, true);
       else
            spawnNormal(lane, slowSpeed, color, false);
        
    }
    public void activateAbility(GameMaster.Abilitys a, int lane)
    {
        abilityActive = true;
        selectingAbility = false;        
        switch (a)
        {
            case Abilitys.SLOW:
                abilitysInLane[lane].x = Time.time + SlowDuration;
                abilitysInLane[lane].y = 1;
                slowRemaning = Mathf.FloorToInt(abilitysInLane[lane].x);
                
                for (int i = 0; i < balls.Count; i++)
                {
                    if(balls[i].activeSelf && balls[i].transform.GetComponent<Ball>().lane == lane)
                    {
                        balls[i].GetComponent<Rigidbody>().velocity = new Vector3(0, balls[i].GetComponent<Rigidbody>().velocity.y/2, 0);
                    }
                }
                lanetextscript[lane].scrollSpeed /= 2;
                lanetextscript[lane].scrollSpeed2 /= 2;
                
                lanerenders[lane].material.SetColor("_EmissionColor", slowcolor);
                SlowObj.SetActive(true);
                switch (lane)
                {
                    case 0:
                        SlowObj.transform.position = new Vector3(-2.07f,3,0);
                        SlowObj.transform.rotation = Quaternion.identity;
                        SlowObj.transform.GetComponentInChildren<ParticleSystem>().gravityModifier = -1;
                        break;
                    case 1:
                        SlowObj.transform.position = new Vector3(-0.64f, 3, 0);
                        SlowObj.transform.rotation = Quaternion.identity;
                        SlowObj.transform.GetComponentInChildren<ParticleSystem>().gravityModifier = -1;
                        break;
                    case 2:
                        SlowObj.transform.position = new Vector3(0.79f, 3, 0);
                        SlowObj.transform.rotation = Quaternion.identity;
                        SlowObj.transform.GetComponentInChildren<ParticleSystem>().gravityModifier = -1;
                        break;
                    case 3:
                        SlowObj.transform.position = new Vector3(-2f, -3, 0);
                        SlowObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        SlowObj.transform.GetComponentInChildren<ParticleSystem>().gravityModifier = 1;
                        break;
                    case 4:
                        SlowObj.transform.position = new Vector3(-0.6f, -3, 0);
                        SlowObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        SlowObj.transform.GetComponentInChildren<ParticleSystem>().gravityModifier = 1;
                        break;
                    case 5:
                        SlowObj.transform.position = new Vector3(0.85f, -3, 0);
                        SlowObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        SlowObj.transform.GetComponentInChildren<ParticleSystem>().gravityModifier = 1;
                        break;
                    default:
                        break;
                }

                break;
            case Abilitys.WALL:
                abilitysInLane[lane + 6].x = Time.time + WallDuration;
                abilitysInLane[lane + 6].y = 1;
                wallRemaning = Mathf.FloorToInt(abilitysInLane[lane + 6].x);
                
                
                switch (lane)
                {
                    case 0:
                        WallObj.transform.position = new Vector3(-1.44f,0,0);
                        WallObj.transform.rotation = Quaternion.identity;

                        break;
                    case 1:
                        WallObj.transform.position = new Vector3(0, 0, 0);
                        WallObj.transform.rotation = Quaternion.identity;
                        break;
                    case 2:
                        WallObj.transform.position = new Vector3(1.44f, 0, 0);
                        WallObj.transform.rotation = Quaternion.identity;
                        break;
                    case 3:
                        WallObj.transform.position = new Vector3(-2.66f, 0, 0);
                        WallObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    case 4:
                        WallObj.transform.position = new Vector3(-1.2f, 0, 0);
                        WallObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    case 5:
                        WallObj.transform.position = new Vector3(0.21f, 0, 0);
                        WallObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    default:
                        break;
                }
                WallObj.SetActive(true);
                lanerenders[lane].material.SetColor("_EmissionColor", wallcolor);
                break;
            case Abilitys.MULTIPLIER:
                abilitysInLane[lane + 12].x = Time.time + MultiDuration;
                abilitysInLane[lane + 12].y = 1;
                multiRemaning = Mathf.FloorToInt(abilitysInLane[lane + 12].x);
                for (int i = 0; i < balls.Count; i++)
                {
                    if (balls[i].activeSelf && balls[i].transform.GetComponent<Ball>().lane == lane)
                    {
                        balls[i].GetComponentInChildren<ParticleSystem>().startColor = purple;
                    }
                }
                switch (lane)
                {
                    case 0:
                        MultiObj.transform.position = new Vector3(-2.07f, 0, 0);
                        MultiObj.transform.rotation = Quaternion.identity;

                        break;
                    case 1:
                        MultiObj.transform.position = new Vector3(-0.64f, 0, 0);
                        MultiObj.transform.rotation = Quaternion.identity;
                        break;
                    case 2:
                        MultiObj.transform.position = new Vector3(0.79f, 0, 0);
                        MultiObj.transform.rotation = Quaternion.identity;
                        break;
                    case 3:
                        MultiObj.transform.position = new Vector3(-2f, 0, 0);
                        MultiObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    case 4:
                        MultiObj.transform.position = new Vector3(-0.6f, 0, 0);
                        MultiObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    case 5:
                        MultiObj.transform.position = new Vector3(0.85f, 0, 0);
                        MultiObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    default:
                        break;
                }
                MultiObj.SetActive(true);
                lanerenders[lane].material.SetColor("_EmissionColor", multicolor);
                multiplier = 3;
                break;
            case Abilitys.SWITCH:
                boxes[lane].transform.GetComponent<Box>().changeColor();
                switchTime = Time.time + switchCD;
                switchRemaning = Mathf.FloorToInt(switchTime);
                SwitchObj.transform.position = boxPoints[lane].transform.position+ new Vector3(0,0,-2);
                SwitchObj.SetActive(true);
                abb[3].isCD(true);
                abb[3].active = false;
                abb[3].setColor();
                break;
            default:
                break;
        }

    }
    int findInactiveBlue()
    {
        for (int i = 0; i < balls.Count/2; i++)
        {
            if (!balls[i].activeSelf)
            {
                return i;
            }
        }
        return 0;
    }
    int findInactiveRed()
    {
        for (int i = balls.Count/2; i < balls.Count; i++)
        {
            if (!balls[i].activeSelf)
            {
                return i;
            }
        }
        return 0;
    }
    public void addRedScore(int lane)
    {
        if (abilitysInLane[lane + 12].y == 1)
            redScore += 1 * multiplier;
        else
            redScore++;
    }
    public void addBlueScore(int lane)
    {
        if (abilitysInLane[lane + 12].y == 1)
            blueScore += 1 * multiplier;
        else
            blueScore++;
    }
    public void swapBox(int CurrentPos, int newPos)
    {
        GameObject temp = boxes[newPos];
        boxes[newPos] = boxes[CurrentPos];
        boxes[CurrentPos] = temp;
        boxes[CurrentPos].transform.GetComponent<Box>().newPos(CurrentPos);
    }
}