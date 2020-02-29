using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;

using Debug = UnityEngine.Debug;

public class WebSocketBridge
{
    public delegate void ReceiveAction(string message);
    public event ReceiveAction OnReceived;

    private ClientWebSocket webSocket;

    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    public WebSocketBridge(string webSocketURL)
    {
        if (!String.IsNullOrEmpty(webSocketURL))
        {
            Connect(webSocketURL);
        }
    }

    public void Close()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }

    public async void Connect(string url)
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

        while (!cancellationToken.IsCancellationRequested)
        {
            using (webSocket = new ClientWebSocket())
            {
                try
                {
                    Debug.Log("<color=cyan>WebSocket connecting.</color>");
                    await webSocket.ConnectAsync(new Uri(url), cancellationToken);

                    Debug.Log("<color=cyan>WebSocket receiving.</color>");
                    await Receive();

                    Debug.Log("<color=cyan>WebSocket closed.</color>");

                }
                catch (OperationCanceledException)
                {
                    Debug.Log("<color=cyan>WebSocket shutting down.</color>");
                }
                catch (WebSocketException)
                {
                    Debug.Log("<color=cyan>WebSocket connection lost.</color>");
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    throw;
                }
            }

            Debug.Log("<color=cyan>Websocket reconnecting.</color>");
            await Task.Delay(1000);
        }
    }

    public void Send(string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);

            webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    private async Task Receive()
    {
        var arraySegment = new ArraySegment<byte>(new byte[8192]);
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(arraySegment, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(arraySegment.Array, 0, result.Count);
                if (OnReceived != null) OnReceived(message);
            }
        }
    }

    private IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];

        for (int i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }

        return new IPAddress(broadcastAddress);
    }

}