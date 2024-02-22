using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NightVisionIndicator : MonoBehaviour
{
    public static NightVisionIndicator instance;

    //[SerializeField] private GameObject indicator;


    [SerializeField]
    private GameObject scanPrefab;

    [SerializeField]
    private SightController sightController;

    

    public GameObject targetObject;

    [SerializeField] private List<GameObject> targets;
    [SerializeField] private List<GameObject> indicators;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        //targets and indicators should have the same list size
        ScanObjects();
        int count = indicators.Count;
        for(int i = 0; i < count; i++)
        {
            if(targets.Count-1 < i)
            {
                Destroy(indicators[i]);
                indicators.RemoveAt(i);
                continue;
            }
            else if(targets[i] == null)
            {
                Destroy(indicators[i]);
                indicators.RemoveAt(i);
                continue;
            }

            TMP_Text inddetails = indicators[i].GetComponentInChildren<TMP_Text>();
            if(inddetails.text != targets[i].GetComponent<ISightObserver>().GetDetails())
            {
                inddetails.text = targets[i].GetComponent<ISightObserver>().GetDetails();
                inddetails.color = targets[i].GetComponent<EnemyBase>()?Color.red:Color.white;
                PlayerAudioController.instance.PlayAudio(AUDIOSOUND.SCROLL);
            }

            Vector3 targetPos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
            indicators[i].transform.position = Vector3.Lerp(indicators[i].transform.position, targetPos, 0.2f);
        }
    }

    //Coroutine co;
    void OnEnable()
    {
        //co = StartCoroutine(ScanObjects());
    }
    
    void OnDisable()
    {
        //StopCoroutine(co);
        //co = null;
    }

    private void ScanObjects()
    {
        List<GameObject> targetsList = sightController.GetObjectsInRange(60f);
        if(targetsList.Count > 0)
        {
            Debug.Log("scan found");
            for(int i = 0; i < targetsList.Count; i++)
            {
                if(targets.Contains(targetsList[i]))
                {
                    continue;
                }
                GameObject ind = Instantiate(scanPrefab, transform);
                ind.transform.position = Input.mousePosition;
                indicators.Add(ind);
            }
            targets = targetsList;
        }
        else
        {
            targets.Clear();

            if(indicators.Count > 0)
            {
                foreach(GameObject obj in indicators)
                {
                    Destroy(obj);
                }
            }
            indicators.Clear();
        }
    }
}
