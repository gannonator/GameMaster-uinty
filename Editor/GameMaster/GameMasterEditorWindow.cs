using UnityEngine;
using UnityEditor;

public class GameMasterEditorWindow : EditorWindow
{
    [MenuItem("GameMaster/MainWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GameMasterEditorWindow));
    }
    #region GUI
    void OnGUI()
    {
    }
    #endregion
}