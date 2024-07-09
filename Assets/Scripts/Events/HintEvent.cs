using UnityEngine;

public class HintEvent : MonoBehaviour
{
    private int currentHint;
    public bool addOne;
    public bool dontAdd;

    public void ChangeHint(int eventIndex)
    {
        bool Check()
        {
            if (dontAdd)
                return false;
            else
                return addOne;
        }

        int index = addOne && eventIndex != 0 ? (Check() ? eventIndex + 1 : eventIndex) : eventIndex;
        if (currentHint == index) return;

        currentHint = index;
        HintController.instance.UpdateText(index);
    }
}
