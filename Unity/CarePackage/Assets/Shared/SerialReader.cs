using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine.Events;

[System.Serializable] public class SerialEvent : UnityEvent<string> { }


public class SerialReader : MonoBehaviour
{
    public string port;

    SerialPort stream;
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

    public SerialEvent OnSerialEvent;

    void Start()
    {
        CancellationToken token = tokenSource.Token;

        Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                stream = new SerialPort(port, 115200);
                stream.ReadTimeout = 500;
                stream.WriteTimeout = 500;

                Debug.Log("Opening port.");
                stream.Open();


                while (stream.IsOpen)
                {
                    try
                    {
                        string message = stream.ReadLine();
                        queue.Enqueue(message.Trim());
                    }
                    catch (System.TimeoutException)
                    {
                        //Debug.Log("Timeout");
                    }
                }

                Debug.Log("Port Closed.");

            }
        }, token);
    }

    void Update()
    {
        string message;
        if (queue.TryDequeue(out message))
        {
            Debug.Log(message);
            OnSerialEvent?.Invoke(message);
        }
    }

    void OnDestroy()
    {
        tokenSource.Cancel();
        if (stream != null) stream.Close();
    }
}