using System;
using UnityEngine;
using UnityEngine.Events;

namespace ModularSpaceVessels.Managers
{
    public class VesselManager : MonoBehaviour
    {
        private const string VesselGameObjectName = "Vessel {0}";

        private static VesselManager instance;

        [SerializeField] private GameObject vesselPrefab;

        public VesselEvent onVesselCreated;
        public VesselEvent onVesselDestroyed;

        public static VesselManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<VesselManager>();

                    if (instance == null)
                    {
                        Debug.LogError("There is no instance of VesselManager in the scene. Please create one.");
                        return null;
                    }
                }

                return instance;
            }
        }

        void Start()
        {
            //creating vessels for all unattached parts in the scene
            VesselPart[] parts = FindObjectsOfType<VesselPart>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].gameObject.activeInHierarchy && parts[i].transform.parent == null)
                {
                    PartsAssembly connected = parts[i].GetAllConnectedParts();

                    CreateVessel(connected.Parts);
                }
            }
        }

        public Vessel CreateVessel(params VesselPart[] parts)
        {
            var vesselGameObject = Instantiate(vesselPrefab);
            var vessel = vesselGameObject.GetComponent<Vessel>();

            vesselGameObject.name = CreateVesselName(vessel);

            Vector3 center = Vector3.zero;
            foreach (var part in parts)
            {
                center += part.transform.position;
            }
            center = center / parts.Length;

            vessel.transform.position = center;
            vessel.transform.rotation = parts[0].transform.rotation;

            foreach (var part in parts)
            {
                part.AssignToVessel(vessel);
            }

            onVesselCreated.Invoke(vessel);

            return vessel;
        }

        public void DestroyVessel(Vessel vessel)
        {
            onVesselDestroyed.Invoke(vessel);
            Destroy(vessel.gameObject);
        }

        protected virtual string CreateVesselName(Vessel vessel)
        {
            return string.Format(VesselGameObjectName, vessel.GetHashCode());
        }

        [Serializable]
        public class VesselEvent : UnityEvent<Vessel> { }
    }
}
