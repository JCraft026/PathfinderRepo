using UnityEngine;
using Mirror;
using System.Linq;
using System.Text.RegularExpressions;

public class BarricadeController : NetworkBehaviour
{
    EngineerAbility engineerAbility; // Instance of the engineer ability script
     GameObject trapper,             // Gameobject instance of the trapper
                engineer,            // Gameobject instance of the engineer
                chaser,              // Gameobject instance of the chaser
                runner;              // Gameobject instance of the runner
    public GameObject healthTop,     // Top health bar
                      healthBottom;  // Bottom health bar
    int hitCount = 0;                // Total number of hits on barricades attacked by a runner
    bool trapperTooltip = false,     // Whether the tooltip to destroy the barricade is active for the trapper
         engineerTooltip = false,    // Whether the tooltip to destroy the barricade is active for the engineer
         chaserTooltip = false,      // Whether the tooltip to destroy the barricade is active for the chaser
         runnerTooltip = false;      // Whether the tooltip to destroy the barricade is active for the runner
    Regex horizontalBarricadeExpression  = new Regex("Horizontal");  
                                     // Match "Horizontal"
    private Player_UI playerUi;      // Player UI management script
    
    // Start is called before the first frame update
    void Start(){
        // Assign Player UI script
        playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();
    }

    void Awake()
    {
        // Get all player instances
        trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        runner   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        
        // Get the EngineerAbility script
        engineerAbility = engineer.GetComponent<EngineerAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!CustomNetworkManager.IsRunner){
            // Tool tip for trapper
            if(Utilities.GetDistanceBetweenObjects(trapper.transform.position, gameObject.transform.position) < 2.5){
                enableTooltip(gameObject.transform.position, trapper.transform.position);
                trapperTooltip = true;
                if((Input.GetKeyDown("j") && CustomNetworkManager.IsRunner == false && trapper.GetComponent<ManageActiveCharacters>().guardId == trapper.GetComponent<ManageActiveCharacters>().activeGuardId)){
                    destroyBarricade();
                }
            }
            else if(trapperTooltip && CustomNetworkManager.IsRunner == false && trapper.GetComponent<ManageActiveCharacters>().guardId == trapper.GetComponent<ManageActiveCharacters>().activeGuardId){
                disableTooltip(gameObject.transform.position, trapper.transform.position);
                trapperTooltip = false;
            }
            // Tool tip for engineer
            if(Utilities.GetDistanceBetweenObjects(engineer.transform.position, gameObject.transform.position) < 2.5){
                enableTooltip(gameObject.transform.position, engineer.transform.position);
                engineerTooltip = true;
                if((Input.GetKeyDown("j") && CustomNetworkManager.IsRunner == false && engineer.GetComponent<ManageActiveCharacters>().guardId == engineer.GetComponent<ManageActiveCharacters>().activeGuardId)){
                    destroyBarricade();
                }
            }
            else if(engineerTooltip && CustomNetworkManager.IsRunner == false && engineer.GetComponent<ManageActiveCharacters>().guardId == engineer.GetComponent<ManageActiveCharacters>().activeGuardId){
                disableTooltip(gameObject.transform.position, engineer.transform.position);
                engineerTooltip = false;
            }
            // Tool tip for chaser
            if(Utilities.GetDistanceBetweenObjects(chaser.transform.position, gameObject.transform.position) < 2.5){
                enableTooltip(gameObject.transform.position, chaser.transform.position);
                chaserTooltip = true;
                if((Input.GetKeyDown("j") && CustomNetworkManager.IsRunner == false && chaser.GetComponent<ManageActiveCharacters>().guardId == chaser.GetComponent<ManageActiveCharacters>().activeGuardId)){
                    destroyBarricade();
                }
            }
            else if(chaserTooltip && CustomNetworkManager.IsRunner == false && chaser.GetComponent<ManageActiveCharacters>().guardId == chaser.GetComponent<ManageActiveCharacters>().activeGuardId){
                disableTooltip(gameObject.transform.position, chaser.transform.position);
                chaserTooltip = false;
            }
        }
        else{
            // Tool tip for runner
            if(Utilities.GetDistanceBetweenObjects(runner.transform.position, gameObject.transform.position) < 2.5 && playerUi.activeSelectedItem == Item.ItemType.Sledge){
                enableTooltip(gameObject.transform.position, runner.transform.position);
                runnerTooltip = true;
                if(Input.GetKeyDown("j") && CustomNetworkManager.IsRunner == true && runner.GetComponent<MoveCharacter>().canMove == true){
                    hitCount += 1;
                    runner.GetComponent<Animator>().SetBool("SwingHammer", true);
                    decreaseBarricadeHealth(gameObject.transform.position, runner.transform.position);
                    if(hitCount >= 3){
                        destroyBarricade();
                        hitCount = 0;
                    }
                }
            }
            else if(runnerTooltip && CustomNetworkManager.IsRunner == true){
                disableTooltip(gameObject.transform.position, runner.transform.position);
                runnerTooltip = false;
            }
        }
    }

    // Destroys both host and client barricade
    [Command(requiresAuthority = false)]
    public void destroyBarricade(){
        NetworkServer.Destroy(gameObject);
    }

    // Enable the barricade tooltip
    void enableTooltip(Vector3 barricadePosition, Vector3 characterPosition){
        if(horizontalBarricadeExpression.IsMatch(gameObject.name)){
            if(barricadePosition.y < characterPosition.y){
                if(CustomNetworkManager.IsRunner && hitCount < 3){
                    healthBottom.SetActive(true);
                }
                transform.GetChild(1).gameObject.SetActive(true);
            }
            else{
                transform.GetChild(0).gameObject.SetActive(true);
                if(CustomNetworkManager.IsRunner && hitCount < 3){
                    healthTop.SetActive(true);
                }
            }
        }
        else{
            transform.GetChild(0).gameObject.SetActive(true);
            if(CustomNetworkManager.IsRunner && hitCount < 3){
                    healthTop.SetActive(true);
                }
        }
    }

    // Disable the barriacde tooltip
    void disableTooltip(Vector3 barricadePosition, Vector3 characterPosition){
        if(horizontalBarricadeExpression.IsMatch(gameObject.name)){
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            if(CustomNetworkManager.IsRunner){
                healthTop.SetActive(false);
                healthBottom.SetActive(false);
            }
        }
        else{
            transform.GetChild(0).gameObject.SetActive(false);
            if(CustomNetworkManager.IsRunner){
                healthTop.SetActive(false);
            }
        }
    }

    // Decrease the health displayed on the barricade health bars
    void decreaseBarricadeHealth(Vector3 barricadePosition, Vector3 characterPosition){
        if(horizontalBarricadeExpression.IsMatch(gameObject.name)){
            var healthTopls = healthTop.transform.localScale;
            var healthBottomls = healthBottom.transform.localScale;
            if(hitCount >= 3){
                healthTopls.x = 0;
                healthBottomls.x = 0;
            }
            else{
                healthTopls.x    -= 1;
                healthBottomls.x    -= 1;
            }
            healthTop.transform.localScale = healthTopls;
            healthBottom.transform.localScale = healthBottomls;
        }
        else{
            var healthTopls = healthTop.transform.localScale;
            if(hitCount >= 3){
                healthTopls.x = 0;
            }
            else{
                healthTopls.x    -= .3f;
            }
            healthTop.transform.localScale = healthTopls;
        }
    }
}
