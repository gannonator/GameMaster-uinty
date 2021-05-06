using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace GameMaster
{
    [ExecuteInEditMode]
    public class GameMasterEditor : MonoBehaviour
    {
        private static SceneSetup[] _sceneSetups;


        [MenuItem("Assets/GameMaster/Load Scenes")]
        public static void LoadDataPackageInToEditor()
        {
            //Get the selected data
            ScriptableObject go = (ScriptableObject)Selection.activeObject;
            LevelSceneData data = (LevelSceneData)go;
            GameMasterTools.LoadDataPackageInToEditor(data);
        }


        [MenuItem("GameMaster/ToggleGame")]
        static void ToggelGame()
        {
            if (EditorApplication.isPlaying == true)
            {
                EditorApplication.isPlaying = false;
                EditorSceneManager.RestoreSceneManagerSetup(_sceneSetups);
                return;
            }
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            _sceneSetups = EditorSceneManager.GetSceneManagerSetup();

            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath < SceneAsset >(GameMasterSettings.GAMEMASTER_SCENE);
    
        EditorApplication.isPlaying = true;
        }


        [MenuItem("GameMaster/Load Master")]
        static void LoadMaster()
        {
            SceneSetup[] sceneSetup = new SceneSetup[1];
            sceneSetup[0] = new SceneSetup();
            sceneSetup[0].path = GameMasterSettings.GAMEMASTER_SCENE;
            sceneSetup[0].isLoaded = true;
            sceneSetup[0].isActive = true;
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
        }

        [MenuItem("GameMaster/Add Master")]
        static void AddMaster()
        {
            SceneSetup[] sceneSetupRaw = EditorSceneManager.GetSceneManagerSetup();
            //Array.Resize(ref sceneSetup, sceneSetup.Length + 1);
            SceneSetup[] sceneSetup = new SceneSetup[sceneSetupRaw.Length+1];
            for(int i =0; i < sceneSetupRaw.Length;  i++ )
            {
                sceneSetup[i + 1] = sceneSetupRaw[i];
            }
            sceneSetup[0] = new SceneSetup();
            sceneSetup[0].path = GameMasterSettings.GAMEMASTER_SCENE;
            sceneSetup[0].isLoaded = true;
            sceneSetup[0].isActive = false;
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
        }


        [MenuItem("GameMaster/Set Start Scene")]
        public static void StartScene()
        {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GameMasterSettings.GAMEMASTER_SCENE);
        }

        [MenuItem("Assets/GameMaster", true)]
        public static bool SceneSetupRootValidate()
        {
            return true;
        }

        [MenuItem("Assets/GameMaster/Load Scenes",true)]
        public static bool LoadDataPackageInToEditorValidation()
        {

            return Selection.activeObject is ScriptableObject;
        }




    //[MenuItem("GameMaster/Compile Data")]
    private static void CompileData()
        {

            string copyPath = GameMasterSettings.GAMEMASTER_DIR + "GameMasterData.cs";
            Debug.Log("Creating Classfile: " + copyPath);
            if (File.Exists(copyPath) == false)
            { // do not overwrite
                using (StreamWriter outfile =
                    new StreamWriter(copyPath))
                {
                    outfile.WriteLine("using UnityEngine;");
                    outfile.WriteLine("using System.Collections;");
                    outfile.WriteLine("");
                    outfile.WriteLine("public  Static class  GameMasterData  {");
                    outfile.WriteLine(" ");
                    outfile.WriteLine(" ");
                    outfile.Write("enum Levels { ");
                    int i = 0;
                    foreach (LevelSceneData data in  GameManager.Instance.Levels)
                    {
                        if(string.IsNullOrEmpty(data.Name))
                        {
                            outfile.Write("LEVEL"+i.ToString());
                        }
                        else
                        {
                            outfile.Write(data.Name.ToUpper().Replace(' ', '-'));
                        }
                        i++;
                        if(i != GameManager.Instance.Levels.Count)
                        {
                            outfile.Write(", ");
                        }
                        
                    }
                    outfile.WriteLine("}");


                    outfile.WriteLine(" ");
                    outfile.WriteLine(" ");                 
                    outfile.WriteLine("}");
                }//File written
            }
            AssetDatabase.Refresh();
        }
    }

}
