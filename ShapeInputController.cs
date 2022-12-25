using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UniRx;

//プレイヤー変形時の入力処理を制御するスクリプト
public class ShapeInputController : MonoBehaviour
{

    [SerializeField]
    private Transform drawPointerTransform;

    public Vector3 getPointerDownPosition { get; private set; }

    private CinemachineTransposer transposer;

    private RaycastHit hit;

    void Awake()
    {
        transposer = ResourceProvider.i.cineCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetDrawVector();
    }

    private void GetDrawVector()
    {
        //ドラッグ終了時に変形させる
        if (Input.GetMouseButtonUp(0))
        {
            ResourceProvider.i.drawer.ChangeShape();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            CalculatePointerPosition();//pointerのポジションを取得

            //描写可能範囲より外から入力を始めた場合はここで返す
            if (hit.collider == null || !hit.collider.gameObject.CompareTag("Player")) return;

            //形を生成するスクリプトのマウス入力時の関数を呼び出す
            ResourceProvider.i.drawer.PenDown(drawPointerTransform.localPosition);

            var fixedPosition
                 = new Vector3(drawPointerTransform.localPosition.x, drawPointerTransform.localPosition.y, drawPointerTransform.localPosition.z + 0.6f);

            getPointerDownPosition = fixedPosition;

            return;
        }

        //GetMouseButtonDownにて描写可能範囲より外から入力を始めた場合はここで返す
        if(hit.collider == null || !hit.collider.gameObject.CompareTag("Player")) return;

        if (Input.GetMouseButton(0))
        {
            if(Time.timeScale == 0) Time.timeScale = 1;
            
            CalculatePointerPosition();//pointerのポジションを取得
            //形を生成するスクリプトのマウスをドラッグした時の関数を呼び出す
            ResourceProvider.i.drawer.PenMove(drawPointerTransform.localPosition);
        }
    }

    //pointerのポジションを取得
    private void CalculatePointerPosition()
    {
        var mouseInput = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        //描写可能範囲外に描いた場合はここで返す
        //マウスをクリックするとPlayerTag持ちのキャンバスが出現し、それが描写可能範囲となる。
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (!hit.collider.gameObject.CompareTag("Player")) return;
        }
        else
        {
            return;
        }

        //Debug.Log(mouseInput);

        //cinemaCineのtransposerのzポジションを代入し、マウス入力とのずれを修正
        mouseInput = new Vector3(mouseInput.x, mouseInput.y, transposer.m_FollowOffset.z);

        var fixedMouseInput = Camera.main.ScreenToWorldPoint(mouseInput);

        //Debug.Log(fixX);

        drawPointerTransform.position = new Vector3(fixedMouseInput.x, fixedMouseInput.y, drawPointerTransform.position.z);

    }
}
