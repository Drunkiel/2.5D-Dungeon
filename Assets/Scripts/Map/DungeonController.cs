using System.Collections;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    [SerializeField] private EntityController _coreController;
    public int timeLeft;

    private void Start()
    {
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        if (timeLeft <= 0)
            yield break;

        yield return new WaitForSeconds(1f);
        timeLeft -= 1;
        _coreController._statistics.mana = timeLeft; 

        StartCoroutine(UpdateTimer());
    }
}
