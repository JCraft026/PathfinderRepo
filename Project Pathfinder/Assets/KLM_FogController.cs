using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=====================================================================
/* Keegan's Developer Notes

Bugs:
- Runner freaks out very intensely when grid height/width is large.

Future Considerations:
- What will update the Fog of War if a wall is broken
  inside the Runner's line of sight?
- Will there be rooms where vision will not be restricted to lines?
  Such as a 2x2 room? In such a case, current algorithm would fail.

//*///=================================================================

public class KLM_FogController : MonoBehaviour
{
    public GameObject    mazeRenderer;
    public GameObject    mazeRunner;
    public WallStatus[,] mazeData;
    
    // Internal variables.
    private GameObject[]  fogObjectsGrid;
    private int           oldRunnerX = 0;
    private int           oldRunnerY = 0;
    private int           newRunnerX = 0;
    private int           newRunnerY = 0;
    
    // Internal Parameters
    //private float x_offset = 0.00f;
    //private float y_offset = 0.00f;
    
    // External Parameters (and their defaults).
    private int   mazeWidth;
    private int   mazeHeight;
    private float cellSize;
    
    // Gets the fog object located at the specified coordinates.
    GameObject getFogObject(int x, int y)
    { return fogObjectsGrid[x + mazeWidth * y]; }
    
    // Reveals the cell by making the fog invisible.
    void RevealCell(int x, int y)
    {
        GameObject fogObject = getFogObject(x,y);
        fogObject.transform.position = new Vector3(
            fogObject.transform.position.x, 
            fogObject.transform.position.y, 
            100
        );
        return;
    }
    
    // Hides the cell by obscuring it with fog.
    void HideCell(int x, int y)
    {
        GameObject fogObject = getFogObject(x,y);
        fogObject.transform.position = new Vector3(
            fogObject.transform.position.x, 
            fogObject.transform.position.y, 
            -10
        );
        return;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //mazeRunner   = GameObject.Find("Runner [connId=0]");
        mazeRenderer = GameObject.Find("MazeRenderer");
        
        mazeHeight = mazeRenderer.GetComponent<RenderMaze>().mazeHeight;
        mazeWidth = mazeRenderer.GetComponent<RenderMaze>().mazeWidth;
        cellSize = mazeRenderer.GetComponent<RenderMaze>().cellSize;
        mazeData = (mazeRenderer.GetComponent<RenderMaze>().GetMazeWallData());
        
        oldRunnerX = newRunnerX = Mathf.FloorToInt(
            (mazeRunner.transform.position.x) / ((float)cellSize)
            + ((float)mazeWidth) / 2f
            + (mazeWidth % 2 == 0 ? 0.5f : 0)
        );
        
        oldRunnerY = newRunnerY = Mathf.FloorToInt(
            (mazeRunner.transform.position.y) / ((float)cellSize)
            + ((float)mazeHeight) / 2f
            + (mazeHeight % 2 == 0 ? 0.5f : 0)
        );
        
        fogObjectsGrid = GameObject.FindGameObjectsWithTag("FogOfWar");
        
        // Initialize the revealed cells.
        RevealCell(newRunnerX, newRunnerY);
        int i = 0; // Iterator variable.
        
        // Reveal all cells north of Runner's location until a wall is hit.
        for (i=0; newRunnerY+i < mazeHeight; i++)
        {
            RevealCell(newRunnerX, newRunnerY+i);
            if (mazeData[newRunnerX,newRunnerY+i].HasFlag(WallStatus.TOP))
                break;
        }
        
        // Reveal all cells south of Runner's location until a wall is hit.
        for (i=0; newRunnerY+i >= 0; i--)
        {
            RevealCell(newRunnerX, newRunnerY+i);
            if (mazeData[newRunnerX,newRunnerY+i].HasFlag(WallStatus.BOTTOM))
                break;
        }
        
        // Reveal all cells east of Runner's location until a wall is hit.
        for (i=0; newRunnerX+i < mazeHeight; i++)
        {
            RevealCell(newRunnerX+i, newRunnerY);
            if (mazeData[newRunnerX+i,newRunnerY].HasFlag(WallStatus.RIGHT))
                break;
        }
        
        // Reveal all cells west of Runner's location until a wall is hit.
        for (i=0; newRunnerX+i >= 0; i--)
        {
            RevealCell(newRunnerX+i, newRunnerY);
            if (mazeData[newRunnerX+i,newRunnerY].HasFlag(WallStatus.LEFT))
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        newRunnerX = Mathf.FloorToInt(
            (mazeRunner.transform.position.x) / ((float)cellSize)
            + ((float)mazeWidth) / 2f
            + (mazeWidth  % 2 == 0 ? 0.5f : 0)
        );
        
        newRunnerY = Mathf.FloorToInt(
            (mazeRunner.transform.position.y) / ((float)cellSize)
            + ((float)mazeHeight) / 2f
            + (mazeHeight % 2 == 0 ? 0.5f : 0)
        );
        
        int i = 0; // iterator
        
        //=============================================================
        // Reveal/hide cells algorithms.
        //=============================================================
        
        Debug.Log(
            "[" + newRunnerX + ", " + newRunnerY + "]"
            + " = "
            + "(" + mazeRunner.transform.position.x + ", " + mazeRunner.transform.position.y + ")"
        );
        
        // Has the runner's X-coordinate changed?
        if (newRunnerX != oldRunnerX)
        {
            // Hide all cells north of Runner's old location until a wall is hit.
            if (!mazeData[oldRunnerX,oldRunnerY].HasFlag(WallStatus.TOP))
            {
                for (i=1; oldRunnerY+i < mazeHeight; i++)
                {
                    HideCell(oldRunnerX, oldRunnerY+i);
                    if (mazeData[oldRunnerX,oldRunnerY+i].HasFlag(WallStatus.TOP))
                        break;
                }
            }
            
            // Hide all cells south of Runner's old location until a wall is hit.
            if (!mazeData[oldRunnerX,oldRunnerY].HasFlag(WallStatus.BOTTOM))
            {
                for (i=-1; oldRunnerY+i >= 0; i--)
                {
                    HideCell(oldRunnerX, oldRunnerY+i);
                    if (mazeData[oldRunnerX,oldRunnerY+i].HasFlag(WallStatus.BOTTOM))
                        break;
                }
            }
            
            // Reveal all cells north of Runner's new location until a wall is hit.
            for (i=0; newRunnerY+i < mazeHeight; i++)
            {
                RevealCell(newRunnerX, newRunnerY+i);
                if (mazeData[newRunnerX,newRunnerY+i].HasFlag(WallStatus.TOP))
                    break;
            }
            
            // Reveal all cells south of Runner's new location until a wall is hit.
            for (i=0; newRunnerY+i >= 0; i--)
            {
                RevealCell(newRunnerX, newRunnerY+i);
                if (mazeData[newRunnerX,newRunnerY+i].HasFlag(WallStatus.BOTTOM))
                    break;
            }
        }
        
        // Has the runner's Y-coordinate changed?
        if (newRunnerY != oldRunnerY)
        {
            // Hide all cells east of Runner's old location until a wall is hit.
            if (!mazeData[oldRunnerX,oldRunnerY].HasFlag(WallStatus.RIGHT))
            {
                for (i=1; oldRunnerX+i < mazeWidth; i++)
                {
                    HideCell(oldRunnerX+i, oldRunnerY);
                    if (mazeData[oldRunnerX+i,oldRunnerY].HasFlag(WallStatus.RIGHT))
                        break;
                }
            }
            
            // Hide all cells west of Runner's old location until a wall is hit.
            if (!mazeData[oldRunnerX,oldRunnerY].HasFlag(WallStatus.LEFT))
            {
                for (i=-1; oldRunnerX+i >= 0; i--)
                {
                    HideCell(oldRunnerX+i, oldRunnerY);
                    if (mazeData[oldRunnerX+i,oldRunnerY].HasFlag(WallStatus.LEFT))
                        break;
                }
            }
            
            // Reveal all cells east of Runner's new location until a wall is hit.
            for (i=0; newRunnerX+i < mazeWidth; i++)
            {
                RevealCell(newRunnerX+i, newRunnerY);
                if (mazeData[newRunnerX+i,newRunnerY].HasFlag(WallStatus.RIGHT))
                    break;
            }
            
            // Reveal all cells west of Runner's new location until a wall is hit.
            for (i=0; newRunnerX+i >= 0; i--)
            {
                RevealCell(newRunnerX+i, newRunnerY);
                if (mazeData[newRunnerX+i,newRunnerY].HasFlag(WallStatus.LEFT))
                    break;
            }
        }
        
        // If the Runner changed both X and Y coordinates, hide their old cell.
        if ((newRunnerX != oldRunnerX) && (newRunnerY != oldRunnerY))
            HideCell(oldRunnerX, oldRunnerY);
        
        oldRunnerX = newRunnerX;
        oldRunnerY = newRunnerY;
        
    }
    
}
