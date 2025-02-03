using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TabSystem))]
public class TabSelectIconController : MonoBehaviour
{
    [SerializeField] Image iconArrowSortByTime;
    [SerializeField] Image iconArrowSortByDic;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TabSystem>().onTabSwitched += this.MoveArrow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MoveArrow(string tabName)
    {
        if (tabName == "ResultWindowSortByTime")
        {
            this.iconArrowSortByTime.gameObject.SetActive(true);
            this.iconArrowSortByDic.gameObject.SetActive(false);
        }
        if (tabName == "ResultWindowSortByDic")
        {
            this.iconArrowSortByTime.gameObject.SetActive(false);
            this.iconArrowSortByDic.gameObject.SetActive(true);
        }
    }
}
