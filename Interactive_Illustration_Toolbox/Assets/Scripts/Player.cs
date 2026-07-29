using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera mainCam;
    private Vector2 position;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(mainCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
            
            Clickable highestPriorityClickable = null;
            
            foreach (RaycastHit2D hit in hits)
            {
                Clickable hitClickable = hit.collider.gameObject.GetComponent<Clickable>();

                if (hitClickable == null) continue;
                if (!hitClickable.IsClickable) continue;

                if (highestPriorityClickable == null)
                {
                    highestPriorityClickable = hitClickable;
                }
                else if (hitClickable.Priority > highestPriorityClickable.Priority)
                {
                    highestPriorityClickable = hitClickable;
                }
            }
            
            highestPriorityClickable?.OnClick();
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}