using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class EMPController : NetworkBehaviour
{
    public SpriteRenderer engineerDisableEffect;
    public SpriteRenderer trapperDisableEffect;
    public SpriteRenderer chaserDisableEffect;
    public AudioSource EmpSound;


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

    void Awake()
    {
        EmpSound.Play();
        engineerDisableEffect = engineer.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault<SpriteRenderer>(x => x.gameObject.name == "DisableEffect");
        trapperDisableEffect  =  trapper.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault<SpriteRenderer>(x => x.gameObject.name == "DisableEffect");
        chaserDisableEffect   =   chaser.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault<SpriteRenderer>(x => x.gameObject.name == "DisableEffect");
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(engineerDisableEffect.enabled == false && Utilities.GetDistanceBetweenObjects(gameObject.transform.position, engineer.transform.position) < 5.0f){
            Debug.Log("Hit The Engineer");
            engineerDisableEffect.enabled = true;
            engineer.GetComponent<MoveCharacter>().startDisableGuard();
        }
        if(trapperDisableEffect.enabled == false && Utilities.GetDistanceBetweenObjects(gameObject.transform.position, trapper.transform.position) < 5.0f){
            Debug.Log("Hit The Trapper");
            trapperDisableEffect.enabled = true;
            trapper.GetComponent<MoveCharacter>().startDisableGuard();
        }
        if(chaserDisableEffect.enabled == false && Utilities.GetDistanceBetweenObjects(gameObject.transform.position, chaser.transform.position) < 5.0f){
            Debug.Log("Hit The Chaser");
            chaserDisableEffect.enabled = true;
            chaser.GetComponent<MoveCharacter>().startDisableGuard();
        }
    }
}
