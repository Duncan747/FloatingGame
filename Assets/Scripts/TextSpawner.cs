using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class TextSpawner : MonoBehaviour
{
    public GameObject textObject;
    private GameObject curObj;
    private TextMeshPro tmpRef;
    private TextHandler textHandleRef;
    public float delayTextBegin;
    public float textDelayMin;
    public float textDelayMax;
    public float textMoveSpeedMin;
    public float textMoveSpeedMax;
    public AudioMixer audioMixer;

    public string[] textLines;
    private int textIterator;

    public Transform[] spawnPoints;
    private Transform curSpawnPoint;
    private int spawnIterator;

    private bool isSubmerging = false;
    private float submergeLerp;
    private float curAudioMix;
    private float curTextSoftness;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnText", delayTextBegin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnText()
    {
        if(spawnIterator < spawnPoints.Length)
        {
            curSpawnPoint = spawnPoints[spawnIterator];
            spawnIterator++;
        }
        else
        {
            spawnIterator = 0;
            curSpawnPoint = spawnPoints[spawnIterator];
        }

        if(textIterator < textLines.Length)
        {
            curObj = Instantiate(textObject, curSpawnPoint.position, Quaternion.Euler(90, 0, 0));
            tmpRef = curObj.GetComponentInChildren<TextMeshPro>();
            textHandleRef = curObj.GetComponent<TextHandler>();
            tmpRef.text = textLines[textIterator];
            textIterator++;
            float randSpeed = Random.Range(textMoveSpeedMin, textMoveSpeedMax);
            textHandleRef.moveSpeed = randSpeed;
            StartCoroutine(TextSpawnDelay());
        }
        else
        {
            //text is done and end game
        }
    }

    IEnumerator TextSpawnDelay()
    {
        float rand = Random.Range(textDelayMin, textDelayMax);
        yield return new WaitForSeconds(rand);

        SpawnText();
    }

    private void BeginSubmerge()
    {
        isSubmerging = true;
        StartCoroutine(SubmergeMuffle());
    }

    private void EndSubmerge()
    {
        isSubmerging = false;
        StartCoroutine(EmergeUnmuffle());
    }

    IEnumerator SubmergeMuffle()
    {
        float t = 0;
        curTextSoftness = tmpRef.fontMaterial.GetFloat(ShaderUtilities.ID_OutlineSoftness);
        while (t < submergeLerp)
        {
            tmpRef.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, Mathf.Lerp(curTextSoftness, 1f, (t / submergeLerp)));
            t += Time.deltaTime;

            if (!isSubmerging)
            {
                break;
            }

            yield return null;
        }
    }

    IEnumerator EmergeUnmuffle()
    {
        float t = 0;
        curTextSoftness = tmpRef.fontMaterial.GetFloat(ShaderUtilities.ID_OutlineSoftness);
        while (t < submergeLerp)
        {
            tmpRef.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, Mathf.Lerp(curTextSoftness, 0f, (t / submergeLerp)));
            t += Time.deltaTime;

            if (isSubmerging)
            {
                break;
            }

            yield return null;
        }
    }
}
