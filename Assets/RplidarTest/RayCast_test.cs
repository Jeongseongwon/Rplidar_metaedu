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

    //1. 마우스 클릭 할 때 레이캐스트 생성 하는거 구현
    //2. 레이 캐스트랑 UI 이미지랑 반응하는 것 확인
    //3. 최종 연동 작업 >> 3D 오브젝트랑 2D UI를 각각 구분해서 작동하도록 방법이 필요할 것으로 생각됨

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
