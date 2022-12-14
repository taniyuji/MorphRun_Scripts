using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UniRx;

//変形時のアニメーションを制御するスクリプト。敵とプレイヤーに使用
public class ShapeAnimation : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer eyesParent;//この親オブジェクトのSpriteを変更する

    [SerializeField]
    private List<SpriteRenderer> eyesSprites;//目のSpriteたち

    [SerializeField]
    private float scaleTime = 0.1f;

    [SerializeField]
    private float scaleAmount = 0.1f;

    private Transform defaultTransform;

    private Sequence sequence;

    private Vector3 scaleVector;

    private PlayerMover playerMover;

    private DrawPhysicsLine drawer;

    private ShapeInputController inputController;

    private EnemyBehavior enemyBehavior;

    private int blinkJudgeNum;

    private bool isGoal = false;

    private float scaleTimeCounter;

    private bool isPlus;

    // Start is called before the first frame update
    void Awake()
    {
        playerMover = GetComponent<PlayerMover>();

        drawer = GetComponent<DrawPhysicsLine>();

        inputController = GetComponent<ShapeInputController>();

        enemyBehavior = GetComponent<EnemyBehavior>();
    }

    void Start()
    {
        defaultTransform = transform;

        if (enemyBehavior != null)//敵を変形させる場合
        {
            //UniRxで目の位置を受け取る
            enemyBehavior.morphed.Subscribe(i =>
            {
                PlayMorphAnimation(i);
            });
        }

        if (drawer != null)
        {
            drawer.changeShape.Subscribe(i =>
            {
                PlayMorphAnimation(inputController.getPointerDownPosition);
            });
        }

        playerMover.isGoal.Subscribe(i =>
        {
            if (isGoal) return;

            isGoal = true;

            SoundManager.i.PlayOneShot(7, 0.5f);
        });
    }

    void Update()
    {
        GoalAnimation();//ゴール通知を受け取ったらゴールアニメーションを作動させる

        if (isGoal) return;

        collideEyes();

        if (UnityEngine.Random.Range(0, 200) == 0)//1/200の確率で瞬きさせる
        {
            StartCoroutine(blink());
        }
    }

    //キラキラ目に変更し、スケールの拡大縮小を繰り返す
    private void GoalAnimation()
    {
        if (!isGoal) return;

        eyesParent.sprite = eyesSprites[3].sprite;

        scaleTimeCounter += Time.deltaTime;

        if (scaleTimeCounter > scaleTime)
        {
            scaleTimeCounter = 0;

            isPlus = !isPlus;
        }

        scaleVector = isPlus ? Vector3.one * 0.001f : Vector3.one * -0.001f;

        transform.localScale += scaleVector;
    }

    //衝突した際の表情を変更
    private void collideEyes()
    {
        if (playerMover.state == PlayerMover.PlayerState.IsCollide)//衝突した際に変更
        {
            eyesParent.sprite = eyesSprites[2].sprite;

            return;
        }
        else if (playerMover.state == PlayerMover.PlayerState.Running && eyesParent.sprite == eyesSprites[2].sprite)//衝突挙動が終わり、まだ目が戻っていない場合
        {
            eyesParent.sprite = eyesSprites[0].sprite;
        }
    }

    //変形時のアニメーションを行うメソッド
    //プレイヤーの場合はタイミングの兼ね合い上、DrawPhysicsLineスクリプトにてこのメソッドが呼ばれる
    public void PlayMorphAnimation(Vector3 eyePosition)
    {
        scaleVector = Vector3.one * scaleAmount;

        sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(transform.localScale + scaleVector, scaleTime))
                .Append(transform.DOScale(defaultTransform.localScale, scaleTime));

        eyesParent.transform.localPosition = new Vector3(eyePosition.x,
                                                         eyePosition.y,
                                                         eyePosition.z);

        if (gameObject.CompareTag("Player")) SoundManager.i.PlayOneShot(1, 0.5f);
    }

    private IEnumerator blink()
    {
        eyesParent.sprite = eyesSprites[1].sprite;

        yield return new WaitForSeconds(0.1f);

        eyesParent.sprite = eyesSprites[0].sprite;
    }


}
