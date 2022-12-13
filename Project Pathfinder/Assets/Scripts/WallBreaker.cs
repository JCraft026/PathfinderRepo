using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreaker : MonoBehaviour
{
    // Called on awake
    void Awake(){
        ManageCrackedWalls.Instance.AddWalls(gameObject);
    }
}