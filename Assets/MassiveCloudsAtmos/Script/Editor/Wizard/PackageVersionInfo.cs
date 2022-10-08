using System.Text.RegularExpressions;
using UnityEditor.PackageManager;

namespace Mewlist.MassiveClouds
{
    public class PackageVersionInfo
    {
        private const string VersionExp = "^([0-9]+)\\.([0-9]+)\\.([0-9]+)$";
        public readonly int Maintenance;
        public readonly int Major;
        public readonly int Minor;
        public readonly string VersionString;

        public PackageVersionInfo(PackageInfo packageInfo)
        {
            VersionString = packageInfo.version;
            var regex = new Regex(VersionExp);
            var match = regex.Match(VersionString);
            Major = int.Parse(match.Groups[1].Value);
            Minor = int.Parse(match.Groups[2].Value);
            Maintenance = int.Parse(match.Groups[3].Value);
        }
    }
}