using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSmokeScreen : MonoBehaviour
{
    public static RenderSmokeScreen Instance;          // Makes an instance of this class to access attribtues

    // Called on Start
    void Start(){
        Instance = this;
    }

    // Update is called once per frame
    public void useSmoke(){
        Transform tranform = Instantiate(ItemAssets.Instance.SmokeScreen,
           MoveCharacter.Instance.rigidBody.position, Quaternion.identity);
    }

    void Awake(){
        Destroy(gameObject, 10.0f);
    }
}