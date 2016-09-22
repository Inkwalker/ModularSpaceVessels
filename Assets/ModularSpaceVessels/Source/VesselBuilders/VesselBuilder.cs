using UnityEngine;

namespace ModularSpaceVessels.VesselBuilders
{
    public class VesselBuilder : MonoBehaviour
    {
        private const string AttachmentNodeTag = "AttachmentNode";
        private const string AttachmentNodeLayerName = "AttachmentNode";
        private const string VesselPartLayerName = "Default";

        protected static AttachmentNode FindNearestNode(VesselPart part, Vector3 point)
        {
            AttachmentNode nearestNode = null;
            for (int i = 0; i < part.AttachmentNodes.Length; i++)
            {
                if (part.AttachmentNodes[i].gameObject.activeSelf)
                {
                    if (nearestNode == null)
                    {
                        nearestNode = part.AttachmentNodes[i];
                    }
                    else
                    {
                        float d1 = (nearestNode.transform.position - point).sqrMagnitude;
                        float d2 = (part.AttachmentNodes[i].transform.position - point).sqrMagnitude;
                        if (d2 < d1)
                        {
                            nearestNode = part.AttachmentNodes[i];
                        }
                    }
                }
            }

            return nearestNode;
        }

        protected static AttachmentNode CastNode(Camera camera, Vector3 screenPoint, float maxDistance)
        {
            AttachmentNode node = null;

            Ray mouseRay = camera.ScreenPointToRay(screenPoint);

            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, maxDistance, LayerMask.GetMask(AttachmentNodeLayerName)))
            {
                if (hit.collider.CompareTag(AttachmentNodeTag))
                {
                    node = hit.collider.GetComponent<AttachmentNode>();
                }
            }

            return node;
        }

        protected static VesselPart CastPart(Camera camera, Vector3 screenPoint, float maxDistance)
        {
            Ray mouseRay = camera.ScreenPointToRay(screenPoint);

            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, maxDistance, LayerMask.GetMask(VesselPartLayerName)))
            {
                VesselPart part = hit.collider.GetComponentInParent<VesselPart>();
                return part;
            }

            return null;
        }

        protected static AttachmentNode CastNearestNode(Camera camera, Vector3 screenPoint, float maxDistance)
        {
            AttachmentNode node = null;

            Ray mouseRay = camera.ScreenPointToRay(screenPoint);

            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, maxDistance, LayerMask.GetMask(AttachmentNodeLayerName, VesselPartLayerName)))
            {
                if (hit.collider.CompareTag(AttachmentNodeTag))
                {
                    node = hit.collider.GetComponent<AttachmentNode>();
                }
                else
                {
                    VesselPart part = hit.collider.GetComponentInParent<VesselPart>();
                    if (part != null)
                    {
                        node = FindNearestNode(part, hit.point);
                    }
                }
            }

            return node;
        }
    }
}
