using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitControl : MonoBehaviour
{
    const float speed = 3.0f;
    const int MAX_HP = 100;
    const int DROP_HP = 4;

    public bool bMovable = false;
    public ParticleSystem fxParticle;
    public GameObject hpBar;
    GameManager gm;
    Vector3 targetPos;
    Vector3 orgPos;
    float timeToDest;
    float elapsed;
    float elapsedDrop;
    public int currentHP;
    int maxHP;
    bool bMoving;


    // Start is called before the first frame update
    void Start()
    {
        // sm = GameObject.Find("GameManager").GetComponent<SocketModule>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        orgPos = transform.position;
        targetPos = orgPos;
        timeToDest = 0;
        bMoving = false;

        maxHP = MAX_HP;
        SetHP(MAX_HP);
    }

    // Update is called once per frame
    void Update()
    {
        if (bMoving)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= timeToDest)
            {
                elapsed = timeToDest;
                transform.position = targetPos;
                bMoving = false;
            }
            else
            {
                Vector3 newPos = Vector3.Lerp(orgPos, targetPos, elapsed / timeToDest);
                transform.position = newPos;
            }
        }

        elapsedDrop += Time.deltaTime;
        if(elapsedDrop >= 1.0f)
        {
            elapsedDrop -= 1.0f;
            DropHP(DROP_HP);
        }
    }

    public void SetTargetPos(Vector3 pos)
    {
        orgPos = transform.position;
        targetPos = pos;
        targetPos.z = orgPos.z;
        timeToDest = Vector3.Distance(orgPos, targetPos) / speed;
        elapsed = 0;
        bMoving = true;
    }
    public void SetColor(Color col)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.color = col;
        }
    }

    public void SetHP(int hp)
    {
        hp = Mathf.Clamp(hp, 0, maxHP);
        currentHP = hp;
        float value = (float)currentHP / (float)MAX_HP;
        hpBar.transform.localScale = new Vector3(value,1,1);
    }

    public void DropHP(int hp)
    {
        currentHP -= hp;
        SetHP(currentHP);
    }

    public void StartFX()
    {
        if(currentHP > 0)
        {
            fxParticle.Play();
        }
    }

    public void Revive()
    {
        SetHP(MAX_HP);
    }

}