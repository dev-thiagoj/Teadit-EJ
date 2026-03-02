using BMV.Dialogs;
using BMV.User;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BMV.Cursor
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        [Header("Configurações de Ícones (Texture2D)")]
        [SerializeField] List<CursorSetup> cursorMap;

        [Header("Configurações de Interação")]
        [SerializeField] float distanciaMaxRaycast = 100f;
        [SerializeField] LayerMask layerDeInteracao;
        [SerializeField] LayerMask layerDeAjuda;
        [SerializeField] LayerMask layerDeBloqueioUI;

        bool isMouseMode = false;

        // Armazena as texturas dos ícones para acesso rápido
        Dictionary<CursorType, Texture2D> cursorDictionary = new Dictionary<CursorType, Texture2D>();
        Texture2D padraoCursor; // Usado para 'None'
        Texture2D interativoCursor; // Usado para 'Interativo'
        Texture2D arrastarCursor; // Usado para 'Orbital'

        const CursorType CURSOR_TIPO_PADRAO = CursorType.None;

        [Header("Configurações de Zoom")]
        [SerializeField] float zoomCooldown = 0f;
        private const float ZOOM_ICON_DURATION = 0.2f;

        bool canChangeCursor = false;
        public bool CanChangeCursor
        {
            get => canChangeCursor;
            private set => canChangeCursor = value;
        }

        private void OnDestroy()
        {
            Textbox.OnClosed -= Init;
        }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (Application.isMobilePlatform)
            {
                enabled = false;
                UnityEngine.Cursor.visible = true;
                return;
            }

            isMouseMode = true;
        }

        void Start()
        {
            ResizeAllCursors();

            cursorDictionary.TryGetValue(CursorType.None, out padraoCursor);
            cursorDictionary.TryGetValue(CursorType.Orbital, out arrastarCursor);
            cursorDictionary.TryGetValue(CursorType.Interativo, out interativoCursor);

            Textbox.OnClosed += Init;

            SetCursor(CURSOR_TIPO_PADRAO);
        }

        private void Init()
        {
            CanChangeCursor = true;
        }

        void ResizeAllCursors()
        {
            // Configurações de Referência
            float alturaBase = 1080f;
            float alturaAtual = Screen.height;

            // Tamanho alvo visual (em pixels) para uma tela 1080p
            // 32 = Padrão Windows | 48 = Visível Jogos
            float tamanhoAlvoEm1080p = 48f;

            // Calcula a proporção da tela atual em relação a 1080p
            float screenScale = alturaAtual / alturaBase;

            // Descobre o tamanho original da imagem (pega o primeiro da lista como base)
            float tamanhoOriginal = 64f;
            if (cursorMap.Count > 0 && cursorMap[0].icon != null)
            {
                tamanhoOriginal = cursorMap[0].icon.width;
            }

            // Fórmula Final: (Tamanho Desejado / Tamanho Real) * Proporção da Tela
            float scaleFactor = (tamanhoAlvoEm1080p / tamanhoOriginal) * screenScale;

            // Loop para aplicar o resize e guardar no dicionário
            foreach (var setup in cursorMap)
            {
                if (setup.icon != null)
                {
                    // Redimensiona usando o método auxiliar da GPU
                    Texture2D iconeFinal = ResizeTexture(setup.icon, scaleFactor);

                    // Salva ou atualiza no dicionário
                    cursorDictionary[setup.type] = iconeFinal;
                }
            }
        }

        void LateUpdate()
        {
            if (!isMouseMode || !CanChangeCursor)
                return;

            if (zoomCooldown > 0)
            {
                zoomCooldown -= Time.deltaTime;
                SetCursor(CursorType.Zoom);
                return;
            }

            bool estaClicando = InputsController.IsMovingCamera;

            if (estaClicando)
            {
                SetCursor(CursorType.Orbital);
                return;
            }

            CheckInteraction();
        }

        void CheckInteraction()
        {
            bool uiBloqueando;
            CursorType uiCursor = GetUICursorType(out uiBloqueando);

            if (uiCursor != CursorType.None)
            {
                SetCursor(uiCursor);
                return;
            }

            if (uiBloqueando)
            {
                SetCursor(CURSOR_TIPO_PADRAO);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(InputsController.PointerPosition);
            RaycastHit hit;
            LayerMask maskCombinada = layerDeInteracao | layerDeAjuda;

            if (Physics.Raycast(ray, out hit, distanciaMaxRaycast, maskCombinada))
            {
                int layerAtingida = hit.collider.gameObject.layer;

                if (((1 << layerAtingida) & layerDeInteracao) != 0)
                {
                    SetCursor(CursorType.Interativo);
                }
                else if (((1 << layerAtingida) & layerDeAjuda) != 0)
                {
                    SetCursor(CursorType.Help);
                }
            }
            else
            {
                SetCursor(CURSOR_TIPO_PADRAO);
            }
        }

        public void NotifyZoomAction()
        {
            zoomCooldown = ZOOM_ICON_DURATION;
            SetCursor(CursorType.Zoom);
        }

        CursorType GetUICursorType(out bool bloqueia3D)
        {
            bloqueia3D = false;

            if (EventSystem.current == null) return CursorType.None;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = InputsController.PointerPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                int layerObjeto = result.gameObject.layer;
                int layerMaskObjeto = 1 << layerObjeto;

                if ((layerMaskObjeto & layerDeInteracao) != 0)
                {
                    bloqueia3D = true;
                    return CursorType.Interativo;
                }

                if ((layerMaskObjeto & layerDeAjuda) != 0)
                {
                    bloqueia3D = true;
                    return CursorType.Help;
                }

                if ((layerMaskObjeto & layerDeBloqueioUI) != 0)
                    bloqueia3D = true; 
            }

            return CursorType.None;
        }

        void SetCursor(CursorType tipo)
        {
            if (cursorDictionary.ContainsKey(tipo) && cursorDictionary[tipo] != null)
            {
                Texture2D cursorTexture = cursorDictionary[tipo];
                Vector2 hotspot = GetHotspot(tipo, cursorTexture);
                UnityEngine.Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
            }
            else if (tipo == CURSOR_TIPO_PADRAO)
                UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        Vector2 GetHotspot(CursorType type, Texture2D texture)
        {
            if (type == CursorType.Interativo)
                return new Vector2(texture.width / 3f, texture.height / 5f);

            return Vector2.zero;
        }

        Texture2D ResizeTexture(Texture2D source, float scaleFactor)
        {
            if (source == null) 
                return null;

            int newWidth = Mathf.RoundToInt(source.width * scaleFactor);
            int newHeight = Mathf.RoundToInt(source.height * scaleFactor);

            if (newWidth < 1) 
                newWidth = 1;
            if (newHeight < 1) 
                newHeight = 1;

            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            RenderTexture.active = rt;

            // Copia a imagem original para o RenderTexture (isso redimensiona)
            Graphics.Blit(source, rt);

            Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return result;
        }
    }

    [Serializable]
    public enum CursorType
    {
        None,
        Orbital,
        Zoom,
        Interativo,
        Help
    }

    [Serializable]
    public class CursorSetup
    {
        public CursorType type;
        public Texture2D icon;
    }
}
