using UnityEngine;

namespace Lumley.AiTest.Editor.AutoBlockCreator
{
    [CreateAssetMenu(fileName = nameof(SpritePrefabProcessorConfig), menuName = "Tools/" + nameof(SpritePrefabProcessorConfig))]
    public class SpritePrefabProcessorConfig : ScriptableObject
    {
        [Tooltip("Folder path to monitor, relative to Assets/")]
        public string FolderPath = string.Empty;

        [Tooltip("The base prefab to create variants from")]
        public GameObject BasePrefab = null!;

        [Tooltip("Where to save the generated prefab variants (relative to Assets/)")]
        public string OutputFolder = "Assets/GeneratedPrefabs";
    }
}