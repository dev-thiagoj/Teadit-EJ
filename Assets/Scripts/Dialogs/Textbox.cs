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

        //[SerializeField] Decision currentDecision;
        [SerializeField] DialogueContent content;
        [SerializeField] List<Button> decisionButtons = new();
        [SerializeField] GameObject buttonPrefab;
        [SerializeField] Transform btnsContainer;

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
        //public static UnityEvent<Decision> OnNewDecisionCalled = new();
        public static UnityEvent<int> OnTextBoxDecisonTaked = new();

        public DialogueContent Content
        {
            get => content;
            set
            {
                content = value;
                text.text = content.Question;
            }
        }

        bool IsOpened
        {
            get => animator.GetBool(openParameter);
            set => animator.SetBool(openParameter, value);
        }

#if UNITY_EDITOR
        [ContextMenu("Abrir")]
#endif
        public void Open()
        {
            RefreshFromContext();
            IsOpened = true;
        }

        void Close()
        {
            //currentDecision = null;
            IsOpened = false;
        }

        void Next()
        {
            speedUp = false;

            if (text.pageToDisplay < text.textInfo.pageCount)
            {
                //Debug.Log($"Going to page {text.pageToDisplay + 1}");
                text.pageToDisplay++;
                StartTipping(text.textInfo.pageInfo[text.pageToDisplay]);
                return;
            }

            OnClosed?.Invoke();
            Close();
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
            //#if UNITY_EDITOR
            //            Debug.Log($"Tipping to {quantity} characters.");
            //#endif
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
            //#if UNITY_EDITOR
            //            Debug.Log($"Page {text.pageToDisplay}/{text.textInfo.pageCount}");
            //#endif
            StartCoroutine(TippingCoroutine(pageInfo.lastCharacterIndex));
        }

        void RefreshFromContext()
        {
            //text.text = content.Question;
            //text.pageToDisplay = 1;
            //text.maxVisibleCharacters = 0;
            //text.ForceMeshUpdate();

            //for (int i = 0; i < decisionButtons.Count; i++)
            //{
            //    Button button = decisionButtons[i];
            //    var text = button.GetComponentInChildren<TMP_Text>();
            //    text.text = content.Answers[i];
            //    int index = i;
            //    decisionButtons[i].onClick.AddListener(() => ReceiveDecision(index));
            //}
        }

        void Awake()
        {
            openParameter = animator.parameters[0].nameHash;
            Close();
        }

        private IEnumerator Start()
        {
            //OnNewDecisionCalled.AddListener(OnNewDecisionComes);
            //OnTextBoxDecisonTaked.AddListener(ReceiveEntryDecision);

            //var startDecision = GetComponent<Decision>();
            //OnNewDecisionComes(startDecision);

            //         for (int i = 0; i < decisionButtons.Count; i++)
            //{
            //	int index = i;
            //	decisionButtons[i].onClick.AddListener(() => ReceiveDecision(index));
            //}

            yield return new WaitForSeconds(2);
            Open();
        }

        void ReceiveEntryDecision(int index)
        {
            if (index == 0)
            {
                speedUp = true;
                waitInstruction = null;
                return;
            }

            if (index == 1)
                Next();
        }

        //void OnNewDecisionComes(Decision decision)
        //{
        //    //Debug.Log($"New decision comes: {decision.Description.Question}");

        //    currentDecision = decision;
        //    Content = decision.Description;

        //    if (decisionButtons.Count > 0)
        //    {
        //        foreach (var btn in decisionButtons)
        //            Destroy(btn.gameObject);
        //    }

        //    decisionButtons.Clear();
        //    var buttonsToUse = decision.Description.Answers.Length;

        //    //Debug.Log($"Creating {buttonsToUse} buttons for the decision.");

        //    for (int i = 0; i < buttonsToUse; i++)
        //    {
        //        var btn = Instantiate(buttonPrefab, btnsContainer);
        //        decisionButtons.Add(btn.GetComponent<Button>());
        //    }

        //    Open();
        //}

        public void ReceiveDecision(int index)
        {
            //if (currentDecision == null)
            //    return;

            //bool isEntryPoint = currentDecision.Description.Id != 0;

            //currentDecision.DecisionTaked(index, isEntryPoint);

            //if (isEntryPoint)
            //    return;

            //Close();
        }

        void AE_OnClosed()
        {
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
