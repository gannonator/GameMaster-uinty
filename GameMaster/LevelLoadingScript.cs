using System;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public abstract class LevelLoadingScript
    {
        /// <summary>
        /// Code to be run during set up while the loading screen is active
        /// </summary>
        public virtual void OnLoading()
        {

        }
        /// <summary>
        /// Code to run when all loading is done
        /// </summary>
        public virtual void OnLoaded()
        {
           
        }
        /// <summary>
        /// Code to run once the Master is loaded
        /// </summary>
        public virtual void OnMasterLoaded()
        {

        }
        /// <summary>
        /// Code To run on ever scen load on a single scene
        /// </summary>
        /// <param name="sceneData"></param>
        public virtual void OnSceneLoaded(Scene sceneData)
        {

        }
        
    }
}
