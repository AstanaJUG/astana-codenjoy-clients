const host = 'codenjoy.astanajug.net:8080/contest';
const userName = 'put_your_bot_email_here'; //replace your user name here
const code = 'put_your_player_code_here'; // replace of your user name code

const server = 'ws://' + host + '/ws';
const WebSocket = require('ws');
const ws = new WebSocket(server + '?user=' + userName + '&code=' + code);

function commandForServer(board) {
    return 'LEFT';
}

ws.on('open', function() {
    console.log('Opened');
});

ws.on('close', function() {
    console.log('Closed');
});

ws.on('message', function(message) {
    const board = message.substring("board=".length, message.length);
    printBoard(board);
    let command = commandForServer();
    ws.send(command);
});

console.log('Web socket client running at ' + server);

function printBoard(message) {
    console.log(message)
}