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
        //ProcessQueue();
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 targetPos;

            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mainChar.GetComponent<UnitControl>().SetTa
        }
    }
}
