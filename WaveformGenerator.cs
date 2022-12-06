using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveformGenerator : MonoBehaviour
{

    [SerializeField]
    Button button;      // Press the button to generate

    [SerializeField]
    AudioSource audio;  // AudioClip works fine too

    [SerializeField]
    RenderTexture renderTexture;    // Output renderTexture

    /// <summary>
    /// Used for applying to texture
    /// </summary>
    public class Colours
    {
        static public Color32 grey =    new Color32(50, 50, 50, 255);
        static public Color32 yellow =  new Color32(255, 255, 0, 255);
        static public Color32 white =   new Color32(255, 255, 255, 255);

    }


    void Start()
    {
        // Register button to perform function when clicked
        button.onClick.AddListener(delegate { GenerateTextureFromAudio(renderTexture, audio); });
    }



    /// <summary>
    /// Generates a waveform texture based on the audio clip
    /// </summary>
    /// <returns></returns>
    public void GenerateTextureFromAudio(RenderTexture rt, AudioSource audio)
    {

        // Obtain parameters from renderTexture
        int width, height;
        width = rt.width;
        height = rt.height;

        // Prepare a output Texture2D
        Texture2D output = new Texture2D(width, height, TextureFormat.RGBA32, false);
        AudioClip clip = audio.clip;

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
        Graphics.Blit(output, rt);
    }



}
