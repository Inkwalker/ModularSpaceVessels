using UnityEngine;

namespace ModularSpaceVessels.VesselBuilders
{
    public class NodePickVesselBuilder : VesselBuilder
    {
        [SerializeField] private PartsProjectionSettings projectionSettings;

        private const float CastDistance = 50f;

        private VesselPart selectedPart;
        private AttachmentNode selectedNode;
        private AttachmentNode lastSnapToNode;
        private PartsProjection selectedPartProjection;
        private float partRotation;

        void Update()
        {
            AttachmentNode mouseOverNode = CastNearestNode(Camera.main, Input.mousePosition, CastDistance);

            if (Input.GetMouseButtonDown(0))
            {
                if (selectedPart == null)
                {
                    //Pick a new part
                    if (mouseOverNode != null)
                    {
                        SelectNode(mouseOverNode);
                    }
                }
                else
                {
                    //Attach or drop selected part
                    if (mouseOverNode != null && mouseOverNode.Parent != selectedPart)
                    {
                        selectedPartProjection.AddToCollisionIgnoreList(mouseOverNode.Parent);
                        selectedPartProjection.SnapTo(mouseOverNode, partRotation);

                        if (selectedPartProjection.IsValidPosition)
                        {
                            selectedNode.SnapAndAttach(mouseOverNode, partRotation);
                            DeselectNode();
                        }
                    }
                    else
                    {
                        DeselectNode();
                    }
                }
            }

            if (selectedPart != null)
            {
                if (mouseOverNode != null && mouseOverNode.Parent != selectedPart && mouseOverNode.Parent.Vessel != selectedPart.Vessel)
                {
                    if (lastSnapToNode != mouseOverNode)
                    {
                        selectedPartProjection.AddToCollisionIgnoreList(mouseOverNode.Parent);

                        selectedPartProjection.SetVisible(true);
                    }

                    selectedPartProjection.SnapTo(mouseOverNode, partRotation);
                }
                else
                {
                    if (lastSnapToNode != null)
                    {
                        selectedPartProjection.RemoveFromCollisionIgnoreList(lastSnapToNode.Parent);
                    }
                    selectedPartProjection.SetVisible(false);
                }

                HandleRotationInput();
            }
            else
            {
                if (mouseOverNode != lastSnapToNode)
                {
                    if (mouseOverNode != null)
                    {
                        mouseOverNode.SetState(AttachmentNode.NodeState.Highlighted);
                    }
                    if (lastSnapToNode != null)
                    {
                        lastSnapToNode.SetState(AttachmentNode.NodeState.Unselected);
                    }
                }
            }
            lastSnapToNode = mouseOverNode;
        }

        private void HandleRotationInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                partRotation += 45f;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                partRotation -= 45f;
            }
        }

        private void SelectNode(AttachmentNode node)
        {
            if (selectedNode != null)
            {
                DeselectNode();
            }

            selectedNode = node;
            selectedPart = selectedNode.Parent;

            selectedPartProjection = PartsProjection.Create(
                selectedPart,
                selectedNode.transform,
                projectionSettings
            );

            selectedPartProjection.AddToCollisionIgnoreList(selectedPart.Vessel.Parts);

            selectedNode.SetState(AttachmentNode.NodeState.Selected);

            partRotation = 0;
        }

        private void DeselectNode()
        {
            if (selectedPartProjection != null)
            {
                Destroy(selectedPartProjection.gameObject);
                selectedPartProjection = null;
            }

            selectedNode.SetState(AttachmentNode.NodeState.Unselected);

            selectedPart = null;
            selectedNode = null;
        }
    }
}
