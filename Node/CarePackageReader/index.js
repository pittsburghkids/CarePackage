const express = require('express');
const app = express();
const server = require('http').createServer(app);

app.use(express.static('public'));

// Websocket setup.

const WebSocket = require('ws');
const wss = new WebSocket.Server({ server });

// Handle new connections.

wss.on('connection', function connection(ws) {

    console.log("Client connected.");

    ws.isAlive = true;

    // Handle client keep-alive.
    ws.on('pong', () => {
        ws.isAlive = true;
    });

    // Handle client messages.
    ws.on('message', function incoming(data) {
        wss.clients.forEach(function each(client) {
            if (client !== ws && client.readyState === WebSocket.OPEN) {
                client.send(data);
            }
        });
    });

});

// Cull idle clients.

const interval = setInterval(function ping() {
    wss.clients.forEach(function each(ws) {
        if (ws.isAlive === false) {
            console.log("Culling client.");
            return ws.terminate();
        }
        ws.isAlive = false;
        ws.ping(() => { });
    });
}, 30000);

// Listen for connections.

const port = process.env.PORT || 8080;
server.listen(port, () => console.log(`Listening on port ${port}!`));

//

wss.broadcast = function broadcast(data) {
    wss.clients.forEach(function each(client) {
        if (client.readyState === WebSocket.OPEN) {
            client.send(data);
        }
    });
};

//

const path = "/dev/cu.usbmodem14601";

const SerialPort = require('serialport')
const Readline = require('@serialport/parser-readline')
const serialPort = new SerialPort(path, { baudRate: 115200 })

const parser = new Readline()
serialPort.pipe(parser)

parser.on('data', line => {
    let value = Number.parseInt(line);
    if (Number.isInteger(value)) {
        wss.broadcast(value);
        console.log(value);
    }
})
