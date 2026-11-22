#if UNITY_EDITOR

using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneTemplatePipeline : ISceneTemplatePipeline
{
    public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
    {
        return true;
    }

    public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
    {
        
    }

    public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
    {
        //Check if the user whishes to save scene
        if (!EditorUtility.DisplayDialog("Save", "Save the scene", "Yes", "No"))
            return;

        //Show the Save File panel
        string path = EditorUtility.SaveFilePanel("Save Scene As", "Assets/Game/Scenes/SeparateScenes", scene.name, "unity");

        //Check if a path was specified 
        if (string.IsNullOrEmpty(path))
            return;

        //Make the path relative to the project folder and save it
        path = FileUtil.GetProjectRelativePath(path);
        EditorSceneManager.SaveScene(scene, path);

        AssetDatabase.Refresh();

        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();


    }
}

#endif