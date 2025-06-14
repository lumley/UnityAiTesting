using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lumley.AiTest.Editor.AutoBlockCreator
{
    public class SpriteAssetPostprocessor : AssetPostprocessor
    {
        private static SpritePrefabProcessorConfig? _config;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0 || !LoadConfig())
            {
                return;
            }

            foreach (string assetPath in importedAssets)
            {
                if (!assetPath.EndsWith(".png") && !assetPath.EndsWith(".jpg"))
                {
                    continue;
                }

                if (!IsInMonitoredFolder(assetPath))
                {
                    continue;
                }

                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null)
                {
                    continue;
                }

                CreatePrefabVariant(sprite, assetPath);
            }
        }

        private static bool LoadConfig()
        {
            if (_config != null)
            {
                return true;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(SpritePrefabProcessorConfig)}");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _config = AssetDatabase.LoadAssetAtPath<SpritePrefabProcessorConfig>(path);
                return true;
            }

            Debug.LogError($"{nameof(SpritePrefabProcessorConfig)} could not be found, please create one first.");
            return false;
        }

        private static bool IsInMonitoredFolder(string assetPath)
        {
            string normalizedPath = Path.Combine("Assets", _config!.FolderPath);
            // Replace backslashes with forward slashes for consistency
            normalizedPath = normalizedPath.Replace('\\', '/').Trim('/');
            return assetPath.StartsWith(normalizedPath);
        }

        private static void CreatePrefabVariant(Sprite sprite, string assetPath)
        {
            // Determine output path
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string outputDir = _config!.OutputFolder.TrimEnd('/');
            string variantPath = $"{outputDir}/{fileName}.prefab";

            if (!AssetDatabase.IsValidFolder(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                AssetDatabase.Refresh();
            }

            if (File.Exists(variantPath))
            {
                Debug.Log($"Prefab variant already exists: {variantPath}");
                return;
            }

            // Instantiate base prefab
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(_config.BasePrefab);
            if (instance == null)
            {
                Debug.LogError("Failed to instantiate base prefab.");
                return;
            }

            // Replace SpriteRenderer sprite
            SpriteRenderer sr = instance.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprite;
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found on base prefab.");
            }

            // Save as prefab variant
            GameObject prefabVariant = PrefabUtility.SaveAsPrefabAsset(instance, variantPath);
            if (prefabVariant != null)
            {
                Debug.Log($"Prefab variant created: {variantPath}");
            }

            Object.DestroyImmediate(instance);
        }
    }
}