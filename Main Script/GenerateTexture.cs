using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI_API;
using OpenAI_API.Models;
using OpenAI_API.Images;
using System;
using System.IO;
using System.Net;
using Random = UnityEngine.Random;
using UnityEditor;

public class GenerateTexture : MonoBehaviour
{
    private OpenAIAPI api;
    public static bool isGenerating = false;
    ImageSize imageSize = ImageSize._512;

    private Texture2D LoadTextureFromFile(string filePath)
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }

        // Read the image file as bytes
        byte[] fileData = File.ReadAllBytes(filePath);

        // Create a new Texture2D
        Texture2D texture = new Texture2D(2, 2);

        // Load the image data into the texture
        if (texture.LoadImage(fileData))
        {
            // Texture loaded successfully
            //Debug.Log("Texture loaded successfully: " + filePath);
            return texture;
        }
        else
        {
            // Failed to load texture
            Debug.LogError("Failed to load texture: " + filePath);
            return null;
        }
    }
    public void GenerateNormalMap(Texture2D inputTexture, string filePath, float strength)  //Sobel filter technique
    {
        if (strength == 0) return;

        // Calculate the pixel step size in UV space
        float stepSizeU = 1.0f / inputTexture.width;
        float stepSizeV = 1.0f / inputTexture.height;

        // Create a new texture to store the normal map
        Texture2D normalMap = new Texture2D(inputTexture.width, inputTexture.height);

        // Iterate over each pixel in the input texture
        for (int y = 0; y < inputTexture.height; y++)
        {
            for (int x = 0; x < inputTexture.width; x++)
            {
                // Sample the surrounding pixels for calculating the gradients
                Color color00 = inputTexture.GetPixel(x - 1, y - 1);
                Color color10 = inputTexture.GetPixel(x, y - 1);
                Color color20 = inputTexture.GetPixel(x + 1, y - 1);
                Color color01 = inputTexture.GetPixel(x - 1, y);
                Color color21 = inputTexture.GetPixel(x + 1, y);
                Color color02 = inputTexture.GetPixel(x - 1, y + 1);
                Color color12 = inputTexture.GetPixel(x, y + 1);
                Color color22 = inputTexture.GetPixel(x + 1, y + 1);

                // Calculate the gradients in the X and Y directions
                float dx = (color20.r + 2 * color21.r + color22.r) - (color00.r + 2 * color01.r + color02.r);
                float dy = (color02.r + 2 * color12.r + color22.r) - (color00.r + 2 * color10.r + color20.r);

                // Calculate the normal from the gradients
                Vector3 normal = new Vector3(dx * strength, dy * strength, 1.0f).normalized * 0.5f + new Vector3(0.5f, 0.5f, 0.5f);

                // Convert the normal to a color and store it in the normal map
                normalMap.SetPixel(x, y, new Color(normal.x, normal.y, normal.z, 1.0f));
            }
        }

        // Apply the changes to the normal map
        normalMap.Apply();

        // Convert the normal map to a byte array
        byte[] bytes = normalMap.EncodeToPNG();

        // Get the original file name without extension
        string originalFileName = Path.GetFileNameWithoutExtension(filePath);

        // Create the new file name for the normal map
        string normalMapFileName = originalFileName + "_NormalMap.png";

        // Combine the new file name with the original file directory
        string normalMapFilePath = Path.Combine(Path.GetDirectoryName(filePath), normalMapFileName);

        // Save the normal map to the new file location
        File.WriteAllBytes(normalMapFilePath, bytes);

        // Import the saved normal map as an asset
        AssetDatabase.ImportAsset(normalMapFilePath, ImportAssetOptions.ForceSynchronousImport);

        // Get the imported asset
        TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(normalMapFilePath);

        // Set the texture type to normal map
        textureImporter.textureType = TextureImporterType.NormalMap;

        // Apply the changes to the texture importer
        EditorUtility.SetDirty(textureImporter);
    }
    public static void GenerateHeightMap(Texture2D inputTexture, string filePath, float strength)   //Grayscale conversion technique
    {
        if (strength == 0) return;

        int width = inputTexture.width;
        int height = inputTexture.height;

        Texture2D heightmap = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = inputTexture.GetPixel(x, y);
                float heightValue = pixelColor.grayscale * strength;
                Color heightColor = new Color(heightValue, heightValue, heightValue);
                heightmap.SetPixel(x, y, heightColor);
            }
        }

        byte[] bytes = heightmap.EncodeToPNG();

        string originalFileName = Path.GetFileNameWithoutExtension(filePath);
        string heightMapFileName = originalFileName + "_HeightMap.png";
        string heightMapFilePath = Path.Combine(Path.GetDirectoryName(filePath), heightMapFileName);

        File.WriteAllBytes(heightMapFilePath, bytes);
    }


    public void GenerateAOMap(Texture2D inputTexture, string filePath, float sampleRadius, float bias, int sampleCount) //SSAO technique
    {
        if (bias == 0) return; 

        // Create a new texture to store the AO map
        Texture2D aoMap = new Texture2D(inputTexture.width, inputTexture.height, TextureFormat.RGB24, false);

        // Iterate over each pixel in the input texture
        for (int y = 0; y < inputTexture.height; y++)
        {
            for (int x = 0; x < inputTexture.width; x++)
            {
                // Calculate the AO value for the current pixel
                float ao = CalculateAO(x, y, inputTexture, sampleRadius, bias, sampleCount);

                // Set the AO value as the intensity in the AO map
                aoMap.SetPixel(x, y, new Color(ao, ao, ao));
            }
        }

        // Apply the changes to the AO map
        aoMap.Apply();

        // Convert the AO map to a byte array
        byte[] bytes = aoMap.EncodeToPNG();

        // Get the original file name without extension
        string originalFileName = Path.GetFileNameWithoutExtension(filePath);

        // Create the new file name for the AO map
        string aoMapFileName = originalFileName + "_AOMap.png";

        // Combine the new file name with the original file directory
        string aoMapFilePath = Path.Combine(Path.GetDirectoryName(filePath), aoMapFileName);

        // Save the AO map to the new file location
        File.WriteAllBytes(aoMapFilePath, bytes);
    }

    // Calculate the AO value for a given pixel
    private float CalculateAO(int x, int y, Texture2D inputTexture, float sampleRadius, float bias, int sampleCount)
    {
        // Initialize the occlusion factor
        float occlusion = 0.0f;

        // Iterate over the specified number of samples
        for (int i = 0; i < sampleCount; i++)
        {
            // Calculate a random sample point within the sample radius
            Vector2 samplePoint = UnityEngine.Random.insideUnitCircle * sampleRadius;

            // Offset the sample point from the current pixel
            int sampleX = Mathf.Clamp(x + Mathf.RoundToInt(samplePoint.x), 0, inputTexture.width - 1);
            int sampleY = Mathf.Clamp(y + Mathf.RoundToInt(samplePoint.y), 0, inputTexture.height - 1);

            // Get the color of the sample point
            Color sampleColor = inputTexture.GetPixel(sampleX, sampleY);

            // Calculate the occlusion factor based on the grayscale value of the sample point color
            float sampleOcclusion = sampleColor.grayscale;

            // Accumulate the occlusion factor
            occlusion += sampleOcclusion;
        }

        // Average the occlusion factor
        occlusion /= sampleCount;

        // Apply bias to the occlusion factor
        occlusion = Mathf.Clamp01(occlusion + bias);

        // Return the final AO value
        return occlusion;
    }

    public async void GenerateImage(string prompt, string apiKey, int imgSizeIndex, float normalMapStrength, float heightMapStrength, float aoMapStrength)
    {
        //Saftey precautions
        while (prompt.EndsWith(" ")) prompt = prompt.Substring(0, prompt.Length - 1);   //Removes spaces from end of prompt
        if (prompt.Length < 1) return;  //Returns when prompt doesn't exist
        string textureName = prompt;    //The files and folders will have this name
        if (textureName.Length > 55) textureName = textureName.Substring(0, 55);    //Shortens the filenames so they don't end up too long. IDK what the limit should be. 55 sounds good though

        Debug.Log("Generating texture.....");
        api = new OpenAIAPI(apiKey);
        try
        {
            isGenerating = true;

            //Image size initialized
            if (imgSizeIndex == 0) imageSize = ImageSize._256;
            else if (imgSizeIndex == 1) imageSize = ImageSize._512;
            else if (imgSizeIndex == 2) imageSize = ImageSize._1024;

            //Generation request
            var result = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest("The following is a texture for use in a video game. " + prompt, 1, imageSize));
            string imageUrl = result.Data[0].Url;

            string prefabPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

            string fileName = textureName + ".png";

            //Creates textures folder
            string textureFolderPath = Path.Combine(prefabPath, "Textures");
            if (!Directory.Exists(textureFolderPath)) AssetDatabase.CreateFolder(prefabPath, "Textures");

            //Creates Generated texture folder
            string generatedTextureFolderPath = Path.Combine(textureFolderPath, textureName);
            if (!Directory.Exists(generatedTextureFolderPath)) AssetDatabase.CreateFolder(textureFolderPath, textureName);

            string filePath = Path.Combine(generatedTextureFolderPath, fileName);   //Path to final file

            //Downloads file from url
            using WebClient webClient = new WebClient();
            webClient.DownloadFile(imageUrl, filePath);

            
            Texture2D baseTexture = LoadTextureFromFile(filePath);  //Base texture saved as Texture2D variable

            //Generating texture details. Details won't generate if strength is set to 0
            GenerateNormalMap(baseTexture, filePath, normalMapStrength);
            GenerateHeightMap(baseTexture, filePath, heightMapStrength);
            GenerateAOMap(baseTexture, filePath, 1, aoMapStrength, 64);

            Debug.Log("Texture generated successfully!");
            isGenerating = false;
            AssetDatabase.Refresh();    //Refreshes assets

        }
        catch (Exception e)
        {
            isGenerating = false;
            Debug.Log("Error occurred in generating texture: " + e);
        }
    }



   


}
