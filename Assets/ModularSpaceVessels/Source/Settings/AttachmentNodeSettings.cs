using UnityEngine;

namespace ModularSpaceVessels
{
    [CreateAssetMenu]
    public class AttachmentNodeSettings : ScriptableObject
    {
        public Material unselectedMaterial;
        public Material selectedMaterial;
        public Material highlightMaterial;
    }
}
