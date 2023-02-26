using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public class ManageExitTunnelCollision : MonoBehaviour
{
    // Manage exit tunnel collisions
    void OnCollisionEnter2D(Collision2D collision){
        Regex runnerExpression = new Regex("Runner");
        
        if(runnerExpression.IsMatch(collision.gameObject.name)){
            var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
            var tunnelEntrance = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Tunnel_Entrance"));
            runner.transform.position = tunnelEntrance.transform.position - new Vector3(0,2,0);
        }
    }
}
