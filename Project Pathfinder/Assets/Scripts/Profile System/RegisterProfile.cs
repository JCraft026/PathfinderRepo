using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[Serializable]
public class PlayerProfile
{
    public string username;
    public string passwordHash;
    public double wins;
    public double losses;
    public List<string> achievements;
}

public class RegisterProfile : MonoBehaviour
{
    public GameObject usernameInputBox;
    public string ProfilesJSONFilePath = "./Savedata/profiles.json";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ButtonPress()
    {
        string text = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        registerUsername(text);
        return;
    }
    
    public void registerUsername(string username)
    {
        var usernameRegexRules = new Regex("^([a-zA-Z_0-9])+$");
        var MAX_USERNAME_LENGTH = 16;
        
        // > Test if the username is too short.
        if (username.Length <= 0) {
            Debug.Log("Username invalid: The username must have at least one character!");
            return;
        }
        
        // > Test if the username is too long.
        if (username.Length > MAX_USERNAME_LENGTH) {
            Debug.Log("Username invalid: The username cannot be longer than 16 characters!");
            //" + MAX_USERNAME_LENGTH + " characters!");
            return;
        }
        
        // > Test if username contains valid characters.
        if (!usernameRegexRules.IsMatch(username)) {
            Debug.Log("Username invalid: The username must only contain numbers, letters, and underscores!");
            return;
        }
        
        // > Test if username is inappropriate.
        
        // > Read in the profiles database file.
        List<PlayerProfile> profilesDB = JsonUtility.FromJson<List<PlayerProfile>>(ReadFileAsString(ProfilesJSONFilePath));
        
        // > Test if username is already in the database.
        foreach (PlayerProfile user_profile in profilesDB) {
            Debug.Log(user_profile);
            //if (false) {
            //    Debug.Log("That username already exists on this computer!");
            //    return;
            //}
        }
        
        // > Insert the new profile in the database.
        PlayerProfile profile = new PlayerProfile();
        profile.username = username;
        profilesDB.Add(profile);
        //profilesDB.Add(profile);
        //profilesDB.Add(profile);
        //profilesDB.Add(profile);
        var profilesDirectory = Directory.GetFiles("./Savedata");
        
        Debug.Log(profilesDirectory);
        
        
        //Debug.Log(profile);
        //Debug.Log(profilesDB);
        
        //Debug.Log("LIST:");
        //Debug.Log($"[ {string.Join(",\n", JsonUtility.ToJson(profilesDB))} ]");
        //foreach (PlayerProfile user_profile in profilesDB) {
        //    Debug.Log(user_profile);//.ToString());
        //    //if (false) {
        //    //    Debug.Log("That username already exists on this computer!");
        //    //    return;
        //    //}
        //}
        
        
        
        // > Write the updated database into the database file.
        //WriteToFile(ProfilesJSONFilePath, );
        string profiles_json_2 = JsonUtility.ToJson(profilesDB);
        //Debug.Log(profiles_json_2);
        
        WriteToFile(ProfilesJSONFilePath, profiles_json_2);
        
        //WriteString("savedata/profiles.txt", username);
        //Debug.Log(username);
        
        return;
    }
    
    //[MenuItem("Tools/Write file")]
    public void WriteToFile(string path, string saved_string)
    {
        if (path == null)
            return;
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(saved_string);
        writer.Close();
    }
    
    //[MenuItem("Tools/Read file")]
    //public List<PlayerProfile> ReadProfilesDatabase(string path)
    //{
    //    if (path == null)
    //        path = ProfilesJSONFilePath;
    //    string fileDataString = ReadFileAsString(path);
    //    
    //    //var fileData = ;
    //    
    //    return fileData;
    //}
    
    //[MenuItem("Tools/Read file")]
    public string ReadFileAsString(string path)
    {
        string fileData;
        StreamReader reader = new StreamReader(path); 
        fileData = reader.ReadToEnd();
        reader.Close();
        return fileData;
    }
}


// Documentation (for future Keegan):
// https://docs.unity3d.com/ScriptReference/Unity.IO.LowLevel.Unsafe.FileHandle.html

// Random code (for future Keegan)
//for (int i = 0; i < username.Length; i++) { char c = username[i]; }
//foreach (char c in username) { Debug.Log(c); }
