using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

public class GenerateObject : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion

    public Transform AirPlane;
    public Transform Transmitter;
    public Transform Receiver;
    public Transform TTextBox;
    public GameObject textPrefab;
    public Transform LineConnectionR;
    public Transform LineConnectionB;
    //public Text textPrefabText;



    // Start is called before the first frame update
    void Start()
    {
        Read_File_Generate_s();
        ConnectToTcpServer();

    }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);

    public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
    {
        SetWindowPos(FindWindow(null, "TimT1"), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    }
#endif

    // Use this for initialization
    void Awake()
    {
        //SetPosition(0, 200);
    }

    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    private int headerlen = 4;
    private int totallen = 0;
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", 8300);
            List<byte> Lbyte = new List<byte>();
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Lbyte.AddRange(bytes);
                        //Array.Copy(bytes, 0, incommingData, 0, length);
                        if(Lbyte.Count >= headerlen)
                        {
                            totallen = Lbyte[1] * 255 + Lbyte[2];
                            Debug.Log("Total Length " + totallen.ToString());
                        }
                        if(Lbyte.Count >= totallen)
                        {
                            Debug.Log("Out of loop server message received as: " + BitConverter.ToString(Lbyte.ToArray()) + "\r\n");
                            //SendMessage(Lbyte.GetRange(0,totallen));
                            Process_UI_Message(Lbyte.GetRange(0, totallen));
                            Debug.Log("Out of loop server message received as: " + BitConverter.ToString(Lbyte.GetRange(0, totallen).ToArray()));
                            Lbyte.RemoveRange(0,1024);
                            //bytes = new byte[1024];
                        }
                        // Convert byte array to string message. 						
                        //string serverMessage = Encoding.ASCII.GetString(incommingData);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    private float[] vec_f_a = new float[28800];
    private bool update_flag = false;
    private void Process_UI_Message(List<byte> x)
    {
        int i = 0;
        int totalfi = 0;
        i = 12;
        int objID = 0;
        int mode = 0;
        float obj_pos_x = 0;
        float obj_pos_y = 0;
        float obj_pos_z = 0;
        totalfi = 0;

        print("REENTER!! \r\n");
        while (i<x.Count)
        {
            Debug.Log(BitConverter.ToDouble(x.GetRange(i-8,8).ToArray(),0));
            if(mode==0)
            {
                obj_pos_x = (float)BitConverter.ToDouble(x.GetRange(i - 8, 8).ToArray(), 0);

                vec_f_a[totalfi] = obj_pos_x;
                mode = 1;
            }
            else if(mode==1)
            {
                obj_pos_y = (float)BitConverter.ToDouble(x.GetRange(i - 8, 8).ToArray(), 0);

                vec_f_a[totalfi] = obj_pos_y;
                mode = 2;
            }
            else if(mode==2)
            {
                obj_pos_z = (float)BitConverter.ToDouble(x.GetRange(i - 8, 8).ToArray(), 0);
                vec_f_a[totalfi] = obj_pos_z;
                //Dic_int_vec3_pos[objID] = new Vector3(obj_pos_x, obj_pos_y, obj_pos_z);
                //Vec_pos_l.Add(new Vector3(obj_pos_x, obj_pos_y, obj_pos_z));
                //print(Dic_int_vec3_pos[0].ToString());
                objID++;
                Thread.Sleep(10);
                mode = 0;
            }
            i += 8;
            totalfi++;
        }
        while(update_flag)
        {

        }
        print("OUT with obj ID: " + objID.ToString());
        update_flag = true;
    }

    private void SendMessage(List<byte> x)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                //string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                //byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(x.ToArray(), 0, x.Count);
                Debug.Log(BitConverter.ToString(x.ToArray()));
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    public float Scale_factor;
    public int objectcount = 0;
    void Read_File_Generate_s()
    {
        //string path = @"C:/Users/timli/TimT1/Assets/Resource/User_Input.dsc";
        string path = @"C:\Users\timli\AppData\Local\Packages\106b18ec-5180-4642-8a0e-198353957681_kbtfgvzxh186t\LocalState\Simulator File\Scenario File\DRBE_New.dsc";
        StreamReader reader = new StreamReader(path);
        string result = reader.ReadToEnd();
        //Read the text from directly from the test.txt file
        reader.Close();
        objectcount = 0;
        List<string> Content = new List<string>();

        //Debug.Log(result.Length);
        Dic_int_transform = new Dictionary<int, Transform>();
        int i = 0;
        i = 0;
        string temp = "";
        int Casei = 0;
        while (i < result.Length)
        {
            if (result[i] == '{')
            {
                temp = "";
            }
            else if (result[i] == '}')
            {
                Content.Add(temp);
                Casei++;
            }
            else
            {
                temp = temp + result[i].ToString();
            }

            i++;
        }
        List<string> templ = new List<string>();
        i = 0;
        while (i < Content.Count)
        {
            if (Content[i] == "EO")
            {
                if(templ[4]=="True")
                {
                    Dic_int_transform[objectcount] = Gen_transmitter_s(templ);
                    Dic_transform_int[Dic_int_transform[objectcount]] = objectcount;
                }
                else
                {
                    Dic_int_transform[objectcount] = Gen_reflector_s(templ);
                    Dic_transform_int[Dic_int_transform[objectcount]] = objectcount;

                }
                templ = new List<string>();
                objectcount++;
            }
            else
            {
                templ.Add(Content[i]);
            }
            i++;
        }
    }
    void Read_File_Generate()
    {
        //string path = @"C:/Users/timli/TimT1/Assets/Resource/User_Input.dsc";
        string path = @"C:/Users/timli/TimT1/Assets/Resource/User_Input.dsc";
        StreamReader reader = new StreamReader(path);
        string result = reader.ReadToEnd();
        //Read the text from directly from the test.txt file
        reader.Close();

        List<string> Content = new List<string>();

        //Debug.Log(result.Length);

        int i = 0;
        i = 0;
        string temp = "";
        int Casei = 0;
        while (i < result.Length)
        {
            if (result[i] == '{')
            {
                temp = "";
            }
            else if (result[i] == '}')
            {
                Content.Add(temp);
                Casei++;
            }
            else
            {
                temp = temp + result[i].ToString();
            }

            i++;
        }
        List<string> templ = new List<string>();
        i = 0;
        while (i < Content.Count)
        {
            if (Content[i] == "OT")
            {
                Gen_transmitter(templ);
                templ = new List<string>();
            }
            else if (Content[i] == "OP")
            {
                Gen_reflector(templ);
                templ = new List<string>();
            }
            else if (Content[i] == "OR")
            {
                Gen_receiver(templ);
                templ = new List<string>();
            }
            else
            {
                templ.Add(Content[i]);
            }
            i++;
        }


    }
    public List<Transform> TL = new List<Transform>();
    public List<Transform> PL = new List<Transform>();
    public List<Transform> RL = new List<Transform>();
    public List<Transform> LL = new List<Transform>();
    public List<GameObject> TextL = new List<GameObject>();

    public Dictionary<int, Transform> Dic_int_transform = new Dictionary<int, Transform>();
    public Dictionary<Transform, int> Dic_transform_int = new Dictionary<Transform, int>();
    Transform Gen_transmitter_s(List<string> x)
    {
        TL.Add(Instantiate(Transmitter, new Vector3((float)S_D(x[6]), (float)S_D(x[8]), (float)S_D(x[7])), Quaternion.Euler((float)S_D(x[21]), (float)S_D(x[23]), (float)S_D(x[22]))));
        TL[TL.Count - 1].localScale = new Vector3(0.04f, 0.04f, 0.04f);
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "ID: " + x[0];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position X: " + x[6];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Y: " + x[7];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Z: " + x[8];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Pitch: " + x[21];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Roll: " + x[22];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Yaw: " + x[23];
        return TL[TL.Count - 1];
    }

    Transform Gen_reflector_s(List<string> x)
    {
        PL.Add(Instantiate(AirPlane, new Vector3((float)S_D(x[6]), (float)S_D(x[8]), (float)S_D(x[7])), Quaternion.Euler((float)S_D(x[21]), (float)S_D(x[23]), (float)S_D(x[22]))));
        PL[PL.Count - 1].localScale = new Vector3(3f, 3f, 3f);
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "ID: " + x[0];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position X: " + x[6];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Y: " + x[7];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Z: " + x[8];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Pitch: " + x[21];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Roll: " + x[22];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Yaw: " + x[23];
        return PL[PL.Count-1];
        //if(PL.Count>=2)
        //{
        //    LL.Add(Instantiate(LineConnectionR));
        //    LL[LL.Count - 1].gameObject.AddComponent<LineRenderer>();
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(0, PL[PL.Count - 1].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(1, TL[0].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetWidth(1F, 1F);
        //}


        //if (PL.Count <= 4)
        //{
        //    LL.Add(Instantiate(LineConnectionB));
        //    LL[LL.Count - 1].gameObject.AddComponent<LineRenderer>();
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(0, PL[PL.Count - 1].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(1, TL[2].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetWidth(1F, 1F);
        //}

    }
    void Gen_transmitter(List<string> x)
    {
        TL.Add(Instantiate(Transmitter, new Vector3((float)S_D(x[2]), (float)S_D(x[3]), (float)S_D(x[4])), Quaternion.Euler((float)S_D(x[17]), (float)S_D(x[18]), (float)S_D(x[19]))));
        TL[TL.Count - 1].localScale = new Vector3(0.04f,0.04f,0.04f);
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "ID: " + x[0];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position X: " + x[2];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Y: " + x[3];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Z: " + x[4];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Pitch: " + x[17];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Roll: " + x[18];
        TL[TL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Yaw: " + x[19];

    }

    void Gen_reflector(List<string> x)
    {
        PL.Add(Instantiate(AirPlane, new Vector3((float)S_D(x[2]), (float)S_D(x[3]), (float)S_D(x[4])), Quaternion.Euler((float)S_D(x[17]), (float)S_D(x[18]), (float)S_D(x[19]))));
        PL[PL.Count - 1].localScale = new Vector3(3f, 3f, 3f);
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "ID: " + x[0];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position X: " + x[2];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Y: " + x[3];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Z: " + x[4];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Pitch: " + x[17];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Roll: " + x[18];
        PL[PL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Yaw: " + x[19];
        //if(PL.Count>=2)
        //{
        //    LL.Add(Instantiate(LineConnectionR));
        //    LL[LL.Count - 1].gameObject.AddComponent<LineRenderer>();
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(0, PL[PL.Count - 1].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(1, TL[0].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetWidth(1F, 1F);
        //}


        //if (PL.Count <= 4)
        //{
        //    LL.Add(Instantiate(LineConnectionB));
        //    LL[LL.Count - 1].gameObject.AddComponent<LineRenderer>();
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(0, PL[PL.Count - 1].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetPosition(1, TL[2].position);
        //    LL[LL.Count - 1].GetComponentInChildren<LineRenderer>().SetWidth(1F, 1F);
        //}

    }

    void Gen_receiver(List<string> x)
    {
        RL.Add(Instantiate(Receiver, new Vector3((float)S_D(x[2]), (float)S_D(x[3]), (float)S_D(x[4])), Quaternion.Euler((float)S_D(x[17]), (float)S_D(x[18]), (float)S_D(x[19]))));
        RL[RL.Count - 1].localScale = new Vector3(0.04f, 0.04f, 0.04f);
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "ID: " + x[0];
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position X: " + x[2];
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Y: " + x[3];
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Position Z: " + x[4];
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Pitch: " + x[17];
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Roll: " + x[18];
        RL[RL.Count - 1].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text += "\r\n Yaw: " + x[19];


    }
    // Update is called once per frame
    public LayerMask clickablelayer;
    public Material Red;
    public Material Blue;

    private List<byte> tosend = new List<byte>();
    void Update()
    {
        int i = 0;
        int iii = 0;
        int object_no = 0;

        if (update_flag)
        {
            update_flag = false;
            print("Enteredllll");
            i = 0;
            while(i<objectcount)
            {
                float ppx = vec_f_a[i * 3 + 0];
                float ppy = vec_f_a[i * 3 + 2];
                float ppz = vec_f_a[i * 3 + 1];
                Dic_int_transform[i].position = new Vector3(ppx,ppy,ppz);
                print(Dic_int_transform[i].position.ToString());
                //Dic_int_transform[i].position = new Vector3(0,0,0);
                i++;
            }
            //vec_f_a = new float[1800];
            print("OUT");

        }


        if (Input.GetMouseButtonDown(0))
        {
            GameObject temp;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f, clickablelayer))
            {
                tosend = new List<byte>();
                if (hit.transform != null)
                {
                    Decolor_All();
                    hit.transform.GetComponentInChildren<Renderer>().material = Red;

                    i = 0;
                    while(i<hit.transform.childCount)
                    {
                        temp = hit.transform.GetChild(i).gameObject;
                        temp.GetComponentInChildren<Renderer>().material = Red;
                        object_no = Dic_transform_int[hit.transform];
                        tosend.Add(0x22);
                        tosend.Add((byte)(object_no / 255));
                        tosend.Add((byte)(object_no % 255));

                        iii = 0;
                        while (iii < hit.transform.GetChild(i).childCount-1)
                        {
                            hit.transform.GetChild(i).GetChild(iii).gameObject.GetComponentInChildren<Renderer>().material = Red;
                            iii++;
                        }
                        i++;
                    }
                    PrintName(hit.transform.gameObject);
                    SendMessage(tosend);
                }
            }
        }
    }
    private void Decolor_All()
    {
        int i = 0;
        int ii = 0;
        int iii = 0;
        i = 0;
        while(i<PL.Count)
        {
            PL[i].GetComponentInChildren<Renderer>().material = Blue;
            ii = 0;
            while(ii< PL[i].childCount)
            {
                PL[i].GetChild(ii).gameObject.GetComponentInChildren<Renderer>().material = Blue;
                ii++;
            }
            i++;
        }

        i = 0;
        while (i < TL.Count)
        {
            TL[i].GetComponentInChildren<Renderer>().material = Blue;
            ii = 0;
            
            while (ii < TL[i].childCount)
            {
                
                TL[i].GetChild(ii).gameObject.GetComponentInChildren<Renderer>().material = Blue;
                print(TL[i].GetChild(ii).childCount.ToString() + TL[i].GetChild(ii).name);
                iii = 0;
                while (iii < TL[i].GetChild(ii).childCount-1)
                {
                    TL[i].GetChild(ii).GetChild(iii).gameObject.GetComponentInChildren<Renderer>().material = Blue;
                    iii++;
                }

                ii++;
            }
            i++;
        }
    }
    private void PrintName(GameObject x)
    {
        print(x.name);
    }

    

    #region others

    private double S_D(string x)
    {
        double result = 0;
        double sign = 1;
        string before = "";
        string after = "";
        int i = 0;
        int tenpower = 1;
        int len = x.Length;
        int beforeflag = 0;
        if (len >= 1)
        {
            if (x[0] == '-')
            {
                sign = -1;
            }
        }
        while (i < len)
        {
            if (beforeflag == 0 && x[i] != '.')
            {
                before += x[i].ToString();
            }
            else if (beforeflag == 1 && x[i] != '.')
            {
                after += x[i].ToString();
                tenpower = tenpower * 10;
            }
            else if (x[i] == '.')
            {
                beforeflag = 1;
            }
            else if (x[i] == '-')
            {
                //sign = -1;
            }
            else
            {
                //sign = -1;
            }
            i++;
        }
        result = (double)S_I(before) + ((double)S_I(after)) / tenpower;
        result = result * sign;
        return result;
    }
    private int S_I(string x)
    {
        int result = 0;
        int index = 0;
        int rindex = 0;
        index = x.Length;
        while (index > 0)
        {
            if (C_I(x[rindex]) != -1)
            {
                result = result * 10 + C_I(x[rindex]);
            }
            else
            {

            }

            rindex++;
            index--;
        }
        return result;
    }
    private int C_I(char x)
    {
        int reint = 0;
        if (x == '0')
        {
            reint = 0;
        }
        else if (x == '1')
        {
            reint = 1;
        }
        else if (x == '2')
        {
            reint = 2;

        }
        else if (x == '3')
        {
            reint = 3;
        }
        else if (x == '4')
        {
            reint = 4;
        }
        else if (x == '5')
        {
            reint = 5;
        }
        else if (x == '6')
        {
            reint = 6;
        }
        else if (x == '7')
        {
            reint = 7;
        }
        else if (x == '8')
        {
            reint = 8;
        }
        else if (x == '9')
        {
            reint = 9;
        }
        else
        {
            reint = -1;
        }
        return reint;
    }
    private int S_H(string x)
    {
        int result = 0;
        int index = 0;
        int rindex = 0;
        index = x.Length;
        while (index > 0)
        {
            if (C_H(x[rindex]) != -1)
            {
                result = result * 16 + C_H(x[rindex]);
            }
            else
            {

            }

            rindex++;
            index--;
        }
        return result;
    }
    private int C_H(char x)
    {
        int reint = 0;
        if (x == '0')
        {
            reint = 0;
        }
        else if (x == '1')
        {
            reint = 1;
        }
        else if (x == '2')
        {
            reint = 2;

        }
        else if (x == '3')
        {
            reint = 3;
        }
        else if (x == '4')
        {
            reint = 4;
        }
        else if (x == '5')
        {
            reint = 5;
        }
        else if (x == '6')
        {
            reint = 6;
        }
        else if (x == '7')
        {
            reint = 7;
        }
        else if (x == '8')
        {
            reint = 8;
        }
        else if (x == '9')
        {
            reint = 9;
        }
        else if (x == 'a' || x == 'A')
        {
            reint = 10;
        }
        else if (x == 'b' || x == 'B')
        {
            reint = 11;
        }
        else if (x == 'c' || x == 'C')
        {
            reint = 12;
        }
        else if (x == 'd' || x == 'D')
        {
            reint = 13;
        }
        else if (x == 'e' || x == 'E')
        {
            reint = 14;
        }
        else if (x == 'f' || x == 'F')
        {
            reint = 15;
        }
        else
        {
            reint = -1;
        }
        return reint;
    }
    #endregion
}
