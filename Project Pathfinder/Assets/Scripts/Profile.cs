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
    public string password;
    public string username = "[Guest]";
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
    
    // Achievement Lists
    public string[] titles = {
        "Beetle",
        "Cat",
        "Dingo",
        "Eagle",
        "Ferret",
        "Gorilla",
        "Jangle",
        "Kangaroo",
        "Leopard",
        "Monkey",
        "Pear",
        "Queen",
        "Roach",
        "Strawberry",
        "Turtle",
        "Zebra",
    };
    public string[] adjectives = {
        "Adept",
        "Beautiful",
        "Cautious",
        "Dramatic",
        "Elegant",
        "Fickle",
        "Grouchy",
        "Huge",
        "Ideal",
        "Jolly",
        "Keen",
        "Lazy",
        "Meek",
        "Neat",
        "Observant",
        "Plain",
        "Quirky",
        "Rare",
        "Small",
        "Tall",
        "Unusual",
        "Valid",
        "Whimsical",
        "Xtra",
        "Youthful",
        "Zesty",
    };
    
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
        //AddNamesToDropdownMenus();
        WriteMessageToUser("You are now logged in as '" + username + "'.");
        return;
    }
    
    // "Register" button pressed.
    public void Button_RegisterAndLogin() {
        string username = (usernameInputBox.GetComponent<TMPro.TMP_InputField>().text);
        string password = (passwordInputBox.GetComponent<TMPro.TMP_InputField>().text);
        if (RegisterProfile(username, password))
            if (!LoginProfile(username, password))
                WriteMessageToUser("ERROR: Impossible condition. " +
                    "Password does not match registered account. " +
                    "Could not login the registered profile.");
            else
            {
                WriteMessageToUser("Welcome to the game," + username + "!"
                    + "\nPress 'Play' up above to get back to the game!");
                usernameInputBox.GetComponent<TMPro.TMP_InputField>().text = "";
                passwordInputBox.GetComponent<TMPro.TMP_InputField>().text = "";
            }
        return;
    }
    
    public string PlayerNewUnlock() { //PlayerProfile playerProfile) {
        PlayerProfile playerProfile = CustomNetworkManager.currentLogin;
        string reward = null;
        int rewardType = UnityEngine.Random.Range(0,2);
        if (rewardType == 0 && (playerProfile.titles.Count < titles.Length))
        {
            // Reward Title
            do {
                reward = titles[UnityEngine.Random.Range(0,titles.Length)];
            } while (playerProfile.titles.ContainsKey(reward) && (playerProfile.titles[reward] == true));
            playerProfile.titles.Add(reward, true);
        }
        else if (playerProfile.adjectives.Count < adjectives.Length) {
            // Reward Adjective
            do {
                reward = adjectives[UnityEngine.Random.Range(0,adjectives.Length)];
            } while (playerProfile.adjectives.ContainsKey(reward) && (playerProfile.adjectives[reward] == true));
            playerProfile.adjectives.Add(reward, true);
        }
        
        if (reward == null)
        {
            return "Play again to get another chance to earn a reward!";
        }
        else if (rewardType == 0)
        {
            return "You've earned a new title: \"" + reward + "\"!";
        }
        else
        {
            return "You've earned a new adjective: \"" + reward + "\"!";
        }
        
        //return null;
    }
    
    // Registers a name into the database, if possible.
    public bool RegisterProfile(string username, string password) {
        // > Test if the username is too short.
        if (username.Length <= 0) {
            WriteMessageToUser("Username invalid:\nThe username must have at least one character.");
            return false;
        }
        
        // > Test if the username is too long.
        if (username.Length > MAX_USERNAME_LENGTH) {
            WriteMessageToUser("Username invalid:\nThe username cannot be longer than "
                + MAX_USERNAME_LENGTH + " characters.");
            return false;
        }
        
        // > Test if username contains valid characters.
        if (!usernameRegexRules.IsMatch(username)) {
            WriteMessageToUser("Username invalid:\nThe username must only contain numbers, letters, and underscores.");
            return false;
        }
        
        if (IsUsernameCursed(username)) {
            WriteMessageToUser("... probably shouldn't use that one, bud.");
            return false;
        }
        
        // > Test if the profile already exists.
        if (DoesProfileExist(username)) {
            WriteMessageToUser("Username invalid:\nThe username '" + username + "' is already in use on this device.");
            return false;
        }
        
        // > Save the newly registered profile as a new file.
        PlayerProfile newProfile = new PlayerProfile();
        newProfile.username = username;
        newProfile.password = password;
        
        SaveEncodedProfile(newProfile, password);
        WriteMessageToUser("The profile '" + username + "' has been saved!");
        
        return true;
    }
    
    // Reads a user's profile and signs them in if the given password is correct.
    public bool LoginProfile(string username, string password) {
        string profileString = LoadDecodedProfileString(username, password);
        string verifyString = "{\"password\":\"" + password + "\","
                             + "\"username\":\"" + username + "\",";
        if (!profileString.StartsWith(verifyString)) {
            //Debug.Log("VERIFY STRING FAILED. STRING = " + verifyString);
            //Debug.Log("PROFILE STRING = " + profileString);
            return false;
        }
        PlayerProfile profile = GetProfileFromString(profileString);
        if (profile.password != password) {
            //Debug.Log("PASSWORD != PASSWORD");
            return false;
        }
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
    
    // Detects if the given username is inappropriate.
    public bool IsUsernameCursed(string username) {
        return false;
    }
    
    // Gets the profile of the currently logged in user.
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
    
    // Determines if a profile exists on the system or not.
    public bool DoesProfileExist(string username) {
        return System.IO.File.Exists(GetProfileFilepath(username));
    }
    
    public string LoadDecodedProfileString(string username, string password) {
        string profile_string = ReadFileAsString(GetProfileFilepath(username));
        return DecodeProfileString(profile_string, password);
    }
    
    // Saves a profile to its local file.
    public bool SaveEncodedProfile(PlayerProfile profile, string password) {
        try {
            WriteStringToFile(GetProfileFilepath(profile), EncodeProfileString(ProfileToString(profile), password));
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
        return JsonConvert.SerializeObject(playerProfile);//, Formatting.Indented);
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
        writer.Write(saved_string);
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
    
    // Shifts a character into a different character using an ASCII code modifer.
    public char ShiftCharacter(char character, int modifier) {
        int newNumber = System.Convert.ToInt32(character)-32 + modifier;
        while (newNumber < 0)
            newNumber += 94;
        while (newNumber >= 94)
            newNumber -= 94;
        return System.Convert.ToChar(newNumber + 32);
        // Characters are contained from ASCII 32 to 126.
        // This way, only non-control characters are used. (They were causing issues...)
        // ASCII MINIMUM = 32
        // ASCII MAXIMUM = 126
        // ASCII RANGE = MAX - MIN = 94
    }
    
    // Encodes a profile string into a string that cannot be decoded without the password.
    public string EncodeProfileString(string originalText, string originalPassword) {
        char[] newText = originalText.ToCharArray();
        char[] passletterArray = originalPassword.ToCharArray();
        
        int passletterIndex;
        int passwordSum = 0;
        for (passletterIndex = 0; passletterIndex < passletterArray.Length; passletterIndex++)
            passwordSum += System.Convert.ToInt32(passletterArray[passletterIndex]);
        
        passletterIndex = 0;
        int newTextIndex;
        int unicodeModifier;
        
        for (newTextIndex = 0; newTextIndex < newText.Length; newTextIndex++) {
            unicodeModifier = passwordSum + newTextIndex + System.Convert.ToInt32(passletterArray[passletterIndex]);
            newText[newTextIndex] = ShiftCharacter(newText[newTextIndex], unicodeModifier);
            passletterIndex++;
            if (passletterIndex >= passletterArray.Length)
                passletterIndex = 0;
        }
        
        Debug.Log(newText);
        Debug.Log(newText.ToString());
        Debug.Log(new String(newText));
        
        return new String(newText);
    }
    
    // Attempts to decode an encoded profile string using a given password.
    // This is the EXACT same funtion as above, but it shifts DOWN instead of shifting UP.
    public string DecodeProfileString(string originalText, string originalPassword) {
        char[] newText = originalText.ToCharArray();
        char[] passletterArray = originalPassword.ToCharArray();
        
        int passletterIndex;
        int passwordSum = 0;
        for (passletterIndex = 0; passletterIndex < passletterArray.Length; passletterIndex++)
            passwordSum += System.Convert.ToInt32(passletterArray[passletterIndex]);
        
        passletterIndex = 0;
        int newTextIndex;
        int unicodeModifier;
        
        for (newTextIndex = 0; newTextIndex < newText.Length; newTextIndex++) {
            unicodeModifier = passwordSum + newTextIndex + System.Convert.ToInt32(passletterArray[passletterIndex]);
            newText[newTextIndex] = ShiftCharacter(newText[newTextIndex], -unicodeModifier);
            passletterIndex++;
            if (passletterIndex >= passletterArray.Length)
                passletterIndex = 0;
        }
        
        return new String(newText);
    }
    
}
