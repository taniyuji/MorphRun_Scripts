using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System;

public class DrawPhysicsLine : MonoBehaviour
{

    [Header("線を構成するブロック")]
    [SerializeField] GameObject linePrefab;
    [Header("線の単位長")]
    [SerializeField] float lineLength = 0.2f;
    [Header("線の幅")]
    [SerializeField] float lineWidth = 0.2f;
    [Header("線の奥行き")]
    [SerializeField] float lineZ = 1f;
    [Header("線の単位長の延長(線がギザギザするときはこの値を調整)")]
    [SerializeField] float lineLengthExtension = 0.1f;

    [SerializeField] private int instantiateAmount;

    [SerializeField]
    private float minimumLength = 2;//線の最小の短さ

    // 一つ前のタッチされたpositionを保持
    private Vector3 touchPos;

    Camera mainCamera;

    //1つ前の線と生成中の線を二つのリストにして使い分け
    public List<GameObject> lines1 { get; private set; } = new List<GameObject>();

    public List<GameObject> lines2 { get; private set; } = new List<GameObject>();

    private int index = 0;

    private bool isUsingList1 = true;

    private MeshRenderer mesh;

    private BoxCollider boxCollider;

    private float shapeLength = 0;

    private bool canDraw = false;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();

        boxCollider = GetComponent<BoxCollider>();

        mainCamera = Camera.main;

        //線生成に必要なオブジェクトをあらかじめ生成
        for (int i = 0; i < instantiateAmount; i++)
        {
            GameObject obj = Instantiate(linePrefab) as GameObject;

            obj.SetActive(false);

            lines1.Add(obj);
        }

        for (int i = 0; i < instantiateAmount; i++)
        {
            GameObject obj = Instantiate(linePrefab) as GameObject;

            obj.SetActive(false);

            lines2.Add(obj);
        }
    }

    public void ChangeShape()//Draw終了時に前の線と描いた線を入れ替えるメソッド。MouseButtonUp時に呼ばれる
    {
        ResourceProvider.i.ok.enabled = false;
        
        if (!canDraw)//線の長さが足りなかった場合は描写途中だったオブジェクトを初期化し返す
        {
            for (int i = 0; i < index; i++)
            {
                if (isUsingList1)
                {
                    lines1[i].SetActive(false);

                    lines1[i].transform.parent = null;
                }
                else
                {
                    lines2[i].SetActive(false);

                    lines2[i].transform.parent = null;
                }
            }

            index = 0;
            shapeLength = 0;

            return;
        }

        if (boxCollider.enabled)
        {
            mesh.enabled = false;
            boxCollider.enabled = false;
        }

        //現在表示中の線のリストを非表示にし、もう一つの方のリストを表示する
        for (int i = 0; i < instantiateAmount; i++)
        {
            if (!isUsingList1)
            {
                lines1[i].SetActive(false);

                lines1[i].transform.parent = null;

                if (i < index) lines2[i].SetActive(true);
            }
            else
            {
                lines2[i].SetActive(false);

                lines2[i].transform.parent = null;

                if (i < index) lines1[i].SetActive(true);
            }
        }

        ResourceProvider.i.shapeAnimation.PlayMorphAnimation(ResourceProvider.i.inputController.getPointerDownPosition);

        //Debug.Log("shifted" + lines1[0].activeSelf + lines2[0].activeSelf);

        index = 0;
        shapeLength = 0;
        canDraw = false;
    }

    public void PenDown(Vector3 drawPos)//次に使用するリストを決めるメソッド。MouseButtonDown時に呼ばれる。
    {
        touchPos = drawPos;

        if (!lines1[0].activeSelf && !lines2[0].activeSelf)
        {
            isUsingList1 = true;
        }
        else
        {
            isUsingList1 = !lines1[0].activeSelf ? true : false;
        }
    }

    public void PenMove(Vector3 drawPos)//引数のPointerポジションをもとに線を生成していくメソッド。MouseButton時に呼ばれる。
    {
        Vector3 startPos = touchPos;
        Vector3 endPos = drawPos;
        float distance = Vector3.Distance(startPos, endPos);

        //Debug.Log(shapeLength);

        if (shapeLength > minimumLength && !canDraw)
        {
            SoundManager.i.PlayOneShot(6, 0.5f);

            ResourceProvider.i.ok.enabled = true;

            canDraw = true;
        }

        if (index >= lines1.Count || index >= lines2.Count)
        {
            Debug.Log("no More Objects to draw");

            return;
        }

        //１フレーム前の入力と現在の入力の距離が線を構成する四角形プレファブ一個分より長い場合
        if (distance > lineLength * Time.deltaTime / Time.fixedDeltaTime)
        {
            // GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            shapeLength += distance;//線の長さを更新

            GameObject obj = isUsingList1 ? lines1[index] : lines2[index];//使用されていないほうのリストを使う

            obj.transform.parent = transform;

            obj.transform.localPosition = (startPos + endPos) / 2;//２つの入力の間に配置
            obj.transform.right = (endPos - startPos).normalized;//２つの入力のベクトル方向に傾ける
            obj.transform.localScale = new Vector3(distance + lineLengthExtension, lineWidth, lineZ);

            touchPos = endPos;

            index++;
        }

       
    }
}
