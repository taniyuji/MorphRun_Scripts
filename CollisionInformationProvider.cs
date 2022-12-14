using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class CollisionInformationProvider : MonoBehaviour
{
    private DrawPhysicsLine drawPhysicsLine;

    private Subject<CollisionMessenger.names> _isCollide = new Subject<CollisionMessenger.names>();

    public IObservable<CollisionMessenger.names> isCollide
    {
        get { return _isCollide; }
    }

    private List<CollisionMessenger> messengers = new List<CollisionMessenger>();

    private CollisionMessenger.names? getName;

    // Start is called before the first frame update
    void Awake()
    {
        drawPhysicsLine = GetComponent<DrawPhysicsLine>();
    }
    void Start()
    {
        StartCoroutine(SetMessenger());
    }

    private IEnumerator SetMessenger()
    {
        for (int i = 0; i < drawPhysicsLine.lines1.Count; i++)
        {
            messengers.Add(drawPhysicsLine.lines1[i].GetComponent<CollisionMessenger>());
        }

        for (int i = 0; i < drawPhysicsLine.lines2.Count; i++)
        {
            messengers.Add(drawPhysicsLine.lines2[i].GetComponent<CollisionMessenger>());
        }

        yield return null;

        for (int i = 0; i < messengers.Count; i++)
        {
            messengers[i].isCollide.Subscribe(i =>
            {
                getName = i;

                Debug.Log(i + "provider collide");
            });
        }

        if(getName != null) _isCollide.OnNext((CollisionMessenger.names)getName);

        getName = null;
    }   
}
