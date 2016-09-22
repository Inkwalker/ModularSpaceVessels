using ModularSpaceVessels.Internal;
using UnityEngine;

namespace ModularSpaceVessels
{
    public class AttachmentNode : MonoBehaviour
    {
        private const float MaxOverlapDistance = 0.1f;

        [SerializeField] private AttachmentNodeSettings settings;
        [SerializeField] private AttachmentNode attachedTo;

        public VesselPart Parent { get; private set; }
        public AttachmentNode AttachedTo { get { return attachedTo; } private set { attachedTo = value; } }

        private new MeshRenderer renderer;

        void Awake()
        {
            Parent = GetComponentInParent<VesselPart>();
            renderer = GetComponentInChildren<MeshRenderer>();

            renderer.sharedMaterial = settings.unselectedMaterial;

            //find attached node for preassembled in editor ships
            if (AttachedTo == null)
            {
                //check if overlaping other nodes
                Collider[] nodes = Physics.OverlapSphere(transform.position, MaxOverlapDistance, 1<<gameObject.layer);
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].gameObject != gameObject && 
                        Vector3.Distance(nodes[i].transform.position, transform.position) < MaxOverlapDistance)
                    {
                        var overlapingNode = nodes[i].GetComponent<AttachmentNode>();
                        if (overlapingNode != null)
                        {
                            AttachedTo = overlapingNode;

                            if (overlapingNode.AttachedTo == null)
                            {
                                overlapingNode.AttachedTo = this;
                            }
                            break;
                        }
                    }
                }
            }

            if (AttachedTo != null)
            {
                gameObject.SetActive(false);
            }
        }

        public void Attach(AttachmentNode node)
        {
            if (AttachedTo == node) return;
            if (AttachedTo != null)
            {
                Detach();
            }

            AttachedTo = node;
            node.AttachedTo = this;

            SendOnAttacheMessage();
            node.SendOnAttacheMessage();

            gameObject.SetActive(false);
            node.gameObject.SetActive(false);
        }

        public void SnapAndAttach(AttachmentNode node, float rotation_z)
        {
            SnapTo(node, rotation_z);
            Attach(node);
        }

        public void Detach()
        {
            if (AttachedTo == null) return;

            gameObject.SetActive(true);
            AttachedTo.gameObject.SetActive(true);

            AttachedTo.AttachedTo = null;
            AttachedTo.SendOnDetachedMessage();

            AttachedTo = null;
            SendOnDetachedMessage();
        }

        public void SnapTo(AttachmentNode node, float rotationZ)
        {
            Quaternion zRotation = Quaternion.AngleAxis(rotationZ, node.transform.forward);
            Quaternion snapRotation = zRotation * Utils.SnapToRotation(transform.up, transform.forward, node.transform.up, node.transform.forward);

            Vector3 snapTranslation = node.transform.position - Utils.RotateAround(node.transform.position, transform.position, snapRotation);

            PartsAssembly connectedParts = Parent.GetAllConnectedParts();

            for (int i = 0; i < connectedParts.Parts.Length; i++)
            {
                VesselPart part = connectedParts.Parts[i];

                Vector3 rotatedPosition = Utils.RotateAround(node.transform.position, part.transform.position, snapRotation);

                part.transform.rotation = snapRotation * part.transform.rotation;
                part.transform.position = rotatedPosition + snapTranslation;
            }
        }

        public void SetState(NodeState state)
        {
            switch (state)
            {
                case NodeState.Highlighted:
                    renderer.sharedMaterial = settings.highlightMaterial;
                    break;
                case NodeState.Selected:
                    renderer.sharedMaterial = settings.selectedMaterial;
                    break;
                case NodeState.Unselected:
                    renderer.sharedMaterial = settings.unselectedMaterial;
                    break;
            }
        }

        private void SendOnAttacheMessage()
        {
            Parent.SendMessage("OnNodeAttached", this);
        }

        private void SendOnDetachedMessage()
        {
            Parent.SendMessage("OnNodeDetached", this);
        }

        public enum NodeState
        {
            Highlighted,
            Selected,
            Unselected
        }
    }
}
