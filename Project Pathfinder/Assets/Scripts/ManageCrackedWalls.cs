using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCrackedWalls : MonoBehaviour
{
    private List<GameObject> crackedWallList;   // List of the cracked walls
    private GameObject closestWall;             // The closest wall to the runner
    public static ManageCrackedWalls Instance; // Access to this class' methods and attributes

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        crackedWallList = new List<GameObject>();
    }

    public void AddWalls(GameObject crackedWall){ 
        crackedWallList.Add(crackedWall);
    }

    public List<GameObject> GetWalls(){
        return crackedWallList;
    }

    public void PrintWalls(){
        foreach (GameObject crackedWall in crackedWallList){
            Debug.Log(crackedWall.transform.position);
        }
    }

    public void findClosestWall(){
        if(crackedWallList.Count >= 1)
        {
            float distanceFromPlayerX = 100,   // 
                  distanceFromPlayerY = 100;   // 
            foreach(GameObject crackedWall in crackedWallList){
                if(Mathf.Abs(MoveCharacter.Instance.rigidBody.position.x - crackedWall.transform.position.x) < distanceFromPlayerX
                && Mathf.Abs(MoveCharacter.Instance.rigidBody.position.y - crackedWall.transform.position.y) < distanceFromPlayerY){
                    closestWall = crackedWall;
                    distanceFromPlayerX = Mathf.Abs(MoveCharacter.Instance.rigidBody.position.x - crackedWall.transform.position.x);
                    distanceFromPlayerY = Mathf.Abs(MoveCharacter.Instance.rigidBody.position.y - crackedWall.transform.position.y);
                }
            }
        }
    }

    public void breakWall(){
        if(crackedWallList.Count >= 1){
            if(Mathf.Abs(MoveCharacter.Instance.rigidBody.position.x - closestWall.transform.position.x) <  2
            && Mathf.Abs(MoveCharacter.Instance.rigidBody.position.y - closestWall.transform.position.y) <  2){
                Debug.Log("HEYO");
                Destroy(closestWall);
                crackedWallList.Remove(closestWall);
            }
        }
    }
}