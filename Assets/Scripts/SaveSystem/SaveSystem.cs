using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {

    public static void SaveDrone(DroneSettings droneSettings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/"+droneSettings.name.Replace("(Clone)", "") + "drone.bal";
        FileStream stream = new FileStream(path, FileMode.Create);

        DroneData data = new DroneData(droneSettings);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static DroneData LoadPlayer(DroneSettings droneSettings)
    {
        string path = Application.persistentDataPath + "/" + droneSettings.name.Replace("(Clone)", "") + "drone.bal";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            //formatter.Deserialize(stream);
            object DeserializedStream = formatter.Deserialize(stream);
            stream.Close();
            DroneData data = (DroneData)DeserializedStream;
            return data;
        }else
        {
            Debug.LogError("Save not found at " + path);
            return null;
        }
    }

    public static void SaveInventory(Inventory inventory)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + "Inventory.bal";
        FileStream stream = new FileStream(path, FileMode.Create);

        InventoryData data = new InventoryData(inventory);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static InventoryData LoadInventory()
    {
        string path = Application.persistentDataPath + "/" + "Inventory.bal";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            //formatter.Deserialize(stream);
            object DeserializedStream = formatter.Deserialize(stream);
            stream.Close();
            InventoryData data = (InventoryData)DeserializedStream;
            return data;
        }
        else
        {
            Debug.LogError("Inventory save not found at " + path);
            return null;
        }
    }
}
