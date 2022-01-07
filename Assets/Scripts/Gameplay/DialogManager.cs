using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public event Action OnDialogFinished;

    int currentLine = 0;
    bool isTyping = false;
    Dialog dialog;

    public bool IsShowing {
        get; private set;
    }

    public static DialogManager Instance {
        get; private set;
    }

    private void Awake() {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished=null) {
        yield return new WaitForEndOfFrame();
        OnShowDialog.Invoke();
        this.dialog = dialog;
        OnDialogFinished = onFinished;

        IsShowing = true;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line) {
        isTyping = true;
        dialogText.text = "";

        foreach (var letter in line.ToCharArray()) {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        isTyping = false;
    }

    public void HandleUpdate() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (!isTyping) {
                ++currentLine;
                if (currentLine < dialog.Lines.Count) {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                } else {
                    currentLine = 0;
                    IsShowing = false;
                    dialogBox.SetActive(false);
                    OnDialogFinished?.Invoke();
                    OnCloseDialog?.Invoke();
                }
            }
        }
    }
}
