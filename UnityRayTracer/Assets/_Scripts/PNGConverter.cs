using System.Collections;
using UnityEngine;
using System.IO;

public class PNGConverter : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return TakeScreenShot();
    }

    IEnumerator TakeScreenShot() 
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        File.WriteAllBytes(Application.dataPath + "/Resources/ScreenShot.png", bytes);
    }
}
