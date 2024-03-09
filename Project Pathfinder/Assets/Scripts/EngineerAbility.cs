using UnityEngine;
using Mirror;

public class EngineerAbility : GuardAbilityBase
{
    public GameObject barricadeHorizontal;       // Horizontal barricade
    public GameObject barricadeVertical;         // Vertical barricade
    private int[] engineerLocation;              // 2D array location (-6 - 6)
    private WallStatus currentCell;              // Contains data about the current cell the engineer is in
    private Vector3 placementDirection;          // The direction the engineer should place down its barricade
    private Vector3 barricadeLocation;           // The in scene location the barricade should be placed
    private Vector3 placementOrientation;        // The rotation the barricade needs based on facing direction
    private float   scaler = 6.9f;               // The default scaler to scale the barricades to the right size
    [SyncVar]
    public int barricadeCount = 0;               // Keeps track of the max number of barricades
    CustomNetworkManager customNetworkManager;   // CustomNetworkManager script instance

    public override float AbilityUseageCost => 25f;

    protected override void Start(){
        // Get engineer's MoveCharacter script
        base.Start();

        // Get parsed maze data from CustomNetworkManager
        customNetworkManager = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>();
        if(customNetworkManager.parsedMazeJson == null){
            Debug.LogError("Parsed Maze Data is null");
        }
    }

    protected override void DoAbility()
    {
        barricadeCount = GameObject.FindGameObjectsWithTag("Barricade").Length;
        if (barricadeCount < 3)
        {
            // Get engineer cell location
            engineerLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.ENGINEER);

            // Test for null maze Data
            if (customNetworkManager.parsedMazeJson == null)
                Debug.LogError("Parsed Maze Data is null");
            

            // Find the engineer's current cell (add 6 to each coordinate to match the orignal 2D array)
            currentCell = customNetworkManager.parsedMazeJson[engineerLocation[0] + 6, engineerLocation[1] + 6];
            Debug.Log(currentCell);

            // Assign the barricade location to the engineer as default
            barricadeLocation = CharacterMovementController.transform.position;

            // See if there is already a wall where the Engineer is facing
            switch (CharacterMovementController.facingDirection)
            {
                case 1f:
                    if (currentCell.HasFlag(WallStatus.BOTTOM))
                        DisplayAbilityAlert("Barricade cannot be placed on top of cell's bottom wall");
                    
                    else
                    {
                        placementDirection = new Vector2(0, -2.5f);
                        placementOrientation = new Vector3(0, 0, 0);
                        barricadeLocation = new Vector2(engineerLocation[0] * 8.0f, CharacterMovementController.transform.position.y);
                        scaler = 9f;
                        PlaceBarricade(1, placementDirection.x, placementDirection.y,
                            placementOrientation.x, placementOrientation.y, placementOrientation.z,
                            barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 1 to spawn Horizontal version
                                                                               // Subtract from steam
                        GenerateSteam.steam -= AbilityUseageCost;
                    }
                    break;
                case 2f:
                    if (currentCell.HasFlag(WallStatus.LEFT))
                        DisplayAbilityAlert("Barricade cannot be placed on top of cell's left wall");

                    else
                    {
                        placementDirection = new Vector2(-2.5f, 0f);
                        placementOrientation = new Vector3(0, 0, 90);
                        barricadeLocation = new Vector2(CharacterMovementController.transform.position.x, engineerLocation[1] * 8.0f);
                        scaler = 10.65f;
                        PlaceBarricade(0, placementDirection.x, placementDirection.y,
                            placementOrientation.x, placementOrientation.y, placementOrientation.z,
                            barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 0 to spawn Vertical version
                                                                               // Subtract from steam
                        GenerateSteam.steam -= AbilityUseageCost;
                    }
                    break;
                case 3f:
                    if (currentCell.HasFlag(WallStatus.TOP))
                       DisplayAbilityAlert("Barricade cannot be placed on top of cell's top wall");
                    else
                    {
                        placementDirection = new Vector2(0f, 2.5f);
                        placementOrientation = new Vector3(0, 0, 0);
                        barricadeLocation = new Vector2(engineerLocation[0] * 8.0f, CharacterMovementController.transform.position.y);
                        scaler = 9f;
                        PlaceBarricade(1, placementDirection.x, placementDirection.y,
                                placementOrientation.x, placementOrientation.y, placementOrientation.z,
                                barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 1 to spawn Horizontal version
                                                                                   // Subtract from steam
                        GenerateSteam.steam -= AbilityUseageCost;
                    }
                    break;
                case 4f:
                    if (currentCell.HasFlag(WallStatus.RIGHT))
                        DisplayAbilityAlert("Barricade cannot be placed on top of cell's right wall");
                    
                    else
                    {
                        placementDirection = new Vector2(2.5f, 0f);
                        placementOrientation = new Vector3(0, 0, 90);
                        barricadeLocation = new Vector2(CharacterMovementController.transform.position.x, engineerLocation[1] * 8.0f);
                        scaler = 10.65f;
                        PlaceBarricade(0, placementDirection.x, placementDirection.y,
                            placementOrientation.x, placementOrientation.y, placementOrientation.z,
                            barricadeLocation.x, barricadeLocation.y, scaler); // Flag of 0 to spawn Vertical version

                        // Subtract from steam
                        GenerateSteam.steam -= AbilityUseageCost;
                    }
                    break;
                default:
                    DisplayAbilityAlert("Unable to place barricade");
                    break;
            }
        }
        else
            DisplayAbilityAlert("Active barricade limit reached");
    }

    // Breaks down vectors to floats to instantiate and spawn the barricades
    [Command]
    public void PlaceBarricade(int axis, float placementDirectionX, float placementDirectionY,
                               float placementOrientationX, float placementOrientationY, float placementOrientationZ,
                               float barricadeLocationX, float barricadeLocationY, float scaler){
        GameObject tempBarricade;

        // If horizontal barricade needed
        if(axis == 1){
            tempBarricade = Instantiate(barricadeHorizontal, new Vector3(barricadeLocationX, barricadeLocationY, 0) + 
               new Vector3(placementDirectionX, placementDirectionY, 0), 
               Quaternion.Euler(new Vector3(placementOrientationX, placementOrientationY, placementOrientationZ)));
            tempBarricade.transform.localScale = new Vector2(tempBarricade.transform.localScale.x * scaler, tempBarricade.transform.localScale.y * scaler);
        }
        // If vertical barricade needed
        else{
            tempBarricade = Instantiate(barricadeVertical, new Vector3(barricadeLocationX, barricadeLocationY, 0) + 
               new Vector3(placementDirectionX, placementDirectionY, 0), 
               Quaternion.Euler(new Vector3(placementOrientationX, placementOrientationY, placementOrientationZ)));
            tempBarricade.transform.localScale = new Vector2(tempBarricade.transform.localScale.x * scaler, tempBarricade.transform.localScale.y * scaler);
        }
        NetworkServer.Spawn(tempBarricade);

        this.rpc_PlaySyncedAbilityAudio();
        return;
    }
}
