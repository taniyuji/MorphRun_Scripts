using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UniRx;

public class ShapeInputController : MonoBehaviour
{

    [SerializeField]
    private Transform drawPointerTransform;

    public Vector3 getPointerDownPosition { get; private set; }

    private CinemachineTransposer transposer;

    void Awake()
    {
        transposer = ResourceProvider.i.cineCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDrawVector();
    }

    private void GetDrawVector()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ResourceProvider.i.drawer.ChangeShape();
        }

        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0)) return;

        var mouseInput = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (!hit.collider.gameObject.CompareTag("Player")) return;
        }
        else
        {
            return;
        }

        //Debug.Log(mouseInput);

        mouseInput = new Vector3(mouseInput.x, mouseInput.y, transposer.m_FollowOffset.z);

        var fixedMouseInput = Camera.main.ScreenToWorldPoint(mouseInput);

        //Debug.Log(fixX);

        drawPointerTransform.position = new Vector3(fixedMouseInput.x, fixedMouseInput.y, drawPointerTransform.position.z);

        drawPointerTransform.localPosition = new Vector3(drawPointerTransform.localPosition.x,
                                                         drawPointerTransform.localPosition.y,
                                                         drawPointerTransform.localPosition.z);

        if (Input.GetMouseButtonDown(0))
        {
            ResourceProvider.i.drawer.PenDown(drawPointerTransform.localPosition);

            var fixedPosition
                 = new Vector3(drawPointerTransform.localPosition.x, drawPointerTransform.localPosition.y, drawPointerTransform.localPosition.z + 0.6f);

            getPointerDownPosition = fixedPosition;
        }

        if (Input.GetMouseButton(0))
        {
            ResourceProvider.i.drawer.PenMove(drawPointerTransform.localPosition);
        }

    }
}
