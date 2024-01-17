////便條紙: 記得把questionText拉進去
//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;
//using TMPro;

//public class WaterController : MonoBehaviour
//{
//    //public field
//    public bool is_ableDestroyed;
//    public Question question = new Question(0, 0, "+");
//    public List<WaterController> water_onScreen;
//    public Transform receivedTrans;
//    //private field (Serialize)
//    [SerializeField] float startSpeed = 5f;
//    [SerializeField] TextMeshProUGUI questionText;
//    [SerializeField] float slideSpeed = 5f; 
//    //private field (Unserialize)
//    float speed;
//    bool is_ableAnsed = false, is_ansed = false;
//    Rigidbody2D rid;
//    Vector3 dir;
//    PlayerController playerController;
//    Animator animator;
//    bool isHit = false;
//    // Start is called before the first frame update
//    void Start()
//    {
//        this.rid = GetComponent<Rigidbody2D>();
//        this.setAbleAnsed(true);
//        this.setIsAnsed(false);
//        this.setQues(true);
//        this.speed = this.startSpeed;
//        this.playerController = FindObjectOfType<PlayerController>();
//        this.animator = GetComponent<Animator>();
//        StartCoroutine(this.questionChange());
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    private void FixedUpdate()
//    {
//        this.rid.velocity = this.dir.normalized * this.speed;
//    }
//    void setAbleAnsed(bool boolContler)
//    {
//        this.is_ableAnsed = boolContler;
//    }
//    void setIsAnsed(bool boolController)
//    {
//        this.is_ansed = boolController;
//    }
//    void setQues(bool boolContler)
//    {
//        if (boolContler)
//        {
//            this.questionText.text = this.question.operand_1.ToString() + " " + this.question.operater + " " + this.question.operand_2.ToString() + " = ?";
//            this.question.setAns();
//        }
//        else
//        {
//            this.questionText.text = this.question.answer.ToString();
//        }
//    }
//    public void setDirection(Vector3 d)
//    {
//        this.dir = d;
//    }
//    public void destroyMe()
//    {
//        if (this.is_ansed == false)
//        {
//            Destroy(gameObject, 2f);
//            this.setQues(false);
//            this.water_onScreen.Remove(this);
//        }
//    }
//    public void correct()
//    {
//        this.setAbleAnsed(false);
//        this.setIsAnsed(true);
//        this.setQues(false);
//        this.speed = 0f;
//        AnimatorCleaer.ResetAllTriggers(this.animator);
//        StartCoroutine(this.slideTo(this.receivedTrans.position, this.slideSpeed));

//    }
//    IEnumerator slideTo(Vector3 target, float speed)
//    {
//        float animationDis = 2f;
//        Vector3 offset = (target - transform.position).normalized * 4;
//        bool tmp = false;
//        while (this.gameObject != null)
//        {
//            transform.position = Vector3.Lerp(transform.position, target + offset, speed * Time.deltaTime);
//            if (!tmp && Vector3.Distance(transform.position, target) < animationDis)
//            {
//                Debug.Log("closs");
//                this.animator.SetTrigger("Small");
//                tmp = true;
//            }
//            yield return null;
//        }
//    }
//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.tag == "waterReceivedSpot" && this.is_ansed)
//        {
//            this.playerController.waters++;
//            Destroy(gameObject);
//        }
//    }
//    IEnumerator questionChange()
//    {
//        while (!this.is_ansed)
//        {
//            yield return new WaitForSeconds(1f);
//            if (this.is_ansed)
//                break;
//            this.question.operand_2++;
//            this.setQues(true);
//        }
//    }
//}
