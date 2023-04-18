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
        if(!CustomNetworkManager.isRunner){
            if(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)")).GetComponent<ManageActiveCharacters>().activeGuardId != ManageActiveCharactersConstants.TRAPPER || Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)")).GetComponent<MoveCharacter>().isDisabled){
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else{
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    // Erase the track after a set number of seconds
    IEnumerator EraseTrack()
    {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
}
