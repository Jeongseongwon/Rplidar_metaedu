using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading;

public class RplidarTest : MonoBehaviour {

    public string port;
    public GameObject Capsule;

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
    private Vector3 Temp_position;
    //

    private void Awake()
    {
        data = new LidarData[720];
    }

    // Use this for initialization
    void Start () {
        distance = 3.0f; //거리는 3.0f로 설정한다.
        angle = new float[DATA_LENGTH]; //데이타 개수만큼 배열 형성
        drawingIndex = 0; //0으로 초기화

        SaveAngleData(); // 게임 시작 시, SaveAngleData를 실행하여 angle배열 변수에 1~360까지의 데이타를 저장.

       
    }
    void GenMesh()
    {
        while (true)
        {
            int datacount = RplidarBinding.GetData(ref data);

            if (datacount == 0)
            {
                Thread.Sleep(20);
            }
            else
            {
                m_datachanged = true;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (Check_onscan)
        {
            //DATA_LENGTH크기만큼 drawingIndex변수를 통해 해당 로직을 수행한다. DATA_LENGTH는 360이라는 값이므로 360번만 로직을 수행하게 됩니다.

            //if (drawingIndex < DATA_LENGTH)
            //{
            //    transform.rotation = Quaternion.Euler(0.0f, angle[drawingIndex], 0.0f); //angle[drawingIndex]에 담겨있는 데이타를 통해 회전을 수행한다.
            //                                                                            //게임 오브젝트를 생성하는 Instantiate함수. 
            //                                                                            //spherePrefab을 생성하고 윗 줄에서 회전을 한 상태의 forward방향 즉, 바라보는 방향으로 * distance만큼 떨어진 위치에 생성을 시킵니다.
            //                                                                            //Quaternion.identity는 기존spherePrefab의 회전 디폴트 값을 받는다는 코드입니다. 즉, 최초에 spherePrefab을 생성할 때 해당 오브젝트를 회전 시킨 상태가 아니라면
            //                                                                            //Quaternion.identity는 0,0,0이 됩니다.

            //    Instantiate(spherePrefab, transform.forward * distance, Quaternion.identity);
            //    drawingIndex++; //drawingIndex값을 1씩 증가시켜 DATA_LENGTH와 비교를 하게 됩니다.
            //}

            //distance mm/angle


            if (m_datachanged)
            {
                for (int i = 0; i < 720; i++)
                {
                    if (data[i].theta > 50 && data[i].theta < 300)
                    {
                            transform.rotation = Quaternion.Euler(0.0f, data[i].theta, 0.0f);
                            //Instantiate(spherePrefab, transform.forward * 0 * 0.01f, Quaternion.identity);

                    }
                    else
                    {
                        if (i % 2 == 0)
                        {

                            transform.rotation = Quaternion.Euler(0.0f, data[i].theta, 0.0f);
                            //Distance
                            if (data[i].distant < 2000)
                            {
                                Instantiate(spherePrefab, transform.forward * data[i].distant * 0.01f, Quaternion.identity);
                                Temp_position = Camera.main.WorldToScreenPoint(transform.forward * data[i].distant * 0.01f);
                                Mouse.current.WarpCursorPosition(new Vector2(Temp_position.x,Temp_position.y));
                            }
                                


                        }
                    }

                }
                m_datachanged = false;
                //Debug.Log("CHECK");
            }
        }
    }

    void OnDestroy()
    {

        RplidarBinding.EndScan();
        RplidarBinding.EndMotor();
        RplidarBinding.OnDisconnect();
        RplidarBinding.ReleaseDrive();

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

            Debug.Log("Connect on " + port +" result:"+ result);
        });

        DrawButton("DisConnect", () =>
        {
            bool r = RplidarBinding.OnDisconnect();
            Debug.Log("Disconnect:"+r);
        });

        DrawButton("StartScan", () =>
        {
            m_onscan = RplidarBinding.StartScan();
            Debug.Log("StartScan:" + m_onscan);
            Check_onscan = true;

            if (m_onscan)
            {
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
            Capsule.GetComponent<LidarRotateScript>().check = true;
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
            if(count > 0)
            {
                for(int i = 0; i < 20; i++)
                {
                    Debug.Log("d:" + data[i].distant + " " + data[i].quality + " " + data[i].syncBit + " " + data[i].theta);
                }
            }

        });
    }

    void DrawButton(string label,Action callback)
    {
        if (GUILayout.Button(label, GUILayout.Width(200), GUILayout.Height(75)))
        {
            if(callback != null)
            {
                callback.Invoke();
            }
        }
    }


    //2D 오브젝트로 만들어서 생성하는것까지 되었음
    //내가 원하는 distance, angle 범위 안에 있는것만 찍어주기
    //
}
