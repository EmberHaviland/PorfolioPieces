using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Action;
using static Player;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance{ get; private set; }
    public ActionList UIList;

    GameObject CardPrefab;
    GameObject ChipPrefab;
    GameObject MenuPrefab;
    public TMP_Text StatusText;

    public List<GameObject> Deck;
    public List<GameObject> Chips;
    public List<GameObject> Pot;
    public List<Player> Players;
    public List<GameObject> MenuList;
    public List<GameObject> SubMenuList;
    Color defaultCol;
    Color selectCol;

    (Random.State, bool) ranState;

    public FileIO fileSys;

    private void Awake() => Instance = this;

    public EaseType Easing = EaseType.Linear;
    public float Drift = 0.25f;
    public float Twist = 179f;

    bool JustStarted = true;
    bool RoundInProgress = false;
    bool RoundOver = false;
    bool newRoundStarting = false;
    bool AutoMode = false;
    bool MenuAuto = false;
    public float SpeedMultiple = 1;
    int RoundCurrPlayer = 1;
    bool MenuOpen = false;
    int currButtonIndex = 0;
    int currSubButtonIndex = 0;

    float statusFadeTimer = 0.0f;

    void Start()
    {
        UIList = Instantiate(Resources.Load<ActionList>("ActionList"), new Vector3(0, 0, 0), Quaternion.identity);
        CardPrefab = Resources.Load<GameObject>("Card");
        ChipPrefab = Resources.Load<GameObject>("Chip");
        MenuPrefab = Resources.Load<GameObject>("MenuItem");
        defaultCol = new Color();
        selectCol = new Color();
        StatusText.alpha = 0;
        StatusText.text = "Testing Test";

        ranState.Item2 = false;
        

        Deck = new List<GameObject>();
        Chips = new List<GameObject>();
        Pot = new List<GameObject>();
        Players = new List<Player>();
        MenuList = new List<GameObject>();
        SubMenuList = new List<GameObject>();

        for (int ii = 0; ii < 52; ii++)
        {
            GameObject createdCard = CreateCard(ii);
            Vector3 tempVec = createdCard.transform.localEulerAngles;
            tempVec.z += Random.Range(-Twist, Twist);
            createdCard.transform.localEulerAngles = tempVec;
            UIList.MoveIt(createdCard, new Vector3(-9.0f, -3.5f, createdCard.transform.localPosition.z), 1.0f, Easing, ii + 1);
            UIList.FlipIt(createdCard, 0.25f, 0, Easing, ii + 1);
        }

        for (int ii = 0; ii < 100; ii++)
        {
            GameObject createdChip = CreateChip(ii);
            UIList.MoveIt(createdChip, new Vector3(9.0f, -3.5f, createdChip.transform.localPosition.z), 1.0f, ii + 1);
        }

        CreateMenu();
        currButtonIndex = 0;
    }

    void Update()
    {
        if (JustStarted)
        {
            if (UIList.Actions.Count == 0)
            {
                newRoundStarting = false;
                if (ranState.Item2)
                {
                    Random.state = ranState.Item1;
                }
                else
                {
                    ranState.Item1 = Random.state;
                    ranState.Item2 = true;
                }
                CreatePlacements(new Vector2(0.0f, 0.25f), 3.5f, 2);
                JustStarted = false;
                defaultCol = MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color;
                defaultCol.a = 1.0f;
                selectCol = new Color(0.25f, 0.25f, 0.25f, 1);
            }
        }

        if (statusFadeTimer > 0.0f)
        {
            StatusText.alpha = 1;
            statusFadeTimer -= Time.deltaTime;
        }
        else
        {
            StatusText.alpha = 0;
        }

        if (fileSys.replaying == false)
        {
            Vector2 originPoint = new(0.0f, 0.5f);
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!MenuOpen)
                {
                    fileSys.flipSwitch();
                    ClearPlacements();
                    JustStarted = true;
                    fileSys.reading = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && UIList.Actions.Count == 0)
            {
                OneLogic(originPoint);
                fileSys.TakeString("1\n" + Time.realtimeSinceStartupAsDouble);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && UIList.Actions.Count == 0)
            {
                TwoLogic(originPoint);
                fileSys.TakeString("2\n" + Time.realtimeSinceStartupAsDouble);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && UIList.Actions.Count == 0)
            {
                ThreeLogic(originPoint);
                fileSys.TakeString("3\n" + Time.realtimeSinceStartupAsDouble);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && UIList.Actions.Count == 0)
            {
                FourLogic(originPoint);
                fileSys.TakeString("4\n" + Time.realtimeSinceStartupAsDouble);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) && UIList.Actions.Count == 0)
            {
                FiveLogic(originPoint);
                fileSys.TakeString("5\n" + Time.realtimeSinceStartupAsDouble);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapeLogic();
                fileSys.TakeString("Escape\n" + Time.realtimeSinceStartupAsDouble);
            }

            if ((Input.GetKeyDown(KeyCode.Space) || AutoMode == true) && !RoundInProgress && !newRoundStarting && !RoundOver)
            {
                SpaceLogic();
                if (!AutoMode)
                    fileSys.TakeString("Space\n" + Time.realtimeSinceStartupAsDouble); 
            }

            if (MenuOpen == true && Input.GetKeyDown(KeyCode.Return))
            {
                EnterLogic();
                fileSys.TakeString("Enter\n" + Time.realtimeSinceStartupAsDouble);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (!MenuOpen)
                    ALogic();
                fileSys.TakeString("A\n" + Time.realtimeSinceStartupAsDouble);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                MLogic();
                fileSys.TakeString("M\n" + Time.realtimeSinceStartupAsDouble);
            }

            int buttCount = UIManager.Instance.MenuList.Count;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (MenuOpen)
                {
                    UpLogic(buttCount);
                }
                UIManager.Instance.fileSys.TakeString("UpArrow\n" + Time.realtimeSinceStartupAsDouble);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (MenuOpen)
                {
                    DownLogic(buttCount);
                    }
                UIManager.Instance.fileSys.TakeString("DownArrow\n" + Time.realtimeSinceStartupAsDouble);
            }
        }

        if (MenuAuto == true)
        {
            MenuAutoLogic();
        }

        if (RoundInProgress == true)
        {
            RoundLogic();
        }
        if (RoundOver == true)
        {
            if (UIList.Actions.Count == 0)
            {
                foreach (Player currPlayer in Players)
                {
                    if (currPlayer.PlayerNum_ != 1)
                    {
                        foreach (GameObject currCard in currPlayer.PlayerHand_)
                        {
                            UIList.FlipIt(currCard, 0.5f, 0.0f, Easing, 1);
                        }
                    }
                    currPlayer.ClearHand();
                }
                RoundInProgress = false;
                RoundOver = false;
                RoundCurrPlayer = 1;
                newRoundStarting = true;
            }
        }

        if (newRoundStarting == true)
        {
            if (UIList.Actions.Count == 0)
            {
                foreach (Player currPlayer in Players)
                {
                    currPlayer.isFolded = false;
                    currPlayer.DealHand(Random.Range(2, 8), 0.75f);
                }
                RoundCurrPlayer = 1;
                newRoundStarting = false;
            }
        }
    }

    public void OneLogic(Vector2 originPoint)
    {
        newRoundStarting = false;
        CreatePlacements(originPoint, 3.75f, 2);
    }

    public void TwoLogic(Vector2 originPoint)
    {
        newRoundStarting = false;
        CreatePlacements(originPoint, 3.75f, 3);
    }
    public void ThreeLogic(Vector2 originPoint)
    {
        newRoundStarting = false;
        CreatePlacements(originPoint, 3.75f, 4);
    }
    public void FourLogic(Vector2 originPoint)
    {
        newRoundStarting = false;
        CreatePlacements(originPoint, 3.75f, 5);
    }
    public void FiveLogic(Vector2 originPoint)
    {
        newRoundStarting = false;
        CreatePlacements(originPoint, 3.75f, 6);
    }
    private enum subState
    {
        MainMenu = 0,
        Sub1 = 1,
        Sub2 = 2,
        Sub3 = 3
    }

    subState currSubState = subState.MainMenu;


    public void DownLogic(int buttCount)
    {
        switch (currSubState)
        {
            case subState.MainMenu:
                UIList.ScaleIt(MenuList[currButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(MenuList[currButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currButtonIndex = ((currButtonIndex + buttCount) + 1) % buttCount;
                MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(MenuList[currButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(MenuList[currButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
            case subState.Sub1:
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currSubButtonIndex = ((currSubButtonIndex + SubMenuList.Count) + 1) % SubMenuList.Count;
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
            case subState.Sub2:
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currSubButtonIndex = ((currSubButtonIndex + SubMenuList.Count) + 1) % SubMenuList.Count;
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
            case subState.Sub3:
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currSubButtonIndex = ((currSubButtonIndex + SubMenuList.Count) + 1) % SubMenuList.Count;
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
        }
    }
    public void UpLogic(int buttCount)
    {
        switch (currSubState)
        {
            case subState.MainMenu:
                UIList.ScaleIt(MenuList[currButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(MenuList[currButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currButtonIndex = ((currButtonIndex + buttCount) - 1) % buttCount;
                MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(MenuList[currButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(MenuList[currButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
            case subState.Sub1:
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currSubButtonIndex = ((currSubButtonIndex + SubMenuList.Count) - 1) % SubMenuList.Count;
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
            case subState.Sub2:
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currSubButtonIndex = ((currSubButtonIndex + SubMenuList.Count) - 1) % SubMenuList.Count;
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
            case subState.Sub3:
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                currSubButtonIndex = ((currSubButtonIndex + SubMenuList.Count) - 1) % SubMenuList.Count;
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1, 1.10f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 5, 0.20f, EaseType.Linear, 0);
                break;
        }
    }
    public void EnterLogic()
    {
        switch (currSubState)
        {
            case subState.MainMenu:
                switch (currButtonIndex)
                {
                    case 0:
                        MenuOpen = false;
                        UIList.ScaleIt(MenuList[currButtonIndex], 1.10f, 1f, 0.20f);
                        UIList.RotateIt(MenuList[currButtonIndex], 0, 0.20f, EaseType.Linear, 0);
                        MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                        for (int ii = 0; ii < MenuList.Count; ++ii)
                        {
                            UIList.FadeOut(MenuList[ii]);
                        }
                        break;
                    case 1:
                        currSubButtonIndex = 0;
                        for (int ii = 0; ii < MenuList.Count; ++ii)
                        {
                            MenuList[ii].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                            Vector3 tempVec1 = MenuList[ii].transform.localPosition;
                            tempVec1.x = -2.0f;
                            UIList.MoveIt(MenuList[ii], tempVec1, 0.5f, 0);
                            UIList.ScaleIt(MenuList[ii], 1, 1.10f, 0.20f);
                            UIList.RotateIt(MenuList[ii], 5, 0.20f, EaseType.Linear, 0);
                        }

                        for (int ii = 0; ii < SubMenuList.Count; ++ii)
                        {
                            Vector3 tempVec2 = SubMenuList[ii].transform.localPosition;
                            tempVec2.x = 2.0f;
                            UIList.MoveIt(SubMenuList[ii], tempVec2, 0.5f, 0);
                            UIList.FadeIn(SubMenuList[ii], 0.5f);
                        }

                        SubMenuList[0].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                        Vector3 tempVec3 = SubMenuList[0].transform.localPosition;
                        tempVec3.x = 2.0f;
                        UIList.MoveIt(SubMenuList[0], tempVec3, 0.5f, 0);
                        UIList.ScaleIt(SubMenuList[0], 1, 1.10f, 0.20f);
                        UIList.RotateIt(SubMenuList[0], 5, 0.20f, EaseType.Linear, 0);

                        SubMenuList[0].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Quality";
                        SubMenuList[1].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Shading";
                        SubMenuList[2].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Textures";

                        currSubState = subState.Sub1;
                        break;
                    case 2:
                        currSubButtonIndex = 0;
                        for (int ii = 0; ii < MenuList.Count; ++ii)
                        {
                            MenuList[ii].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                            Vector3 tempVec1 = MenuList[ii].transform.localPosition;
                            tempVec1.x = -2.0f;
                            UIList.MoveIt(MenuList[ii], tempVec1, 0.5f, 0);
                            UIList.ScaleIt(MenuList[ii], 1, 1.10f, 0.20f);
                            UIList.RotateIt(MenuList[ii], 5, 0.20f, EaseType.Linear, 0);
                        }

                        for (int ii = 0; ii < SubMenuList.Count; ++ii)
                        {
                            Vector3 tempVec2 = SubMenuList[ii].transform.localPosition;
                            tempVec2.x = 2.0f;
                            UIList.MoveIt(SubMenuList[ii], tempVec2, 0.5f, 0);
                            UIList.FadeIn(SubMenuList[ii], 0.5f);
                        }

                        SubMenuList[0].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                        Vector3 tempVec4 = SubMenuList[0].transform.localPosition;
                        tempVec4.x = 2.0f;
                        UIList.MoveIt(SubMenuList[0], tempVec4, 0.5f, 0);
                        UIList.ScaleIt(SubMenuList[0], 1, 1.10f, 0.20f);
                        UIList.RotateIt(SubMenuList[0], 5, 0.20f, EaseType.Linear, 0);

                        SubMenuList[0].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Monitor";
                        SubMenuList[1].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Subtitles";
                        SubMenuList[2].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Resolution";

                        currSubState = subState.Sub2;
                        break;
                    case 3:
                        currSubButtonIndex = 0;
                        for (int ii = 0; ii < MenuList.Count; ++ii)
                        {
                            MenuList[ii].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                            Vector3 tempVec1 = MenuList[ii].transform.localPosition;
                            tempVec1.x = -2.0f;
                            UIList.MoveIt(MenuList[ii], tempVec1, 0.5f, 0);
                            UIList.ScaleIt(MenuList[ii], 1, 1.10f, 0.20f);
                            UIList.RotateIt(MenuList[ii], 5, 0.20f, EaseType.Linear, 0);
                        }

                        for (int ii = 0; ii < SubMenuList.Count; ++ii)
                        {
                            Vector3 tempVec2 = SubMenuList[ii].transform.localPosition;
                            tempVec2.x = 2.0f;
                            UIList.MoveIt(SubMenuList[ii], tempVec2, 0.5f, 0);
                            UIList.FadeIn(SubMenuList[ii], 0.5f);
                        }

                        SubMenuList[0].GetComponentInChildren<SpriteRenderer>().color = selectCol;
                        Vector3 tempVec5 = SubMenuList[0].transform.localPosition;
                        tempVec5.x = 2.0f;
                        UIList.MoveIt(SubMenuList[0], tempVec5, 0.5f, 0);
                        UIList.ScaleIt(SubMenuList[0], 1, 1.10f, 0.20f);
                        UIList.RotateIt(SubMenuList[0], 5, 0.20f, EaseType.Linear, 0);

                        SubMenuList[0].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Master Volume";
                        SubMenuList[1].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Music Volume";
                        SubMenuList[2].transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "SFX Volume";

                        currSubState = subState.Sub3;
                        break;
                    case 4:
                        Application.Quit();
                        break;
                    default:
                        break;
                }
                break;
            case subState.Sub1:
                switch (currSubButtonIndex)
                {
                    case 0:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Quality settings have been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                    case 1:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Shading settings have been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                    case 2:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Texture settings have been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                }
                break;
            case subState.Sub2:
                switch (currSubButtonIndex)
                {
                    case 0:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Monitor has been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                    case 1:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Subtitles have been enabled.";
                            statusFadeTimer = 1;
                        }
                        break;
                    case 2:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Resolution has been set.";
                            statusFadeTimer = 1;
                        }
                        break;
                }
                break;
            case subState.Sub3:
                switch (currSubButtonIndex)
                {
                    case 0:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Master volume has been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                    case 1:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "Music volume has been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                    case 2:
                        if (statusFadeTimer <= 0.0f)
                        {
                            StatusText.text = "SFX volume has been changed.";
                            statusFadeTimer = 1;
                        }
                        break;
                }
                break;
        }


    }


    public void ALogic()
    {
        if (MenuOpen || MenuAuto)
            return;
        if (AutoMode == false)
        {
            AutoMode = true;
        }
        else if (AutoMode == true && SpeedMultiple == 1.0f && !fileSys.replaying)
        {
            SpeedMultiple = 2.0f;
            Time.timeScale *= SpeedMultiple;
        }
        else if (AutoMode == true && SpeedMultiple == 2.0f && !fileSys.replaying)
        {
            Time.timeScale /= SpeedMultiple;
            SpeedMultiple = 5.0f;
            Time.timeScale *= SpeedMultiple;
        }
        else if (AutoMode == true && (SpeedMultiple == 5.0f || !fileSys.replaying))
        {
            AutoMode = false;
            Time.timeScale /= SpeedMultiple;
            SpeedMultiple = 1.0f;
        }

        if (fileSys.replaying)
        {
            Time.timeScale = 1.0f;
            SpeedMultiple = 1.0f;
        }


        if (fileSys.replaying == true && !RoundInProgress && !newRoundStarting && !RoundOver)
        {
            SpaceLogic();
        }
    }

    public void MLogic()
    {
        if (MenuAuto == true)
        {
            flagForComplete = true;
        }
        else
        {
            if (!MenuOpen)
            {
                MenuAuto = true;
            }
        }
    }

    private int menuModeInt = 0;
    private bool menuStepping = false;
    private float ActionTimer = 0.5f;
    private bool flagForComplete = false;
    public void MenuAutoLogic()
    {
        if (UIList.Actions.Count > 0 || RoundInProgress || RoundOver || newRoundStarting)
        {
            return;
        }

        if (menuStepping)
        {
            if (ActionTimer <= 0.0f)
            {
                
                int buttCount = MenuList.Count;
                switch (menuModeInt)
                {
                    case 0: // Open menu
                        EscapeLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 1: // Down to second option
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 2: // Enter to sub1
                        EnterLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 3:  // Press sub1 buttons
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 4:
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 5:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 6:
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 7:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 8:
                        EscapeLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 9: // Down to sub2
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 10: // Enter to sub2
                        EnterLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 11: // Press sub2 buttons
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 12:
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 13:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 14:
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 15:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 16:
                        EscapeLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 17: // Down to sub3
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 18: // Enter to sub3
                        EnterLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 19: // Press sub3 buttons
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 20:
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 21:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 22:
                        DownLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 23:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 24:
                        EscapeLogic();
                        ActionTimer = 0.5f;
                        break;
                    case 25:
                        UpLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 26:
                        UpLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 27:
                        UpLogic(buttCount);
                        ActionTimer = 0.25f;
                        break;
                    case 29:
                        EnterLogic();
                        ActionTimer = 1.0f;
                        break;
                    case 30:
                        menuModeInt = 0;
                        menuStepping = false;
                        if (flagForComplete == true)
                        {
                            MenuAuto = false;
                            flagForComplete = false;
                        }
                        break;
                    default:
                        break;
                }
                if (menuStepping)
                    menuModeInt++;
            }
            else
            {
                ActionTimer -= Time.deltaTime;
            }
            return;
        }

        if (flagForComplete == true)
        {
            MenuAuto = false;
            menuStepping = false;
            flagForComplete = false;
            return;
        }

        if (Random.Range(0, 100) < 40)
        {
            menuStepping = true;
        }
        else
        {
            SpaceLogic();
        }
        
    }


    private void HideText()
    {
        StatusText.text = " ";
        StatusText.alpha = 0.0f;
    }

    public void SpaceLogic()
    {
        if (MenuOpen)
            return;
        for(int ii = 0; ii < Players.Count; ++ii)
        {
            if (Players[ii].PlayerCoins_.Count <= 0)
            {
                Vector2 originPoint = new(0.0f, 0.25f);
                CreatePlacements(originPoint, 3.75f, Players.Count);
                break;
            }
        }
        RoundInProgress = true;
    }

    public void EscapeLogic()
    {
        if (UIList.Actions.Count > 0)
            return;
        if (MenuOpen == false)
        {
            MenuOpen = true;
            for (int ii = 0; ii < MenuList.Count; ++ii)
            {
                currButtonIndex = 0;
                UIList.FadeIn(MenuList[ii]);
            }
            UIList.ScaleIt(MenuList[currButtonIndex], 1, 1.10f, 0.20f);
            UIList.RotateIt(MenuList[currButtonIndex], 5, 0.20f, EaseType.Linear, 0);
            MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = selectCol;
        }
        else if (MenuOpen == true && currSubState == subState.MainMenu)
        {
            MenuOpen = false;
            UIList.ScaleIt(MenuList[currButtonIndex], 1.10f, 1f, 0.20f);
            UIList.RotateIt(MenuList[currButtonIndex], 0, 0.20f, EaseType.Linear, 0);
            MenuList[currButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
            for (int ii = 0; ii < MenuList.Count; ++ii)
            {
                UIList.FadeOut(MenuList[ii]);
            }
        }
        else
        {
            for (int ii = 0; ii < MenuList.Count; ++ii)
            {
                Vector3 tempVec = MenuList[ii].transform.localPosition;
                tempVec.x = 0f;
                UIList.MoveIt(MenuList[ii], tempVec, 0.5f, 0);

                if (ii != currButtonIndex)
                {
                    MenuList[ii].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                    UIList.ScaleIt(MenuList[ii], 1.10f, 1f, 0.20f);
                    UIList.RotateIt(MenuList[ii], 0, 0.20f, EaseType.Linear, 0);

                }
            }

            for (int ii = 0; ii < SubMenuList.Count; ++ii)
            {
                UIList.ScaleIt(SubMenuList[currSubButtonIndex], 1.10f, 1f, 0.20f);
                UIList.RotateIt(SubMenuList[currSubButtonIndex], 0, 0.20f, EaseType.Linear, 0); 
                SubMenuList[currSubButtonIndex].GetComponentInChildren<SpriteRenderer>().color = defaultCol;
                
                Vector3 tempVec = SubMenuList[ii].transform.localPosition;
                tempVec.x = 0f;
                UIList.MoveIt(SubMenuList[ii], tempVec, 0.5f, 0);
                UIList.FadeOut(SubMenuList[ii], 0.5f);

            }

            currSubButtonIndex = 0;
            currSubState = subState.MainMenu;
        }
    }

    void RoundLogic()
    {
        if (UIList.Actions.Count == 0)
        {
            if (RoundCurrPlayer > Players.Count)
            {
                foreach (Player currPlayer in Players)
                {
                    if (currPlayer.PlayerNum_ != 1)
                    {
                        foreach (GameObject currCard in currPlayer.PlayerHand_)
                        {
                            UIList.FlipIt(currCard, 0.5f, 0.0f, Easing, 1);
                        }
                    }
                }
                Player winner = Players[Random.Range(0, Players.Count)];
                while (winner.isFolded == true || winner.PlayerCoins_.Count <= 0)
                {
                    winner = Players[Random.Range(0, Players.Count)];
                }
                int origCount = Pot.Count;
                for (int ii = 0; ii < origCount; ++ii)
                {
                    Vector3 coinPos = winner.PlayerCoins_[0].transform.localPosition;
                    coinPos.z = Random.Range(-10f, 10f);
                    coinPos.x = coinPos.x + Random.Range(-UIManager.Instance.Drift, UIManager.Instance.Drift);
                    coinPos.y = coinPos.y + Random.Range(-UIManager.Instance.Drift, UIManager.Instance.Drift);

                    UIList.MoveIt(Pot[0], coinPos, 0.5f, (0.0f + (0.10f * ii)), 1);
                    winner.PlayerCoins_.Add(Pot[0]);
                    Pot.RemoveAt(0);
                }
                RoundInProgress = false;
                RoundOver = true;
                RoundCurrPlayer = 1;
            }
            else
            {
                Players[RoundCurrPlayer - 1].DoThing();
                RoundCurrPlayer++;
            }
        }
    }

    void CreatePlacements(Vector2 origin, float radius, int playerNum)
    {
        float angIncrement = 360.0f / playerNum;
        float currAngle = 270.0f; // Player one initial position(bottom of screen)
        Vector2 playerPos = Vector2.zero;

        if (Players.Count > 0)
        {
            ClearPlacements();
        }

        for (int ii = 1; ii <= playerNum; ++ii)
        {
            playerPos.x = ((Mathf.Cos(Mathf.Deg2Rad * currAngle) * radius) + origin.x) * 1.5f;
            playerPos.y = (Mathf.Sin(Mathf.Deg2Rad * currAngle) * radius) + origin.y;
            currAngle = currAngle + angIncrement;

            Player currPlayer = new(playerPos, ii);
            Players.Add(currPlayer);

            if (newRoundStarting == false)
            {
                currPlayer.DealHand(Random.Range(2, 8), 0.75f);
                currPlayer.DealCoins();
            }
        }
    }

    void ClearPlacements()
    {
        foreach (Player currPlayer in Players)
        {
            currPlayer.ClearHand();
            currPlayer.ClearCoins();
        }
        Players.Clear();
    }

    GameObject CreateCard(int number)
    {
        GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, (number + 1) / 10.0f), Quaternion.identity);
        newCard.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "" + (number % 13 + 1);
        Deck.Add(newCard);
        return newCard;
    }

    GameObject CreateChip(int number)
    {
        GameObject newChip = Instantiate(ChipPrefab, new Vector3(0, 0, (number + 1) / 10.0f), Quaternion.identity);
        Chips.Add(newChip);
        return newChip;
    }

    void CreateMenu()
    {

        // Make menu
        float startLocY = 3;
        // Resume button
        GameObject resButt = Instantiate(MenuPrefab, new Vector3(0, startLocY, -11), Quaternion.identity);
        resButt.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Resume";
        MenuList.Add(resButt);
        UIList.FadeOut(resButt, 1);

        // Settings button
        GameObject setButt = Instantiate(MenuPrefab, new Vector3(0, (startLocY -= 1.4f), -11), Quaternion.identity);
        setButt.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Video";
        MenuList.Add(setButt);
        UIList.FadeOut(setButt, 1);

        // Credits button
        GameObject credButt = Instantiate(MenuPrefab, new Vector3(0, (startLocY -= 1.4f), -11), Quaternion.identity);
        credButt.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Display";
        MenuList.Add(credButt);
        UIList.FadeOut(credButt, 1);

        // Mute button
        GameObject mutButt = Instantiate(MenuPrefab, new Vector3(0, (startLocY -= 1.4f), -11), Quaternion.identity);
        mutButt.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Audio";
        MenuList.Add(mutButt);
        UIList.FadeOut(mutButt, 1);

        // Exit button
        GameObject exButt = Instantiate(MenuPrefab, new Vector3(0, (startLocY -= 1.4f), -11), Quaternion.identity);
        exButt.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Exit";
        MenuList.Add(exButt);
        UIList.FadeOut(exButt, 1);

        startLocY = 3;

        // Make sub menu buttons(hidden)
        // Sub button 1
        GameObject subButt1 = Instantiate(MenuPrefab, new Vector3(0, startLocY, -10.5f), Quaternion.identity);
        subButt1.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "SubButt1";
        SubMenuList.Add(subButt1);
        UIList.FadeOut(subButt1, 1);

        // Sub button 
        GameObject subButt2 = Instantiate(MenuPrefab, new Vector3(0, (startLocY -= 1.4f), -10.5f), Quaternion.identity);
        subButt2.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "SubButt2";
        SubMenuList.Add(subButt2);
        UIList.FadeOut(subButt2, 1);
        // Sub button 3
        GameObject subButt3 = Instantiate(MenuPrefab, new Vector3(0, (startLocY -= 1.4f), -10.5f), Quaternion.identity);
        subButt3.transform.Find("Canvas/Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "SubButt3";
        SubMenuList.Add(subButt3);
        UIList.FadeOut(subButt3, 1);

    }

}
