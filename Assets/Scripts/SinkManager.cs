using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkManager : MonoBehaviour
{
    public float breathTracker;
    private bool breathExhaleFlag = true;
    public float legsTracker;
    private bool legsFlag;
    public float armsTracker;
    private bool armsFlag;
    private float mouseX;
    private float mouseY;
    private float mouseXHeld;
    private float mouseYHeld;

    public float maxBreath;
    public float maxSwim;

    public bool swimLegs = false;
    public bool swimArms = false;
    public bool swimBreath = false;

    public bool torsoSunk;
    public bool armsSunk;
    public bool legsSunk;
    public float autoRecoverTime;
    public bool autoRecover;
    private bool runOnce;
    public float autoRecoverSwimSpeed;
    private float legsAutoTarg;
    private float armsAutoTarg;

    public float sensitivityX;
    public float sensitivityY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            mouseXHeld = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
            mouseYHeld = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;
        }
        else
        {
            mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
            mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;
        }

        //BREATH TRACKER
        if(breathTracker <= maxBreath && breathTracker >= 0)
        {
            breathTracker += mouseY;
        }
        else if(breathTracker < 0)
        {
            breathTracker = 0;
        }
        else
        {
            breathTracker = maxBreath;
        }


        //LEGS TRACKER
        if (legsTracker <= maxSwim && legsTracker >= 0)
        {
            legsTracker += mouseX;
        }
        else if (legsTracker < 0)
        {
            legsTracker = 0;
        }
        else
        {
            legsTracker = maxSwim;
        }


        //ARMS TRACKER
        if (armsTracker <= maxSwim && armsTracker >= 0)
        {
            armsTracker += mouseXHeld;
        }
        else if (armsTracker < 0)
        {
            armsTracker = 0;
        }
        else
        {
            armsTracker = maxSwim;
        }

        CheckBreath();
        CheckLegs();
        CheckArms();

        if(swimBreath)
        {
            Debug.Log("Breathed successfully");
                
        }

        if(torsoSunk && !runOnce)
        {
            runOnce = true;
            StartCoroutine(AutoRecoverDelay());
        }
        else if (!torsoSunk)
        {
            autoRecover = false;
            runOnce = false;
        }

        if(autoRecover)
        {
            if (legsTracker > (maxSwim * 0.9f))
            {
                legsAutoTarg = 0;
            }
            else if (legsTracker < (maxSwim * 0.1))
            {
                legsAutoTarg = maxSwim;
            }

            if (armsTracker > (maxSwim * 0.9f))
            {
                armsAutoTarg = 0;
            }
            else if (armsTracker < (maxSwim * 0.1))
            {
                armsAutoTarg = maxSwim;
            }

            legsTracker = Mathf.Lerp(legsTracker, legsAutoTarg, autoRecoverSwimSpeed);
            armsTracker = Mathf.Lerp(armsTracker, armsAutoTarg, autoRecoverSwimSpeed);
        }
    }

    private void CheckBreath()
    {
        if(!swimBreath && breathExhaleFlag && breathTracker > (maxBreath * 0.8f))
        {
            breathExhaleFlag = false;
            swimBreath = true;
        }
        else if(!breathExhaleFlag && breathTracker < (maxBreath * 0.2))
        {
            breathExhaleFlag = true;
        }
        else
        {
            swimBreath = false;
        }
    }

    private void CheckLegs()
    {
        if (!swimLegs && legsFlag && legsTracker > (maxSwim * 0.8f))
        {
            legsFlag = false;
            swimLegs = true;
        }
        else if (!legsFlag && legsTracker < (maxSwim * 0.2))
        {
            legsFlag = true;
        }
        else
        {
            swimLegs = false;
        }
    }

    private void CheckArms()
    {
        if (!swimArms && armsFlag && armsTracker > (maxSwim * 0.8f))
        {
            armsFlag = false;
            swimArms = true;
        }
        else if (!armsFlag && armsTracker < (maxSwim * 0.2))
        {
            armsFlag = true;
        }
        else
        {
            swimArms = false;
        }
    }

    IEnumerator AutoRecoverDelay()
    {
        float t = 0;
        while (t < autoRecoverTime)
        {
            t += Time.deltaTime;

            if (!torsoSunk)
            {
                runOnce = false;
                break;
            }
            yield return null;
        }
        autoRecover = true;
    }
}
