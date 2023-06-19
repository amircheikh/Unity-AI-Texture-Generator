# Unity-AI-Texture-Generator
This project uses OpenAI's DALL·E 2 to generate textures directly from the Unity Editor.
The API calls are made directly in game by using OkGoDoIt/OpenAI-API-dotnet, a C# OpenAI sdk.
## Showcase video
Coming soon!
## Asset store link
Pending approval!
## Installation
1. Clone this repo or download it as a zip
2. Place the files in a folder inside your Assets folder
## Usage
1. Click on the "Texture Generator" prefab. Do not put it in your scene.
2. Enter your OpenAI API key. You can find it at: https://platform.openai.com/account/api-keys
3. Enter a prompt for the texture you want to generate.\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;a. Example: "Oak tree bark, realistic"\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;b. Note: All prompts have the string "The following is a texture for use in a video game." appended automatically to the beginning of your prompt.
4. Change the strength for each texture map to increase their intensity.
5. Click generate and wait for your result.
6. The textures will be outputted to a folder in the same directory as the prefab.


I am not affiliated with OpenAI and this project is not endorsed or supported by them.\
Enjoy!
