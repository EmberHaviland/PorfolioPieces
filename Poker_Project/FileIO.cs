using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileIO : MonoBehaviour
{
    public string filePath_ = "./testTelemetryFile.txt";
    StreamWriter sw;
    StreamReader sr;

    List<(KeyCode, double)> replayList;
    double replayTimer = 0.0;
    bool initReplay = true;

    public bool reading = false;
    public bool replaying = false;

    public void flipSwitch()
    {
        sw.Close();
        sr = new StreamReader(filePath_);
    }

    void OnDestroy()
    {
        sw.Close();
        if (sr != null)
            sr.Close();
    }

    // Start is called before the first frame update
    void Start()
    {
        // RandomSeed = Random.Range(0, 100);
        // RandomSeed = 1;
        //Random.seed = RandomSeed;
        if (!File.Exists(filePath_))
        {
            // Create a file to write to.
            sw = File.CreateText(filePath_);
        }
        else
        {
            sw = new StreamWriter(filePath_);
        }
        replayList = new List<(KeyCode, double)>();

    }

    // Update is called once per frame
    void Update()
    {
        if (reading == true)
        {
            if (UIManager.Instance.UIList.Actions.Count == 0)
            {
                sw.Close();
                (KeyCode, double) tempPair = ReadLine();
                if (tempPair.Item1 != KeyCode.None && tempPair.Item2 != 0.0)
                {
                    replayList.Add(tempPair);
                }
                else
                {
                    reading = false;
                }
            }
        }

        if (replayList.Count > 0)
        {
            replaying = true;
            if (initReplay)
            {
                replayTimer = (replayList[0].Item2 - 2) > 0 ?
                                replayList[0].Item2 - 2 :
                                replayList[0].Item2;
                initReplay = false;
            }
            replayTimer += Time.unscaledDeltaTime;

            if (replayList[0].Item2 <= replayTimer || initReplay)
            {
                int buttCount = UIManager.Instance.MenuList.Count;
                Vector2 originPoint = new(0.0f, 0.25f);
                switch (replayList[0].Item1)
                {
                    case KeyCode.A:
                        UIManager.Instance.ALogic();
                        break;
                    case KeyCode.M:
                        UIManager.Instance.MLogic();
                        break;
                    case KeyCode.UpArrow:
                        UIManager.Instance.UpLogic(buttCount);
                        break;
                    case KeyCode.DownArrow:
                        UIManager.Instance.DownLogic(buttCount);
                        break;
                    case KeyCode.Alpha1:
                        UIManager.Instance.OneLogic(originPoint);
                        break;
                    case KeyCode.Alpha2:
                        UIManager.Instance.TwoLogic(originPoint);
                        break;
                    case KeyCode.Alpha3:
                        UIManager.Instance.ThreeLogic(originPoint);
                        break;
                    case KeyCode.Alpha4:
                        UIManager.Instance.FourLogic(originPoint);
                        break;
                    case KeyCode.Alpha5:
                        UIManager.Instance.FiveLogic(originPoint);
                        break;
                    case KeyCode.Escape:
                        UIManager.Instance.EscapeLogic();
                        break;
                    case KeyCode.Space:
                        UIManager.Instance.SpaceLogic();
                        break;
                    case KeyCode.Return:
                        UIManager.Instance.EnterLogic();
                        break;
                    default:

                        break;
                }
                replayList.RemoveAt(0);
            }
        }
        else
        {
            replaying = false;
            replayTimer = 0;
        }
    }

    public void TakeString(string inputString)
    {
        sw.WriteLine(inputString);
    }
    public (KeyCode, double) ReadLine()
    {
        KeyCode tempCode = KeyCode.None;
        double tempTime = -1.0;
        for (int ii = 0; ii < 2; ++ii)
        {
            if (sr.EndOfStream)
                return (KeyCode.None, -1.0);
            string tempString = sr.ReadLine();

            switch (tempString)
            {
                case "A":
                    tempCode = KeyCode.A;
                    break;
                case "M":
                    tempCode = KeyCode.M;
                    break;
                case "UpArrow":
                    tempCode = KeyCode.UpArrow;
                    break;
                case "DownArrow":
                    tempCode = KeyCode.DownArrow;
                    break;
                case "1":
                    tempCode = KeyCode.Alpha1;
                    break;
                case "2":
                    tempCode = KeyCode.Alpha2;
                    break;
                case "3":
                    tempCode = KeyCode.Alpha3;
                    break;
                case "4":
                    tempCode = KeyCode.Alpha4;
                    break;
                case "5":
                    tempCode = KeyCode.Alpha5;
                    break;
                case "Escape":
                    tempCode = KeyCode.Escape;
                    break;
                case "Space":
                    tempCode = KeyCode.Space;
                    break;
                case "Enter":
                    tempCode = KeyCode.Return;
                    break;
                default:
                    if (tempString == null)
                        return (KeyCode.None, -1.0);
                    tempTime = System.Convert.ToDouble(tempString);
                    break;
            }
        }
        return (tempCode, tempTime);
    }
}
