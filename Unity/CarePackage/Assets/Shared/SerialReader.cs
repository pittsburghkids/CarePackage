using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class SerialReader
{
    public delegate void SerialEvent(string serialMessage);
    public event SerialEvent OnSerialMessage;

    SerialPort stream;

    CancellationTokenSource tokenSource = new CancellationTokenSource();
    CancellationToken token;

    ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

    public SerialReader(string port)
    {
        token = tokenSource.Token;

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

        ProcessQueue();
    }

    async void ProcessQueue()
    {
        while (!token.IsCancellationRequested)
        {
            string message;
            if (queue.TryDequeue(out message))
            {
                //Debug.Log(message);
                OnSerialMessage?.Invoke(message);
            }
            await Task.Delay(System.TimeSpan.FromSeconds(1 / 30f));
        }
    }

    public void Destroy()
    {
        tokenSource.Cancel();
        if (stream != null) stream.Close();
    }
}