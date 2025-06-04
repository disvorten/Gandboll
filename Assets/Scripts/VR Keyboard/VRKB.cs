using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class VRKB : MonoBehaviour
{
    public Action<AutofillUserData> AutofillTriggered;

    [Header("Fill or leave default")]
    [SerializeField] private Transform linesLayout;
    [SerializeField] private Transform autofillLayout;
    [SerializeField] private List<string> linesForButtons;
    [SerializeField] private TMPro.TMP_InputField targetTextField;
    [SerializeField] private GameObject linePrefab, buttonPrefab, autofillButtonPrefab;
    [SerializeField] private float lineWidthOverride, lineHeightOverride, buttonWidthOverride, buttonHeightOverride;
    [SerializeField] private VRKB_SymbolButton spacebar;
    [SerializeField] private bool activateOnStart = false, autofillOn = false;

    [Header("Always fill")]
    [SerializeField] private List<TMPro.TMP_InputField> inputFields;
    [SerializeField] private List<GameObject> outlines;

    private List<VRKB_Line> lineScripts = new List<VRKB_Line>();
    private bool capsLockIsOn = false;
    private List<GameObject> autofillButtons = new List<GameObject>();    
    List<AutofillUserData> autofillData = new List<AutofillUserData>();

    public Action OnClose;

    private void Start()
    {
        Init();
        if (activateOnStart)
        {
            Open(0);
        }
        else
        {
            Close();
        }
    }

    public void Init()
    {
        ReadAutofillData(Path.Join(Application.persistentDataPath, "Autofill.txt"));

        if (linesForButtons != null && linesForButtons.Count > 0)
        {
            foreach (string line in linesForButtons)
            {
                if (line != null && line.Length > 0)
                {
                    GameObject go = Instantiate(linePrefab, linesLayout);
                    VRKB_Line lineScript = go.GetComponent<VRKB_Line>();
                    if (lineScript)
                    {
                        lineScripts.Add(lineScript);
                        lineScript.Init(line, lineWidthOverride, lineHeightOverride, buttonWidthOverride, buttonHeightOverride,
                            this, buttonPrefab);
                    }
                }
            }
        }

        if (spacebar != null)
        {
            spacebar.Init(" ", "Пробел", 0, 0, this);
        }
        foreach (GameObject outline in outlines)
        {
            outline.SetActive(false);
        }
    }

    public void Open(int listElement)
    {
        gameObject.SetActive(true);
        ClearAutofillButtons();
        targetTextField = inputFields[listElement];
        targetTextField.text = "";
        foreach (GameObject outline in outlines)
        {
            outline.SetActive(false);
        }
        outlines[listElement].SetActive(true);
        CapsLockOn();
    }

    public void Close()
    {
        foreach (GameObject outline in outlines)
        {
            outline.SetActive(false);
        }
        targetTextField = null;
        ClearAutofillButtons();
        autofillOn = false;
        gameObject.SetActive(false);
        OnClose?.Invoke();
    }

    public void CapsLockOn()
    {
        capsLockIsOn = true;

        if (lineScripts == null || lineScripts.Count == 0)
        {
            return;
        }

        foreach (var lineScript in lineScripts)
        {
            lineScript.CapsLockOn();
        }
    }

    public void CapsLockOff()
    {
        capsLockIsOn = false;

        if (lineScripts == null || lineScripts.Count == 0)
        {
            return;
        }

        foreach (var lineScript in lineScripts)
        {
            lineScript.CapsLockOff();
        }
    }

    public void SymbolButtonPressed(string symbol)
    {
        if (targetTextField)
        {
            targetTextField.text = targetTextField.text + symbol;
            if (targetTextField.text.Length == 1) 
            {
                CapsLockOff();
            }
        }
        DoAutofill();
    }

    public void CapsLockPressed()
    {
        if (capsLockIsOn)
        {
            CapsLockOff();
        }
        else
        {
            CapsLockOn();
        }
    }

    public void BackspacePressed()
    {
        if (targetTextField.text == null || targetTextField.text.Length == 0)
        {
            return;
        }

        targetTextField.text = targetTextField.text.Substring(0, targetTextField.text.Length - 1);
        DoAutofill();
    }

    public void EnterPressed()
    {
        Close();
    }

    #region Autofill
    private void DoAutofill() 
    {
        if (!autofillOn)
        {
            return;
        }

        // Get suitable variants
        List<AutofillUserData> variants = new List<AutofillUserData>();
        if (targetTextField.text.Length > 0)
        {
            foreach (AutofillUserData entry in autofillData)
            {
                if (entry.name.StartsWith(targetTextField.text, StringComparison.OrdinalIgnoreCase))
                {
                    variants.Add(entry);
                }
            }
        }

        // Destroy old buttons
        ClearAutofillButtons();

        // Spawn new buttons
        foreach (AutofillUserData variant in variants)
        {
            GameObject go = Instantiate(autofillButtonPrefab, autofillLayout);
            autofillButtons.Add(go);
            // Initialize button with variant text
            VRKB_AutofillButton buttonScript = go.GetComponent<VRKB_AutofillButton>();
            if (buttonScript)
            {
                buttonScript.Init(variant, variant.name, this);
            }
        }
    }

    private void ClearAutofillButtons()
    {
        foreach (GameObject old in autofillButtons)
        {
            Destroy(old);
        }
        autofillButtons = new List<GameObject>();
    }

    private void ReadAutofillData(string filepath)
    {
        autofillData = new List<AutofillUserData>();
        try
        {
            List<string> autofillLines = File.ReadAllLines(filepath).ToList();
            foreach (string line in autofillLines)
            {
                string[] splitLine = line.Split(",");
                if (splitLine.Length > 0)
                {
                    AutofillUserData newEntry = new AutofillUserData();
                    newEntry.name = splitLine[0];
                    newEntry.id = "";
                    newEntry.rank = "";
                    if (splitLine.Length > 1)
                    {
                        newEntry.id = splitLine[1];
                    }
                    if (splitLine.Length > 2)
                    {
                        newEntry.rank = splitLine[2];
                    }
                    autofillData.Add(newEntry);
                }
            }
        }
        catch { }
    }

    public void AutofillButtonPressed(AutofillUserData data)
    {
        AutofillTriggered?.Invoke(data);
        ClearAutofillButtons();
    }

    public struct AutofillUserData
    {
        public string name, id, rank;
    }
    #endregion Autofill
}
