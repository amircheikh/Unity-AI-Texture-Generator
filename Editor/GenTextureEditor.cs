using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTexture))]

public class GenTextureEditor : Editor
{
    private string prompt;
    private string apiKey;
    private int imgSizeIndex;
    private string[] sizeOptions = new string[]
    {
        "256x256", 
        "512x512",
        "1024x1024"
    };
    private float normalMapStrength = 1;
    private float heightMapStrength = 1;
    private float aoMapStrength = 0.5f;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GenerateTexture gen = (GenerateTexture)target;
        
        //Restore save
        apiKey = EditorPrefs.GetString("apiKey", "");
        imgSizeIndex = EditorPrefs.GetInt("imgSizeIndex", 1);
        

        apiKey = EditorGUILayout.PasswordField("OpenAI API Key", apiKey);
        prompt = EditorGUILayout.TextField("Prompt", prompt, GUILayout.Height(EditorGUIUtility.singleLineHeight * 4));

        imgSizeIndex = EditorGUILayout.Popup("Texture size" , imgSizeIndex, sizeOptions);

        normalMapStrength = EditorGUILayout.Slider("Normal map strength", normalMapStrength, 0, 1);
        heightMapStrength = EditorGUILayout.Slider("Height map strength", heightMapStrength, 0, 1);
        aoMapStrength = EditorGUILayout.Slider("Ambient occlusion map strength", aoMapStrength, 0, 0.8f);


        //Save
        EditorPrefs.SetString("apiKey", apiKey);
        EditorPrefs.SetInt("imgSizeIndex", imgSizeIndex);
        
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(prompt) || GenerateTexture.isGenerating); //Disable the button if the API key is empty or generation in progress
        if (GUILayout.Button("Generate Texture", GUILayout.Height(40)))
        {
            gen.GenerateImage(prompt, apiKey, imgSizeIndex, normalMapStrength, heightMapStrength, aoMapStrength);
        }
        EditorGUI.EndDisabledGroup();

    }
}
