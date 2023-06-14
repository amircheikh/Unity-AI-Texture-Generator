using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTexture))]

public class GenTextureEditor : Editor
{
    private string prompt;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GenerateTexture gen = (GenerateTexture)target;
        prompt = EditorGUILayout.TextField("Prompt", prompt, GUILayout.Height(40));
        if(GUILayout.Button("Generate Texture"))
        {

            gen.GenerateImage(prompt);
        }

    }
}
