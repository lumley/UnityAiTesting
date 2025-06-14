using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Lumley.AiTest.Journey
{
    /// <summary>
    /// Serializable information about a given game
    /// </summary>
    [Serializable]
    public sealed class GameInfo
    {
        public AssetReference Scene;
        public Sprite GameSprite;
        public string Name; // TODO (slumley): Use localized string
    }
}