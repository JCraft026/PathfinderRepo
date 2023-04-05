using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class EMPController : NetworkBehaviour
{

    public GameObject engineer{
        get
        {
            return GameObject.Find("Engineer(Clone)");
        }
    }
    public GameObject chaser{
        get
        {
            return GameObject.Find("Chaser(Clone)");
        }
    }
    public GameObject trapper{
        get
        {
            return GameObject.Find("Trapper(Clone)");
        }
    }

    public GameObject EMP;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Utilities.GetDistanceBetweenObjects(gameObject.transform.position, engineer.transform.position) < 5.0f){
            Debug.Log("Hit The Engineer");
        }
        if(Utilities.GetDistanceBetweenObjects(gameObject.transform.position, trapper.transform.position) < 5.0f){
            Debug.Log("Hit The Trapper");
        }
        if(Utilities.GetDistanceBetweenObjects(gameObject.transform.position, chaser.transform.position) < 5.0f){
            Debug.Log("Hit The Chaser");
        }
    }
}
