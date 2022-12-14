using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeDrawer : MonoBehaviour
{
    // 線の親オブジェクト
    [SerializeField] Rigidbody lineWrapper;
    // 線を構成するブロック
    [SerializeField] GameObject linePrefab;
    // 線の単位長
    [SerializeField] float lineLength = 0.2f;
    // 線の高さ
    [SerializeField] float lineWidth = 0.2f;
    // 線の奥行き
    [SerializeField] float lineZ = 1f;

    [SerializeField]
    private int InstantiateAmount = 50;

    [SerializeField]
    private float drawInterval;

    [SerializeField]
    private float fixJudgeLength = 0.01f;

    private ShapeInputController shapeInputController;

    private Vector3 touchPos;

    private List<GameObject> linePrefabs = new List<GameObject>();

    private int prefabIndex = 0;

    private bool canDraw = false;

    private float intervalCounter = 0;

    void Awake()
    {
        shapeInputController = GetComponent<ShapeInputController>();
    }

    void Start()
    {
        for (int i = 0; i < InstantiateAmount; i++)
        {
            var obj = Instantiate(linePrefab, transform.position, transform.rotation) as GameObject;

            obj.SetActive(false);

            linePrefabs.Add(obj);
        }
    }

    void FixedUpdate()
    {
        DrawLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            prefabIndex = 0;

            canDraw = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ResetLine());
        }
    }

    void DrawLine()
    {
        if (canDraw)
        {
            Vector3 startPos = touchPos;

            Vector3 endPos = shapeInputController.getPointerPosition;

            var judgeLength = lineLength - fixJudgeLength;

            var obj = linePrefabs[prefabIndex];

            obj.SetActive(true);

            obj.transform.position = (startPos + endPos) / 2;

            //obj.transform.right = (endPos - startPos).normalized;

            obj.transform.localScale = new Vector3(lineLength, lineWidth, lineZ);

            obj.transform.parent = lineWrapper.transform;

            touchPos = endPos;

            prefabIndex++;
        }
    }

    private IEnumerator ResetLine()
    {
        yield return null;

        for (int i = 0; i < InstantiateAmount; i++)
        {
            linePrefabs[i].transform.parent = null;
            linePrefabs[i].SetActive(false);
        }

        touchPos = shapeInputController.getPointerPosition;

        canDraw = true;
    }
}
