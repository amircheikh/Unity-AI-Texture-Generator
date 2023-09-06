# Unity-AI-Texture-Generator
This project uses OpenAI's DALL·E 2 to generate textures directly from the Unity Editor.
The API calls are made directly in game by using OkGoDoIt/OpenAI-API-dotnet, a C# OpenAI sdk. Now available on the Unity Asset Store!
## Showcase video
A video I made showcasing this project: https://www.youtube.com/watch?v=j_V2LVeXV4k
## Asset store link
Download for free from the official Unity Asset Store: https://assetstore.unity.com/packages/tools/generative-ai/amir-s-ai-texture-generator-258109
## Installation
1. Install the latest version (or version 3.2.1) of _com.unity.nuget.newtonsoft-json_ through the Unity Package Manager. Instructions: https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM
2. Clone this repo or download it as a zip
3. Place the files in a folder inside your **Assets** folder
## Usage
1. Click on the "**Texture Generator**" prefab. Do not put it in your scene.
2. Enter your OpenAI API key. You can find it at: https://platform.openai.com/account/api-keys
3. Enter a prompt for the texture you want to generate.\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;a. Example: "_Oak tree bark, realistic_"\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;b. Note: All prompts have the string "_The following is a texture for use in a video game._" appended automatically to the beginning of your prompt.
4. Change the strength for each texture map to increase their intensity.
5. Click generate and wait for your result.
6. The textures will be outputted to a folder in the same directory as the prefab.


I am not affiliated with OpenAI or Unity and this project is not endorsed or supported by them.\
Enjoy!
