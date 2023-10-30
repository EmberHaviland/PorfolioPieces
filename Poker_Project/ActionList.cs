using System.Collections.Generic;
using UnityEngine;
using static Action;

public class ActionList : MonoBehaviour
{
    public List<Action> Actions;
    // Start is called before the first frame update
    void Start()
    {
        if (Actions == null)
        {
            Actions = new List<Action>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Actions == null)
        {
            Actions = new List<Action>();
        }
        float dt = Time.deltaTime * UIManager.Instance.SpeedMultiple;
        int BlockingGroupSet = 0;
        for (int ii = 0; ii < Actions.Count; ii++)
        {
            if (Actions[ii].IsInGroupSet(BlockingGroupSet))
                continue;
            if (Actions[ii].IncrementTime(dt) == false)
                continue;
            if (Actions[ii].Update(dt) == false)
            {
                Actions.RemoveAt(ii);
                ii--;
                continue;
            }
            if (Actions[ii].Blocking == true)
                BlockingGroupSet = BlockingGroupSet | Actions[ii].Groups;
        }
    }

    public Action MoveIt(GameObject objectToMove, Vector3 start, Vector3 end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear,int group = 0)
    {
        if (Actions == null)
        {
            Actions = new List<Action>();
        }
        Move newAction = new Move(objectToMove, start, end, duration, delay, ease, group);
        Actions.Add(newAction);
        return newAction;
    }

    public Action MoveIt(GameObject objectToMove, Vector3 end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        return MoveIt(objectToMove, objectToMove.transform.localPosition, end, duration, delay, ease, group);
    }

    public Action MoveIt(GameObject objectToMove, Vector3 end, float duration = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        return MoveIt(objectToMove, objectToMove.transform.localPosition, end, duration, 0.0f, ease, group);
    }

    public Action MoveIt(GameObject objectToMove, Vector3 start, Vector3 end, float duration = 0.0f, float delay = 0.0f, int group = 0)
    {
        return MoveIt(objectToMove, start, end, duration, delay, EaseType.Linear, group);
    }

    public Action MoveIt(GameObject objectToMove, Vector3 end, float duration = 0.0f, float delay = 0.0f, int group = 0)
    {
        return MoveIt(objectToMove, objectToMove.transform.localPosition, end, duration, delay, EaseType.Linear, group);
    }

    public Action MoveIt(GameObject objectToMove, Vector3 end, float duration = 0.0f, int group = 0)
    {
        return MoveIt(objectToMove, objectToMove.transform.localPosition, end, duration, 0.0f, EaseType.Linear, group);
    }

    public Action RotateIt(GameObject objectToRotate, float start, float end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        Rotate newAction = new Rotate(objectToRotate, new Vector3(objectToRotate.transform.localEulerAngles.x, objectToRotate.transform.localEulerAngles.y, start), new Vector3(0.0f, 0.0f, end), duration, delay, ease, group);
        Actions.Add(newAction);
        return newAction;
    }

    public Action RotateIt(GameObject objectToRotate, float end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        return RotateIt(objectToRotate, objectToRotate.transform.localEulerAngles.z, end, duration, delay, ease, group);
    }

    public Action RotateIt(GameObject objectToRotate, float end, float duration = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        return RotateIt(objectToRotate, objectToRotate.transform.localEulerAngles.z, end, duration, 0.0f, ease, group);
    }

    public Action FlipIt(GameObject objectToFlip, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        Flip newAction = new Flip(objectToFlip, duration, delay, ease, group);
        Actions.Add(newAction);
        return newAction;
    }

    public Action FadeIn(GameObject objectToFade, float duration = 0.5f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        FadeIn newAction = new FadeIn(objectToFade, duration, delay, ease, group);
        Actions.Add(newAction);
        return newAction;
    }
    public Action FadeOut(GameObject objectToFade, float duration = 0.5f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        FadeOut newAction = new FadeOut(objectToFade, duration, delay, ease, group);
        Actions.Add(newAction);
        return newAction;
    }

    public Action ScaleIt(GameObject objectToScale, float start, float end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        Scale newAction = new Scale(objectToScale, start, end, duration, delay, ease, group);
        Actions.Add(newAction);
        return newAction;
    }
    public void ReverseAll()
    {
        foreach(Action action in Actions)
        {
            action.Reverse();
        }
    }
}

public class Action
{
    public float Delay;
    public float TimePassed;
    public float Duration;
    public float Percent;
    public bool Blocking;
    public int Groups;
    public EaseType Easing;
    public GameObject ActionObject = null;

    public virtual bool Update(float dt)
    {
        return Percent < 1.0f;
    }

    public enum EaseType
    {
        Linear,
        EaseIn,
        EaseOut,
        FastIn,
        FastOut,
        InAndOut,
        Bounce,
    };

    public float Ease(float percent, EaseType type)
    {
        if (percent <= 0.0f)
            return 0.0f;
        if (percent >= 1.0f)
            return 1.0f;

        switch (type)
        {
            case EaseType.Linear: return percent;
            case EaseType.EaseIn: return Mathf.Sqrt(percent);
            case EaseType.EaseOut: return Mathf.Pow(percent, 2.0f);
            case EaseType.FastIn: return Mathf.Sqrt(Mathf.Sqrt(percent));
            case EaseType.FastOut: return Mathf.Pow(percent, 4.0f);
            case EaseType.InAndOut:
                if (percent < 0.5f)
                    return Mathf.Pow(percent * 2.0f, 2.0f) * 0.5f;
                else
                    return Mathf.Sqrt((percent - 0.5f) * 2.0f) * 0.5f + 0.5f;
            case EaseType.Bounce:
                float n = 7.5625f;
                float d = 2.75f;
                if (percent < 1.0f / d)
                    return n * percent * percent;
                else if (percent < 2.0f / d)
                    return n * (percent -= 1.5f / d) * percent + 0.75f;
                else if (percent < 2.5f / d)
                    return n * (percent -= 2.25f / d) * percent + 0.9375f;
                else
                    return n * (percent -= 2.625f / d) * percent + 0.984375f;
        }
        return percent;
    }

    public bool IncrementTime(float dt)
    {
        if (Delay > 0.0f)
        {
            Delay -= dt;
            if (Delay > 0.0f)
                return false;
            TimePassed -= Delay;
            Delay = 0.0f;
        }    
        else
        {
            TimePassed += dt;
        }

        if (TimePassed >= Duration)
        {
            TimePassed = Duration;
            Percent = 1.0f;
        }
        else
        {
            Percent = TimePassed / Duration;
        }

        Percent = Ease(Percent, Easing);

        return true;
    }

    public void AddToGroup(int groupNum)
    {
        if (groupNum < 1 || groupNum > 30)
            return;
        Groups = Groups | (1 << (groupNum - 1));
    }

    public bool IsInGroup(int groupNum)
    {
        if (groupNum < 1 || groupNum > 30)
        {
            return false;
        }
        return (Groups & (1 << (groupNum - 1))) != 0;
    }

    public bool IsInGroupSet(int groupSet)
    {
        if (groupSet < 0)
            return false;
        return (Groups & groupSet) != 0;
    }

    public void ReverseTime()
    {
        if (Delay > 0)
            return;
        TimePassed = Duration - TimePassed;
        Percent = TimePassed / Duration;
        Percent = Ease(Percent, Easing);
    }

    public virtual void Reverse()
    {
        ReverseTime();
    }
};

public class Move : Action
{
    Vector3 Start;
    Vector3 End;

    public Move(GameObject objectToMove, Vector3 start, Vector3 end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        ActionObject = objectToMove;
        Start = start;
        End = end;
        Duration = duration;
        Delay = delay;
        Easing = ease;
        AddToGroup(group);
    }

    public override bool Update(float dt)
    {
        if (ActionObject == null)
            return false;
        ActionObject.transform.localPosition = Start + (End - Start) * Percent;
        return Percent < 1.0f;
    }

    public override void Reverse()
    {
        if (Delay > 0)
            return;
        Vector3 temp = Start;
        Start = End;
        End = temp;
        ReverseTime();
    }
}

public class Rotate : Action
{
    Vector3 Start;
    Vector3 End;
    public Rotate(GameObject objectToRotate, Vector3 start, Vector3 end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        ActionObject = objectToRotate;
        if (start.z - end.z > 180.0f)
            start.z -= 360.0f;
        if (start.z - end.z < -180.0f)
            end.z -= 360.0f;
        Start = start;
        End = end;
        Duration = duration;
        Delay = delay;
        Easing = ease;
        AddToGroup(group);
    }

    public override bool Update(float dt)
    {
        if (ActionObject == null)
            return false;
        ActionObject.transform.localEulerAngles = Start + (End - Start) * Percent;
        return Percent < 1.0f;
    }

    public override void Reverse()
    {
        if (Delay > 0)
            return;
        Vector3 temp = Start;
        Start = End;
        End = temp;
        ReverseTime();
    }
}

public class Scale : Action
{
    float Start;
    float End;
    public Scale(GameObject objectToScale, float start, float end, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        ActionObject = objectToScale;
        Start = start;
        End = end;
        Duration = duration;
        Delay = delay;
        Easing = ease;
        AddToGroup(group);
    }

    public override bool Update(float dt)
    {
        float scalarVal;
        scalarVal = Start + ((End - Start) * Percent);

        Vector3 tempVec = new Vector3(scalarVal, scalarVal, scalarVal);
        if (ActionObject == null)
            return false;
        ActionObject.transform.localScale = tempVec;
        return Percent < 1.0f;
    }

    public override void Reverse()
    {
        if (Delay > 0)
            return;
        float temp = Start;
        Start = End;
        End = temp;
        ReverseTime();
    }
}

public class Flip : Action
{
    Vector3 Start;
    Vector3 End;
    public Flip(GameObject objectToRotate, float duration = 0.0f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        ActionObject = objectToRotate;
        Start = objectToRotate.transform.localEulerAngles;
        End = Start;
        End.y += 180.0f;
        Duration = duration;
        Delay = delay;
        Easing = ease;
        AddToGroup(group);
    }

    public override bool Update(float dt)
    {
        if (ActionObject == null)
            return false;
        ActionObject.transform.localEulerAngles = Start + (End - Start) * Percent;
        return Percent < 1.0f;
    }

    public override void Reverse()
    {
        if (Delay > 0)
            return;
        Vector3 temp = Start;
        Start = End;
        End = temp;
        ReverseTime();
    }
}

public class FadeOut : Action
{
    float Start;
    float End;

    Canvas actingCanv = null;
    SpriteRenderer actingSprite = null;
    public FadeOut(GameObject objectToFade, float duration = 0.5f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        ActionObject = objectToFade;
        Canvas tempCanv = objectToFade.GetComponentInChildren<Canvas>();
        if (tempCanv != null)
        {
            actingCanv = tempCanv;
        }
        SpriteRenderer tempInt = objectToFade.GetComponentInChildren<SpriteRenderer>();
        if (tempInt != null)
        {
            actingSprite = tempInt;
            Start = tempInt.color.a;
            End = 0;
        }
        if (!tempCanv && !tempInt)
            return;
        Duration = duration;
        Delay = delay;
        Easing = ease;
        AddToGroup(group);
    }

    public override bool Update(float dt)
    {
        if ((ActionObject == null) || (actingCanv == null && actingSprite == null))
            return false;

        Color newCol;
        newCol = actingSprite.color;
        newCol.a = Start + ((End - Start) * Percent);
        actingSprite.color = newCol;
        if (actingCanv)
        {
            newCol = actingCanv.GetComponentInChildren<TMPro.TextMeshProUGUI>().color;
            newCol.a = Start + (End - Start) * Percent;
            actingCanv.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = newCol;
        }
        return Percent < 1.0f;
    }

    public override void Reverse()
    {
        if (Delay > 0)
            return;
        float temp = Start;
        Start = End;
        End = temp;
        ReverseTime();
    }
}

public class FadeIn : Action
{
    float Start;
    float End;

    Canvas actingCanv = null;
    SpriteRenderer actingSprite = null;
    public FadeIn(GameObject objectToFade, float duration = 0.5f, float delay = 0.0f, EaseType ease = EaseType.Linear, int group = 0)
    {
        ActionObject = objectToFade;
        Canvas tempCanv = objectToFade.GetComponentInChildren<Canvas>();
        if (tempCanv != null)
        {
            actingCanv = tempCanv;
        }
        SpriteRenderer tempInt = objectToFade.GetComponentInChildren<SpriteRenderer>();
        if (tempInt != null)
        {
            actingSprite = tempInt;
            Start = 0;
            End = 1;
        }
        if (!tempCanv && !tempInt)
            return;
        Duration = duration;
        Delay = delay;
        Easing = ease;
        AddToGroup(group);
    }

    public override bool Update(float dt)
    {
        if ((ActionObject == null) || (actingCanv == null && actingSprite == null))
            return false;

        Color newCol;
        newCol = actingSprite.color;
        newCol.a = (Start + (End - Start)) * Percent;
        actingSprite.color = newCol;
        if (actingCanv)
        {
            newCol = actingCanv.GetComponentInChildren<TMPro.TextMeshProUGUI>().color;
            newCol.a = (Start + (End - Start)) * Percent;
            actingCanv.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = newCol;
        }
        return Percent < 1.0f;
    }

    public override void Reverse()
    {
        if (Delay > 0)
            return;
        float temp = Start;
        Start = End;
        End = temp;
        ReverseTime();
    }
}