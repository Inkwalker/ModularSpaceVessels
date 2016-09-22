using UnityEngine;

namespace ModularSpaceVessels.Internal
{
    internal static class PartsProjectionFactory
    {
        private const string AnchorGameObjectName = "Anchor";
        private const string ProjectionGameObjectNameFormat = "Projection [{0}]";
        private const string MeshGameObjectNameFormat = "Mesh - {0}";
        private const string TriggerGameObjectNameFormat = "Trigger - {0}";
        private const float  TriggerScale = 0.95f;

        internal static PartsProjection Create(VesselPart original, Transform node, Material defaultMaterial)
        {
            var projectionGameObject = new GameObject(string.Format(ProjectionGameObjectNameFormat, original.name));
            var projection = projectionGameObject.AddComponent<PartsProjection>();

            var rigidbody = projectionGameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            
            projectionGameObject.transform.position = original.transform.position;
            projectionGameObject.transform.rotation = original.transform.rotation;

            projection.Anchor = CreateAnchorGO(projectionGameObject.transform, node.position, node.rotation);

            VesselPart[] connectedParts = original.Vessel.Parts;

            foreach (var part in connectedParts)
            {
                var partProjection = CreatePartProjection(part, defaultMaterial);
                partProjection.transform.parent = projectionGameObject.transform;
            }

            return projection;
        }

        private static GameObject CreatePartProjection(VesselPart original, Material mat)
        {
            GameObject GO = new GameObject(original.name);
            GO.transform.position = original.transform.position;
            GO.transform.rotation = original.transform.rotation;

            //NOTE: CombineMeshes doesn't realy work for meshes with several sub meshes.
            MeshFilter[] meshes = original.gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshes)
            {
                if (meshFilter.gameObject.activeInHierarchy == false) continue;

                var projectionMeshGO = new GameObject(string.Format(MeshGameObjectNameFormat, meshFilter.name));
                var projectionMesh = projectionMeshGO.AddComponent<MeshFilter>();
                var projectionRenderer = projectionMeshGO.AddComponent<MeshRenderer>();

                projectionMesh.sharedMesh = meshFilter.sharedMesh;

                //Make sure you set parent position first
                projectionMeshGO.transform.parent = GO.transform;
                projectionMeshGO.transform.position = meshFilter.transform.position;
                projectionMeshGO.transform.rotation = meshFilter.transform.rotation;
                projectionMeshGO.transform.localScale = meshFilter.transform.lossyScale;

                //No shadows
                projectionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                projectionRenderer.receiveShadows = false;

                //Set the same material for all sub meshes
                var projectionMaterials = new Material[meshFilter.mesh.subMeshCount];
                for (int i = 0; i < projectionMaterials.Length; i++)
                {
                    projectionMaterials[i] = mat;
                }
                projectionRenderer.sharedMaterials = projectionMaterials;
            }

            MeshCollider[] colliders = original.gameObject.GetComponentsInChildren<MeshCollider>();
            foreach (var collider in colliders)
            {
                if (collider.gameObject.activeInHierarchy == false) continue;

                var trigger = CreateTrigger(collider);
                trigger.parent = GO.transform;
            }

            return GO;
        }

        private static Transform CreateTrigger(Collider original)
        {
            var triggerGO = new GameObject(string.Format(TriggerGameObjectNameFormat, original.name));

            triggerGO.transform.position = original.transform.position;
            triggerGO.transform.rotation = original.transform.rotation;
            triggerGO.transform.localScale = original.transform.lossyScale * TriggerScale;

            triggerGO.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (original is MeshCollider)
            {
                var meshTrigger = triggerGO.AddComponent<MeshCollider>();
                
                meshTrigger.sharedMesh = ((MeshCollider)original).sharedMesh;
                meshTrigger.convex = true;
                meshTrigger.isTrigger = true;
            }
            else if (original is BoxCollider)
            {
                var boxTrigger = triggerGO.AddComponent<BoxCollider>();

                var originalBox = original as BoxCollider;

                boxTrigger.size = originalBox.size;
                boxTrigger.center = originalBox.center;
                boxTrigger.isTrigger = true;
            }
            else if (original is SphereCollider)
            {
                var sphereTrigger = triggerGO.AddComponent<SphereCollider>();

                var originalSphere = original as SphereCollider;

                sphereTrigger.radius = originalSphere.radius;
                sphereTrigger.center = originalSphere.center;

                sphereTrigger.isTrigger = true;
            }
            else if (original is CapsuleCollider)
            {
                var capsuleTrigger = triggerGO.AddComponent<CapsuleCollider>();

                var origianlCapsule = original as CapsuleCollider;

                capsuleTrigger.radius = origianlCapsule.radius;
                capsuleTrigger.height = origianlCapsule.height;
                capsuleTrigger.direction = origianlCapsule.direction;
                capsuleTrigger.center = origianlCapsule.center;

                capsuleTrigger.isTrigger = true;
            }

            return triggerGO.transform;
        }

        private static Transform CreateAnchorGO(Transform parent, Vector3 pos, Quaternion rot)
        {
            var anchor = new GameObject(AnchorGameObjectName).transform;
            anchor.parent = parent;
            anchor.position = pos;
            anchor.rotation = rot;

            return anchor;
        }
    }
}
