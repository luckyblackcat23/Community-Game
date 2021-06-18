using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetValidation : MonoBehaviour
{
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  static void Init()
  {
    if (!Application.isEditor) return;
    var lines = File.ReadAllLines("AssetManifest.list");
    var missingAsset = false;
    var lineNumber = 0;
    foreach (var line in lines)
    {
      lineNumber++;
      var trimmed = line.Trim();
      if (string.IsNullOrEmpty(trimmed)) continue;
      if (trimmed.StartsWith("#") || trimmed.StartsWith(";")) continue;
      var parts = trimmed.Split('|');
      if (parts.Length != 3)
      {
        Debug.LogError($"Asset Manifest invalid on line {lineNumber}");
        Debug.LogError($"Line: {line}");
        missingAsset = true;
        continue;
      }

      missingAsset |= CheckAsset(parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
    }

    if (missingAsset)
    {
#if UNITY_EDITOR
      EditorApplication.isPlaying = false;
#endif
    }
  }

  static bool CheckAsset(string path, string assetName, string downloadURL)
  {
    if (Directory.Exists("Assets/" + path)) return false;
    Debug.LogError($"Missing asset: {assetName} URL: {downloadURL}");
    return true;
  }
}