using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

public class FireballSysrem : MonoBehaviour
{
    public FireballController fireball;
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
    private OpenAIAPI gpt;
    private ChatMessage systemMessage;
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
        this.gpt = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_KEY", EnvironmentVariableTarget.User));
        this.systemMessage = new ChatMessage(ChatMessageRole.System, "The user will give you an English vocabulary and you have to make an example sentence. Let the sentences be similar to the texts appear in a Tofel or Toeic test. You should randomly choose a field of study, and let the sentence be related to it. When you answer the question, just print only the sentence.");
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

        FireballController temp = Instantiate(this.fireball.gameObject, generateTransforms[posIndex].position, Quaternion.identity).GetComponent<FireballController>();
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
        FireballController temp = Instantiate(this.fireball.gameObject, genPos, Quaternion.identity).GetComponent<FireballController>();
        temp.speed = -this.fireballSpeed;
        temp.type = TypeMode.Fireball;
        temp.question = question;

        this.fire_onScreen.Add(temp);
    }
    public void generateEnemyPartForDragon(Transform parent, Vector3 genPos, float duration, string vocabulary)
    {
        FireballController temp = Instantiate(this.fireball.gameObject, genPos - new Vector3(0f, z_delta_enemy_part_position, 0f), Quaternion.identity).GetComponent<FireballController>();
        temp.transform.SetParent(parent);
        temp.type = TypeMode.EnemyPart;
        temp.setMaxTimeForPart(duration);
        temp.question = new Question(vocabulary, "<        >");
        this.fire_onScreen.Add(temp);
        this.currentParts++;
    }
    public void clearAllParts()
    {
        foreach (FireballController i in fire_onScreen)
        {
            if (i.type == TypeMode.EnemyPart)
            {
                i.partWrong();
            }
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

