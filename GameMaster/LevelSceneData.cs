using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    [System.Serializable]
    public class SceneData
    {
        public string Path;
        public string QuickName;
        public bool LoadOnPackageLoad;
        internal bool _wasLoaded;
        public bool WasLoaded { get { return _wasLoaded;} }
    }

    [CreateAssetMenu(fileName = "newLevelSceneData", menuName = "LevelData/LevelSceneData", order = 1)]
    [System.Serializable]
    public class LevelSceneData : ScriptableObject
    {
        /// <summary>
        /// A List of Scenes in order to be loaded
        /// </summary>
        public List<SceneData> SceneList = new List<SceneData>();
        /// <summary>
        /// The package name
        /// </summary>
        public string Name;
        /// <summary>
        /// A Scene thats is will be loaded and unloaded with the package
        /// Lave this blanks to not use;
        /// </summary>
        public string MasterScene = "";
        internal int _activeScene = 0;
        /// <summary>
        /// The Type Of The Level Script 
        /// </summary>
        public string levelScriptType;


        public bool BypassBundleInEditor = false;
        #region AssetBundle
        /// <summary>
        /// The scenes are in an  AssetBundel 
        /// </summary>
        public bool IsAseetBundle;
        /// <summary>
        /// Has An Additional AseetBundle the contain non scenes as unity requires Scenes ans Assets be packed in deterrent bundles  
        /// </summary>
        public bool HasAdditionalAseetBundle;
        /// <summary>
        /// Should  the bundle be loaded when the game start or just in time 
        /// </summary>
        public bool LoadAseetBundleOnStart;
        /// <summary>
        /// the Bundle name
        /// </summary>
        public string AssestBundeName;
        /// <summary>
        /// The name of the Additional AseetBundlel
        /// </summary>
        public string AdditionalAseetBundleName;
        /// <summary>
        /// Should the assets bundle be unloaded when the package is unloaded or when the game is closed
        /// </summary>
        public bool UnloadAseetBundleOnPakageUnload;
        /// <summary>
        /// Public accses to the AdditionalAseetBundle as this script only loded the bundle not the assets but dose unload them
        /// </summary>
        public AssetBundle AdditionalAseetBundle { get { return _additionalAseetBundle; } }
        private AssetBundle _assetBundle;
        private AssetBundle _additionalAseetBundle;
        #endregion
        /// <summary>
        /// The Script
        /// </summary>
        public LevelLoadingScript LevelScript
        {
            get
            {
                if (_levelScript == null && levelScriptType.Trim() != "")
                {
                    _levelScript = (LevelLoadingScript)Activator.CreateInstance(Type.GetType(levelScriptType));
                }
                return _levelScript;
            }
        }

        private LevelLoadingScript _levelScript;


        /// <summary>
        /// Get all loaded scenes but not the master
        /// </summary>
        /// <returns></returns>
        public List<SceneData> GetActiveScenes()
        {
            List<SceneData> temp = new List<SceneData>();
            foreach(SceneData scene in  SceneList)
            {
                if(scene.WasLoaded)
                {
                    temp.Add(scene);
                }
            }
            return temp;
        }

        public  SceneData Unload(int index)
        {
            SceneList[index]._wasLoaded = false;
            return SceneList[index];
        }

        public SceneData this[string key]
        {
            get
            {
                SceneData returnVal = new SceneData();
                foreach (SceneData data in SceneList)
                {
                    if (data.QuickName == key)
                    {
                        return data;
                    }
                }
                return null;
            }
            set
            {

                for (int i = 0;  i < SceneList.Count; i++ ) 
                {
                    SceneData data = SceneList[i];
                    if (data.QuickName == key)
                    {
                        SceneList[i] = value;
                    }
                }

            }

        }
        public SceneData this[int key]
        {
            get
            {

                return SceneList[key];
            }
            set 
            {
                SceneList[key] = value;
            }

        }

        public void LoadAssestBundels()
        {
#if UNITY_EDITOR
            if (IsAseetBundle && !BypassBundleInEditor)
#else
            if (IsAseetBundle)
#endif

            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Loading " + AssestBundeName);
                }
                _assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, AssestBundeName));                
            }
            if(HasAdditionalAseetBundle)
            {
                _additionalAseetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, AdditionalAseetBundleName));
            }
        }

        public void UnloadAssestBundels()
        {
            if (_assetBundle)
            {
                _assetBundle.Unload(true);
                if (Debug.isDebugBuild)
                {
                    Debug.Log("UnLoading " + AssestBundeName);
                }

            }
            if (HasAdditionalAseetBundle)
            {
                _assetBundle.Unload(true);
            }
        }
    }
}
