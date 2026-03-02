using System;
using UnityEngine;

namespace BMV.Views
{
	public class ViewModeController : MonoBehaviour
	{
		[SerializeField]
		ViewMode mode = ViewMode.Entry;

		static ViewMode current;

		public static event Action<ViewMode> OnModeChanged;

		public static ViewMode CurrentMode
		{
			get => current;
			private set
			{
				if (value == CurrentMode)
					return;

				current = value;
				OnModeChanged?.Invoke(current);
#if UNITY_EDITOR
				if (instance)
					instance.mode = current;
#endif
			}
		}

		static ViewModeController instance;

		public static void ToExplodedView() => ToMode(ViewMode.Exploded);
		public static void ToGeneralView() => ToMode(ViewMode.General);
		public static void ToIntroView() => ToMode(ViewMode.Entry);

		static void ToMode(ViewMode mode)
		{
			if (!instance)
				return;

#if UNITY_EDITOR
			Debug.Log(mode);
#endif
			CurrentMode = mode;
		}

		//void OnTargetSelected(Target target)
		//{
		//	// Não permitir que o estado global seja Normal.
		//	ViewMode newMode = target ? target.View : ViewMode.General;

		//	if(newMode is ViewMode.Normal)
		//		newMode = ViewMode.Exploded;
			
		//	mode = newMode;
		//	ToMode(mode);
		//}

		void Awake()
		{
			if (instance)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			//Target.OnTargetSelected += OnTargetSelected;
		}

		void OnDestroy()
		{
			//Target.OnTargetSelected -= OnTargetSelected;
		}

		void Start()
		{
			CurrentMode = mode;
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			CurrentMode = mode;
		}
#endif
	}
}