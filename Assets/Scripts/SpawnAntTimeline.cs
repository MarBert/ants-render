using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAntTimeline : MonoBehaviour
{
    [SerializeField] private int quantity;
    [SerializeField] private int groups;
    [SerializeField] private float delay;

    private void OnEnable(){
        AntsManager.instance.SpawnAntsWithGroup(quantity, groups, delay);
    }
}
