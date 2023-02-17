using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance {get; private set;}

    private void Awake(){
        Instance = this;
    }

    public GameObject pfItemWorld,    // Transform of prefab item in the scene
                      SmokeScreen;    // Prefab smoke screen to instantiate
    public Sprite    sledgeSprite,    // Sledge Hammer sprite
                     smokeBombSprite, // Smoke Bomb Sprite
                     coffeeSprite,    // Common Grounds Coffee Sprite
                     greenScreenSuitSprite,
                                      // 
                     Keys0Sprite,     // Red key sprite
                     Keys1Sprite,     // Blue key sprite
                     Keys2Sprite,     // Green key sprite
                     Keys3Sprite;     // Yellow key sprite
}