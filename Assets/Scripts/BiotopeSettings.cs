using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiotopeSettings : MonoBehaviour
{
    [Tooltip("offset for ph-value (higher-value means more fish will die)")]
    public float phBoost = 0.0f;

    [Tooltip("percentage of fish that will be killed here additionally (negative value means this will spawn fish)")]
    public float killFish = 0.0f;
}
