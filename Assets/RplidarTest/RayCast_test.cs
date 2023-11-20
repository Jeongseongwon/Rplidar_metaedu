using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RayCast_test : MonoBehaviour
{
    public Canvas Test_UI_Canvas;
    GraphicRaycaster GR;
    PointerEventData PED;

    // Start is called before the first frame update
    void Start()
    {
        GR = Test_UI_Canvas.GetComponent<GraphicRaycaster>();
        PED = new PointerEventData(null);

        Debug.Log("RAYCAST");
    }
    // Update is called once per frame

    //1. ���콺 Ŭ�� �� �� ����ĳ��Ʈ ���� �ϴ°� ����
    //2. ���� ĳ��Ʈ�� UI �̹����� �����ϴ� �� Ȯ��
    //3. ���� ���� �۾� >> 3D ������Ʈ�� 2D UI�� ���� �����ؼ� �۵��ϵ��� ����� �ʿ��� ������ ������

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MOUSE CLICKED");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);

            }

            PED.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            GR.Raycast(PED, results);


            if (results[0].gameObject)
            {
                Debug.Log(results[0].gameObject.name);
            }

        }


    }

}
