using System;
using System.Collections.Generic;
using BMV.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace BMV.Tutorial
{
	public class HelpMenu : MonoBehaviour
	{
		readonly Dictionary<HelpTopic, HelpArticle> topics = new ();

		[SerializeField]
		Button closeButton;
		[SerializeField]
		Button helpButton;
		[SerializeField]
		Animator animator;
		[SerializeField]
		Textbox introText;

		static HelpMenu instance;

		int disableParameter;
		int openParameter;

		public static event Action OnMenuOpened;
		public static event Action OnMenuClosed;
		public static event Action OnIntroEnded;

		bool IsOpened
		{
			get => animator.GetBool(openParameter);
			set
			{
				animator.SetBool(openParameter, value);

				Action action = value ? OnMenuOpened : OnMenuClosed;
				action?.Invoke();
			}
		}

		public static void OpenMenu()
		{
			if(!instance)
				return;
			
			instance.IsOpened = true;
		}

		public static void ShowIntro()
		{
			if(!instance)
				return;
			
			instance.StartIntro();
		}

		void ShowCloseButton(bool show)
		{
			closeButton.gameObject.SetActive(show);
		}

		void StartIntro()
		{
			helpButton.gameObject.SetActive(false);
			introText.Open();
		}

		void OnCloseClick()
		{
			HelpTopic.CloseLast();
		}

		void OnDialogClosed()
		{
			helpButton.gameObject.SetActive(true);
			OnIntroEnded?.Invoke();
		}

		void OnHelpClick()
		{
			IsOpened = !IsOpened;

			if(IsOpened)
				return;
			
			HelpTopic.CloseLast();
		}

		void OnTopicSelected(HelpTopic topic)
		{
			ShowCloseButton(topic.IsSelected);
		}

		void Awake()
		{
			instance = this;
			closeButton.onClick.AddListener(OnCloseClick);
			helpButton.onClick.AddListener(OnHelpClick);
			Textbox.OnClosed += OnDialogClosed;
			HelpTopic.OnTopicSelected += OnTopicSelected;
			ShowCloseButton(false);

			AnimatorControllerParameter[] parameters = animator.parameters;

            if (parameters.Length > 0)
				disableParameter = parameters[0].nameHash;
			if(parameters.Length > 1)
				openParameter = parameters[1].nameHash;
		}

		void OnDestroy()
		{
			HelpTopic.OnTopicSelected -= OnTopicSelected;
		}

		void OnDisable()
		{
			animator.SetBool(disableParameter, true);
		}

		void OnEnable()
		{
			animator.SetBool(disableParameter, false);
		}

#if UNITY_EDITOR
		void Reset()
		{
			Button[] buttons = GetComponentsInChildren<Button>(true);
			if(buttons.Length > 0)
				closeButton = buttons[0];
			if(buttons.Length > 1)
				helpButton = buttons[1];
			animator = GetComponent<Animator>();
			introText = GetComponentInChildren<Textbox>(true);
		}
#endif
	}
}