using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneParser))]
public class SceneParserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneParser sceneParser = (SceneParser)target;

        if (GUILayout.Button("Display Scene"))
        {
            if (sceneParser.generateScene == SceneParser.GenerateScene.SceneEditor)
            {
                sceneParser.SetSceneData();
            }
        }
    }
}
