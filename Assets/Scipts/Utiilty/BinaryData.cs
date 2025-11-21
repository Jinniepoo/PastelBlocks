using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinaryData
{
    public static void Save<T>(T serializedObject, string fileName)
    {
        string path = Application.dataPath + "/SavedFiles/";
        Directory.CreateDirectory(path);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(path + fileName + ".dat", FileMode.Create);

        try
        {
            formatter.Serialize(fs, serializedObject);
        }
        catch (SerializationException e)
        {
            Debug.Log("Saved Failed" + e.Message);
            throw;
        }
        finally { fs.Close(); }
    }

    public static bool Exist(string fileName)
    {
        string path = Application.dataPath + "/SavedFiles/";
        string fullFileName = fileName + ".dat";
        return File.Exists(path + fullFileName);
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.dataPath + "/SavedFiles/";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(path + fileName + ".dat", FileMode.Open);
        T returnType = default(T);

        try
        {
            returnType = (T)formatter.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.Log("Read Failed" + e.Message);
            throw;
        }
        finally { fs.Close(); }

        return returnType;
    }
}
