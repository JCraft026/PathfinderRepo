using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : NetworkBehaviour
{
    public Animator animator;     // Character's animator manager

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer){
            if(Input.GetKeyDown("e")){
                animator.SetTrigger("Attack");
            }
        }
    }
}
