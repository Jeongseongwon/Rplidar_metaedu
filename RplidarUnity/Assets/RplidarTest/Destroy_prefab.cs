using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_prefab : MonoBehaviour
{
    public float second = 0f;
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 1f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            Destroy_obj(second);
        }
    }

    void Destroy_obj(float sec)
    {
        Destroy(this.gameObject);
    }
}
