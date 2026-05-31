using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath =>
        Application.persistentDataPath +
        "/save.json";

    // =========================
    // QUERY
    // =========================

    public static bool HasSave()
    {
        return File.Exists(savePath);
    }

    // =========================
    // SAVE
    // =========================

    public static void Save(GameData data)
    {
        string json =
            JsonUtility.ToJson(
                data,
                true);

        File.WriteAllText(
            savePath,
            json);

        Debug.Log(
            "SAVE SUCCESS");
    }

    // =========================
    // LOAD
    // =========================

    public static GameData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log(
                "NO SAVE FILE");

            return null;
        }

        string json =
            File.ReadAllText(
                savePath);

        GameData data =
            JsonUtility.FromJson<GameData>(
                json);

        Debug.Log(
            "LOAD SUCCESS");

        return data;
    }

    // =========================
    // DELETE
    // =========================

    public static void DeleteSave()
    {
        if (!File.Exists(savePath))
            return;

        File.Delete(savePath);

        Debug.Log(
            "SAVE DELETED");
    }
}
