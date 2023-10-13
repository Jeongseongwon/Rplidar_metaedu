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


    //=====0714
    public GameObject spherePrefab; //spherePrefab을 받을 변수 
    private float distance; //distance정보를 저장할 변수
    private float[] angle; //angle정보를 저장할 배열 변수
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

    private double Data_angle=0;
    private float Data_distance;
    //=====


    //====1012
    public float lidarSpinSpeed; //라이다가 도는 속도를 설정할 변수.
    public bool check = false;
    double number = 0f;
    //


    private void Awake()
    {
        data = new LidarData[720];
    }

    // Use this for initialization
    void Start()
    {
        distance = 3.0f; //거리는 3.0f로 설정한다.
        angle = new float[DATA_LENGTH]; //데이타 개수만큼 배열 형성
        drawingIndex = 0; //0으로 초기화

        SaveAngleData(); // 게임 시작 시, SaveAngleData를 실행하여 angle배열 변수에 1~360까지의 데이타를 저장.

        lidarSpinSpeed = 1300.0f;
    }

    void Update()
    {
        if (check)
        {
            if (Data_angle != 0)
            {
                Debug.Log("1. data_angle" + Data_angle);
                transform.rotation = Quaternion.Euler(0.0f, (float)Data_angle, 0.0f);
            }
            //transform.Rotate(-Vector3.forward * Time.deltaTime * lidarSpinSpeed);
        }
    }

    //0824 위의 coroutine 대체로 주석처리
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_datachanged)
        {

            for (int i = 0; i < 720; i++)
            {
                Data_angle = data[i].theta;
                Data_distance = data[i].distant;

                if (Data_angle != 0)
                {
                    number = Math.Round(Data_angle, 2);
                    Debug.Log("1. data_angle" + number);
                    transform.Rotate(-Vector3.forward * (float)number * Time.deltaTime);
                    //transform.Rotate(-Vector3.forward * Time.deltaTime * lidarSpinSpeed);

                    //transform.rotation = Quaternion.Euler(0.0f, Data_angle, 0.0f);
                    //transform.rotation = Quaternion.Euler(0.0f, 0.0f, Data_angle);

                    if (i % 8 == 0)
                    {
                        //transform.rotation = Quaternion.Euler((float)number, 0.0f, 0.0f);
                        //transform.Rotate(-Vector3.forward * Time.deltaTime * lidarSpinSpeed);
                    }
                }

                    //transform.rotation = Quaternion.Euler(0.0f, data[i].theta, 0.0f);

                    if (Data_angle > 50 && Data_angle < 300)
                {
                    //transform.rotation = Quaternion.Euler(0.0f, data[i].theta, 0.0f);
                    //Instantiate(spherePrefab, transform.forward * 0 * 0.01f, Quaternion.identity);
                }
                else
                {
                    if (i % 8 == 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, data[i].theta, 0.0f);
                        //Distance
                        if (Data_distance > distance_min && Data_distance < distance_max)
                        {
                            //transform.rotation = Quaternion.Euler(0.0f, Data_angle, 0.0f);

                            //Debug.Log("2. data,a" + data[i].theta + " // data,d" + data[i].distant);
                            //Capsule.transform.rotation = Quaternion.Euler(0.0f, data[i].distant, 0.0f);
                            //24로 고정댐

                            //rotation이 실제로 회전하지는 않는 것 같음
                            //capsule이 회전하면 오브젝트 이동은 정상적으로 작동하는 것 처럼 보임

                            //data가 어느순간 0이 되버림..

                            //instantiate로 만든 오브젝트를 리스트에 넣고
                            //데이터는 update문보다 빨리 들어오는데 update를 돌면서 데이터가 이미 바뀌어버림


                            /*1012 테스트용으로 주석처리
                            
                            GameObject temp_pos = Instantiate(spherePrefab, transform.forward * Data_distance * 0.01f, Quaternion.identity);
                            Temp_position = Camera.main.WorldToScreenPoint(temp_pos.transform.position);

                            Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x, Temp_position.y));
                            */



                        }

                    }
                }
            }
            m_datachanged = false;
            //Debug.Log("CHECK");
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
    public void SaveAngleData()
    {
        for (int i = 0; i < DATA_LENGTH; i++)
        {
            angle[i] = i + 1; //i+1로 설정하여 1부터 360까지의 데이타를 저장한다.
        }
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
