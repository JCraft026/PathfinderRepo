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
    public string username = "[Guest]";
    public string password;
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
        { "blue", true },
        { "red", true }
    };
    public Dictionary<string, bool> titles = new Dictionary<string, bool>
    {
        { "participator", true },
        { "victor", false },
        { "gamer", true }
    };
}

public class Profile : MonoBehaviour
{
    public GameObject usernameInputBox;
    public GameObject passwordInputBox;
    public string profilesJSONFilePath = "./Savedata/profiles.json";
    public string profilesDirectory = "./Savedata/";
    public Regex usernameRegexRules = new Regex("^([a-zA-Z_0-9])+$");
    public int MAX_USERNAME_LENGTH = 16;
    public int MAX_PROFILE_COUNT = 4;
    
    // Start is called before the first frame update.
    void Start () {
    }
    
    // Update is called once per frame.
    void Update() {
    }
    
    // "Login" button pressed.
    public void Button_Login() {
        string username = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        string password = (passwordInputBox.GetComponent<TMPro.TMP_InputField>().text);
        if (!DoesProfileExist(username)) {
            WriteMessageToUser("That profile does not exist on this device.");
            return;
        }
        if (!LoginProfile(username, password)) {
            WriteMessageToUser("Incorrect password.");
            return;
        }
        AddNamesToDropdownMenus();
        WriteMessageToUser("You are now logged in as '" + username + "'.");
        return;
    }
    
    // "Register" button pressed.
    public void Button_RegisterAndLogin() {
        string username = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        string password = (passwordInputBox.GetComponent<TMPro.TMP_InputField>().text);
        RegisterProfile(username, password);
        if (!LoginProfile(username, password))
            WriteMessageToUser("ERROR: Impossible condition. " +
                "Password does not match registered account. " +
                "Could not login the registered profile.");
        return;
    }
    
    // Registers a name into the database, if possible.
    public void RegisterProfile(string username, string password)
    {
        // > Test if the username is too short.
        if (username.Length <= 0) {
            WriteMessageToUser("Username invalid:\nThe username must have at least one character.");
            return;
        }
        
        // > Test if the username is too long.
        if (username.Length > MAX_USERNAME_LENGTH) {
            WriteMessageToUser("Username invalid:\nThe username cannot be longer than "
                + MAX_USERNAME_LENGTH + " characters.");
            return;
        }
        
        // > Test if username contains valid characters.
        if (!usernameRegexRules.IsMatch(username)) {
            WriteMessageToUser("Username invalid:\nThe username must only contain numbers, letters, and underscores.");
            return;
        }
        
        // > Test if the profile already exists.
        if (DoesProfileExist(username)) {
            WriteMessageToUser("Username invalid:\nThe username '" + username + "' is already in use on this device.");
            return;
        }
        
        // > Save the newly registered profile as a new file.
        PlayerProfile newProfile = new PlayerProfile();
        newProfile.username = username;
        newProfile.password = password;
        
        SaveProfile(newProfile);
        WriteMessageToUser("The profile '" + username + "' has been saved!");
        
        return;
    }
    
    public bool LoginProfile(string username, string password) {
        PlayerProfile profile = ReadProfileFromFile(GetProfileFilepath(username));
        if (profile.password != password)
            return false;
        CustomNetworkManager.currentLogin = profile;
        return true;
    }
    
    // Future Function Ideas:
    //   AddGameToStats()
    //   Class GameStats
    //   {
    //       host_team: 'R' or 'GM'
    //       host_username: 'username1'
    //       client_username: 'username2'
    //       game_result: 'host', 'client', 'disconnect'
    //       remaining_seconds: 296
    //       doors_unlocked: 'R__Y'
    //       trapped_chest_explosions: 4
    //       cracked_walls_destroyed: 2
    //   }
    
    
    public PlayerProfile GetCurrentLogin() {
        return CustomNetworkManager.currentLogin;
    }
    
    public void WriteMessageToUser(string message) {
        try {
            GameObject responseTextbox = GameObject.Find("Response Text");
            responseTextbox.GetComponent<TMPro.TMP_Text>().text = message;
        } catch (Exception exception) {
            Debug.Log("Error: Couldn't send message to the user for this reason:" + exception);
        }
        Debug.Log(message);
    }
    
    public bool DoesProfileExist(string username) {
        return System.IO.File.Exists(GetProfileFilepath(username));
    }
    
    // Saves a profile to its local file.
    public bool SaveProfile(PlayerProfile profile) {
        try {
            WriteStringToFile(GetProfileFilepath(profile), ProfileToString(profile));
            return true;
        } catch (Exception error) {
            Debug.Log(error);
            return false;
        }
    }
    
    // Loads an already saved player profile from its local file.
    //public PlayerProfile LoadProfile(string username, string password) {
    //    return ReadProfileFromFile(GetProfileFilepath(username));
    //}
    
    // Serializes a profile object into a json string.
    public string ProfileToString(PlayerProfile playerProfile) {
        return JsonConvert.SerializeObject(playerProfile, Formatting.Indented);
    }
    
    // Deserializes a json string into a profile object.
    public PlayerProfile GetProfileFromString(string playerProfile) {
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
        return GetProfileFromString(ReadFileAsString(filepath));
    }
    
    // Reads in a text file as a string.
    public string ReadFileAsString(string path) {
        if (path == null)
            return null;
        string fileData;
        StreamReader reader = new StreamReader(path); 
        fileData = reader.ReadToEnd();
        reader.Close();
        return fileData;
    }
    
    // Overwrites a file as the contents of a string.
    public void WriteStringToFile(string path, string saved_string) {
        if (path == null)
            return;
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(saved_string);
        writer.Close();
        return;
    }
    
    // Adds the adjectives and titles to the server name selection windows.
    public void AddNamesToDropdownMenus() {
        PlayerProfile currentProfile = CustomNetworkManager.currentLogin;
        
        GameObject dropdownAdjective1 = GameObject.Find("Dropdown (Adjective1)");
        GameObject dropdownAdjective2 = GameObject.Find("Dropdown (Adjective2)");
        GameObject dropdownTitle      = GameObject.Find("Dropdown (Title)");
        
        List<string> options_adjectives = new List<string>();
        foreach (KeyValuePair<string,bool> adj in currentProfile.adjectives) {
            if (adj.Value == true)
                options_adjectives.Add(adj.Key);
        }
        List<string> options_titles = new List<string>();
        foreach (KeyValuePair<string,bool> adj in currentProfile.titles) {
            if (adj.Value == true)
                options_titles.Add(adj.Key);
        }
        
        dropdownAdjective1.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownAdjective1.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options_adjectives);
        dropdownAdjective2.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownAdjective2.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options_adjectives);
        dropdownTitle.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownTitle.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options_titles);
    }
    
}





















//=============================================================================
// Documentation (for future Keegan):

// https://docs.unity3d.com/ScriptReference/Unity.IO.LowLevel.Unsafe.FileHandle.html
// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-6.0



//=============================================================================
// Random code (for future Keegan):




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
        
        
        //foreach(var component in responseTextbox.GetComponent<TMPro.TMP_Text>()) Debug.Log(component);
        
        