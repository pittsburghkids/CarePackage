using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SpriteData
{
    public SpriteRenderer spriteRenderer;
    public int pixelsPerUnit = 100;
}

public class SpriteLoader : MonoBehaviour
{

    public List<SpriteData> spriteDatas;

    IEnumerator Start()
    {
        foreach (SpriteData spriteData in spriteDatas)
        {
            string spriteName = spriteData.spriteRenderer.gameObject.name + ".png";
            string url = Path.Combine("file://", Application.streamingAssetsPath, spriteName);

            Debug.Log("Searching for sprite: " + gameObject.name);

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("No sprite found for: " + gameObject.name);
                }
                else
                {
                    Texture2D spriteTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    Debug.Log("Creating sprite for: " + gameObject.name);

                    Sprite sprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(.5f, .5f), spriteData.pixelsPerUnit);
                    spriteData.spriteRenderer.sprite = sprite;
                }
            }

        }
    }
}
