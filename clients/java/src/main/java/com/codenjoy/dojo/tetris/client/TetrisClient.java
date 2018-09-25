package com.codenjoy.dojo.tetris.client;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;

/**
 * User: your name
 */
public class TetrisClient extends WebSocketListener {

    // this is your email
    private static final String USER_NAME = "put_your_bot_email_here";
    // you can get this code after registration on the server with your email
    private static final String CODE = "put_your_player_code_here";

    private static final String CONNECTION_URL = "ws://codenjoy.astanajug.net:8080/contest/ws?user=%s&code=%s";

    public static void main(String[] args) {
        new TetrisClient().execute();
    }

    private void execute() {
        OkHttpClient client = new OkHttpClient.Builder().build();

        Request request = new Request.Builder().url(String.format(CONNECTION_URL, USER_NAME, CODE)).build();

        client.newWebSocket(request, this);

        client.dispatcher().executorService().shutdown();
    }

    @Override
    public void onMessage(WebSocket webSocket, String text) {

        String board = text.substring("board=".length());
        System.out.println(board);

        webSocket.send(calculateNextCommand(board));
    }

    private String calculateNextCommand(String board) {
        return "ACT";
    }
}
