using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WebReader : MonoBehaviour
{
    public GameObject testGameObject;

    void Start()
    {
        StartCoroutine(GetTexture());
    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://localhost:3333/view");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Debug.Log(myTexture);

            testGameObject.GetComponent<MeshRenderer>().material.mainTexture = myTexture;
        }
    }
}