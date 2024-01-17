using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[System.Serializable]
public class Wave
{
    public float attr1 = 10;
    public float attr2 = 303f;
    public List<Subwave> subwaves = new List<Subwave>();
}
public class WaveSystem : MonoBehaviour
{
    public List<Wave> waves;
    public int nowWave = 0;
    PlayerController playerController;
    DragonController dragon;
    FireballSysrem fireballsystem;

    // Start is called before the first frame update
    void Start()
    {
        this.playerController = FindObjectOfType<PlayerController>();
        this.dragon = FindObjectOfType<DragonController>();
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
        StartCoroutine(this.gameProcess());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator gameProcess()
    {
        foreach (Wave wave in this.waves)
        {
            yield return StartCoroutine(this.implementWaveProcess(wave));
            this.nowWave++;
        }
        this.dragon.Born();
    }
    IEnumerator implementWaveProcess(Wave wave)
    {
        foreach (Subwave subwave in wave.subwaves)
        {
            yield return StartCoroutine(this.implementSubwaveProcess(subwave));
        }
        while (this.fireballsystem.fire_onScreen.Count != 0) // busy waiting until the fire on screen is empty
        {
            yield return null;
        }
    }
    IEnumerator implementSubwaveProcess(Subwave subwave)
    {
        yield return new WaitForSeconds(subwave.startDelay);
        for (int i = 0; i < subwave.numOfEmmisions; i++)
        {
            float delayTime = Random.Range(subwave.durationMin, subwave.durationMax);
            this.fireballsystem.generateFireball();
            yield return new WaitForSeconds(delayTime);
        }
    }
    float getTimeRandPos(float duration, float td)
    {
        float tmp = Random.Range(0, duration);
        if (tmp < td)
        {
            int rePick = Random.Range(0, 2);
            if (rePick == 0)
                tmp = getTimeRandPos(duration, td);
        }
        return tmp;
    }
}
