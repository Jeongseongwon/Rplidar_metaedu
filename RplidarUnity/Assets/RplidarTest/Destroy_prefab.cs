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
        //Temp_position = Camera.main.WorldToScreenPoint(this.gameObject.GetComponent<Transform>().position);
        //Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));
        //Debug.Log(Temp_position);
        ////중요, 이렇게 할 경우 오브젝트의 위치에 마우스가 위치하는 것을 확인함

        //Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0.5f)
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
