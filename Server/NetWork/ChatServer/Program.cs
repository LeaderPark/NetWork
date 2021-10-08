using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace ConsoleApplication1
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Loopback, 8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            byte[] bytesFrom = new byte[1024];
            string dataFromClient = "";

            serverSocket.Start();
            Console.WriteLine("Chat Server Started ....");
            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();

                dataFromClient = "";
                NetworkStream networkStream = clientSocket.GetStream();
                int numBytesRead;

                // 접속된 클라이언트의 닉네임 가져오기
                while (networkStream.DataAvailable)
                {
                    numBytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    dataFromClient += Encoding.ASCII.GetString(bytesFrom, 0, numBytesRead);
                }
                int idx = dataFromClient.IndexOf("$");
                if (idx >= 0)
                {
                    dataFromClient = dataFromClient.Substring(0, idx);
                }

                // dataFromClient is nick name of the user.
                clientsList.Add(dataFromClient, clientSocket);

                broadcast(dataFromClient + " Joined ", dataFromClient, false);

                Console.WriteLine(dataFromClient + " Joined chat room ");
                handleClinet client = new handleClinet();
                client.startClient(clientSocket, dataFromClient, clientsList);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }  //end broadcast function

        public static void UserLeft(string clientNo)
        {
            if (clientsList.ContainsKey(clientNo))
            {
                Console.WriteLine("client Left:" + clientNo);
                TcpClient clientSocket = (TcpClient)clientsList[clientNo];
                clientsList.Remove(clientNo);
                clientSocket.Close();
            }
        }
    }//end Main class


    public class handleClinet
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;
        bool noConnection = false;

        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = cList;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[1024];
            string dataFromClient = "";
            //Byte[] sendBytes = null;
            //string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while (!noConnection)
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();

                    // networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    int numBytesRead;
                    if (!SocketConnected(clientSocket.Client))
                    {
                        // socket closed
                        noConnection = true;
                    }
                    else
                    {
                        if (networkStream.DataAvailable)
                        {
                            dataFromClient = "";
                            while (networkStream.DataAvailable)
                            {
                                numBytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                                dataFromClient += Encoding.ASCII.GetString(bytesFrom, 0, numBytesRead);
                            }

                            // dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                            int idx = dataFromClient.IndexOf("$");
                            if (idx >= 0)
                            {
                                dataFromClient = dataFromClient.Substring(0, idx);
                            }
                            Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                            rCount = Convert.ToString(requestCount);

                            Program.broadcast(dataFromClient, clNo, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // connection broken - delete current user.
                    noConnection = true;
                    Console.WriteLine(ex.ToString());
                }
            }//end while

            Program.UserLeft(clNo);
            Program.broadcast("User left:" + clNo, clNo, false);

        }//end doChat
    } //end class handleClinet
}//end namespace