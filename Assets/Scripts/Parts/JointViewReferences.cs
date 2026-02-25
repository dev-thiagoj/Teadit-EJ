using UnityEngine;

public class JointViewReferences : MonoBehaviour
{
    [field: SerializeField] public Transform orbitPivot { get; private set;  }
    [field: SerializeField] public Transform panPivot { get; private set; }
}
