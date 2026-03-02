using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BMV.Tutorial
{
	public class HelpTopic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField]
		Animator animator;

		[field: SerializeField]
		public HelpArticle Article { get; private set; }

		int selectedParameter;
		int hoverParameter;

		public static event Action<HelpTopic> OnTopicSelected;

		static HelpTopic current;

		public bool IsSelected
		{
			get => current == this;
			private set
			{
				SelectedDecoration = value;

				if(value)
				{
					if(IsSelected)
						return;
					else if(current)
						current.IsSelected = false;

					current = this;
					Article.Open();
				}
				else
				{
					Article.Close();
					current = null;
				}

				OnTopicSelected?.Invoke(this);
			}
		}

		bool SelectedDecoration
		{
			get => animator.GetBool(selectedParameter);
			set => animator.SetBool(selectedParameter, value);
		}

		bool IsHovered
		{
			get => animator.GetBool(hoverParameter);
			set => animator.SetBool(hoverParameter, value);
		}

		public static void CloseLast()
		{
			if (!current)
				return;

			current.Article.Close();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			IsSelected = !IsSelected;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			IsHovered = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			IsHovered = false;
		}

		void OnArticleOpenedChanged(bool isOpened)
		{
			IsSelected = isOpened;
		}

		void Awake()
		{
			AnimatorControllerParameter[] parameters = animator.parameters;

			if(parameters.Length > 0)
				hoverParameter = parameters[0].nameHash;
			if(parameters.Length > 1)
				selectedParameter = parameters[1].nameHash;
			
			Article.OnOpenedChanged += OnArticleOpenedChanged;
		}

#if UNITY_EDITOR
		void Reset()
		{
			animator = GetComponent<Animator>();
		}
#endif
	}
}