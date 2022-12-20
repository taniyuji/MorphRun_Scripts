using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

//敵の挙動を制御するスクリプト
public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float morphInterval;//形を変化させる間隔の時間

    [SerializeField]
    private List<Transform> shapes;//このリストの中からランダムで形が一つ選ばれる

    private float intervalCounter;

    private MeshRenderer meshRenderer;

    private BoxCollider boxCollider;

    public Vector3 eyePosition { get; private set; }

    private Subject<Vector3> _morphed = new Subject<Vector3>();

    public IObservable<Vector3> morphed//変形したことをアニメーションスクリプトに通知
    {
        get { return _morphed; }
    }

    private int beforeIndex;

    private PlayerMover playerMover;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        boxCollider = GetComponent<BoxCollider>();

        playerMover = GetComponent<PlayerMover>();

        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        intervalCounter += Time.deltaTime;

        if (intervalCounter < morphInterval) return;

        if (playerMover.isCollide) return;

        intervalCounter = 0;

        if (meshRenderer.enabled)//初めの四角形を非表示にする
        {
            meshRenderer.enabled = false;

            boxCollider.enabled = false;
        }

        ChangeShapes();
    }

    private void ChangeShapes()
    {
        //ひとつ前の形を非表示にする
        shapes[beforeIndex].gameObject.SetActive(false);

        int index = UnityEngine.Random.Range(0, shapes.Count);

        shapes[index].gameObject.SetActive(true);

        //変更した形の先端部分のポジションを取得
        var addPos = (shapes[index].localScale.x / 2 - 0.1f) * shapes[index].right;

        //目がオブジェクトの先端かつ目の前に配置されるように設置
        eyePosition = new Vector3(shapes[index].localPosition.x,
                                  shapes[index].localPosition.y,
                                  shapes[index].localPosition.z + shapes[index].localScale.z / 2 + 0.1f)
                                   + addPos;

        //変形アニメーションスクリプトに通知
        _morphed.OnNext(eyePosition);

        //Debug.Log(shapes[index].right);

        beforeIndex = index;
    }
}
