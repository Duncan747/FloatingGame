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
        if(this.transform.position.z > despawnTransform.position.z)
        {
            this.transform.position += new Vector3(moveDir.x * moveSpeed, moveDir.y * moveSpeed, moveDir.z * moveSpeed);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
