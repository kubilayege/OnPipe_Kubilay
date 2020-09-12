using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public Vector3 defaultScale;
    public float minForce;
    public float maxForce;
    public static bool holding;
    public bool gameStarted;


    void Start()
    {
        defaultScale = transform.localScale;
        gameStarted = false;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            if(GameManager.instance.currentGameState == GameManager.GameState.Idle)
            {
                GameManager.instance.ChangeGameState(GameManager.GameState.Playing);
            }
            AdjustScaleToPipe();
            holding = true;
        }
        else
        {
            ResetScale();
            holding = false;
        }

    }

    private void ResetScale()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, Time.deltaTime*16);
    }

    private void AdjustScaleToPipe()
    {
        RaycastHit hit;
        Vector3 from = new Vector3(transform.position.x, transform.position.y, transform.position.x - 5);
        Vector3 dir = (transform.position - from).normalized;
        if(Physics.Raycast(from, dir, out hit, 9))
        {
            if(hit.transform.localScale.x >= transform.localScale.x)
            {
                
                Explode();
                return;
            }

            transform.localScale = Vector3.Lerp(transform.localScale, hit.collider.transform.localScale + new Vector3(0.2f,
                                                                                   -hit.collider.transform.localScale.y + transform.localScale.y,
                                                                                   -hit.collider.transform.localScale.z + hit.collider.transform.localScale.y + 0.2f), Time.deltaTime * 16) ;
        }
    }

    private void Explode()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.localScale = defaultScale;
        GameObject brokenPieces = transform.GetChild(0).gameObject;
        foreach (var piece in brokenPieces.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 dir = (-transform.position + piece.transform.localPosition).normalized;
            piece.AddForce(dir * Random.Range(minForce,maxForce), ForceMode.Impulse);
        }

        GameManager.instance.ChangeGameState(GameManager.GameState.Fail);

        this.enabled = false;

    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Trap"))
        {
            Explode();
        }else if (col.CompareTag("Stripe"))
        {
            Debug.Log(col.transform.parent.name);
            foreach(var block in col.transform.parent.GetComponentsInChildren<Rigidbody>())
            {
                block.useGravity = true;
                block.isKinematic = false;
                Vector3 dir = (-transform.position + block.GetComponent<MeshRenderer>().bounds.center).normalized;
                block.AddForce(dir * Random.Range(minForce/2, maxForce/2), ForceMode.Impulse);
                Destroy(block.gameObject, 0.5f);
            }
            LevelManager.instance.curLevel.ScorePoint();
            Destroy(col.transform.parent.gameObject,3f);
        }else if (col.CompareTag("Finish"))
        {
            GameManager.instance.ChangeGameState(GameManager.GameState.Win);
        }
    }
}
