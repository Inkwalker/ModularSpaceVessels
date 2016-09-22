using UnityEngine;
using System.Collections.Generic;
using ModularSpaceVessels.Managers;

namespace ModularSpaceVessels
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vessel : MonoBehaviour
    {
        public VesselPart[] Parts { get { return parts.ToArray(); } }
        public Rigidbody Rigidbody { get; private set; }

        private List<VesselPart> parts;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            parts = new List<VesselPart>(GetComponentsInChildren<VesselPart>());

            UpdateMass();
        }

        void OnPartAttached(VesselPart part)
        {
            if (parts.Contains(part)) return;

            parts.Add(part);

            UpdateMass();
        }

        void OnPartDetached(VesselPart part)
        {
            parts.Remove(part);

            if (parts.Count > 0)
            {
                UpdateMass();
            }
            else
            {
                VesselManager.Instance.DestroyVessel(this);
            }
        }

        public void Split()
        {
            var assemblies = PartsAssembly.FindAssemblies(parts);
            if (assemblies.Length > 1)
            {
                parts.Clear();
                parts.AddRange(assemblies[0].Parts);

                for (int i = 1; i < assemblies.Length; i++)
                {
                    Vessel vessel = VesselManager.Instance.CreateVessel(assemblies[i].Parts);

                    vessel.Rigidbody.velocity = Rigidbody.GetPointVelocity(vessel.transform.position);
                    vessel.Rigidbody.angularVelocity = Rigidbody.angularVelocity;
                }

                UpdateCenter();
                UpdateMass();
      
            }
        }

        public void Marge(Vessel vessel)
        {
            foreach (var part in vessel.Parts)
            {
                part.AssignToVessel(this);
            }

            VesselManager.Instance.DestroyVessel(vessel);

            UpdateCenter();
            UpdateMass();
        }

        private void UpdateMass()
        {
            Vector3 CoM = Vector3.zero;

            float totalMass = 0;

            foreach (var part in parts)
            {
                CoM += part.transform.position * part.Mass;
                totalMass += part.Mass;
            }

            CoM = totalMass == 0 ? Vector3.zero : CoM / totalMass;

            Vector3 localCoM = transform.InverseTransformPoint(CoM);

            Rigidbody.mass = totalMass;
            Rigidbody.centerOfMass = localCoM;
        }

        private void UpdateCenter()
        {

            Vector3 center = Vector3.zero;
            for (int i = 0; i < parts.Count; i++)
            {
                center += parts[i].transform.position;
            }
            center = center / parts.Count;

            SetCenter(center);
        }

        private void SetCenter(Vector3 worlPosition)
        {
            Vector3 offset = worlPosition - transform.position;

            foreach (Transform chid in transform)
            {
                chid.position -= offset;
            }

            transform.position += offset;
        }
    }
}
