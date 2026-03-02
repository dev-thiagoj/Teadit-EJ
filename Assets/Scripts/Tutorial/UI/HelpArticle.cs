using System;
using UnityEngine;

namespace BMV.Tutorial
{
	public class HelpArticle : MonoBehaviour
	{
		[SerializeField]
		Animator animator;

		public event Action<bool> OnOpenedChanged;

		int openParameter;

		public bool IsOpened
		{
			get => animator.GetBool(openParameter);
			private set
			{
				animator.SetBool(openParameter, value);
				OnOpenedChanged?.Invoke(value);
			}
		}

		public void Close()
		{
			if(!IsOpened)
				return;
			
			IsOpened = false;
		}

		public void Open()
		{
			if(IsOpened)
				return;

			IsOpened = true;
		}

		void Awake()
		{
			
			AnimatorControllerParameter[] parameters = animator.parameters;

			if(parameters.Length > 0)
				openParameter = parameters[0].nameHash;
		}

#if UNITY_EDITOR
		void Reset()
		{
			animator = GetComponent<Animator>();
		}
#endif
	}
}