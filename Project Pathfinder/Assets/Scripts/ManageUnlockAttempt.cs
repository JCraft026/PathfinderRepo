using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageUnlockAttempt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        if(CustomNetworkManager.isRunner && runner.GetComponent<Animator>().GetBool("Unlock Attempted") == true){
            Debug.Log("Unlock Exits");
            runner.GetComponent<Animator>().SetBool("Unlock Attempted", false);
        }
    }
}
