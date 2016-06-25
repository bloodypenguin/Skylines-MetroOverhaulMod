using MetroOverhaul.Redirection;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(MetroTrainAI))]
    public class MetroTrainAIDetour : TrainAI
    {
        [RedirectMethod]
        public override void CreateVehicle(ushort vehicleID, ref Vehicle data)
        {
            base.CreateVehicle(vehicleID, ref data);
            //begin mod
            //end mod
        }

        [RedirectMethod]
        public override void LoadVehicle(ushort vehicleID, ref Vehicle data)
        {
            base.LoadVehicle(vehicleID, ref data);
            //begin mod
            //end mod
        }
    }
}