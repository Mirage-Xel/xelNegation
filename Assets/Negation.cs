using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
public class Negation : MonoBehaviour
{
    public KMBombModule module;
    public KMBombInfo bomb;
    public KMAudio sound;
    public KMSelectable butTrue;
    public KMSelectable butFalse;
    public TextMesh[] displayTexts;
    string[] negations = new string[] { "¬", "¬¬", "¬¬¬", "¬¬¬¬" };
    string[] statements = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };
    int stageCounter;
	public class display
    {
        public int negationIndex;
        public int statementIndex;
        public bool truth;
    }
    public display[] displays = new display[3];
    bool moduleSolved, hasStruck;
    int moduleId;
    static int moduleIdCounter = 1;
    // Use this for initialization
    void Awake()
    {
        moduleId = moduleIdCounter++;
        butTrue.OnInteract += delegate () { pressBut(true); return false; };
        butFalse.OnInteract += delegate () { pressBut(false); return false; };
    }
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            displays[i] = new display();
            displays[i].negationIndex = rnd.Range(0, 4);
            displays[i].statementIndex = rnd.Range(0, 16);
            displayTexts[i].text = "";
            var toDisplay = negations[displays[i].negationIndex] + statements[displays[i].statementIndex];
            StartCoroutine(TypeText(displayTexts[i], toDisplay));
            Debug.LogFormat("[Negation #{0}] Display {1} is {2}", moduleId, i+1, toDisplay);
        }
        foreach (display display in displays)
        {
            handleDisplay(display);
        }
		Debug.LogFormat("[Negation #{0}] The expected truth values are {1}, {2}, and {3}", moduleId, displays[0].truth, displays[1].truth, displays[2].truth);
    }
    IEnumerator TypeText(TextMesh textToDisplay, string value)
    {
        yield return null;
        for (int x = 0; x < value.Length; x++)
        {
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
            textToDisplay.text = value.Substring(0, x + 1);
            yield return new WaitForSeconds(0.05f);
        }
    }
    void handleDisplay(display display)
    {
        switch (display.statementIndex)
        {
            case 0:
                if (bomb.GetBatteryCount() > 2)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 1:
                if (bomb.IsIndicatorPresent("FRQ") || bomb.IsIndicatorPresent("TRN"))
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 2:
                if (bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'E' || x == 'I' || x == 'O' || x == 'U'))
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 3:
                if (bomb.GetPortCount(Port.RJ45) >= 1)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 4:
                if (bomb.GetSerialNumberNumbers().Last() % 2 == 1)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 5:
                if (bomb.GetOnIndicators().Count() > bomb.GetOffIndicators().Count())
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 6:
                if (bomb.GetPortCount(Port.Parallel) >= 1)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 7:
                if (bomb.GetPortCount(Port.StereoRCA) >= 1)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 8:
                if (bomb.IsIndicatorOn("BOB"))
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 9:
                if (bomb.GetBatteryCount(Battery.D) >= 1)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 10:
                if (bomb.IsIndicatorOff("FRK"))
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 11:
                if (bomb.GetBatteryCount() % 2 == 0)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 12:
                if (bomb.GetSerialNumberNumbers().Last() > 5)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 13:
                if (bomb.GetPortCount() > 2)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 14:
                if (bomb.IsIndicatorPresent("SIG"))
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
            case 15:
                if (bomb.GetPortCount(Port.DVI) >= 1)
                {
                    display.truth = true;
                }
                else
                {
                    display.truth = false;
                }
                break;
        }
        if ((display.negationIndex % 2) == 0)
        {
            display.truth = !display.truth;
        }
    }		
    void pressBut(bool but)
    {
        if (!moduleSolved)
        {
            GetComponent<KMSelectable>().AddInteractionPunch(.5f);
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
            Debug.LogFormat("[Negation #{0}] You pressed the {1} button.", moduleId, but);
            if (but == displays[stageCounter].truth)
            {
                displayTexts[stageCounter].text = "";
                if (stageCounter < 2)
                {
                    stageCounter++;
                    Debug.LogFormat("[Negation #{0}] That was correct.", moduleId);
                }
                else
                {
                    module.HandlePass();
                    moduleSolved = true;
                    Debug.LogFormat("[Negation #{0}] Module solved.", moduleId);
                    sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    // Solve anim courtsey of VFlyer
                    var solveText = new[] { "Xel's", "Nega", "tion" };
                    for (int x = 0; x < displayTexts.Length; x++)
                    {
                        StartCoroutine(TypeText(displayTexts[x], solveText[x]));
                    }
                }
            }
            else
            {
                hasStruck = true; // Tell the TP command to stop running the command since a strike is being received.
                module.HandleStrike();
                Debug.LogFormat("[Negation #{0}] That was incorrect. Strike", moduleId);
                stageCounter = 0;
                Start();
            }
        }
    }
#pragma warning disable 414
    private string TwitchHelpMessage = "Use \"!{0} press true/false true/false true/false\" to press the true and false buttons.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        string[] commandArray = command.Split(' ');
        if (commandArray[0] != "press"||commandArray.Length != 4)
        {
            yield return "sendtochaterror Invalid command.";
            yield break;
        }
        hasStruck = false; // Set the value to false to allow the for-loop underneath to process.
        for (int i = 0; i < commandArray.Length && !hasStruck; i++)
        {
            string[] validCommands = new string[3] { "press", "true", "false"};
            if (!validCommands.Contains(commandArray[i]))
            {
                yield return "sendtochaterror Invalid command.";
                yield break;
            }
            if (commandArray[i] == "true")
            {
                yield return null;
                butTrue.OnInteract();
            }
            if (commandArray[i] == "false")
            {
                yield return null;
                butFalse.OnInteract();
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
        for (int i1 = stageCounter; i1 < displays.Length; i1++) // Only press based on what current stage it is on.
        {
            display i = displays[i1];
            if (i.truth)
            {
                yield return null;
                butTrue.OnInteract();
            }
            else
            {
                yield return null;
                butFalse.OnInteract();
            }
        }
    }

}