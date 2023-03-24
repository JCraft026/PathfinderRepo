using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCrackedWalls : MonoBehaviour
{
    private List<GameObject> crackedWallList;  // List of the cracked walls
    private GameObject closestWall;            // The closest wall to the runner
    public static ManageCrackedWalls Instance; // Access to an object of this class' methods and attributes
    private float cellSize = 0.0f;             // Used to adjust the detection size of wall breaking

    [SerializeField]
    RenderMaze mazeRenderer;

    // Start is called before the first frame update
    void Start(){
        Instance = this;
        crackedWallList = new List<GameObject>();
        cellSize = mazeRenderer.GetCellSize();
    }

    // Adds a cracked wall object to a list of cracked wall objects
    public void AddWalls(GameObject crackedWall){ 
        crackedWallList.Add(crackedWall);
    }

    // Returns the list of cracked wall objects
    public List<GameObject> GetWalls(){
        return crackedWallList;
    }

    // Displays each cracked wall's location in the scene
    public void PrintWalls(){
        foreach (GameObject crackedWall in crackedWallList){
            Debug.Log(crackedWall.transform.position);
        }
    }

    // Finds the closest wall to the player
    public void findClosestWall(){
        if(crackedWallList.Count >= 1)
        {
            float distanceFromPlayer = 100; // Total Distance From Player
            foreach(GameObject crackedWall in crackedWallList){
                if(Mathf.Sqrt(Mathf.Pow(MoveCharacter.Instance.rigidBody.position.x - crackedWall.transform.position.x, 2)
                            + Mathf.Pow(MoveCharacter.Instance.rigidBody.position.y - crackedWall.transform.position.y, 2))
                            < distanceFromPlayer){
                    distanceFromPlayer = Mathf.Sqrt(Mathf.Pow(MoveCharacter.Instance.rigidBody.position.x - crackedWall.transform.position.x, 2)
                            + Mathf.Pow(MoveCharacter.Instance.rigidBody.position.y - crackedWall.transform.position.y, 2));
                    closestWall = crackedWall;
                }
            }
        }
    }

    // Checks to see if the player is within range, then breaks the closest cracked wall
    public void breakWall(){
        if(crackedWallList.Count >= 1){
            if(Mathf.Abs(MoveCharacter.Instance.rigidBody.position.x - closestWall.transform.position.x) < cellSize * 0.25f
                && Mathf.Abs(MoveCharacter.Instance.rigidBody.position.y - closestWall.transform.position.y) < cellSize * 0.25f){
                GameObject.Find("MM" + closestWall.name.Substring(8)).SetActive(false);
                Destroy(closestWall);
                crackedWallList.Remove(closestWall);
            }
        }
    }
}