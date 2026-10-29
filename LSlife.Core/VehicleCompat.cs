using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife
{
    internal static class VehicleCompat
    {
        internal static float HalfLength(Model model)
        {
            model.GetDimensions(out Vector3 min, out Vector3 max);
            return (max.Y - min.Y) * 0.5f;
        }

        internal static bool IsDoorOpen(Vehicle vehicle, int door)
        {
            return Function.Call<float>(Hash.GET_VEHICLE_DOOR_ANGLE_RATIO, vehicle.Handle, door) > 0.01f;
        }

        internal static void OpenDoor(Vehicle vehicle, int door, bool loose, bool instantly)
        {
            Function.Call(Hash.SET_VEHICLE_DOOR_OPEN, vehicle.Handle, door, loose, instantly);
        }

        internal static void CloseDoor(Vehicle vehicle, int door, bool instantly)
        {
            Function.Call(Hash.SET_VEHICLE_DOOR_SHUT, vehicle.Handle, door, instantly);
        }
    }
}
