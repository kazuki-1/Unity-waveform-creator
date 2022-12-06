using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
public class WaveformGenerator_EditorWindow : EditorWindow
{
    [SerializeField]
    AudioClip audioClip;

    //[SerializeField]
    //string imageName = "Waveform";



    public class Colours
    {
        static public Color32 grey = new Color32(50, 50, 50, 255);
        static public Color32 yellow = new Color32(255, 255, 0, 255);
        static public Color32 white = new Color32(255, 255, 255, 255);

    }


    void Start()
    {
        // Register button to perform function when clicked
        //button.onClick.AddListener(delegate { GenerateTextureFromAudio(renderTexture, audioSource); });
    }



    /// <summary>
    /// Generates a waveform texture based on the audio clip
    /// </summary>
    /// <returns></returns>
    public void GenerateTextureFromAudio(/*RenderTexture rt, AudioSource audio*/)
    {

        // Obtain parameters from renderTexture
        int width, height;
        width = 1280;
        height = 500;

        //return;
        // Prepare a output Texture2D
        Texture2D output = new Texture2D(width, height, TextureFormat.RGBA32, false);
        AudioClip clip = audioClip;

        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        // Because not all of the audio buffer can fit in the texture
        // We don't output all the data
        // Instead, we output data with an offset
        double dataPerPixel = 2.0f / height;          // The waveform is limited to -1.0f ~ 1.0f. This will dictate the height of the samples
        int samplePerPixel = (int)(data.Length / width);
        int center_index = (int)(height / 2.0f);      // Since the data goes from -1.0f ~ 1.0f, 0.0f will be the center

        // Texture params
        int size = width * height;
        Color32[] pixels = new Color32[(width * height)];


        // Zero clear all textures
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[x + y * width] = Colours.grey;


        for (int x = 0; x < width; ++x)
        {
            // Exceed size
            if (samplePerPixel * x > data.Length - 1)
                break;
            float sample = data[x * (int)samplePerPixel];

            // Vertical height
            int sample_height = (int)(Mathf.Abs(sample / (float)dataPerPixel));
            int cur_ind = center_index * width + x;

            for (int y = 0; y < sample_height; ++y)
            {
                int below, above;
                below = cur_ind - y * width;
                above = cur_ind + y * width;

                if (below >= 0 && below < size)
                    pixels[below] = Colours.yellow;

                if (above >= 0 && above < size)
                    pixels[above] = Colours.yellow;

            }


        }
        output.SetPixels32(pixels);
        output.Apply();





        string filePath = EditorUtility.SaveFilePanel("Save to ", Application.dataPath, "Waveform", "png");
        if (filePath != null)
        {
            UnityEngine.Windows.File.WriteAllBytes(filePath, output.EncodeToPNG());
        }
    }



    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/WaveformGenerator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        WaveformGenerator window = (WaveformGenerator)EditorWindow.GetWindow(typeof(WaveformGenerator));
        window.Show();
    }

    void OnGUI()
    {
        //imageName = EditorGUILayout.TextField("Image Name ", imageName);
        audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio File", audioClip, typeof(AudioClip), false);
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Waveform PNG"))
            GenerateTextureFromAudio();
    }
}