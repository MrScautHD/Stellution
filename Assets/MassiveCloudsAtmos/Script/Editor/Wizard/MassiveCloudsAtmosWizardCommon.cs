using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal static class MassiveCloudsAtmosWizardCommon
    {
        public static void CommonInfo()
        {
            var info = new EnvironmentDetector();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Unity Version");
                switch (info.UnityVersion.Supported)
                {
                    case VersionSupported.Supported:
                        MassiveCloudsAtmosWizard.Ok(info.UnityVersion.VersionString);
                        MassiveCloudsAtmosWizard.Ok("Supported");
                        break;
                    case VersionSupported.Unsupported:
                        MassiveCloudsAtmosWizard.Ng(info.UnityVersion.VersionString);
                        MassiveCloudsAtmosWizard.Ng("Unsupported");
                        break;
                    case VersionSupported.Unverified:
                        MassiveCloudsAtmosWizard.Ng(info.UnityVersion.VersionString);
                        MassiveCloudsAtmosWizard.Ng("Unverified");
                        break;
                    case VersionSupported.Unknown:
                    default:
                        MassiveCloudsAtmosWizard.Warn(info.UnityVersion.VersionString);
                        MassiveCloudsAtmosWizard.Warn("Unknown");
                        break;
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Render Pipeline");
                MassiveCloudsAtmosWizard.Ok(info.Pipeline.ToString());
            }

            if (info.Pipeline == EnvironmentDetector.PipelineType.UniversalRP)
            { 
                VerifyUniversalRP();
            }

            if (info.Pipeline == EnvironmentDetector.PipelineType.HDRP)
            { 
                VerifyHDRP();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Color Space");
                MassiveCloudsAtmosWizard.Ok(PlayerSettings.colorSpace.ToString());
                if (GUILayout.Button("Open Project Settings"))
                {
                    EditorApplication.ExecuteMenuItem( "Edit/Project Settings...");
                }
            }
            if (PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                EditorGUILayout.HelpBox("Linear Color Space is highly recommended." +
                                        "Please adjust \"Sun Intensity Scale\" in \"Massive Clouds Renderer\" > \"Advanced Setting\"", MessageType.Warning);
            }
            MassiveCloudsEditorGUI.Space();
        }

        private static void VerifyUniversalRP()
        {
            var finder = MassiveCloudsAtmosWizard.URPPackageFinder;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("UniversalRP Version");
                if (finder.IsCompleted)
                {
                    var packageInfo = finder.PackageInfo;
                    if (packageInfo == null)
                        MassiveCloudsAtmosWizard.Ng("No package found.");
                    else
                    {
                        var supported = VersionSupported.Unknown;
                        if (finder.PackageVersionInfo.Major < 7)
                        {
                            supported = VersionSupported.Unsupported;
                        }
                        else if (finder.PackageVersionInfo.Major == 7)
                        {
                            if (finder.PackageVersionInfo.Minor >= 1)
                                supported = VersionSupported.Supported;
                            else
                                supported = VersionSupported.Unsupported;
                        }
                        else if (finder.PackageVersionInfo.Major == 10)
                        {
                            if (finder.PackageVersionInfo.Minor < 2)
                                supported = VersionSupported.Unsupported;
                            else if (finder.PackageVersionInfo.Minor == 2 && finder.PackageVersionInfo.Maintenance == 2)
                                supported = VersionSupported.Supported;
                            else
                                supported = VersionSupported.Unverified;
                        }
                        else
                            supported = VersionSupported.Unverified;

                        if (supported == VersionSupported.Supported)
                        {
                            MassiveCloudsAtmosWizard.Ok(packageInfo.version);
                            MassiveCloudsAtmosWizard.Ok(supported.ToString());
                        }
                        else if (supported == VersionSupported.Unsupported)
                        {
                            MassiveCloudsAtmosWizard.Ng(packageInfo.version);
                            MassiveCloudsAtmosWizard.Ng(supported.ToString());
                        }
                        else
                        {
                            MassiveCloudsAtmosWizard.Warn(packageInfo.version);
                            MassiveCloudsAtmosWizard.Warn(supported.ToString());
                        }
                    }
                }
                else
                {
                    MassiveCloudsAtmosWizard.Warn("Finding package information...");
                }
            }
        }

        private static void VerifyHDRP()
        {
            var finder = MassiveCloudsAtmosWizard.HDRPPackageFinder;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("HDRP Version");
                if (finder.IsCompleted)
                {
                    var packageInfo = finder.PackageInfo;
                    if (packageInfo == null)
                        MassiveCloudsAtmosWizard.Ng("No package found.");
                    else
                    {
                        var supported = VersionSupported.Unknown;
                        if (finder.PackageVersionInfo.Major < 7)
                        {
                            supported = VersionSupported.Unsupported;
                        }
                        else if (finder.PackageVersionInfo.Major == 7)
                        {
                            if (finder.PackageVersionInfo.Minor >= 1)
                                supported = VersionSupported.Supported;
                            else
                                supported = VersionSupported.Unsupported;
                        }
                        else if (finder.PackageVersionInfo.Major == 10)
                        {
                            if (finder.PackageVersionInfo.Minor < 2)
                                supported = VersionSupported.Unsupported;
                            else if (finder.PackageVersionInfo.Minor == 2 && finder.PackageVersionInfo.Maintenance == 2)
                                supported = VersionSupported.Supported;
                            else
                                supported = VersionSupported.Unverified;
                        }
                        else
                            supported = VersionSupported.Unverified;

                        if (supported == VersionSupported.Supported)
                        {
                            MassiveCloudsAtmosWizard.Ok(packageInfo.version);
                            MassiveCloudsAtmosWizard.Ok(supported.ToString());
                        }
                        else if (supported == VersionSupported.Unsupported)
                        {
                            MassiveCloudsAtmosWizard.Ng(packageInfo.version);
                            MassiveCloudsAtmosWizard.Ng(supported.ToString());
                        }
                        else
                        {
                            MassiveCloudsAtmosWizard.Warn(packageInfo.version);
                            MassiveCloudsAtmosWizard.Warn(supported.ToString());
                        }
                    }
                }
                else
                {
                    MassiveCloudsAtmosWizard.Warn("Finding package information...");
                }
            }
        }
    }
}