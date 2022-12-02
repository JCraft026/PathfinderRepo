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


//=====================================================================
/* Plans for v2

DONE! - Attach script instead to CustomNetworkManager

DONE? - It has a gameObject that is assigned to it at the start
NOT  -     and interacted with by the Active Character Manager.

DONE? - Give the script an AssignNewObject() function:

Assign(newGameObject)
{
    delayFogUpdate = true;
    HideAllCellsFromVector(gameObject's coordinates);
    gameObject = newGameObject;
    RevealAllCellsFromVector(gameObject's coordinates);
    delayFogUpdate = false;
}

// while delayFogUpdate is set to true, Update() will just return.


//*///=================================================================



public class KLM_FogController : MonoBehaviour
{
    public GameObject    mazeRenderer;
    public GameObject    mazeEntity;
    public WallStatus[,] mazeData;
    
    // Internal variables.
    private GameObject[]  fogObjectsGrid;
    private int           oldEntityX = 0;
    private int           oldEntityY = 0;
    private int           newEntityX = 0;
    private int           newEntityY = 0;
    private bool          delayFogUpdate = false;
    
    // External Parameters (and their defaults).
    private int   mazeWidth;//  = 13;
    private int   mazeHeight;// = 13;
    private float cellSize;//   = 5;
    
    // Gets the fog game object located at the specified grid coordinates.
    GameObject getFogObject(int x, int y)
    { return fogObjectsGrid[x + mazeWidth * y]; }
    
    // Reveals the cell by making the fog invisible.
    void RevealCell(int x, int y)
    {
        Debug.Log("Reveal ( " + x + " , " + y + " )");
        //GameObject fogObject = getFogObject(x,y);
        //fogObject.transform.position = new Vector3(
        //    fogObject.transform.position.x, 
        //    fogObject.transform.position.y, 
        //    100
        //);
        return;
    }
    
    // Hides the cell by obscuring it with fog.
    void HideCell(int x, int y)
    {
        Debug.Log("Hide ( " + x + " , " + y + " )");
        //GameObject fogObject = getFogObject(x,y);
        //fogObject.transform.position = new Vector3(
        //    fogObject.transform.position.x, 
        //    fogObject.transform.position.y, 
        //    -10
        //);
        return;
    }
    
    // Hides all cells north, south, east, and west of a coordinate (until it hits a wall).
    void HideAllCellsFromVector(int x, int y)
    {
        HideCell(x,y);
        int i = 0; // Iterator variable.
        
        // Hide all cells north of the location until a wall is hit.
        if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.TOP))
        {
            for (i=1; oldEntityY+i < mazeHeight; i++)
            {
                HideCell(oldEntityX, oldEntityY+i);
                if (mazeData[oldEntityX,oldEntityY+i].HasFlag(WallStatus.TOP))
                    break;
            }
        }
        
        // Hide all cells south of the location until a wall is hit.
        if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.BOTTOM))
        {
            for (i=-1; oldEntityY+i >= 0; i--)
            {
                HideCell(oldEntityX, oldEntityY+i);
                if (mazeData[oldEntityX,oldEntityY+i].HasFlag(WallStatus.BOTTOM))
                    break;
            }
        }
        
        // Hide all cells east of the location until a wall is hit.
        if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.RIGHT))
        {
            for (i=1; oldEntityX+i < mazeWidth; i++)
            {
                HideCell(oldEntityX+i, oldEntityY);
                if (mazeData[oldEntityX+i,oldEntityY].HasFlag(WallStatus.RIGHT))
                    break;
            }
        }
        
        // Hide all cells west of the location until a wall is hit.
        if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.LEFT))
        {
            for (i=-1; oldEntityX+i >= 0; i--)
            {
                HideCell(oldEntityX+i, oldEntityY);
                if (mazeData[oldEntityX+i,oldEntityY].HasFlag(WallStatus.LEFT))
                    break;
            }
        }
        
        return;
    }
    
    // Reveals all cells north, south, east, and west of a coordinate (until it hits a wall).
    void RevealAllCellsFromVector(int x, int y)
    {
        RevealCell(x, y);
        int i = 0; // Iterator variable.
        
        // Reveal all cells north of the location until a wall is hit.
        for (i=0; y+i < mazeHeight; i++)
        {
            RevealCell(x, y+i);
            Debug.Log(mazeData);
            Debug.Log(mazeData[x,y+i]);
            if (mazeData[x,y+i].HasFlag(WallStatus.TOP))
                break;
        }
        
        // Reveal all cells south of the location until a wall is hit.
        for (i=0; y+i >= 0; i--)
        {
            RevealCell(x, y+i);
            if (mazeData[x,y+i].HasFlag(WallStatus.BOTTOM))
                break;
        }
        
        // Reveal all cells east of the location until a wall is hit.
        for (i=0; x+i < mazeHeight; i++)
        {
            RevealCell(x+i, y);
            if (mazeData[x+i,y].HasFlag(WallStatus.RIGHT))
                break;
        }
        
        // Reveal all cells west of the location until a wall is hit.
        for (i=0; x+i >= 0; i--)
        {
            RevealCell(x+i, y);
            if (mazeData[x+i,y].HasFlag(WallStatus.LEFT))
                break;
        }
        
        return;
    }
    
    // Converts a transformX coordinate into a gridX coordinate.
    int getGridX(float transformX)
    {
        Debug.Log("cellSize = " + cellSize);
        Debug.Log("mazeWidth = " + mazeWidth);
        
        return Mathf.FloorToInt(
            transformX / ((float)cellSize)
            + ((float)mazeWidth) / 2f
            + (mazeWidth % 2 == 0 ? 0.5f : 0));
    }
    
    // Converts a transformXY coordinate into a gridY coordinate.
    int getGridY(float transformY)
    {
        return Mathf.FloorToInt(
            transformY / ((float)cellSize)
            + ((float)mazeHeight) / 2f
            + (mazeHeight % 2 == 0 ? 0.5f : 0));
    }
    
    public void initializeNewEntity(GameObject newMazeEntity)
    {
        delayFogUpdate = true;
        
        // If there was an old entity, hide old maze cells.
        if (true) // (mazeEntity != null)
        {
            Debug.Log(
                "GameSpace = ( " + newMazeEntity.transform.position.x
                + " , " + newMazeEntity.transform.position.y + " )"
            );
            //HideAllCellsFromVector(
            //    getGridX(mazeEntity.transform.position.x),
            //    getGridY(mazeEntity.transform.position.y)
            //);
        }
        mazeEntity = newMazeEntity;
        
        oldEntityX = newEntityX = getGridX(newMazeEntity.transform.position.x);
        oldEntityY = newEntityY = getGridY(newMazeEntity.transform.position.y);
        Debug.Log("newEntity X,Y --- " + newEntityX + " , " + newEntityY);
        RevealAllCellsFromVector(newEntityX, newEntityY);
        
        delayFogUpdate = false;
    }
    
    // When the mazeData is generated/loaded, this is called.
    public void initializeFogRenderer()
    {
        mazeHeight = mazeRenderer.GetComponent<RenderMaze>().mazeHeight;
        mazeWidth = mazeRenderer.GetComponent<RenderMaze>().mazeWidth;
        cellSize = mazeRenderer.GetComponent<RenderMaze>().cellSize;
        mazeData = (mazeRenderer.GetComponent<RenderMaze>().GetMazeWallData());
        Debug.Log(mazeData);
        
        fogObjectsGrid = GameObject.FindGameObjectsWithTag("FogOfWar");
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //mazeEntity   = gameObject;//GameObject.Find("Runner [connId=0]");
        if (mazeRenderer == null)
            mazeRenderer = GameObject.Find("MazeRenderer");
        
        if (mazeRenderer != null)
            initializeFogRenderer();
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if ((mazeEntity == null) || delayFogUpdate)
            return;
        
        newEntityX = getGridX(mazeEntity.transform.position.x);
        newEntityY = getGridY(mazeEntity.transform.position.y);
        
        int i = 0; // Iterator
        
        // Has the entity's X-coordinate changed?
        if (newEntityX != oldEntityX)
        {
            // Hide all cells north of Entity's old location until a wall is hit.
            if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.TOP))
            {
                for (i=1; oldEntityY+i < mazeHeight; i++)
                {
                    HideCell(oldEntityX, oldEntityY+i);
                    if (mazeData[oldEntityX,oldEntityY+i].HasFlag(WallStatus.TOP))
                        break;
                }
            }
            
            // Hide all cells south of Entity's old location until a wall is hit.
            if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.BOTTOM))
            {
                for (i=-1; oldEntityY+i >= 0; i--)
                {
                    HideCell(oldEntityX, oldEntityY+i);
                    if (mazeData[oldEntityX,oldEntityY+i].HasFlag(WallStatus.BOTTOM))
                        break;
                }
            }
            
            // Reveal all cells north of Entity's new location until a wall is hit.
            for (i=0; newEntityY+i < mazeHeight; i++)
            {
                RevealCell(newEntityX, newEntityY+i);
                if (mazeData[newEntityX,newEntityY+i].HasFlag(WallStatus.TOP))
                    break;
            }
            
            // Reveal all cells south of Entity's new location until a wall is hit.
            for (i=0; newEntityY+i >= 0; i--)
            {
                RevealCell(newEntityX, newEntityY+i);
                if (mazeData[newEntityX,newEntityY+i].HasFlag(WallStatus.BOTTOM))
                    break;
            }
        }
        
        // Has the Entity's Y-coordinate changed?
        if (newEntityY != oldEntityY)
        {
            // Hide all cells east of Entity's old location until a wall is hit.
            if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.RIGHT))
            {
                for (i=1; oldEntityX+i < mazeWidth; i++)
                {
                    HideCell(oldEntityX+i, oldEntityY);
                    if (mazeData[oldEntityX+i,oldEntityY].HasFlag(WallStatus.RIGHT))
                        break;
                }
            }
            
            // Hide all cells west of Entity's old location until a wall is hit.
            if (!mazeData[oldEntityX,oldEntityY].HasFlag(WallStatus.LEFT))
            {
                for (i=-1; oldEntityX+i >= 0; i--)
                {
                    HideCell(oldEntityX+i, oldEntityY);
                    if (mazeData[oldEntityX+i,oldEntityY].HasFlag(WallStatus.LEFT))
                        break;
                }
            }
            
            // Reveal all cells east of Entity's new location until a wall is hit.
            for (i=0; newEntityX+i < mazeWidth; i++)
            {
                RevealCell(newEntityX+i, newEntityY);
                if (mazeData[newEntityX+i,newEntityY].HasFlag(WallStatus.RIGHT))
                    break;
            }
            
            // Reveal all cells west of Entity's new location until a wall is hit.
            for (i=0; newEntityX+i >= 0; i--)
            {
                RevealCell(newEntityX+i, newEntityY);
                if (mazeData[newEntityX+i,newEntityY].HasFlag(WallStatus.LEFT))
                    break;
            }
        }
        
        // If the Entity changed both X and Y coordinates, hide their old cell.
        if ((newEntityX != oldEntityX) && (newEntityY != oldEntityY))
            HideCell(oldEntityX, oldEntityY);
        
        oldEntityX = newEntityX;
        oldEntityY = newEntityY;
        
    }
    
}
