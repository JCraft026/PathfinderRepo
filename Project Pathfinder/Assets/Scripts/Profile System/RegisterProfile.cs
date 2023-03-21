using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

[Serializable]
public class PlayerProfile
{
    public string username = "Guest";
    public string passwordHash;
    public Dictionary<string, int> stats = new Dictionary<string, int>
    {
        { "wins_runner", 0 },
        { "wins_guards", 0 },
        { "losses_runner", 0 },
        { "losses_guards", 0 },
        { "fake_stat", 100 }
    };
    public Dictionary<string, bool> adjectives = new Dictionary<string, bool>
    {
        { "green", true },
        { "blue",  true },
        { "red",   true }
    };
}

public class RegisterProfile : MonoBehaviour
{
    public GameObject usernameInputBox;
    public string profilesJSONFilePath = "./Savedata/profiles.json";
    public string profilesDirectory = "./Savedata/";
    public Regex usernameRegexRules = new Regex("^([a-zA-Z_0-9])+$");
    //public Regex profilesFilenameRegex = new Regex("^profile[1-5].profile$");
    public int MAX_USERNAME_LENGTH = 16;
    public int MAX_PROFILE_COUNT = 4;
    public int MAX_PROFILE_ID;
    
    // Start is called before the first frame update.
    void Start () {
        MAX_PROFILE_ID = MAX_PROFILE_COUNT - 1;
    }
    
    // Update is called once per frame.
    void Update() {
    }
    
    public void ButtonPress()
    {
        string username_text = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        registerUsername(username_text);
        return;
    }
    
    public void registerUsername(string username)
    {
        // > Test if the username is too short.
        if (username.Length <= 0) {
            Debug.Log("Username invalid: The username must have at least one character!");
            return;
        }
        
        // > Test if the username is too long.
        if (username.Length > MAX_USERNAME_LENGTH) {
            Debug.Log("Username invalid: The username cannot be longer than "
                + MAX_USERNAME_LENGTH + " characters!");
            return;
        }
        
        // > Test if username contains valid characters.
        if (!usernameRegexRules.IsMatch(username)) {
            Debug.Log("Username invalid: The username must only contain numbers, letters, and underscores!");
            return;
        }
        
        // > Test if the profile already exists and that it isn't empty or corrupt.
        string profilePath = GetProfileFilepath(username);
        
        if (System.IO.File.Exists(profilePath)
            && ("" != ReadFileAsString(profilePath))) {
            Debug.Log("Username invalid: The username '" + username + "' is already in use on this device!");
            Debug.Log(ReadFileAsString(profilePath));
            return;
        }
        
        // > Save the newly registered profile as a new file.
        PlayerProfile newProfile = new PlayerProfile();
        newProfile.username = username;
        newProfile.passwordHash = "";
        
        WriteToFile(profilePath, ProfileToString(newProfile));
        Debug.Log("The profile '" + username + "' has been saved!");
        
        return;
    }
    
    // Future Function Ideas:
    // LoadProfile(string username?)
    // SaveProfile(PlayerProfile)
    // AddGameToStats()
    // Class GameStats
    // {
    //     host_team: 'R' or 'GM'
    //     host_username: 'username1'
    //     client_username: 'username2'
    //     game_result: 'host', 'client', 'disconnect'
    //     remaining_seconds: 296
    //     doors_unlocked: 'R__Y'
    //     trapped_chest_explosions: 4
    //     cracked_walls_destroyed: 2
    // }
    
    
    
    public string ProfileToString(PlayerProfile playerProfile) {
        return JsonConvert.SerializeObject(playerProfile, Formatting.Indented);
    }
    
    public PlayerProfile StringToProfile(string playerProfile) {
        return JsonConvert.DeserializeObject<PlayerProfile>(playerProfile);
    }
    
    public string GetProfileFilepath(PlayerProfile playerProfile) {
        return GetProfileFilepath(playerProfile.username);
    }
    
    public string GetProfileFilepath(string username) {
        return "./Savedata/" + username + ".profile";
    }
    
    public PlayerProfile ReadProfileFromFile(string filepath) {
        return StringToProfile(ReadFileAsString(filepath));
    }
    
    public string ReadFileAsString(string path)
    {
        if (path == null)
            return null;
        string fileData;
        StreamReader reader = new StreamReader(path); 
        fileData = reader.ReadToEnd();
        reader.Close();
        return fileData;
    }
    
    public void WriteToFile(string path, string saved_string)
    {
        if (path == null)
            return;
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(saved_string);
        writer.Close();
        return;
    }
}





















//=============================================================================
// Documentation (for future Keegan):

// https://docs.unity3d.com/ScriptReference/Unity.IO.LowLevel.Unsafe.FileHandle.html
// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-6.0



//=============================================================================
// Random code (for future Keegan):

//for (int i = 0; i < username.Length; i++) { char c = username[i]; }
//foreach (char c in username) { Debug.Log(c); }

//Debug.Log("LIST:");
//Debug.Log($"[ {string.Join(",\n", JsonUtility.ToJson(profilesDB))} ]");
//foreach (PlayerProfile user_profile in profilesDB) {
//    Debug.Log(user_profile);//.ToString());
//    //if (false) {
//    //    Debug.Log("That username already exists on this computer!");
//    //    return;
//    //}
//}

// = Directory.GetFiles(".\\Savedata");



// > Read in the profiles database file.
        /*
        PlayerProfile[] profilesDB = new PlayerProfile[MAX_PROFILE_COUNT];
        //string[] profileFilepaths = new string[MAX_PROFILE_COUNT];
        for (int i=0; i <= MAX_PROFILE_ID; i++) {
            string filepath = "./Savedata/profile" + (i+1) + ".profile";
            try {
                profilesDB[MAX_PROFILE_ID] = ReadProfileFromFile(filepath);
            } catch (FileNotFoundException error) {
                profilesDB[MAX_PROFILE_ID] = null;
            }
            
            if (profilesDB[MAX_PROFILE_ID] == null) {
                profilesDB[MAX_PROFILE_ID] = new PlayerProfile();
                WriteToFile(filepath, JsonUtility.ToJson(profilesDB[MAX_PROFILE_ID]));
            }
            
            Debug.Log("loop" + i);
        }
        */
        
        
        
        //PlayerProfile[] profilesDB = JsonConvert.DeserializeObject<PlayerProfile[]>(ReadFileAsString(profilesJSONFilePath));
        
        // > Test if username is already in the database.
        //foreach (PlayerProfile savedPlayerProfile in profilesDB) {
        //    Debug.Log(user_profile);
        //    //if (false) {
        //    //    Debug.Log("That username already exists on this computer!");
        //    //    return;
        //    //}
        //}
        
        // > Insert the new profile in the database.
        //PlayerProfile profile = new PlayerProfile();
        //profile.username = username;
        //profilesDB.Add(profile);
        
        
        //foreach (string filepath in profileFilepaths) {
        //    if (profilesFilenameRegex.IsMatch(filepath))
        //        profileFilepaths.Remove(filepath);
        //}
        
        //Debug.Log(string.Join(",\n", profileFilepaths));
        
        //Debug.Log(profile);
        //Debug.Log(profilesDB);
        
        
        // > Write the updated database into the database file.
        
        