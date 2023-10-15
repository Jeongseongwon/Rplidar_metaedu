using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using System;
using System.Threading;

public class RplidarTest : MonoBehaviour
{

    public string port;
    //public GameObject Capsule;

    private LidarData[] data;
    private RectTransform Img_Rect_transform;
    private GameObject CANVAS;
    public GameObject Testobj;

    //=====0714
    public GameObject spherePrefab; //spherePrefab을 받을 변수 
    private const int DATA_LENGTH = 720; //360개의 데이터의 개수를 상수로 설정
    private int drawingIndex; //DATA_LENGTH만큼 Update문을 수행하기 위한 변수 설정

    private bool Check_onscan = false;

    public bool m_onscan = false;
    private Thread m_thread;
    private bool m_datachanged = false;
    //=====

    //=====0822
    public float distance_min = 100;
    public float distance_max = 2000;
    private Vector3 Temp_position;

    public GameObject sphere_obj;
    //private int i = 0;
    public GameObject Moving_obj;

    public float Data_angle = 0;
    public float Data_distance = 2f;
    //=====


    //====1012
    public float lidarSpinSpeed; //라이다가 도는 속도를 설정할 변수.
    public bool Test_check = false;
    double number = 0f;
    //

    //1015

    private float Resolution_Y = 1080;
    private float Resolution_X = 1920;
    private float min_x;
    private float min_y;
    private float max_x;
    private float max_y;


    private float Setting_min_x;
    private float Setting_min_y;
    private float Setting_max_x;
    private float Setting_max_y;

    private Camera cameraToLookAt;
    private void Awake()
    {
        data = new LidarData[720];
    }

    [SerializeField]
    public float Test_degree = 0;
    public float Test_distance = 0;
    public GameObject temp_pos;

    public float Sensor_rotation = 0;
    private void Sensing()
    {
        //// 1013 스크립트 들어있는 오브젝트 회전 확인용
        // Img_Rect_transform.Rotate(new Vector3(0, 0, Time.deltaTime * lidarSpinSpeed));

        //// 1015 캔버스 위 오브젝트 위치 확인용(1:1 비율로 캔버스장 좌표에 대입)
        // Img_Rect_transform.anchoredPosition = new Vector3(143, 0, 0);


        Test_degree = Data_angle;
        Test_distance = Data_distance;
        Debug.Log("1. data_angle : " + Test_degree + "  2. data_distance : " + Test_distance);
        // degree, angle값을 기준으로 x,y좌표 계산
        Vector3 pos = new Vector3(Mathf.Cos(Test_degree * Mathf.Deg2Rad) * Test_distance, Mathf.Sin(Test_degree * Mathf.Deg2Rad) * Test_distance, 0);
        temp_pos.GetComponent<RectTransform>().anchoredPosition = pos;

        //디버그 로그에서는 값이 정상적으로 바뀐것을 확인함 -> 콘솔창에서 값 출력 확인
        //public에서는 값이 정상적으로 바뀌지 않음
        //Data angle이랑 distance는 그냥 0으로 고정이고 새로 만든 Test_degree와 Test_distance의 경우 값이 순차적으로 한 번 오르다가 그 이후로는 변경이 없으

        //그렇다면 믿음의 영역이라 치고 그냥 그 변수 그대로 활용하는 

    }

    // Use this for initialization
    void Start()
    {
        cameraToLookAt = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Img_Rect_transform = this.GetComponent<RectTransform>();
        CANVAS = GameObject.Find("Canvas");

        lidarSpinSpeed = 1300.0f;

        //이걸로 기준점 position입력해주고 기준 위치 0,0,0에 생성
        //temp_pos = Instantiate(spherePrefab, Testobj.transform.position, Quaternion.Euler(0, 0, 90), CANVAS.transform);

        //degree, angle값을 기준으로 x,y좌표 계산
        //Vector3 pos = new Vector3(Mathf.Cos(Test_degree * Mathf.Deg2Rad) * Test_distance, Mathf.Sin(Test_degree * Mathf.Deg2Rad) * Test_distance, 0);
        //temp_pos.GetComponent<RectTransform>().anchoredPosition = pos;

        //초기 해상도로 intializing
        min_x = (float)(Resolution_X * -0.5);
        min_y = (float)(Resolution_Y * -0.5);
        max_x = (float)(Resolution_X * 0.5);
        max_y = (float)(Resolution_Y * 0.5);


        //설정한 범위로 intializing
        Setting_min_y = -540;
        Setting_max_y = 0;

        min_y = -100;
        max_y = 0;
        max_x = 100;
        min_x = -100;
    }

    void Update()
    {
        //if (Test_check)
        //    Sensing();
    }

    void GenMesh()
    {
        while (true)
        {
            int datacount = RplidarBinding.GetData(ref data);

            if (datacount == 0)
            {
                Thread.Sleep(5);
            }
            else
            {
                m_datachanged = true;
            }
        }
    }

    private float x;
    private float y;

    //해상도 1920,1080

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_datachanged)
        {
            for (int i = 0; i < 720; i++)
            {
                Data_angle = data[i].theta;
                Data_distance = data[i].distant;

                //1. 화면과 센서를 일치화 시키기 위해서 theta를 마이너스 곱해줌, 추가로 회전 시켜주기 위해 Sensor_rotation 추가했고 위에서 아래 방향으로 내려다 보는것 기준으 90도 입력하면 댐
                x = 0.25f * Mathf.Cos((-data[i].theta + Sensor_rotation) * Mathf.Deg2Rad) * data[i].distant;
                y = 0.25f * Mathf.Sin((-data[i].theta + Sensor_rotation) * Mathf.Deg2Rad) * data[i].distant;

                if (i % 4 == 0)
                {

                    //2. instantiate 부분 테스트, 결과 문제 없이 이미지 프리팹을 센서 센싱 위치로 생성할 수 잇는 것을 확인하였음
                    /*
                    GameObject Prefab_pos = Instantiate(spherePrefab, this.transform.position, Quaternion.identity, CANVAS.transform);
                    Prefab_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                    Prefab_pos.transform.LookAt(transform.position + cameraToLookAt.transform.rotation * Vector3.back,
                    cameraToLookAt.transform.rotation * Vector3.down);*/

                    //3. 프리팹 생성 없이 해당 지점 그냥 가져와서 마우스 바로 이동시키는걸로 넘어감
                    //데이터가 너무 많아서 해당 지점이 찍히지 않는 다는 가정하에
                    //직접 가이드라인 만들어놓고 데이터를 측정해서 점이 몇개 없으면 측정이 가능할지 확인이 필요함
                    /*
                    temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                    Temp_position = Camera.main.WorldToScreenPoint(temp_pos.transform.position);
                    Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));
                    */

                    //4. 가이드라인 만들어놓고 측정되는 데이터 갯수를 줄이는거 테스트 해봤고 min, max값 조절해서 가이드라인을 조절할 수 있음 확인함
                    //그리고 난 다음에 마우스가 이동가능한지 확인함
                    if (x != 0 || y != 0)
                    {
                        if (min_x < x && x < max_x)
                        {
                            if (min_y < y && y < max_y)
                            {

                                GameObject Prefab_pos = Instantiate(spherePrefab, this.transform.position, Quaternion.identity, CANVAS.transform);
                                Prefab_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                                Prefab_pos.transform.LookAt(transform.position + cameraToLookAt.transform.rotation * Vector3.back,
                                cameraToLookAt.transform.rotation * Vector3.down);

                                if (i % 8 == 0)
                                {
                                    Temp_position = Camera.main.WorldToScreenPoint(Prefab_pos.transform.position);
                                    Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));
                                }
                            }
                        }
                    }
                    Debug.Log("BEFORE 1. x : " + x + "  2. y : " + y);



                    //위치 추출해서 마우스 이동시키는 함수
                    //Temp_position = Camera.main.WorldToScreenPoint(Prefab_pos.transform.position);
                    //Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));


                    //이미지 카메라 바라보게 하는 함수
                    //Prefab_pos.transform.LookAt(transform.position + cameraToLookAt.transform.rotation * Vector3.back,
                    //cameraToLookAt.transform.rotation * Vector3.down);

                }



                /* 가이드라인 구현해놓은거
                if (x != 0 || y != 0)
                {
                    //Temp pos 위치 바꾸는 코드
                    //temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                    //Debug.Log("NOT 0 AFTER 1. x : " + x + "  2. y : " + y);

                    //화면 해상도에 따라서 그 안에서만 생성이 될 수 있게 가이드라인 해주는 코드
                    if (min_x < x && x < max_x)
                    {
                        if (min_y < y && y < max_y)
                        {
                            //마우스 커서 위치 바꾸는 코드
                            //Mouse.current.WarpCursorPosition(new Vector2(x, y));

                            //테스트 오브젝트 위치 바꾸고 해당 좌표에 마우스 이동시키는 코드
                            temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);

                            Temp_position = Camera.main.WorldToScreenPoint(temp_pos.transform.position);

                            Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));

                            //느낌상 오브젝트가 직접적으로 거기로 변경은 안되나
                            //계산은 이미 되어서 마우스는 이동가능 할 것이라는 가정
                        }
                    }
                }*/


                //만약에 실제 세계의 콘텐츠 조건이랑 이걸 연동 시키려면 어떻게 해야하나?
                //그냥 x,y범위 정해놓고 그 안에 들어갈 수 있게끔만 하면 됨

                //각도 min 0 ~ 360


            }
            m_datachanged = false;
        }

    }
    void Test_NONSENSOR()
    {
        //그냥 같은 속도로 랜덤 함수를 호출해서 위치를 바꾸는 테스트 진행함
        // 결과 : 정상적으로 범위 안에서 랜덤함수 통해 위치를 변경할 경우 잘 바뀌는 것을 확인하였음

        //x = 0.5f * Mathf.Cos(data[i].theta * Mathf.Deg2Rad) * data[i].distant;
        //y = 0.5f * Mathf.Sin(data[i].theta * Mathf.Deg2Rad) * data[i].distant;

        x = UnityEngine.Random.Range(-1080, 1080);
        y = UnityEngine.Random.Range(-1920, 1920);

        //Debug.Log("BEFORE 1. x : " + x + "  2. y : " + y);

        if (x != 0 || y != 0)
        {
            //Temp pos 위치 바꾸는 코드
            //temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
            //Debug.Log("NOT 0 AFTER 1. x : " + x + "  2. y : " + y);

            //화면 해상도에 따라서 그 안에서만 생성이 될 수 있게 가이드라인 해주는 코드
            if (min_x < x && x < max_x)
            {
                if (min_y < y && y < max_y)
                {
                    //마우스 커서 위치 바꾸는 코드
                    //Mouse.current.WarpCursorPosition(new Vector2(x, y));

                    //테스트 오브젝트 위치 바꾸고 해당 좌표에 마우스 이동시키는 코드
                    temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                    // Debug.Log("Image position  1. x : " + x + "  2. y : " + y);

                    Temp_position = Camera.main.WorldToScreenPoint(temp_pos.transform.position);

                    Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));
                    // Debug.Log("mouse position  1. x : " + Temp_position.x + "  2. y : " + Temp_position.y);

                }
            }
        }
    }

    void Test_SENSOR()
    {
        //x = 0.5f * Mathf.Cos(data[i].theta * Mathf.Deg2Rad) * data[i].distant;
        //y = 0.5f * Mathf.Sin(data[i].theta * Mathf.Deg2Rad) * data[i].distant;

        //x = UnityEngine.Random.Range(-1080, 1080);
        //y = UnityEngine.Random.Range(-1920, 1920);

        //Debug.Log("BEFORE 1. x : " + x + "  2. y : " + y);

        if (x != 0 || y != 0)
        {
            //Temp pos 위치 바꾸는 코드
            //temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
            //Debug.Log("NOT 0 AFTER 1. x : " + x + "  2. y : " + y);

            //화면 해상도에 따라서 그 안에서만 생성이 될 수 있게 가이드라인 해주는 코드
            if (min_x < x && x < max_x)
            {
                if (min_y < y && y < max_y)
                {
                    //마우스 커서 위치 바꾸는 코드
                    //Mouse.current.WarpCursorPosition(new Vector2(x, y));

                    //테스트 오브젝트 위치 바꾸고 해당 좌표에 마우스 이동시키는 코드
                    temp_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);

                    Temp_position = Camera.main.WorldToScreenPoint(temp_pos.transform.position);

                    Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));

                }
            }
        }
    }
    void OnDestroy()
    {
        RplidarBinding.EndScan();
        RplidarBinding.EndMotor();
        RplidarBinding.OnDisconnect();
        RplidarBinding.ReleaseDrive();

        //StopCoroutine(GenMesh());
        m_thread.Abort();

        m_onscan = false;
    }

    private void OnGUI()
    {
        DrawButton("Connect", () =>
        {
            if (string.IsNullOrEmpty(port))
            {
                return;
            }

            int result = RplidarBinding.OnConnect(port);

            Debug.Log("Connect on " + port + " result:" + result);
        });

        DrawButton("DisConnect", () =>
        {
            bool r = RplidarBinding.OnDisconnect();
            Debug.Log("Disconnect:" + r);
        });

        DrawButton("StartScan", () =>
        {
            m_onscan = RplidarBinding.StartScan();
            Debug.Log("StartScan:" + m_onscan);
            Check_onscan = true;

            if (m_onscan)
            {
                //    StartCoroutine(GenMesh());
                m_thread = new Thread(GenMesh);
                m_thread.Start();
            }
        });

        DrawButton("EndScan", () =>
        {
            bool r = RplidarBinding.EndScan();
            Debug.Log("EndScan:" + r);
        });

        DrawButton("StartMotor", () =>
        {
            bool r = RplidarBinding.StartMotor();
            Debug.Log("StartMotor:" + r);
            //Capsule.GetComponent<LidarRotateScript>().check = true;
        });

        DrawButton("EndMotor", () =>
        {
            bool r = RplidarBinding.EndMotor();
            Debug.Log("EndMotor:" + r);
        });


        DrawButton("Release Driver", () =>
        {
            bool r = RplidarBinding.ReleaseDrive();
            Debug.Log("Release Driver:" + r);
        });


        DrawButton("GrabData", () =>
        {
            int count = RplidarBinding.GetData(ref data);

            Debug.Log("GrabData:" + count);
            if (count > 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Debug.Log("d:" + data[i].distant + " " + data[i].quality + " " + data[i].syncBit + " " + data[i].theta);
                }
            }

        });
    }

    void DrawButton(string label, Action callback)
    {
        if (GUILayout.Button(label, GUILayout.Width(200), GUILayout.Height(75)))
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }


    //2D 오브젝트로 만들어서 생성하는것까지 되었음
    //내가 원하는 distance, angle 범위 안에 있는것만 찍어주기
    //
}
