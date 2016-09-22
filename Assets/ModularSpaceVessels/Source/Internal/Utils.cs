using UnityEngine;

namespace ModularSpaceVessels.Internal
{
    internal static class Utils
    {
        public static Vector3 RotateAround(Vector3 origin, Vector3 vector, Quaternion rotation)
        {
            return rotation * (vector - origin) + origin;
        }

        public static Quaternion SnapToRotation(Vector3 fromNodeUp, Vector3 fromNodeForward, Vector3 toNodeUp, Vector3 toNodeForward)
        {
            Quaternion rotationToNode = Quaternion.FromToRotation(fromNodeForward, -toNodeForward);

            Vector3 rotatedFromNodeUp = rotationToNode * fromNodeUp;

            //FromToRotation doesn't work if vectors are exactly oposite to each other
            Quaternion upAlignmentRotation;
            if (Mathf.Approximately(Vector3.Dot(rotatedFromNodeUp, toNodeUp), -1f))
            {
                upAlignmentRotation = Quaternion.AngleAxis(180f, toNodeForward);
            }
            else
            {
                upAlignmentRotation = Quaternion.FromToRotation(rotatedFromNodeUp, toNodeUp);
            }

            return upAlignmentRotation * rotationToNode;
        }

        
    }
}
