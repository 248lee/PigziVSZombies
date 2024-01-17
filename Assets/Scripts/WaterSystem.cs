//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//public class WaterSystem : MonoBehaviour
//{
//    public WaterController waterBall;
//    public List<WaterGenerate> generaters = new List<WaterGenerate>();
//    public List<WaterController> waterBall_onScreen = new List<WaterController>();
//    public int min_1, max_1;
//    public int min_2, max_2;
//    public string operater;
//    [SerializeField] Transform receivedTrans;
//    int prePos = -1;
//    // Start is called before the first frame update
//    void Start()
//    {
//        this.prePos = -1;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.W))
//        {
//            this.generateWaterball();
//        }
//    }
//    void generateWaterball()
//    {
//        int posIndex = Random.Range(0, 4);
//        while (posIndex == this.prePos)
//        {
//            posIndex = Random.Range(0, 4);
//        }
//        this.prePos = posIndex;
//        WaterGenerate wg = generaters[posIndex];
//        WaterController temp = Instantiate(this.waterBall.gameObject, wg.transform.position, Quaternion.identity).GetComponent<WaterController>();
//        float t = Random.Range(0f, 1f);
//        temp.setDirection(Vector3.Slerp(wg.dir1, wg.dir2, t));
//        temp.receivedTrans = this.receivedTrans;
//        this.waterBall_onScreen.Add(temp);
//        temp.water_onScreen = this.waterBall_onScreen;
//    }
//}
