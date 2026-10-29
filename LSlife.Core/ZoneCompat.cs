using GTA.Math;
using GTA.Native;

namespace LSlife
{
    internal static class ZoneCompat
    {
        internal static string GetZoneNameLabel(Vector3 position)
        {
            return Function.Call<string>(Hash.GET_NAME_OF_ZONE, position.X, position.Y, position.Z);
        }

        internal static string GetZoneName(Vector3 position)
        {
            string label = GetZoneNameLabel(position);
            return Function.Call<string>(Hash.GET_LABEL_TEXT, label);
        }
    }
}
