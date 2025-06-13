using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class GetGuidMenu
{
    [MenuItem("Assets/Copy ROTA GUID")]
    public static void GetGUID()
    {
        if (!Selection.activeObject)
            return;

        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var guid = AssetDatabase.AssetPathToGUID(path);
        UnityEngine.GUIUtility.systemCopyBuffer = $"public const string {Selection.activeObject.name} = \"{guid}\";";
    }
}