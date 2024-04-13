using UnityEngine;

public class GameInput : Singleton<GameInput>
{
    public bool SelectDown ()
    {
        if (Application.isMobilePlatform)
            return GetSingleTouchInPhase(TouchPhase.Began);

        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool SelectHold ()
    {
        if (Application.isMobilePlatform)
            return GetSingleTouchInPhase(TouchPhase.Moved);

        return Input.GetKey(KeyCode.Mouse0);
    }

    public bool SelectUp ()
    {
        if (Application.isMobilePlatform)
            return GetSingleTouchInPhase(TouchPhase.Ended);

        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    bool GetSingleTouchInPhase (TouchPhase _phase)
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == _phase)
                return true;
        }

        return false;
    }
}
