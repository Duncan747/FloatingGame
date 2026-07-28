using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBobber : MonoBehaviour
{
    public bool trackTorso;
    public bool trackLegs;
    public bool trackArms;
    private bool sinkToggle;
    public SinkManager SinkManagerRef;
    private bool mustSwimToRecover = false;

    public bool rotateBody = false;
    public float heightThreshold;
    public float heightLerpSpeed;
    public float rotLerpSpeed;

    public GameObject bodyPart;
    private Vector3 restingPos;
    public Quaternion restingRot;

    public float heightTarg;
    public float sinkMax;
    public float sinkSpeed;
    public float floatSpeed;
    public float sinkCooldown;
    public int sinkTimeRecoverScaler;

    private float sinkTimer;
    private float sinkTarg = 0f;
    private bool isSinking;
    private bool hasSunk;
    private float recoveryMultipler = 1f;

    public Transform mainCastOrigin;
    public Transform horiCastOrigin;
    public Transform vertCastOrigin;

    private Ray mainCast;
    private RaycastHit mainHitInfo;
    private Ray horiCast;
    private RaycastHit horiHitInfo;
    private Ray vertCast;
    private RaycastHit vertHitInfo;

    private LayerMask oceanMask;


    // Start is called before the first frame update
    void Start()
    {
        restingPos = bodyPart.transform.position;
        restingRot = bodyPart.transform.rotation;
        oceanMask = LayerMask.GetMask("Ocean");
    }

    private void Update()
    {
        if(trackTorso)
        {
            SinkManagerRef.torsoSunk = hasSunk;
            if(hasSunk)
            {
                if(SinkManagerRef.swimArms && !sinkToggle || SinkManagerRef.swimLegs && !sinkToggle)
                {
                    Debug.Log("Swim Recovery should occur");
                    sinkToggle = true;
                }
                else
                {
                    sinkToggle = false;
                }
            }
            else
            {
                sinkToggle = SinkManagerRef.swimBreath;
            }
        }
        else if(trackArms)
        {
            sinkToggle = SinkManagerRef.swimArms;
            SinkManagerRef.armsSunk = hasSunk;
        }
        else if(trackLegs)
        {
            sinkToggle = SinkManagerRef.swimLegs;
            SinkManagerRef.legsSunk = hasSunk;
        }
        else
        {
            sinkToggle = Input.GetKeyDown(KeyCode.Space);
        }

        if(sinkToggle)
        {
            if (trackTorso && SinkManagerRef.armsSunk && SinkManagerRef.legsSunk)
                return;

            isSinking = false;

            sinkTimer -= sinkCooldown / sinkTimeRecoverScaler;

            if(sinkTimer < 0)
            {
                sinkTimer = 0;
            }

            if(recoveryMultipler < 2f)
            {
                recoveryMultipler += 0.2f;
            }

            StartCoroutine(FloatUp());
        }
        else if(sinkTimer < sinkCooldown)
        {
            sinkTimer += Time.deltaTime;
        }
        else if(sinkTimer > sinkCooldown && !isSinking)
        {
            isSinking = true;
            recoveryMultipler = 1f;
            StartCoroutine(SinkDown());
        }    
    }

    void FixedUpdate()
    {
        mainCast = new Ray(mainCastOrigin.position, Vector3.down);
        horiCast = new Ray(horiCastOrigin.position, Vector3.down);
        vertCast = new Ray(vertCastOrigin.position, Vector3.down);

        Physics.Raycast(mainCast, out mainHitInfo, 1000f, oceanMask);
        Physics.Raycast(horiCast, out horiHitInfo, 1000f, oceanMask);
        Physics.Raycast(vertCast, out vertHitInfo, 1000f, oceanMask);

        heightTarg = heightThreshold - mainHitInfo.distance;

        Vector3 mainPoint = mainHitInfo.point;
        Vector3 horiPoint = horiHitInfo.point;
        Vector3 vertPoint = vertHitInfo.point;

        Vector3 horiNormal = mainPoint - horiPoint;
        Vector3 vertNormal = mainPoint - vertPoint;

        Quaternion horiQuat = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.right, horiNormal, Vector3.forward), Vector3.forward);
        Quaternion vertQuat = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, vertNormal, Vector3.right), Vector3.right);


        bodyPart.transform.position = Vector3.Lerp(bodyPart.transform.position, new Vector3(bodyPart.transform.position.x, restingPos.y + (heightTarg - sinkTarg), bodyPart.transform.position.z), heightLerpSpeed);
        if (rotateBody) 
        bodyPart.transform.rotation = Quaternion.Lerp(bodyPart.transform.rotation, (horiQuat * vertQuat * restingRot), rotLerpSpeed);
    }

    IEnumerator SinkDown()
    {
        float t = 0;
        float curSinkTarg = sinkTarg;
        while (t < sinkSpeed)
        {
            sinkTarg = Mathf.Lerp(curSinkTarg, sinkMax, (t / sinkSpeed));
            t += Time.deltaTime;

            if(!isSinking)
            {
                break;
            }
            else if (sinkTarg > sinkMax * (trackTorso ? 0.4f : 0.9f))
            {
                hasSunk = true;
            }

            yield return null;
        }
    }

    IEnumerator FloatUp()
    {
        float t = 0; 
        float curSinkTarg = sinkTarg;
        float newFloatSpeed = floatSpeed / recoveryMultipler;
        while (t < newFloatSpeed)
        {
            sinkTarg = Mathf.Lerp(curSinkTarg, 0f, (t / newFloatSpeed));
            t += Time.deltaTime;

            if (isSinking)
            {
                break;
            }
            else if (sinkTarg < sinkMax * (trackTorso ? 0.4f : 0.9f))
            {
                hasSunk = false;
            }

            yield return null;

            if(sinkToggle)
            {
                break;
            }
        }
    }
}
