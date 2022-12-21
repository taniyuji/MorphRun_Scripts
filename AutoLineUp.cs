/*
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

//オブジェクトをエディタ上で自動的に羅列させるスクリプト
[ExecuteInEditMode]
public class AutoLineUp : MonoBehaviour
{
    [Header("シーン上のオブジェクトを整理するのみ")]
    [Space(20)]
    [SerializeField]
    private bool justReArranging;

    [Header("整理するオブジェクトリスト")]
    [ShowIf("@justReArranging")]
    [SerializeField]
    private List<GameObject> ReArrangingObjects;

    [Header("生成するオブジェクト")]
    [ShowIf("@!justReArranging")]
    [SerializeField]
    private List<GameObject> InstantiateObjects;

    [Header("対象のポジションから生成")]
    [ShowIf("@!remainPrefabInformation")]
    [SerializeField]
    private bool arrangeFromTargetPosition;

    [Header("基準とするオブジェクトポジション")]
    [ShowIf("@justReArranging")]
    [SerializeField]
    private GameObject reArrangeTargetObject;

    [Header("生成する数")]
    [ShowIf("@!justReArranging")]
    [SerializeField]
    private int generateAmount;

    [Header("直前に生成したオブジェクト達を削除")]
    [ShowIf("@!justReArranging")]
    [SerializeField]
    private bool delete = false;

    [Header("prefabから生成時そのprefab情報を維持")]
    [ShowIf("@!justReArranging")]
    [Space(20)]
    [SerializeField]
    private bool remainPrefabInformation;

    [Header("列数")]
    [Space(20)]
    [SerializeField]
    private int column;

    [Header("列の生成方向")]
    [SerializeField]
    private Vector3 columnGenerateDirection;

    [Header("行の生成方向")]
    [ShowIf("@column > 1")]
    [SerializeField]
    private Vector3 rowGenerateDirection;

    [Header("間隔設定に対象のスケールを使用")]
    [Space(20)]
    [SerializeField]
    private bool useObjectScale;

    [Header("オブジェクト同士の間隔")]
    [ShowIf("@useObjectScale")]
    public Vector3 generateInterval;

    [Header("スケール非使用時")]
    [ShowIf("@!useObjectScale")]
    public Vector3 originalGenerateInterval;

    [Header("rotationを変更")]
    [Space(20)]
    [SerializeField]
    private bool isEditRotation;

    [Header("Rotationをランダムにセット")]
    [ShowIf("@isEditRotation")]
    [SerializeField]
    private bool setRandomRotation;

    [Header("生成時のRotation")]
    [ShowIf("@isEditRotation && !setRandomRotation")]
    [SerializeField]
    private Vector3 generateRotation;

    [Header("生成開始")]
    [Space(20)]
    [SerializeField]
    private bool play = false;

    private List<GameObject> generatedObjectList = new List<GameObject>();

    private int indexNumber = 0;

    private Vector3 scale;

    private Vector3 fixedGenerateInterval;

    private Vector3 fixedGeneratePosition;

    private Vector3 fixedAddingRowVector;

    private Vector3 fixedAddingColumnVector;

    private int columnCounter = 1;

    private GameObject target;

    void Update()
    {
        if (remainPrefabInformation)
        {
            arrangeFromTargetPosition = true;
        }

        ArrangeObjects();//オブジェクトを羅列させるメソッド

        DeleteGeneratedObjects();//直前に生成したオブジェクト群を削除するメソッド
    }

    private void ArrangeObjects()//オブジェクトを羅列させる親メソッド
    {
        if (!play) return;

        SetDefaultValues();//整理や生成をする対象のオブジェクトの情報を取得

        for (int i = 0; i < generateAmount; i++)
        {
            FixPosition(i);//対象オブジェクトの位置を算出

            InstantiateAndMovePosition();//生成する場合は生成し算出したポジションに動かす
        }

        UnSetValues();//全ての変数を初期化

        play = false;
    }

    private void SetDefaultValues()
    {
        if (!remainPrefabInformation && arrangeFromTargetPosition)
        {
            transform.position = justReArranging
                ? reArrangeTargetObject.transform.position : InstantiateObjects[0].transform.position;
        }

        if (justReArranging)//整頓だけの場合は先頭から順にポジションを修正していく
        {
            target = ReArrangingObjects[0];
        }
        else//生成の場合はリストの中から対象のオブジェクトをランダムに1つ選ぶ
        {
            var index = Random.Range(0, InstantiateObjects.Count);

            if (remainPrefabInformation)
            {    
                target = PrefabUtility.GetCorrespondingObjectFromSource(InstantiateObjects[index]);

                if (target == null) target = InstantiateObjects[index];
            }
            else
            {
                target = InstantiateObjects[index];
            }
        }

        generatedObjectList.Clear();

        scale = useObjectScale ? target.transform.localScale : originalGenerateInterval;

        fixedGenerateInterval = scale + generateInterval;

        //行方向に加える値に指定された生成間隔を加えたものを取得
        fixedAddingRowVector = Vector3.Scale(rowGenerateDirection, fixedGenerateInterval);

        //列方向に加える値に指定された生成間隔を加えたものを取得
        fixedAddingColumnVector = Vector3.Scale(columnGenerateDirection, fixedGenerateInterval);

        columnCounter = 1;

        //Debug.Log("generateStart");
    }

    private void FixPosition(int i)
    {
        if (i == 0)
        {
            if (arrangeFromTargetPosition && column == 1)
            {
                fixedGeneratePosition += fixedAddingColumnVector;
            }
            else if (arrangeFromTargetPosition && column > 1)
            {
                fixedGeneratePosition += fixedAddingRowVector;
                columnCounter++;
            }
        }
        else if (columnCounter != column)//行方向の生成
        {
            columnCounter++;
            fixedGeneratePosition += fixedAddingRowVector;
        }
        else//列方向の生成
        {
            columnCounter = 1;

            //マイナス同士をかけると方向が変わってしまうためここで絶対値を取得
            fixedGeneratePosition
                 = new Vector3(Mathf.Abs(fixedGeneratePosition.x), Mathf.Abs(fixedGeneratePosition.y), Mathf.Abs(fixedGeneratePosition.z));

            //行方向の値を更新し、それ以外の方向の値を初期化
            fixedGeneratePosition
                = Vector3.Scale(fixedGeneratePosition, columnGenerateDirection) + fixedAddingColumnVector;
        }
    }

    private void InstantiateAndMovePosition()
    {
        GameObject generatedTarget;

        if (!justReArranging)//生成させる場合
        {
            generatedTarget = remainPrefabInformation
                 ? PrefabUtility.InstantiatePrefab(target) as GameObject : Instantiate(target);
        }
        else
        {
            generatedTarget = target;
        }

        generatedTarget.transform.position = fixedGeneratePosition + transform.position;

        generatedTarget.transform.parent = transform;

        //Debug.Log(generatedTarget.transform.position);

        if (isEditRotation)//rotationをいじる場合はここでセット
        {
            if (setRandomRotation) generatedTarget.transform.rotation = Random.rotation;
            else generatedTarget.transform.localEulerAngles = generateRotation;
        }

        generatedObjectList.Add(generatedTarget);

        if (justReArranging)//整頓の場合はここでインデックス番号を更新
        {
            indexNumber++;

            if (indexNumber > ReArrangingObjects.Count - 1) indexNumber = 0;

            target = ReArrangingObjects[indexNumber];
        }
        else//生成の場合はリストの中から新たにランダムに一つのオブジェクトを選択する
        {
            var index = Random.Range(0, InstantiateObjects.Count);

            if (remainPrefabInformation)
            {
                target = PrefabUtility.GetCorrespondingObjectFromSource(InstantiateObjects[index]);

                if (target == null) target = InstantiateObjects[index];
            }
            else
            {
                target = InstantiateObjects[index];
            }
        }
    }

    private void UnSetValues()
    {
        scale = Vector3.zero;

        fixedGenerateInterval = Vector3.zero;

        fixedGeneratePosition = Vector3.zero;

        //行方向に加える値に指定された生成間隔を加えたものを取得
        fixedAddingRowVector = Vector3.zero;

        //列方向に加える値に指定された生成間隔を加えたものを取得
        fixedAddingColumnVector = Vector3.zero;

        columnCounter = 1;

        indexNumber = 0;
    }

    private void DeleteGeneratedObjects()
    {
        if (!delete) return;

        if (generatedObjectList.Count <= 0)
        {
            Debug.LogWarning("No Object to delete");
            delete = false;
            return;
        }
        else if (justReArranging)
        {
            Debug.LogWarning("Error : Deleting scene object");
            delete = false;
            return;
        }

        for (int i = 0; i < generatedObjectList.Count; i++)
        {
            DestroyImmediate(generatedObjectList[i].gameObject);
        }

        generatedObjectList.Clear();

        delete = false;
    }
}
*/