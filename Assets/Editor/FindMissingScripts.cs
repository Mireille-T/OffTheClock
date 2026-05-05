using UnityEditor;
using UnityEngine;

public static class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    static void FindInScene()
    {
        int total = 0;
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (count > 0)
            {
                Debug.LogWarning($"[Missing Script x{count}] {GetPath(go)}", go);
                total += count;
            }
        }
        Debug.Log($"Done. Found {total} missing-script components in scene.");
    }

    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    static void RemoveInScene()
    {
        int total = 0;
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removed > 0)
            {
                Debug.Log($"Removed {removed} missing on {GetPath(go)}", go);
                total += removed;
            }
        }
        Debug.Log($"Done. Removed {total} missing-script components.");
        AssetDatabase.SaveAssets();
    }

    static string GetPath(GameObject go)
    {
        string p = go.name;
        var t = go.transform.parent;
        while (t != null) { p = t.name + "/" + p; t = t.parent; }
        return p;
    }
}
