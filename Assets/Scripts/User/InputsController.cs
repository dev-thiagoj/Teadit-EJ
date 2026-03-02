using BMV.Cursor;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BMV.User
{
	public class InputsController : MonoBehaviour
	{
		[Header("Ações")]
		[SerializeField]
		InputActionReference _selectAction;
		[SerializeField]
		InputActionReference _positionAction;
		[SerializeField]
		InputActionReference _moveAction;
		[SerializeField]
		InputActionReference _startMoveAction;
		[SerializeField]
		InputActionReference _zoomAction;

		[Header("Navegação")]
		[SerializeField]
		InputActionReference _cancelAction;
		[SerializeField]
		InputActionReference _previousAction;
		[SerializeField]
		InputActionReference _nextAction;

		[Header("Especificos")]
		[SerializeField]
		ZoomInput zoomInput;

		InputAction SelectAction => _selectAction?.action;
		InputAction PositionAction => _positionAction?.action;
		InputAction MoveAction => _moveAction?.action;
		InputAction StartMoveAction => _startMoveAction?.action;
		InputAction CancelAction => _cancelAction?.action;
		InputAction PreviousAction => _previousAction?.action;
		InputAction NextAction => _nextAction?.action;
		InputAction ZoomAction => _zoomAction?.action;

		static InputsController instance;

        private bool _isMovingInternal;
		bool isZoomPerforming;

        public static bool IsMovingCamera
        {
            get
            {
                if (!instance)
					return false;

                return instance._isMovingInternal;
            }
        }

        /// <summary>
        /// Evento disparado quando o usuário tenta selecionar um alvo.
        /// </summary>
        /// <param name="mousePosition">Posição do mouse na tela.</param>
        public static event Action<Vector3> OnTrySelect;
		public static event Action<Vector2> OnMove;
		public static event Action<float> OnZoom;

		public static Vector2 PointerPosition
		{
			get
			{
				if (!instance)
					return Vector2.zero;

				return instance.PositionAction?.ReadValue<Vector2>() ?? Vector2.zero;
			}
		}

		public static event Action OnCancel;
		public static event Action OnPrevious;
		public static event Action OnNext;

		bool isOverUI;

		bool IsThisTheInstance()
			=> instance == this;

		void OnDisable()
		{
			SelectAction?.Disable();
			PositionAction?.Disable();
			MoveAction?.Disable();
			CancelAction?.Disable();
			PreviousAction?.Disable();
			NextAction?.Disable();
			StartMoveAction?.Disable();
			ZoomAction?.Disable();

			SelectAction.performed -= OnSelectPerformed;
			CancelAction.performed -= OnCancelPerformed;
			PreviousAction.performed -= OnPreviousPerformed;
			NextAction.performed -= OnNextPerformed;
			StartMoveAction.performed -= OnStartMovePerformed;
			StartMoveAction.started -= OnStartMoveStarted;
			ZoomAction.performed -= OnZoomPerformed;
			zoomInput.OnZoomChanged -= ZoomPerforme;
		}

		void OnEnable()
		{
			SelectAction?.Enable();
			PositionAction?.Enable();
			MoveAction?.Enable();
			CancelAction?.Enable();
			PreviousAction?.Enable();
			NextAction?.Enable();
			StartMoveAction?.Enable();
			ZoomAction?.Enable();

			SelectAction.performed += OnSelectPerformed;
			CancelAction.performed += OnCancelPerformed;
			PreviousAction.performed += OnPreviousPerformed;
			NextAction.performed += OnNextPerformed;
			StartMoveAction.performed += OnStartMovePerformed;
			StartMoveAction.started += OnStartMoveStarted;
			ZoomAction.performed += OnZoomPerformed;
			zoomInput.OnZoomChanged += ZoomPerforme;
		}

		void Awake()
		{
			if (instance && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
		}

		void OnMovePerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			if(isZoomPerforming)
				return;

			Vector2 movement = MoveAction.ReadValue<Vector2>();
			OnMove?.Invoke(movement);
		}

		void OnSelectPerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			OnTrySelect?.Invoke(PointerPosition);
		}

		void OnCancelPerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			OnCancel?.Invoke();
		}

		void OnPreviousPerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			OnPrevious?.Invoke();
		}

		void OnNextPerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			OnNext?.Invoke();
		}

		void OnStartMovePerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			MoveAction.performed -= OnMovePerformed;

            _isMovingInternal = false;
        }

		void OnStartMoveStarted(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			if (isOverUI)
				return;

			if(isZoomPerforming)
				return;

			MoveAction.performed += OnMovePerformed;

            _isMovingInternal = true;
		}

		void OnZoomPerformed(InputAction.CallbackContext context)
		{
			if (!IsThisTheInstance())
				return;

			if (isOverUI)
				return;

			float zoomDelta = ZoomAction.ReadValue<float>();
			ZoomPerforme(zoomDelta);
		}

		void ZoomPerforme(float delta)
		{
			//HACKME Referencia cruzada.
			if(CursorManager.Instance)
				CursorManager.Instance.NotifyZoomAction();

			
			OnZoom?.Invoke(delta);
		}

		void Update()
		{
			isOverUI = EventSystem.current.IsPointerOverGameObject();
			isZoomPerforming = zoomInput?.Update() ?? false;
		}
	}
}