using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Image_Move : MonoBehaviour
{
    public float moveSpeed = 5f; // �̹����� �̵� �ӵ�

    public Image imageA; // A �̹���

    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;

    void Start()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(null);
    }
    void Update()
    {
        // ���� �� ���� �Է��� �޾� �̵� ������ ���
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // �̹��� �̵�
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

        // ����ĳ��Ʈ�� ���� ���콺 �����Ͱ� ��ġ�� UI ��� ����
        graphicRaycaster.Raycast(pointerEventData, results);

        // �浹�� UI ����� ������ ����
        foreach (RaycastResult result in results)
        {
            Image hitImage = result.gameObject.GetComponent<Image>();
            if (hitImage != null)
            {
                // ���̸� ���� �̹����� ������ ���� (���÷� ������)
                hitImage.color = Color.red;
            }
        }
    }
}
