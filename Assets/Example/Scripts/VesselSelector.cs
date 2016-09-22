using UnityEngine;
using ModularSpaceVessels.Managers;
using ModularSpaceVessels;
using System.Collections.Generic;

public class VesselSelector : MonoBehaviour
{
    [SerializeField] private FreeLookCam cameraRig;

    private List<Vessel> vessels = new List<Vessel>();
    private int activeVesselIndex = 0;

    void Awake()
    {
        if (cameraRig == null)
        {
            cameraRig = FindObjectOfType<FreeLookCam>();
        }

        Vessel[] sceneVessels = FindObjectsOfType<Vessel>();
        for (int i = 0; i < sceneVessels.Length; i++)
        {
            RegisterVessel(sceneVessels[i]);
        }

        if (vessels.Count > 0)
        {
            cameraRig.SetTarget(vessels[activeVesselIndex].Rigidbody);
        }

        VesselManager.Instance.onVesselCreated.AddListener(OnVesselCreated);
        VesselManager.Instance.onVesselDestroyed.AddListener(OnVesselDestroyed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SelectPrevVessel();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SelectNextVessel();
        }
    }

    private void OnVesselCreated(Vessel vessel)
    {
        vessels.Add(vessel);
    }

    private void OnVesselDestroyed(Vessel vessel)
    {
        if (vessels[activeVesselIndex] == vessel)
            SelectPrevVessel();

        vessels.Remove(vessel);
    }

    private void SelectNextVessel()
    {
        Vessel nextTarget = null;

        if (vessels.Count > 0)
        {
            activeVesselIndex = (activeVesselIndex + 1) % vessels.Count;

            nextTarget = vessels[activeVesselIndex];
        }

        cameraRig.SetTarget(nextTarget.Rigidbody);
    }

    private void SelectPrevVessel()
    {
        Vessel nextTarget = null;

        if (vessels.Count > 0)
        {
            activeVesselIndex = activeVesselIndex - 1;

            if (activeVesselIndex < 0)
            {
                activeVesselIndex = vessels.Count - 1;
            }

            nextTarget = vessels[activeVesselIndex];
        }

        cameraRig.SetTarget(nextTarget.Rigidbody);
    }

    private void RegisterVessel(Vessel vessel)
    {
        if (vessels.Contains(vessel)) return;

        vessels.Add(vessel);
    }

    private void UnregisterVessel(Vessel vessel)
    {
        if (vessels[activeVesselIndex] == vessel)
        {
            SelectPrevVessel();
        }

        vessels.Remove(vessel);
    }
}
