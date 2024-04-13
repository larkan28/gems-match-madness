using UnityEngine;

public class Util : MonoBehaviour
{
    public static Bounds CameraBounds (Camera _camera)
    {
        Vector3 origin = _camera.transform.position;
        origin.z = 0;

        float screenAspect = (float) Screen.width / (float) Screen.height;
        float cameraHeight = _camera.orthographicSize * 2;

        return new Bounds(origin, new Vector3(cameraHeight * screenAspect, cameraHeight, 0f));
    }

    public static Vector2 CameraBoundsToVec (Camera _camera)
    {
        Bounds bounds = CameraBounds(_camera);
        return new Vector2(bounds.max.x * 2f, bounds.max.y * 2f);
    }

    public static Vector3 ScreenToWorldPoint (RectTransform _rectTransform)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(_rectTransform.position);
        pos.z = 0;

        return pos;
    }

    public static ItemData NotNullColorIn (Item[] _items)
    {
        foreach (var item in _items)
        {
            ItemData itemData = item.Data;

            if (itemData.type != ItemData.Type.None)
                return itemData;
        }

        return null;
    }

    public static bool IntToBool (int _value)
    {
        return _value != 0;
    }

    public static int BoolToInt (bool _value)
    {
        return _value ? 1 : 0;
    }
}
