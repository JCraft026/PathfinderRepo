using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadWriteFile : MonoBehaviour
{
    //[MenuItem("Tools/Write file")]
    public void WriteString(string path, string saved_string)
    {
        //string path = "Savedata/profiles.txt";
        if (path == null)
            return;
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(saved_string);
        writer.Close();
        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path); 
        TextAsset asset = (TextAsset)(Resources.Load("./../Savedata/profiles.txt"));
        //Print the text from the file
        Debug.Log(asset);
    }
    //[MenuItem("Tools/Read file")]
    public void ReadString()
    {
        string path = "Savedata/profiles.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}

// Documentation (for future Keegan):
// https://docs.unity3d.com/ScriptReference/Unity.IO.LowLevel.Unsafe.FileHandle.html