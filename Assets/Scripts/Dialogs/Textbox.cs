using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BMV.Dialogs
{
    public class Textbox : MonoBehaviour
    {
        [SerializeField]
        TMP_Text text;

        [SerializeField]
        Animator animator;

        [Header("Parameters")]
        [Range(0.01f, 0.1f)]
        [SerializeField] float typingSpeed = 0.75f;

        bool isWritting;
        bool isStarted = false;
        bool speedUp;
        YieldInstruction waitInstruction;
        int openParameter;

        public static event Action OnClosed;

        bool IsOpened
        {
            get => animator.GetBool(openParameter);
            set
            {
                animator.SetBool(openParameter, value);

                if (value)
                    return;

                StartCoroutine(WaitForCloseAnimation());
            }
        }

        void Awake()
        {
            openParameter = animator.parameters[0].nameHash;
            //Close();
        }

#if UNITY_EDITOR
        [ContextMenu("Abrir")]
#endif
        public void Open()
        {
            IsOpened = true;
        }

        IEnumerator WaitForCloseAnimation()
        {
            yield return null;

            float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;

            var amount = animationDuration + 1;
            yield return new WaitForSeconds(amount);

            OnClosed?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isStarted)
            {
                Show();
                return;
            }

            if (!isWritting)
            {
                Next();
                return;
            }

            if (!speedUp)
            {
                speedUp = true;
                waitInstruction = null;
                return;
            }

            if (speedUp)
            {
                StopAllCoroutines();
                text.maxVisibleCharacters = text.textInfo.characterCount;
                isWritting = false;
                return;
            }
        }

        void Close()
        {
            IsOpened = false;
            //OnClosed?.Invoke();
        }

        public void Next()
        {
            speedUp = false;

            if (text.pageToDisplay < text.textInfo.pageCount)
            {
                Debug.Log($"Going to page {text.pageToDisplay + 1}");
                text.pageToDisplay++;
                StartTipping(text.textInfo.pageInfo[text.pageToDisplay]);
                return;
            }

            Close();
        }

        public void FastForward()
        {
            speedUp = true;
            waitInstruction = null;
        }

        void Show()
        {
            isStarted = true;
            text.maxVisibleCharacters = 0;

            if (text.overflowMode is TextOverflowModes.Page)
            {
                int page = text.pageToDisplay;
                TMP_PageInfo pageInfo = text.textInfo.pageInfo[page];
                StartTipping(pageInfo);
                return;
            }

            StartTipping();
        }

        IEnumerator TippingCoroutine(int quantity)
        {
            yield return null;
#if UNITY_EDITOR
            Debug.Log($"Tipping to {quantity} characters.");
#endif
            isWritting = true;
            waitInstruction = new WaitForSeconds(typingSpeed);

            while (text.maxVisibleCharacters <= quantity)
            {
                text.maxVisibleCharacters++;
                yield return waitInstruction;
            }
            isWritting = false;
        }

        void StartTipping()
        {
            StartCoroutine(TippingCoroutine(text.textInfo.characterCount));
        }

        void StartTipping(TMP_PageInfo pageInfo)
        {
#if UNITY_EDITOR
            Debug.Log($"Page {text.pageToDisplay}/{text.textInfo.pageCount}");
#endif
            StartCoroutine(TippingCoroutine(pageInfo.lastCharacterIndex));
        }

        void AE_OnClosed()
        {
            return;

            if (!isStarted)
                return;

            OnClosed?.Invoke();
        }

#if UNITY_EDITOR
        void Reset()
        {
            text = GetComponentInChildren<TMP_Text>(true);
            animator = GetComponent<Animator>();
        }
#endif
    }
}
