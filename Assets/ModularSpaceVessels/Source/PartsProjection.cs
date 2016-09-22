using ModularSpaceVessels.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace ModularSpaceVessels
{
    public class PartsProjection : MonoBehaviour
    {
        public Transform Anchor { get; internal set; }
        public bool IsValidPosition { get { return activeCollisions.Count == 0; } }
        public bool Visible { get; private set; }

        private CollisionsList activeCollisions = new CollisionsList();

        private PartsProjectionSettings settings;
        private Material instanceMaterial;
        private Renderer[] renderers;

        /// <summary>
        /// Creates projection for all connected to the original parts.
        /// </summary>
        public static PartsProjection Create(VesselPart original, Transform node, PartsProjectionSettings projectionSettings)
        {
            var material = Instantiate(projectionSettings.projectionMaterial);
            var projection = PartsProjectionFactory.Create(original, node, material);

            projection.settings = projectionSettings;
            projection.instanceMaterial = material;

            return projection;
        }

        void Awake()
        {
            Visible = true;      
        }

        void OnDisable()
        {
            activeCollisions.ClearCollisions();
        }

        void OnTriggerEnter(Collider other)
        {
            activeCollisions.AddCollision(other);

            UpdateColor();
        }

        void OnTriggerExit(Collider other)
        {
            activeCollisions.RemoveCollision(other);

            UpdateColor();
        }

        public void SetVisible(bool visible)
        {
            if (visible == Visible) return;

            if (renderers == null)
                renderers = GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = visible;
            }

            Visible = visible;
        }

        public void AddToCollisionIgnoreList(params VesselPart[] parts)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                activeCollisions.AddToIgnoreList(parts[i].Colliders);
            }

            UpdateColor();
        }

        public void RemoveFromCollisionIgnoreList(params VesselPart[] parts)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                activeCollisions.RemoveFromIgnoreList(parts[i].Colliders);
            }
        }

        /// <summary>
        /// Snaps projection to the attachment node.
        /// </summary>
        public void SnapTo(AttachmentNode node, float rotationZ)
        {
            Quaternion zRotation = Quaternion.AngleAxis(rotationZ, node.transform.forward);
            Quaternion snapRotation = zRotation * Utils.SnapToRotation(Anchor.up, Anchor.forward, node.transform.up, node.transform.forward);

            Vector3 snapTranslation = node.transform.position - Utils.RotateAround(transform.position, Anchor.position, snapRotation);

            transform.rotation = snapRotation * transform.rotation;
            transform.position = transform.position + snapTranslation;
        }

        private void UpdateColor()
        {
            Color color;

            if (activeCollisions.Count == 0)
            {
                color = settings.validPositionColor;
            }
            else
            {
                color = settings.invalidPositionColor;
            }

            instanceMaterial.SetColor("_TintColor", color);
        }

        private class CollisionsList
        {
            private List<Collider> activeCollisions = new List<Collider>();
            private List<Collider> ignoreCollisions = new List<Collider>();

            public int Count { get { return activeCollisions.Count; } }

            public void AddCollision(Collider collider)
            {
                if (ignoreCollisions.Contains(collider) == false)
                {
                    activeCollisions.Add(collider);
                }
            }

            public void RemoveCollision(Collider collider)
            {
                activeCollisions.Remove(collider);
            }

            public void ClearCollisions()
            {
                activeCollisions.Clear();
            }

            public void AddToIgnoreList(params Collider[] colliders)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    activeCollisions.Remove(colliders[i]);
                    ignoreCollisions.Add(colliders[i]);
                }
            }

            public void RemoveFromIgnoreList(params Collider[] colliders)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    ignoreCollisions.Remove(colliders[i]);
                }
            }

            public void ClearIgnoreList()
            {
                ignoreCollisions.Clear();
            }
        }
    }
}
