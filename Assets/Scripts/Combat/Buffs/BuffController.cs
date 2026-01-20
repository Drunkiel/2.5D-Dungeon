using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    public List<BuffCell> _buffCells = new();
    [SerializeField] private BuffCell buffCellPrefab;

    public void AddBuff(Buff _buffData)
    {
        BuffCell _newBuffCell = Instantiate(buffCellPrefab, transform);
        _newBuffCell.iconImage.sprite = _buffData.sprite;
        _buffCells.Add(_newBuffCell);

        // Uruchom animację tylko jeśli buff nie jest permanentny
        if (!_buffData.isPermanent)
        {
            StartCoroutine(MoveBackgroundOverTime(
                _newBuffCell.backgroundImage.GetComponent<RectTransform>(),
                _buffData.duration,
                _buffData.timer
            ));
        }
    }

    public void RemoveAllBuffs()
    {
        for (int i = 0; i < _buffCells.Count; i++)
            Destroy(_buffCells[i].gameObject);

        _buffCells.Clear();
    }

    private IEnumerator MoveBackgroundOverTime(RectTransform background, float duration, float timer)
    {
        float startY = 0f;
        float endY = -25f;
        float progress = Mathf.Clamp01(timer / duration);

        while (progress < 1f)
        {
            progress += Time.deltaTime / duration;

            float currentY = Mathf.Lerp(startY, endY, progress);
            if (background != null)
                background.anchoredPosition = new Vector2(
                    background.anchoredPosition.x,
                    currentY
                );

            yield return null;
        }
    }
}
