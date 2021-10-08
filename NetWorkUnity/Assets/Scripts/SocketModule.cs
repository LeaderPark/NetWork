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
    // private Thread clientReceiveThread;
    private NetworkStream serverStream = default(NetworkStream);
    /*
    TcpClient clientSocket = new TcpClient();
    */
    private Queue<string> msgQueue;
    private string nickname;
    //string readData = null;
    bool bRunning = false;

    public static SocketModule GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        msgQueue = new Queue<string>();
        gm = GetComponent<GameManager>();
    }

    public void Login(string id)
    {
        if (!bRunning)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect("localhost", 8888);
            serverStream = clientSocket.GetStream();
            // register login name as string id
            byte[] outStream = Encoding.ASCII.GetBytes(id + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
            bRunning = true;
            nickname = id;
        }
    }

    public void SendData(string str)
    {
        if (bRunning && serverStream != null)
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
        // stop thread
        if (bRunning)
        {
            StopThread();
            msgQueue.Clear();
            nickname = "";
        }

        if (serverStream != null)
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
        if (msgQueue.Count > 0)
        {
            string nextMsg = msgQueue.Dequeue();
            return nextMsg;
        }
        return null;
    }

    // 소켓으로부터데이터를받는부분- 스레드처리
    private void getMessage()
    {
        byte[] inStream = new byte[1024];
        string returndata = "";
        try
        {
            while (bRunning)
            {
                serverStream = clientSocket.GetStream();

                int buffSize = 0;
                buffSize = clientSocket.ReceiveBufferSize;
                int numBytesRead;

                if (serverStream.DataAvailable)
                {
                    returndata = "";
                    while (serverStream.DataAvailable)
                    {
                        numBytesRead = serverStream.Read(inStream, 0, inStream.Length);
                        returndata += Encoding.ASCII.GetString(inStream, 0, numBytesRead);
                    }
                    //readData = "" + returndata;
                    //msgQueue.Enqueue(returndata);
                    // command detected
                    gm.QueueCommand(returndata);
                    Debug.Log(returndata);
                }
            }
        }
        catch (Exception ex)
        {
            StopThread();
        }
    }
}