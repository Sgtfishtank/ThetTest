using UnityEngine;
using System.Collections;

public class PhoneMove : MonoBehaviour
{
    private float maxPickingDistance = 10000;// increase if needed, depending on your scene size

    private Transform pickedObject = null;
    private GameMaster.Abilitys selectedAbility = GameMaster.Abilitys.NONE;
    private GameMaster GM;
    // Use this for initialization
    void Start ()
    {
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        foreach (Touch touch in Input.touches)
        {
            //Create horizontal plane
            Plane horPlane = new Plane(Vector3.up, Vector3.zero);

            //Gets the ray at position where the screen is touched
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, maxPickingDistance))
                {
                    if (hit.transform.tag == "Blue Box" || hit.transform.tag == "Red Box")
                    {
                        if (pickedObject == null)
                        {
                            pickedObject = hit.transform;
                            pickedObject.GetComponent<Box>().onTheMove(1);
                        }
                    }
                    else if(hit.transform.tag == "Ability Button")
                    {
                        if (selectedAbility != hit.transform.GetComponent<AbilityButton>().buttontype)
                        {
                            selectedAbility = hit.transform.GetComponent<AbilityButton>().buttontype;
                            GM.highlightLanes(selectedAbility);
                        }
                        else
                        {
                            selectedAbility = GameMaster.Abilitys.NONE;
                            GM.unlightLanes();
                        }

                    }
                    else if(hit.transform.tag == "Lane")
                    {
                        switch (hit.transform.name)
                        {
                            case "mesh_lane1":
                                GM.activateAbility(selectedAbility, 0);

                                break;
                            case "mesh_lane2":
                                GM.activateAbility(selectedAbility, 1);

                                break;
                            case "mesh_lane3":
                                GM.activateAbility(selectedAbility, 2);

                                break;
                            case "mesh_lane4":
                                GM.activateAbility(selectedAbility, 3);

                                break;
                            case "mesh_lane5":
                                GM.activateAbility(selectedAbility, 4);

                                break;
                            case "mesh_lane6":
                                GM.activateAbility(selectedAbility, 5);

                                break;
                            default:
                                break;
                        }
                        selectedAbility = GameMaster.Abilitys.NONE;
                    }
                    
                }
                else
                {
                    pickedObject = null;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (pickedObject != null)
                {
                    float distance1 = 0f;
                    if (horPlane.Raycast(ray, out distance1))
                    {
                        pickedObject.transform.position = ray.GetPoint(distance1) + new Vector3(0, 0, 1);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                pickedObject.GetComponent<Box>().onTheMove(2);
                pickedObject = null;
            }
        }
    }
}
