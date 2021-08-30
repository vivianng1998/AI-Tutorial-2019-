using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] formationA;
    [SerializeField]
    private Transform[] formationB;
    [SerializeField]
    private Transform[] formationC;
    public Transform[] selectedFormation;

    [SerializeField]
    private GameObject formPrefab;
    private int prefabNum;
    private List<GameObject> formList;

    [SerializeField]
    private GameObject leader;

    // Start is called before the first frame update
    void Start()
    {
        formList = new List<GameObject>();
        selectedFormation = new Transform[formationA.Length];

        for (int i = 0; i < formationA.Length; i++)
        {
            formationA[i].gameObject.SetActive(false);
            formationB[i].gameObject.SetActive(false);
            formationC[i].gameObject.SetActive(false);
        }

        prefabNum = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (prefabNum < 3)
            {
                GameObject prefab = Instantiate(formPrefab, leader.transform.position, Quaternion.identity);
                prefabNum++;
                prefab.GetComponent<ArriveForm>().formationPosition = selectedFormation[prefabNum - 0];
                formList.Add(prefab);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < formationA.Length; i++)
            {
                formationA[i].gameObject.SetActive(true);
                formationB[i].gameObject.SetActive(false);
                formationC[i].gameObject.SetActive(false);
                selectedFormation[i] = formationA[i];
                if (i < formList.Count)
                {
                    formList[i].GetComponent<ArriveForm>().formationPosition = selectedFormation[i];
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 0; i < formationB.Length; i++)
            {
                formationA[i].gameObject.SetActive(false);
                formationB[i].gameObject.SetActive(true);
                formationC[i].gameObject.SetActive(false);
                selectedFormation[i] = formationB[i];
                if (i < formList.Count)
                {
                    formList[i].GetComponent<ArriveForm>().formationPosition = selectedFormation[i];
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            for (int i = 0; i < formationC.Length; i++)
            {
                formationA[i].gameObject.SetActive(false);
                formationB[i].gameObject.SetActive(false);
                formationC[i].gameObject.SetActive(true);
                selectedFormation[i] = formationC[i];
                if (i < formList.Count)
                {
                    formList[i].GetComponent<ArriveForm>().formationPosition = selectedFormation[i];
                }
            }
        }
    }
}
