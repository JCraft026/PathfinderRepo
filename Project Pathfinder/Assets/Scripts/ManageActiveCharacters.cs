using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ManageActiveCharacters : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Vector3 offset;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        cameraHolder.SetActive(true);
    }

    public void Update()
    {
        cameraHolder.transform.position = transform.position + offset;
    }
}
