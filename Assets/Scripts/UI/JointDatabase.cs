using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Joints/Joint Database")]
public class JointDatabase : ScriptableObject
{
    [field: SerializeField] public List<JointData> Joints {  get; private set; }
}
