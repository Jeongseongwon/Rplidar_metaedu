using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Destroy_prefab : MonoBehaviour
{
    public float second = 0f;
    private float timer = 0f;
    // Start is called before the first frame update


    //=====0822
    private Vector3 Temp_position;
    //

    void Start()
    {
        //여기에서 나머지 position 확인하고 대입하는걸로

        //Temp_position = Camera.main.WorldToScreenPoint(this.gameObject.GetComponent<Transform>().position);
        //Debug.Log(Temp_position.x);
        //Debug.Log(Temp_position.y);
        //Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));

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
