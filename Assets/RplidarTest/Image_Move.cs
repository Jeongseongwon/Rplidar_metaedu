using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Image_Move : MonoBehaviour
{
    public float moveSpeed = 5f; // 이미지의 이동 속도

    public Image imageA; // A 이미지

    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;

    void Start()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(null);
    }
    void Update()
    {
        // 수평 및 수직 입력을 받아 이동 방향을 계산
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // 이미지 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeImageColor();
        }
    }

    //void ShootRay()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(transform.position);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        Debug.Log(hit.transform.name);
    //    }
    //}

    void ChangeImageColor()
    {
        pointerEventData.position = transform.position;

        List<RaycastResult> results = new List<RaycastResult>();

        // 레이캐스트를 통해 마우스 포인터가 위치한 UI 요소 검출
        graphicRaycaster.Raycast(pointerEventData, results);

        // 충돌한 UI 요소의 색상을 변경
        foreach (RaycastResult result in results)
        {
            Image hitImage = result.gameObject.GetComponent<Image>();
            if (hitImage != null)
            {
                // 레이를 맞은 이미지의 색상을 변경 (예시로 빨간색)
                hitImage.color = Color.red;
            }
        }
    }
}
