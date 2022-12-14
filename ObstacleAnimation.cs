using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObstacleAnimation : MonoBehaviour
{
    private float ScaleAmount = 0.2f;

    private float scaleTime = 0.1f;

    private Vector3 scaleVector;

    private Vector3 defaultScale;

    private Sequence sequence;

    private bool isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isPlaying)
        {
            isPlaying = true;
            scaleVector = Vector3.one * ScaleAmount;

            sequence = DOTween.Sequence();

            sequence.Append(transform.DOScale(transform.localScale + scaleVector, scaleTime))
                    .Append(transform.DOScale(defaultScale, scaleTime))
                    .AppendCallback(() => isPlaying = false);

            SoundManager.i.PlayOneShot(6, 1f);

            Debug.Log("obstacle Scale");
        }
    }
}
