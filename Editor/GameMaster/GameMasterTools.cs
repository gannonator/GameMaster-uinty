using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameMaster
{
   public static class GameMasterTools
    {

        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;

        }

        public static void LoadDataPackageInToEditor(LevelSceneData sceneData)
        {
            //Get the selected data
            if (sceneData)
            {
                int loadcount = sceneData.SceneList.Count;
                if (!String.IsNullOrEmpty(sceneData.MasterScene))
                {
                    loadcount++;
                }
                SceneSetup[] setup = new SceneSetup[loadcount];
                int i = 0;
                if (loadcount > sceneData.SceneList.Count)
                {
                    setup[0] = new SceneSetup();
                    setup[0].path = sceneData.MasterScene;
                    setup[0].isLoaded = true;
                    setup[0].isActive = true;

                    i = 1;
                }
                foreach (SceneData sdata in sceneData.SceneList)
                {
                    setup[i] = new SceneSetup();
                    setup[i].path = sdata.Path;
                    setup[i].isLoaded = true;
                    i++;
                }
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            }

        }
    }
}
