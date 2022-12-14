using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float morphInterval;

    [SerializeField]
    private List<Transform> shapes;

    private float intervalCounter;

    private MeshRenderer meshRenderer;

    private BoxCollider boxCollider;

    public Vector3 eyePosition { get; private set; }

    private Subject<Vector3> _morphed = new Subject<Vector3>();

    public IObservable<Vector3> morphed
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

        if(intervalCounter < morphInterval) return;

        if(playerMover.isCollide) return;

        intervalCounter = 0;

        meshRenderer.enabled = false;

        boxCollider.enabled = false;

        shapes[beforeIndex].gameObject.SetActive(false);

        int index = UnityEngine.Random.Range(0, shapes.Count);

        shapes[index].gameObject.SetActive(true);



        var addPos = (shapes[index].localScale.x / 2 - 0.1f) * shapes[index].right;

        eyePosition = shapes[index].localPosition + addPos;

        _morphed.OnNext(eyePosition);

        //Debug.Log(shapes[index].right);

        beforeIndex = index;
    }
}
