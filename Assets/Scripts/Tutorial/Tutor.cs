using System.Collections;
using BMV.Views;
using UnityEngine;

namespace BMV.Tutorial
{
	public class Tutor : MonoBehaviour
	{
		ViewMode lastMode;

		void OnIntroDialogClosed()
		{
			ViewModeController.ToGeneralView();
		}

		void OnHelpMenuClosed()
		{
			switch (lastMode)
			{
				case ViewMode.Exploded:
					ViewModeController.ToExplodedView();
					break;
				default:
					ViewModeController.ToGeneralView();
					break;
			}
		}

		void OnHelpMenuOpened()
		{
			if (ViewModeController.CurrentMode is ViewMode.Entry)
				return;

			lastMode = ViewModeController.CurrentMode;
			ViewModeController.ToIntroView();
		}

		void Awake()
		{
			HelpMenu.OnIntroEnded += OnIntroDialogClosed;
			HelpMenu.OnMenuClosed += OnHelpMenuClosed;
			HelpMenu.OnMenuOpened += OnHelpMenuOpened;
		}

		IEnumerator Start()
		{
			yield return new WaitForSeconds(1);
			HelpMenu.ShowIntro();
		}

		void OnDestroy()
		{
			HelpMenu.OnIntroEnded -= OnIntroDialogClosed;
			HelpMenu.OnMenuClosed -= OnHelpMenuClosed;
			HelpMenu.OnMenuOpened -= OnHelpMenuOpened;
		}
	}
}