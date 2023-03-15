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
    public string   username = "Guest";
    public string   passwordHash;
    public int      wins = 0;
    public int      losses = 0;
    public string[] achievements;
}

public class RegisterProfile : MonoBehaviour
{
    public GameObject usernameInputBox;
    //public string profilesJSONFilePath = "./Savedata/profiles.json";
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
        string result_string = "{\"username\":\"sdafhsdlhfk\",\"passwordHash\":\"\",\"wins\":2,\"losses\":6,\"achievements\":[]}";
        
        //JsonConvert.DeserializeObject<PlayerProfile>();
        
        
        
        PlayerProfile   testProfile   = new PlayerProfile();
        PlayerProfile[] profilesArray = new PlayerProfile[3];
        
        testProfile.username = username;
        testProfile.wins     = UnityEngine.Random.Range(1,10);
        testProfile.losses   = UnityEngine.Random.Range(1,10);
        
        profilesArray[0] = testProfile;
        
        var result = JsonConvert.DeserializeObject<PlayerProfile[]>("[" + result_string + "," + result_string + "]");
        
        Debug.Log(result);
        
        //WriteToFile("./Savedata/profile_test.profile", JsonUtility.ToJson(profilesArray[0]));
        //var result = ReadFileAsString("./Savedata/profile_test.profile");
        //var result = JsonUtility.FromJson(result_string);
        
        //Debug.Log(result);
        
        
        return;
        
        
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
        
        // > Test if username is inappropriate.
        
        // > Read in the profiles database file.
        PlayerProfile[] profilesDB = new PlayerProfile[MAX_PROFILE_COUNT];
        //string[] profileFilepaths = new string[MAX_PROFILE_COUNT];
        for (int i=0; i <= MAX_PROFILE_ID; i++) {
            string filepath = "./Savedata/profile" + (i+1) + ".profile";
            try
            {
                profilesDB[MAX_PROFILE_ID] = ReadProfileFromFile(filepath);
            }
            catch (FileNotFoundException error)
            {
                profilesDB[MAX_PROFILE_ID] = null;
            }
            
            if (profilesDB[MAX_PROFILE_ID] == null) {
                profilesDB[MAX_PROFILE_ID] = new PlayerProfile();
                WriteToFile(filepath, JsonUtility.ToJson(profilesDB[MAX_PROFILE_ID]));
            }
            
            Debug.Log("loop" + i);
        }
        
        Debug.Log(profilesDB);
        Debug.Log(profilesDB[0]);
        Debug.Log(profilesDB[1]);
        Debug.Log(profilesDB[2]);
        
        //Debug.Log(string.Join(",\n", profileFilepaths));
        
        
        //List<PlayerProfile> profilesDB = JsonUtility.FromJson<List<PlayerProfile>>(ReadFileAsString(profilesJSONFilePath));
        
        // > Test if username is already in the database.
        //foreach (PlayerProfile user_profile in profilesDB) {
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
        
        
        return;
        
        
        // > Write the updated database into the database file.
        
        
        return;
    }
    
    public PlayerProfile ReadProfileFromFile(string path) {
        string fileString = ReadFileAsString(path);
        return JsonUtility.FromJson<PlayerProfile>(fileString);
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