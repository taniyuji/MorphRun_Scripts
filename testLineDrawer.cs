using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testLineDrawer : MonoBehaviour
{
    [SerializeField]
    private float lineWidth;

    private LineRenderer lineRenderer;

    private int positionCount;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;

        positionCount = 0;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            positionCount++;
            lineRenderer.positionCount = positionCount;
            lineRenderer.SetPosition(positionCount - 1, Input.mousePosition);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }

        if (Input.GetMouseButtonUp(0))
        {
            positionCount = 0;
        }
    }
}
