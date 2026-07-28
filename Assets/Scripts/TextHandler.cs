using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHandler : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 moveDir;

    public Transform despawnTransform;

    private void Start()
    {
        despawnTransform = GameObject.FindGameObjectWithTag("despawner").transform;
    }

    void Update()
    {
        if(this.transform.localPosition.z > despawnTransform.localPosition.z)
        {
            this.transform.localPosition += new Vector3(0, 0, moveDir.z * moveSpeed);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
