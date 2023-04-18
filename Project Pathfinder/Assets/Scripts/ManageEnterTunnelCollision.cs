using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public class ManageEnterTunnelCollision : MonoBehaviour
{
    // Manage enter tunnel collisions
    void OnCollisionEnter2D(Collision2D collision){
        Regex runnerExpression = new Regex("Runner");
        
        if(runnerExpression.IsMatch(collision.gameObject.name)){
            var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
            var controlRoom = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("crcf"));
            runner.transform.position = controlRoom.transform.position - new Vector3(0,1,0);
        }
    }
}
