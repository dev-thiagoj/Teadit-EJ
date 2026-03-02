using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BMV.User
{
	[Serializable]
	public class ZoomInput
	{
		// [Header("Configurações de Zoom")]
		public float zoomSpeed = 0.005f;
		// public float minZoom = 1f;
		// public float maxZoom = 10f;
		// public float smoothTime = 0.1f;
		
		// [Header("Configurações de Câmera")]
		// public bool zoomInCenter = true;
		// public float perspectiveZoomSpeed = 0.5f;
		
		// Camera mainCamera;
		// float targetZoom;
		// float velocity = 0f;
		float zoom;

		Touchscreen touchscreen;

		public float Zoom
		{
			get => zoom;
			set
			{
				zoom = value;
				OnZoomChanged?.Invoke(zoom);
			}
		}

		public event Action<float> OnZoomChanged;

		// void Start()
		// {
		// 	mainCamera = Camera.main;
			
		// 	if (mainCamera.orthographic)
		// 		targetZoom = mainCamera.orthographicSize;
		// 	else
		// 		targetZoom = mainCamera.fieldOfView;
		// }
		
		public bool Update()
		{
			touchscreen ??= Touchscreen.current;

			if(touchscreen is null)
				return false;

			return HandlePinchZoom();
			// ApplySmoothZoom();
		}

		bool HandlePinchZoom()
		{
			if (touchscreen.touches.Count < 2)
			{
				return false;
			}

			for(int i = 0; i < 2; i++)
			{
				if(!touchscreen.touches[i].isInProgress)
					return false;
			}

			Vector2 touch1Pos = touchscreen.touches[0].position.ReadValue();
			Vector2 touch2Pos = touchscreen.touches[1].position.ReadValue();

			Vector2 touch1Delta = touchscreen.touches[0].delta.ReadValue();
			Vector2 touch2Delta = touchscreen.touches[1].delta.ReadValue();
			Vector2 touch1PrevPos = touch1Pos - touch1Delta;
			Vector2 touch2PrevPos = touch2Pos - touch2Delta;

			float prevMagnitude = (touch1PrevPos - touch2PrevPos).magnitude;
			float currentMagnitude = (touch1Pos - touch2Pos).magnitude;
			float difference = currentMagnitude - prevMagnitude;

			Zoom -= difference * zoomSpeed;
			return true;
		}

		// void ApplySmoothZoom()
		// {
		// 	if (mainCamera.orthographic)
		// 	{
		// 		mainCamera.orthographicSize = Mathf.SmoothDamp(
		// 			mainCamera.orthographicSize,
		// 			targetZoom,
		// 			ref velocity,
		// 			smoothTime
		// 		);
		// 	}
		// 	else
		// 	{
		// 		mainCamera.fieldOfView = Mathf.SmoothDamp(
		// 			mainCamera.fieldOfView,
		// 			targetZoom,
		// 			ref velocity,
		// 			smoothTime
		// 		);
		// 	}
		// }

		// void UpdateZoomCenter(Vector2 screenPosition)
		// {
		// 	// Converte a posição da tela para uma posição no mundo
		// 	Ray ray = mainCamera.ScreenPointToRay(screenPosition);
		// 	RaycastHit hit;
			
		// 	if (Physics.Raycast(ray, out hit))
		// 	{
		// 		// Ajusta a posição da câmera para focar no ponto de pinch
		// 		// Implementação depende da sua necessidade específica
		// 	}
		// }
	}
}