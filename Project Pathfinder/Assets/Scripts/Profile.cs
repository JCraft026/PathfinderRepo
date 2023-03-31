using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using static Utilities;

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
        { "chaser_dashes_used", 0 },
        { "trapper_trapchests_created", 0 },
        { "trapper_trapchests_triggered", 0 },
        { "engineer_walls_placed", 0 },
        { "runner_trapchests_triggered", 0 },
        { "profile_version", 1 }
    };
    public Dictionary<string, bool> adjectives = new Dictionary<string, bool>
    {
        { "green", true },
        { "blue",  true },
        { "red",   true }
    };
}

public class Profile : MonoBehaviour
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
    
    // "Register" button pressed
    public void ButtonPress()
    {
        return;
    }
    
    public void Button_Login() {
        string username_text = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        loginProfile(LoadProfile(username_text));
        return;
    }
    
    public void Button_RegisterAndLogin() {
        string username_text = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        registerUsername(username_text);
        loginProfile(LoadProfile(username_text));
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
        
        SaveProfile(newProfile);
        //WriteStringToFile(profilePath, ProfileToString(newProfile));
        Debug.Log("The profile '" + username + "' has been saved!");
        
        return;
    }
    
    public void loginProfile(PlayerProfile playerProfile) {
        //var CNM_cl = GetObject("CustomNetworkManager").GetComponent<CustomNetworkManager>();
        //CNM.GetComponent<CustomNetworkManager>().currentLogin = playerProfile.username;
        Debug.Log(CustomNetworkManager.currentLogin);
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
    
    
    //public PlayerProfile GetCurrentProfile() {
    //    return new PlayerProfile();
    //}
    
    // ORIGIN: CustomNetworkManagerDAO.71
    // Sets the name that should be advertised on the server browser (NOTE: The name can only come from the dropdown box in the host options screen)
    //public void UpdateServerName()
    //{
    //    var dropdown = gameObject.GetComponent<TMPro.TMP_Dropdown>(); // the dropdown box that sets the server name
    //    if(dropdown == null)
    //    {
    //        Debug.LogError("Dropdown is null");
    //    }
    //    Debug.Log(dropdown.captionText.text);
    //    GetServerBrowserBackend().serverName = dropdown.captionText.text;
    //}
    
    // Saves a profile to its local file.
    public bool SaveProfile(PlayerProfile profile) {
        WriteStringToFile(GetProfileFilepath(profile), ProfileToString(profile));
        return true;
    }
    
    // Loads an already saved player profile from its local file.
    public PlayerProfile LoadProfile(string username) {
        return ReadProfileFromFile(GetProfileFilepath(username));
    }
    
    // Serializes a profile into a json string.
    public string ProfileToString(PlayerProfile playerProfile) {
        return JsonConvert.SerializeObject(playerProfile, Formatting.Indented);
    }
    
    // Deserializes a json string into a profile.
    public PlayerProfile StringToProfile(string playerProfile) {
        return JsonConvert.DeserializeObject<PlayerProfile>(playerProfile);
    }
    
    // Gets the save file path for a given profile.
    public string GetProfileFilepath(PlayerProfile playerProfile) {
        return GetProfileFilepath(playerProfile.username);
    }
    public string GetProfileFilepath(string username) {
        return "./Savedata/" + username + ".profile";
    }
    
    // Reads a profile from a given filepath.
    public PlayerProfile ReadProfileFromFile(string filepath) {
        return StringToProfile(ReadFileAsString(filepath));
    }
    
    // Reads in a text file as a string.
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
    
    // Overwrites a file as the contents of a string.
    public void WriteStringToFile(string path, string saved_string)
    {
        if (path == null)
            return;
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(saved_string);
        writer.Close();
        return;
    }
    
    
    
    public void AddAdjectivesToDropdown() {
        // > Get User Profile
        PlayerProfile currentProfile = new PlayerProfile();
        
        // > Get Dropdown Object
        GameObject dropdownObject = GameObject.Find("Dropdown");
        
        // > Add Options
        List<string> options = new List<string>();
        foreach (KeyValuePair<string,bool> adj in currentProfile.adjectives) {
            if (adj.Value == true)
                options.Add(adj.Key);
        }
        //Debug.Log(JsonConvert.SerializeObject(options));
        foreach(var component in dropdownObject.GetComponents<Component>()) Debug.Log(component);
        //Debug.Log(dropdownObject.GetComponent<TMPro.TMP_Dropdown>());
        dropdownObject.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownObject.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options);
        //AddOptions(List<String>);
        
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
                WriteStringToFile(filepath, JsonUtility.ToJson(profilesDB[MAX_PROFILE_ID]));
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
        
        