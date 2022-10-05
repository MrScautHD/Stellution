using System;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
    sealed class EnviroAddDefineSymbolsLW
    {
        const string k_Define = "ENVIRO_LW";

        static EnviroAddDefineSymbolsLW()
        {
            var targets = Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(x => x != BuildTargetGroup.Unknown)
                .Where(x => !IsObsolete(x));

            foreach (var target in targets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

                var list = defines.Split(';', ' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                if (list.Contains(k_Define))
                    continue;

                list.Add(k_Define);
                defines = list.Aggregate((a, b) => a + ";" + b);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
        }

        static bool IsObsolete(BuildTargetGroup group)
        {
            var attrs = typeof(BuildTargetGroup)
                .GetField(group.ToString())
                .GetCustomAttributes(typeof(ObsoleteAttribute), false);

            return attrs != null && attrs.Length > 0;
        }
    }