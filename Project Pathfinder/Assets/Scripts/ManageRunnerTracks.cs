using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageRunnerTracks : MonoBehaviour
{
    public Transform runnerTrack; // Runner track object
    private float waitTime = 0.5f,   // Time in between spawning each track
                  nextSpawnTime;  // Next time to spawn a track

    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = Time.time + waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawnTime){
            nextSpawnTime += waitTime;
            if(!CustomNetworkManager.isRunner){
                var track      = Instantiate(runnerTrack, transform) as Transform;
                track.transform.SetParent(GameObject.Find("MazeRenderer").transform, false);
                track.position = gameObject.transform.position + new Vector3(0, -0.5f, 0);
                if(gameObject.GetComponent<Animator>().GetFloat("Facing Direction") == MoveCharacterConstants.LEFT || gameObject.GetComponent<Animator>().GetFloat("Facing Direction") == MoveCharacterConstants.RIGHT){
                    track.eulerAngles = new Vector3(0, 0, 90);
                }
            }
        }
    }
}
