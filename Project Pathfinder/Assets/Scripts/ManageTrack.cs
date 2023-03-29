using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageTrack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Erase the track a set time after spawning
        StartCoroutine(EraseTrack());
    }

    void Update(){
        if(!CustomNetworkManager.isRunner && Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)")).GetComponent<ManageActiveCharacters>().activeGuardId != ManageActiveCharactersConstants.TRAPPER){
            Destroy(gameObject);
        }
    }

    // Erase the track after a set number of seconds
    IEnumerator EraseTrack()
    {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
}
