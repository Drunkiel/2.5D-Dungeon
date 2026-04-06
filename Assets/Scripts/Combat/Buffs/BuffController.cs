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

        if (!_buffData.isPermanent)
        {
            StartCoroutine(MoveBackgroundOverTime(
                _newBuffCell.backgroundImage.GetComponent<RectTransform>(),
                _buffData
            ));
        }
    }

    public void RemoveAllBuffs()
    {
        for (int i = 0; i < _buffCells.Count; i++)
            Destroy(_buffCells[i].gameObject);

        _buffCells.Clear();
    }

    private IEnumerator MoveBackgroundOverTime(RectTransform background, Buff _buffData)
    {
        float startY = 0f;
        float endY = -25f;
        float progress = 0f;

        while (progress <= 1f)
        {
            progress = Mathf.Clamp01(_buffData.timer / _buffData.duration);

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
