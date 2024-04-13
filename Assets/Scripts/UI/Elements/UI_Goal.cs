using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Goal : MonoBehaviour
{
    public Image imageIcon;
    public GameObject textObject;
    public GameObject checkObject;
    public TextMeshProUGUI textAmount;

    Goal m_parentGoal;
    Animator m_animator;
    RectTransform m_rectTransform;

    public ItemData Data
    {
        get
        {
            if (m_parentGoal == null)
                return null;

            return m_parentGoal.itemToCollect;
        }
    }

    public void Init (Goal _parentGoal)
    {
        m_parentGoal = _parentGoal;
        
        m_animator = GetComponent<Animator>();
        m_rectTransform = imageIcon.GetComponent<RectTransform>();
        imageIcon.sprite = _parentGoal.itemToCollect.icon;

        textObject.SetActive(true);
        checkObject.SetActive(false);
    }

    public void SetAmount (int _amount)
    {
        if (_amount <= 0)
        {
            textObject.SetActive(false);
            checkObject.SetActive(true);
        }
        else
        {
            textAmount.text = "x" + _amount.ToString();
            m_animator.SetTrigger("Add");
        }
    }

    public bool IsParentOf (Goal _goal)
    {
        return m_parentGoal == _goal;
    }

    public bool IsContaining (Item _item)
    {
        return Data.EqualsTo(_item.Data) && m_parentGoal != null && !m_parentGoal.IsCompleted();
    }

    public Vector2 GetPoistionUI ()
    {
        return m_rectTransform.position; //Camera.main.ScreenToWorldPoint(m_rectTransform.position);
    }
}
