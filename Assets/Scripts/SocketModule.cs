using Microsoft.VisualBasic.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

public class SocketModule : MonoBehaviour
{
    static SocketModule instance = null;

    private TcpClient clientSocket;
    private GameManager gm;

    private NetworkStream serverStream = default(NetworkStream);

    private Queue<string> msgQueue;
    private string nickname;
    bool bRunning = false;

    public static SocketModule GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    void Start()
    {
        msgQueue = new Queue<string>();
        gm = GetComponent<GameManager>();
    }

    private void Update() {
        
    }

    public void Login(string id)
    {
        clientSocket = new TcpClient();
        clientSocket.Connect("localhost", 8888);
        serverStream = clientSocket.GetStream();

        byte[] outStream = Encoding.ASCII.GetBytes(id + "$");
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();

        Thread ctThread = new Thread(getMessage);
        ctThread.Start();
        bRunning = true;
        nickname = id;
    }

    public void SendData(string str)
    {
        if(bRunning && serverStream != null)
        {
            byte[] outStream = Encoding.ASCII.GetBytes("$" + str);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }
    }

    private void StopThread()
    {
        bRunning = false;
    }
    
    public void Logout()
    {
        if(bRunning)
        {
            StopThread();
            msgQueue.Clear();
            nickname = "";
        }

        if(serverStream != null)
        {
            serverStream.Close();
            serverStream = null;
        }
        clientSocket.Close();
    }

    public bool IsOnline()
    {
        return bRunning;
    }

    public string GetNextData()
    {
        if(msgQueue.Count > 0)
        {
            string nextMsg = msgQueue.Dequeue();
            return nextMsg;
        }
        return null;
    }
}

