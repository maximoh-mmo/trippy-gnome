using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNumbers : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private float comboUpDisplayDuration = 0.8f;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowNumber(int comboLevel)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayNumber(comboLevel));
    }
    
    IEnumerator DisplayNumber(int element)
    {
        spriteRenderer.sprite = spriteList[element];
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(comboUpDisplayDuration);
        spriteRenderer.enabled = false;
    }
}
