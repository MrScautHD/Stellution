namespace Mewlist.MassiveClouds
{
    public class AtmosWeightedProfile
    {
        public readonly AtmosProfile Profile;
        public readonly float Weight;

        public AtmosWeightedProfile(AtmosProfile profile, float weight)
        {
            Profile = profile;
            Weight = weight;
        }
    }
}