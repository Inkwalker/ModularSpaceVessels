using System.Collections.Generic;

namespace ModularSpaceVessels
{
    public class PartsAssembly
    {
        public VesselPart[] Parts { get; private set; }

        #region Static methods

        public static PartsAssembly[] FindAssemblies(IEnumerable<VesselPart> parts)
        {
            var assemblies = new List<PartsAssembly>();

            var unprocessedParts = new List<VesselPart>(parts);

            var connectedParts = new List<VesselPart>();
            while (unprocessedParts.Count > 0)
            {
                VesselPart current = unprocessedParts[0];

                GetConnectedParts(current, ref connectedParts);

                foreach (var part in connectedParts)
                {
                    unprocessedParts.Remove(part);
                }

                PartsAssembly assembly = new PartsAssembly(connectedParts.ToArray());
                assemblies.Add(assembly);

                connectedParts.Clear();
            }


            return assemblies.ToArray();
        }

        public static PartsAssembly FromConnected(VesselPart origin)
        {
            var connectedParts = new List<VesselPart>();
            GetConnectedParts(origin, ref connectedParts);

            var assembly = new PartsAssembly(connectedParts.ToArray());

            return assembly;
        }

        private static void GetConnectedParts(VesselPart part, ref List<VesselPart> connected)
        {
            if (connected.Contains(part) == false)
            {
                connected.Add(part);

                foreach (var item in part.GetAttachedParts())
                {
                    GetConnectedParts(item, ref connected);
                }
            }
        }

        #endregion

        public PartsAssembly(VesselPart[] parts)
        {
            Parts = parts;
        }

    }
}
