using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class FireballSysrem : MonoBehaviour
{
    [SerializeField] private FireFireballController fireball;
    [SerializeField] private EnemypartFireballController enemypart;
    public List<Transform> generateTransforms = new List<Transform>();
    public List<FireballController> fire_onScreen = new List<FireballController>();
    public DragonController bossDragon;
    public float shootAnimSpeed = 5f;
    public int min_1, max_1;
    public int min_2, max_2;
    public string operater;
    public int currentParts;
    public float healRatio = 0.2f;
    public GameObject bullet;
    public Vector3 bulletStartPosition;
    public GameObject dust;
    public float z_delta_enemy_part_position = 0.7f;
    int prePos = -1;
    [SerializeField] float fireballSpeed = 0.7f;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        this.prePos = -1;
        this.currentParts = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //this.generateFireball();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //this.generateHealball();
        }
        //²M°£©U§£
        for (int i = 0; i < this.fire_onScreen.Count; i++)
        {
            if (this.fire_onScreen[i].ableToBeDestroyed)
            {
                this.fire_onScreen[i].DestroyMe();
                this.fire_onScreen.RemoveAt(i);
                i--;
            }

        }
    }

    public void generateFireball(Question question)
    {
        int posIndex = UnityEngine.Random.Range(0, 4);
        while (posIndex == this.prePos)
        {
            posIndex = UnityEngine.Random.Range(0, 4);
        }
        this.prePos = posIndex;

        FireFireballController temp = Instantiate(this.fireball.gameObject, generateTransforms[posIndex].position, Quaternion.identity).GetComponent<FireFireballController>();
        temp.speed = -this.fireballSpeed;
        temp.type = TypeMode.Fireball;
        temp.question = question;

        this.fire_onScreen.Add(temp);
    }
    void generateHealball(Question question)
    {
        int posIndex = UnityEngine.Random.Range(0, 4);
        while (posIndex == this.prePos)
        {
            posIndex = UnityEngine.Random.Range(0, 4);
        }
        this.prePos = posIndex;

        FireballController temp = Instantiate(this.fireball.gameObject, generateTransforms[posIndex].position, Quaternion.identity).GetComponent<FireballController>();
        temp.type = TypeMode.Healball;
        temp.question = question;
        this.fire_onScreen.Add(temp);
    }

    public void generateFireballForDragon(Vector3 genPos, Question question)
    {
        FireFireballController temp = Instantiate(this.fireball.gameObject, genPos, Quaternion.identity).GetComponent<FireFireballController>();
        temp.speed = -this.fireballSpeed;
        temp.type = TypeMode.Fireball;
        temp.question = question;

        this.fire_onScreen.Add(temp);
    }
    public void generateEnemyPartForDragon(Transform parent, Vector3 genPos, float duration, string vocabulary_in_paragraph, string vocabulary_to_ans)
    {
        EnemypartFireballController temp = Instantiate(this.enemypart.gameObject, genPos - new Vector3(0f, z_delta_enemy_part_position, 0f), Quaternion.identity).GetComponent<EnemypartFireballController>();
        temp.transform.SetParent(parent);
        temp.setMaxTimeForPart(duration);
        temp.question = new Question(vocabulary_to_ans, vocabulary_in_paragraph, 8);
        this.fire_onScreen.Add(temp);
        this.currentParts++;
    }
    public void clearAllParts()
    {
        List<EnemypartFireballController> enemyparts_onScreen = fire_onScreen.OfType<EnemypartFireballController>().ToList();
        foreach (EnemypartFireballController i in fire_onScreen)
        {
            i.partWrong();
        }
    }
    public void SetPause(bool set)
    {
        foreach (FireballController i in this.fire_onScreen)
        {
            i.SetPause(set);
        }
    }
}

