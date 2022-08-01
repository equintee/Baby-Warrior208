using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class collectableAnimator : MonoBehaviour
{
    void Start()
    {
        transform.DOMoveY(transform.position.y + 1, 0.5f).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Yoyo);
        transform.DORotate(new Vector3(90, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void OnTriggerEnter(Collider other)
    {
        DOTween.Kill(transform);
        other.GetComponent<playerController>().updateMana(10);
        Destroy(gameObject);
    }
}
