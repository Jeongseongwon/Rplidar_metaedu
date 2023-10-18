using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MOUSE_prefab : MonoBehaviour
{

    private Vector3 Temp_position;
    // Start is called before the first frame update
    void Start()
    {
        Destroy_obj();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Destroy_obj()
    {
        Temp_position = Camera.main.WorldToScreenPoint(this.transform.position);
        Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, 1080 - Temp_position.y));

        Destroy(this.gameObject);
    }
}
