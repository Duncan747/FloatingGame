using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TextSpawner : MonoBehaviour
{
    public SinkManager SinkManager;
    public GameObject textObject;
    private GameObject curObj;
    private TextMeshPro tmpRef;
    public TextMeshPro fontRef;
    private TextHandler textHandleRef;
    public float delayTextBegin;
    public float textDelayMin;
    public float textDelayMax;
    public float textMoveSpeedMin;
    public float textMoveSpeedMax;
    public AudioMixer audioMixer;
    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot underwater;
    public float underwaterTransition;
    public float normalTransition;
    public float submergeLerp;

    public string[] textLines;
    private int textIterator;

    public Transform[] spawnPoints;
    private Transform curSpawnPoint;
    private int spawnIterator;

    private bool isSubmerging = false;
    private float curAudioMix;
    private float curTextSoftness;
    private bool runOnStart;

    private bool endGame = false;
    public float gameEndTimer;
    public GameObject floatScene;
    public GameObject skyScene;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnText", delayTextBegin);
    }

    // Update is called once per frame
    void Update()
    {
        if(!runOnStart && tmpRef != null)
        {
            runOnStart = true;
            tmpRef.fontSharedMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0);
        }

        if(SinkManager.torsoSunk && !isSubmerging)
        {
            isSubmerging = true;
            BeginSubmerge();
            Debug.Log("Submerge has begun");
        }
        else if (!SinkManager.torsoSunk && isSubmerging)
        {
            isSubmerging = false;
            EndSubmerge();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
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
            curObj = Instantiate(textObject, curSpawnPoint.position, curSpawnPoint.localRotation, this.transform);
            //curObj.transform.localRotation = Quaternion.identity;
            tmpRef = curObj.GetComponentInChildren<TextMeshPro>();
            textHandleRef = curObj.GetComponent<TextHandler>();
            tmpRef.text = textLines[textIterator];
            textIterator++;
            float randSpeed = Random.Range(textMoveSpeedMin, textMoveSpeedMax);
            textHandleRef.moveSpeed = randSpeed;
            StartCoroutine(TextSpawnDelay());
        }
        else if (!endGame)
        {
            endGame = true;
            StartCoroutine(EndGameEvents());
            //text is done and end game
        }
    }

    IEnumerator TextSpawnDelay()
    {
        float rand = Random.Range(textDelayMin, textDelayMax);
        yield return new WaitForSeconds(rand);

        SpawnText();
    }

    IEnumerator EndGameEvents()
    {
        yield return new WaitForSeconds(gameEndTimer);
        floatScene.SetActive(false);
        skyScene.SetActive(true);
    }

    private void BeginSubmerge()
    {
        underwater.TransitionTo(underwaterTransition);
        StartCoroutine(SubmergeMuffle());
    }

    private void EndSubmerge()
    {
        normal.TransitionTo(normalTransition);
        StartCoroutine(EmergeUnmuffle());
    }

    IEnumerator SubmergeMuffle()
    {
        float t = 0;
        curTextSoftness =  tmpRef.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineSoftness);
        while (t < submergeLerp)
        {
            tmpRef.fontSharedMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, Mathf.Lerp(curTextSoftness, 0.9f, (t / submergeLerp)));
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
        curTextSoftness = tmpRef.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineSoftness);
        while (t < submergeLerp)
        {
            tmpRef.fontSharedMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, Mathf.Lerp(curTextSoftness, 0f, (t / submergeLerp)));
            t += Time.deltaTime;

            if (isSubmerging)
            {
                break;
            }

            yield return null;
        }
    }
}
