using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreaker : MonoBehaviour
{
    // Adds itself to a list of cracked wall objects
    void Awake(){
        ManageCrackedWalls.Instance.AddWalls(gameObject);
    }
}