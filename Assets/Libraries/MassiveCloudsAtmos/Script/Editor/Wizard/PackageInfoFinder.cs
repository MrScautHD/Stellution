using System;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Mewlist.MassiveClouds
{
    internal class PackageInfoFinder
    {
        public PackageInfo PackageInfo;
        public PackageVersionInfo PackageVersionInfo;
        public bool IsCompleted = false;

        private Action<PackageInfo> callback;
        private ListRequest request;
        private string packageName;
        
        public void Find(string packageName, Action<PackageInfo> callback)
        {
            IsCompleted = false;
            this.packageName = packageName;
            this.callback = callback;
            request = Client.List();
            EditorApplication.update += Progress;
        }

        private void Progress()
        {
            if (request.IsCompleted)
            {
            
                if (request.Status == StatusCode.Success)
                {
                    if (request.Result.Any(x => x.name == packageName))
                    {
                        var packageInfo = request.Result.First(x => x.name == packageName);
                        IsCompleted = true;
                        PackageInfo = packageInfo;
                        PackageVersionInfo = new PackageVersionInfo(packageInfo);
                        callback(packageInfo);
                        EditorApplication.update -= Progress;
                        return;
                    }
                }
                else if (request.Status >= StatusCode.Failure)
                    Debug.Log(request.Error.message);

                IsCompleted = true;
                PackageInfo = null;
                callback(null);

                EditorApplication.update -= Progress;
            }
        }
    }
}