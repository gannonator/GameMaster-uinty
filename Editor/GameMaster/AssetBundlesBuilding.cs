using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
namespace GameMaster.Build
{
    public class AssetBundlesBuilding
    {
        [MenuItem("Build/AssetBundles/Build AssetBundles For Editor")]
        static void BuildAssetBundlesForEditor()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            BuildAllAssetBundles(assetBundleDirectory);
        }

        [MenuItem("Build/AssetBundles/Build AssetBundles For Editor and Run")]
        static void BuildAssetBundlesForEditorAndRun()
        {
           
            string assetBundleDirectory = "Assets/StreamingAssets";
            if (BuildAllAssetBundles(assetBundleDirectory))
            {
                EditorApplication.isPlaying = true;
            }
            
        }
        [MenuItem("Build/AssetBundles/Build AssetBundles Build")]
        static void BuildAssetBundlesForBuild()
        {
            string assetBundleDirectory = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget).Replace(".exe", "_Data/StreamingAssets");
            if (!Directory.Exists(assetBundleDirectory))
            {
                UnityEngine.Debug.LogError("Can not find Build you need to build at least once ");
                return;
            }
            BuildAllAssetBundles(assetBundleDirectory);
        }
        [MenuItem("Build/AssetBundles/Build AssetBundles Build and Run ")]
        static void BuildAssetBundlesForBuildAndRun()
        {
            string assetBundleDirectory = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget).Replace(".exe", "_Data/StreamingAssets");
            if (!Directory.Exists(assetBundleDirectory))
            {
                UnityEngine.Debug.LogError("Can not find Build you need to build at least once ");
                return;
            }
            if (BuildAllAssetBundles(assetBundleDirectory))
            {
                Process.Start(EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget));
            }
        }

        private static bool BuildAllAssetBundles(string assetBundleDirectory)
        {
            UnityEngine.Debug.Log("Starting Assets Bundle Build");
            if (string.IsNullOrEmpty(assetBundleDirectory))
            {
                UnityEngine.Debug.LogError("Bundle path is not valid");
                return false;
            }
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            UnityEngine.Debug.Log("Complete Assets Bundle Build");
            return true;
        }

    }
}

