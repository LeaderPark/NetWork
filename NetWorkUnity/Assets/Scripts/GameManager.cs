using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private SocketModule tcp;
    [SerializeField]
    private InputField nickname;
    string myID;

    public GameObject prefavUnit;
    public GameObject mainChar;

    Dictionary<string, UnitControl> remoteUnits;
    Queue<string> commandQueue;
    void Start()
    {
       tcp = GetComponent<SocketModule>();
       remoteUnits = new Dictionary<string, UnitControl>();
       commandQueue = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessQueue();
        if(Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 targetPos;

                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mainChar.GetComponent<UnitControl>().SetTargetPos(targetPos);

                string data = "#Move#" + targetPos.x + "," + targetPos.y;
                SendCommand(data);
            }
        }
    }

    void ProcessQueue()
    {
        while(commandQueue.Count > 0)
        {
            string nextCommand = commandQueue.Dequeue();
            ProcessCommand(nextCommand);
        }
    }

    public void OnLogin()
    {
        string id = nickname.text;
        myID = id;

        if(id.Length > 0)
        {
            tcp.Login(id);
            mainChar.transform.position = Vector3.zero;
        }
    }

    public void OnLogOut()
    {
        tcp.Logout();
        foreach (var remotePair in remoteUnits)
        {
            Destroy(remotePair.Value.gameObject);
        }
        remoteUnits.Clear();
    }

    public UnitControl AddUnit(string id)
    {
        UnitControl uc = null;

        if(!remoteUnits.ContainsKey(id))
        {
            GameObject newUnit = Instantiate(prefavUnit);
            uc = newUnit.GetComponent<UnitControl>();
            remoteUnits.Add(id, uc);
        }
        return uc;
    }

    public void SetMove(string id, string coordinates)
    {
        if(remoteUnits.ContainsKey(id))
        {
            UnitControl uc = remoteUnits[id];

            var strs = coordinates.Split(',');

            Vector3 pos = new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), 0);
            uc.SetTargetPos(pos);
        }
    }

    public void UesrLeft(string id)
    {
        if(remoteUnits.ContainsKey(id))
        {
            UnitControl uc = remoteUnits[id];
            Destroy(uc.gameObject);
            remoteUnits.Remove(id);
        }
    }

    private void LoadHistory(string history)
    {
        var strs = history.Split(',');
        int max = strs.Length;
        for (int i = 0; i +2 < max; i+=3)
        {
            string id = strs[i];
            if(myID.CompareTo(id) != 0)
            {
                UnitControl uc = AddUnit(id);
                if(uc != null)
                {
                    float x = float.Parse(strs[i + 1]);
                    float y = float.Parse(strs[i + 2]);
                    uc.transform.position = new Vector3(x,y,0);
                }
            }
        }
    }

    public void SendCommand(string cmd)
    {
        SocketModule.GetInstance().SendData(cmd);
        Debug.Log("cmd sent: " + cmd);
    }

    public void QueueCommand(string cmd)
    {
        commandQueue.Enqueue(cmd);
    }

    public void ProcessCommand(string cmd)
    {
        bool bMore = true;

        while(bMore)
        {
            Debug.Log("process cmd = " + cmd);
            int idx = cmd.IndexOf("$");
            string id = "";
            if(idx > 0)
            {
                id = cmd.Substring(0, idx);
            }
            int idx2 = cmd.IndexOf("#");
            if(idx2 > idx)
            {
                int idx3 = cmd.IndexOf("#", idx2 + 1);
                if(idx3 > idx2)
                {
                    string command = cmd.Substring(idx2 + 1, idx3 - idx2 - 1);
                    string remain = cmd.Substring(idx3 + 1);
                    Debug.Log("command= " + command + " id=" + id + " remain=" + remain);

                    if (myID.CompareTo(id) != 0)
                    {
                        switch (command)
                        {
                            case "Enter":
                                AddUnit(id);
                                break;
                            case "Move":
                                SetMove(id, remain);
                                break;
                            case "Left":
                                UesrLeft(id);
                                break;
                            case "History":
                                LoadHistory(remain);
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log("ignore remote command");
                    }
                    cmd = remain;
                    if(cmd.Length <= 0)
                    {
                        bMore = false;
                    }
                }
                else{
                    bMore = false;
                }
            }
            else
            {
                bMore = false;
            }
        }
    }
}
