using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


namespace GameMaster
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public GMGame Game;
        /// <summary>
        /// The levels of the game in an Abstract sense. this includes menu
        /// </summary>
        public List<LevelSceneData> Levels;
        /// <summary>
        /// The First Level to load
        /// </summary>
        public LevelSceneData StartingLevel;
        /// <summary>
        /// The loaded Data package
        /// </summary>
        public LevelSceneData LoadedDatatpackage { get { return _loadedDatatpackage; } }
        /// <summary>
        /// The Loading Overlay to use
        /// </summary>
        public LoadingOverlay LoadingOverlay;

        public static event UnityAction<Scene, LoadSceneMode> OnSceneLoadedGameManager;
        public static event UnityAction<Scene> OnSceneUnloadedGameManager;
        public static event UnityAction<Scene> OnSceneReloadedGameManager;

        public enum LoadingState { LOADING, WATINGONLOCKS, LOADED}
        public LoadingState LoadState = LoadingState.LOADING;

        /// <summary>
        ///  The Sub Scene of the dataPackage
        /// </summary>
        private List<string> _LoadedSubScenes = new List<string>();
        /// <summary>
        /// A list of Sub Scene Thats Are loading
        /// </summary>
        private List<string> _loadingSubSceneList = new List<string>();

        /// <summary>
        /// A list of Sub Scene Thats Are loading
        /// </summary>
        private List<string> _sceneToReload = new List<string>();

        public string GetLastLoadedScene { get { if (_lastLoaedIndex < 0 || _lastLoaedIndex > LoadedDatatpackage.SceneList.Count) { return ""; }  return LoadedDatatpackage.SceneList[_lastLoaedIndex].Path; } }

        private int _lastLoaedIndex = -1;
        private LevelSceneData _loadedDatatpackage;
        private bool _isScenePackageLoaded;
        private AsyncOperation _asyncLoadScene;
        private int _sceneLoadingCount = 0;


        protected static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        public bool Loading()
        {
            return (_loadingSubSceneList.Count > 0);
        }



        protected virtual void Awake()
        {
            //Is this the first manager to be crated
            if (_instance == null)
            {
                _instance = this;
                if(Game)
                {
                    Levels = Game.Levels;
                    StartingLevel = Game.StartingLevel;
                }
                foreach(LevelSceneData level in Levels )
                {
                    if(level.IsAseetBundle && level.LoadAseetBundleOnStart)
                    {
                        level.LoadAssestBundels();
                    }
                }
            }
            else
            {
                //If not the first Destroy
                Destroy(gameObject);
            }
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        protected virtual void Start()
        {
            if (StartingLevel != null)
                LoadPackagecenes(StartingLevel);
        }

        protected virtual void Update()
        {
          
        }

        public void SetMasterAsActive()
        {
            if (LoadedDatatpackage)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(LoadedDatatpackage.MasterScene));
            }

        }
        public void SetDataPackageIndexAsActive(int index)
        {
            if (LoadedDatatpackage)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(LoadedDatatpackage[index].Path));
            }

        }

        #region SceneLoadingAndUnloading
        /// <summary>
        /// Dose not unload Master Scenes 
        /// </summary>
        public void UnloadAllSubScenes()
        {
            foreach (string sceneName in _LoadedSubScenes)
            {
                StartCoroutine(UnloadAsyncScene(sceneName));
            }
            _LoadedSubScenes.Clear();

        }
        /// <summary>
        /// Unload A Scene using the data package index
        /// </summary>
        /// <param name="index">The index of the scene to unload</param>
        public void UnloadScene(int index)
        {            
            UnloadScene(LoadedDatatpackage.SceneList[index].Path);
        }
        /// <summary>
        /// Unload a scene using a path 
        /// </summary>
        /// <param name="path"></param>
        public void UnloadScene(string path)
        {
            if (LoadingOverlay)
            {
                LoadingOverlay.Show();
            }
            Scene scene = SceneManager.GetSceneByPath(path);
            if (scene.IsValid())
            {
                StartCoroutine(UnloadAsyncScene(path));
            }
        }
        /// <summary>
        /// Loads a scene using the index in the data package
        /// </summary>
        /// <param name="index">The index if the scene in the loaded data package</param>
        /// <param name="unloadAll">Should All other scene be unloaded</param>
        public void LoadSceneWithDataPackageIndex(int index, bool unloadAll = false)
        {
            LoadSceneFromSceneData(LoadedDatatpackage.SceneList[index], unloadAll);
        }
        /// <summary>
        /// Loads a scene using the index in the data package
        /// </summary>
        /// <param name="index">The index if the scene in the loaded data package</param>
        /// <param name="unloadAll">Should All other scene be unloaded</param>
        public void LoadSceneWithDataPackageIndex(string quickName, bool unloadAll = false)
        {
            LoadSceneFromSceneData(LoadedDatatpackage[quickName], unloadAll);
        }

        /// <summary>
        /// Unloads And reloaded A scene
        /// </summary>
        /// <param name="path"></param>
        public void ReloadSceneInLoadedDataPackage(int index)
        {
            if(LoadedDatatpackage != null)
            {
                _sceneToReload.Add(LoadedDatatpackage[index].Path);
                UnloadScene(LoadedDatatpackage[index].Path);
            }
        }


        /// <summary>
        /// Unloads And reloaded A scene
        /// </summary>
        /// <param name="path"></param>
        public void ReloadScene(string path)
        {
            _sceneToReload.Add(path);
            UnloadScene(path);
        }

        /// <summary>
        /// Load a level from the level array
        /// </summary>
        /// <param name="index"></param>
        public void LoadLevelByIndex(int index)
        {
            if (index >= 0 && index < Levels.Count)
            {
                LoadPackagecenes(Levels[index]);
            }
        }
        /// <summary>
        /// Loaded A from the SceneData
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="unloadAll"></param>
        public void LoadSceneFromSceneData(SceneData scene, bool unloadAll = false)
        {
            scene._wasLoaded = true;
            _sceneLoadingCount = 0;
            if (unloadAll)
                UnloadAllSubScenes();
            _sceneLoadingCount = 1;
            _loadingSubSceneList.Add(scene.Path);
            StartCoroutine(LoadAsyncScene(scene.Path));
        }

        /// <summary>
        /// Loaded A from a Scene Path
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="unloadAll"></param>
        public void LoadSceneFromPath(string path, bool unloadAll = false)
        {
            _sceneLoadingCount = 0;
            if (unloadAll)
                UnloadAllSubScenes();
            _sceneLoadingCount = 1;
            _loadingSubSceneList.Add(path);
            StartCoroutine(LoadAsyncScene(path));
        }
        /// <summary>
        /// Loads all defult scen in a level and runs the LevelScript;
        /// </summary>
        /// <param name="dataPackage"></param>
        public void LoadPackagecenes(LevelSceneData dataPackage)
        {
            //Time.timeScale = 0;
            if (LoadingOverlay)
            {
                LoadingOverlay.Show();
            }
            if (dataPackage.IsAseetBundle && !dataPackage.LoadAseetBundleOnStart)//is a just in time loaded bundle
            {
                dataPackage.LoadAssestBundels();
            }
                UnloadAllSubScenes();
            if (_loadedDatatpackage != null)
            {
                _loadedDatatpackage = null;
                PakageUnloaded(dataPackage);
            }
                _sceneLoadingCount = 0;
            _loadedDatatpackage = dataPackage;
            
            if (dataPackage.MasterScene != "")
            {
                StartCoroutine(LoadAsyncScene(dataPackage.MasterScene));
                _loadingSubSceneList.Add(dataPackage.MasterScene);
                _sceneLoadingCount++;
            }
            for (int index = 0; index < dataPackage.SceneList.Count; index++)
            {
                if (dataPackage.SceneList[index].LoadOnPackageLoad)
                {
                    StartCoroutine(LoadAsyncScene(dataPackage.SceneList[index].Path));

                    _loadingSubSceneList.Add(dataPackage.SceneList[index].Path);
                    _sceneLoadingCount++;
                }
            }
            

        }

        private void SceneUnloaded(Scene unloadedScene)
        {
            if (_LoadedSubScenes.Contains(unloadedScene.path)) 
            _LoadedSubScenes.Remove(unloadedScene.path);
            if (_sceneToReload.Contains(unloadedScene.path))
            {
                LoadSceneFromPath(unloadedScene.path);
            }
            OnSceneUnloadedGameManager?.Invoke(unloadedScene);
        }

        private void SceneLoaded(Scene loadedScene, LoadSceneMode mode)
        {

            if (_loadingSubSceneList.Contains(loadedScene.path))
            {
                _loadingSubSceneList.Remove(loadedScene.path);
                _LoadedSubScenes.Add(loadedScene.path);
            }
            _sceneLoadingCount--;
            if (_sceneLoadingCount == 0)
            {
                StartCoroutine(DoneLoadingCheck());
            }
            if (_loadedDatatpackage != null && _loadedDatatpackage.LevelScript != null)
            {
                if (loadedScene.path == _loadedDatatpackage.MasterScene)
                {
                    SceneManager.SetActiveScene(loadedScene);
                    _loadedDatatpackage.LevelScript.OnMasterLoaded();
                }
               
                _loadedDatatpackage?.LevelScript.OnSceneLoaded(loadedScene);
            }
            if (_sceneToReload.Contains(loadedScene.path))
            {
                _sceneToReload.Remove(loadedScene.path);

                OnSceneReloadedGameManager?.Invoke(loadedScene);
            }
            OnSceneLoadedGameManager?.Invoke(loadedScene, mode);
            
        }


        IEnumerator DoneLoadingCheck()
        {
            LoadState = LoadingState.WATINGONLOCKS;
            _loadedDatatpackage?.LevelScript?.OnLoading();
            if (LoadingOverlay)// if an loading overlay exist 
            {
                // Wait until the asynchronous scene fully loads
                while (LoadingLocks.IsLocked())
                {
                    //Debug.Log("waiting on loading lock");
                    yield return null;
                }
            }
            //Time.timeScale = 1;
            LoadingOverlay.Hide();
            if (_loadedDatatpackage)
            {
                _loadedDatatpackage?.LevelScript?.OnLoaded();
            }
            LoadState = LoadingState.LOADED;
        }


        IEnumerator LoadAsyncScene(string name)
        {

            _asyncLoadScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            
            // Wait until the asynchronous scene fully loads
            while (!_asyncLoadScene.isDone)
            {
                if (LoadingOverlay)
                {
                    //Debug.Log(_asyncLoadScene.progress);
                    LoadingOverlay.loadingAmount = _asyncLoadScene.progress;
                }
                yield return null;
            }
            yield return null;
        }

        IEnumerator UnloadAsyncScene(string name)
        {
            _asyncLoadScene = SceneManager.UnloadSceneAsync(name);


            // Wait until the asynchronous scene fully loads
            while (!_asyncLoadScene.isDone)
            {
                yield return null;
            }
            yield return null;
        }

        private void PakageUnloaded(LevelSceneData dataPackage)
        {
            if (dataPackage.IsAseetBundle && dataPackage.UnloadAseetBundleOnPakageUnload)//is a just in time unloaded bundle
            {
                dataPackage.UnloadAssestBundels();
            }
        }
        #endregion

        protected virtual void DoneLoadingDataPagkage()
        {
            if (_loadedDatatpackage != null && _loadedDatatpackage.LevelScript != null)
            {
                LoadedDatatpackage.LevelScript.OnLoaded();
            }
        }

    }
}
