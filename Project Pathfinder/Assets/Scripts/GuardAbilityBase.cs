using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GuardAbilityBase : NetworkBehaviour
{
    public MoveCharacter CharacterMovementController { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        CharacterMovementController = gameObject.GetComponent<MoveCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
