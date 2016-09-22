using UnityEngine;
using System.Collections.Generic;

namespace ModularSpaceVessels
{
    public class VesselPart : MonoBehaviour
    {
        private const string OnPartAttachedMessage = "OnPartAttached";

        [SerializeField] private float mass = 1f;

        public Vessel Vessel { get; set; }
        public float Mass { get { return mass; } set { mass = value; } }
        public AttachmentNode[] AttachmentNodes { get; private set; }
        public Collider[] Colliders { get; private set; }

        void Awake()
        {
            Vessel = GetComponentInParent<Vessel>();
            AttachmentNodes = GetComponentsInChildren<AttachmentNode>(true);
            Colliders = GetComponentsInChildren<Collider>();
        }

        //add the part to a vessel without connecting to other parts
        public void AssignToVessel(Vessel vessel)
        {
            transform.parent = vessel.transform;
            Vessel = vessel;

            vessel.SendMessage(OnPartAttachedMessage, this);
        }

        //Returns directly attached parts
        public VesselPart[] GetAttachedParts()
        {
            var parts = new List<VesselPart>();

            foreach (var node in AttachmentNodes)
            {
                if (node.AttachedTo != null)
                {
                    if (parts.Contains(node.AttachedTo.Parent) == false)
                    {
                        parts.Add(node.AttachedTo.Parent);
                    }
                }
            }

            return parts.ToArray();
        }

        public PartsAssembly GetAllConnectedParts()
        {
            return PartsAssembly.FromConnected(this);
        }

        #region AttachmentNode messages

        void OnNodeDetached(AttachmentNode sender)
        {
            if (Vessel != null)
            {
                Vessel.Split();
            }
        }

        void OnNodeAttached(AttachmentNode sender)
        {
            if (sender.AttachedTo.Parent == null)
            {
                Debug.LogWarning("The part was attached to a node without a parent.");
                return;
            }

            Vessel attachedToVessel = sender.AttachedTo.Parent.Vessel;

            if (attachedToVessel != Vessel)
            {
                attachedToVessel.Marge(Vessel);
            }
        }

        #endregion
    }
}
