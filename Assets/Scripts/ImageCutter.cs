using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ImageCutter : MonoBehaviour
{
    [System.Serializable]
    public class ImageRect
    {
        public string imageName;
        public Vector2 start; // górny-lewy (px)
        public Vector2 end;   // dolny-prawy (px)
    }

    public List<ImageRect> images = new();

    public Texture2D sourceImage;

    public void ExtractImages(string outputPath)
    {
        if (sourceImage == null)
        {
            Debug.LogError("Brak sourceImage – najpierw LoadSourceImage()");
            return;
        }

        if (string.IsNullOrEmpty(outputPath))
        {
            Debug.LogError("Niepoprawna ścieżka wyjściowa");
            return;
        }

        if (!Directory.Exists(SaveLoadSystem.skinsSavePath + outputPath))
        {
            Directory.CreateDirectory(SaveLoadSystem.skinsSavePath + outputPath);
        }

        foreach (var img in images)
        {
            int x1 = Mathf.RoundToInt(img.start.x);
            int y1 = Mathf.RoundToInt(img.start.y);
            int x2 = Mathf.RoundToInt(img.end.x);
            int y2 = Mathf.RoundToInt(img.end.y);

            int width = x2 - x1;
            int height = y2 - y1;

            if (width <= 0 || height <= 0)
            {
                Debug.LogWarning($"Niepoprawny rozmiar: {img.imageName}");
                continue;
            }

            // Unity Y = od dołu
            int y = sourceImage.height - y2;

            if (x1 < 0 || y < 0 ||
                x1 + width > sourceImage.width ||
                y + height > sourceImage.height)
            {
                Debug.LogWarning($"Poza zakresem: {img.imageName}");
                continue;
            }

            Color[] pixels = sourceImage.GetPixels(x1, y, width, height);

            Texture2D newTex = new(width, height, TextureFormat.RGBA32, false);
            newTex.SetPixels(pixels);
            newTex.Apply();

            byte[] png = newTex.EncodeToPNG();
            string filePath = Path.Combine(SaveLoadSystem.skinsSavePath + outputPath, img.imageName + ".png");

            File.WriteAllBytes(filePath, png);

            Debug.Log($"Zapisano: {filePath}");
        }
    }
}
