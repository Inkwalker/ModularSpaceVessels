using UnityEngine;

namespace ModularSpaceVessels
{
    [CreateAssetMenu]
    public class PartsProjectionSettings : ScriptableObject
    {
        public Material projectionMaterial;
        public Color validPositionColor;
        public Color invalidPositionColor;
    }
}
