using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    private bool showConsole;
    private string input;
    private bool showHelp;

    public static DebugCommand HELP;
    public static DebugCommand TOGGLE_FPS;
    public static DebugCommand<int> ADD_LUNES;

    public List<object> commandList;

    public void OnToggleDebug(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            showConsole = !showConsole;
            if (showConsole)
            {
                PlayerController.Instance.currentState = Controller.characterStates.paused;
            }
            else
            {
                PlayerController.Instance.currentState = Controller.characterStates.active;
            }
        }
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(showConsole)
            {
                HandleInput();
                input = "";
            }
        }
    }

    private void Awake()
    {
        HELP = new DebugCommand("help", "Shows a list of commands.", "help", () =>
        {
            showHelp = true;
        });

        TOGGLE_FPS = new DebugCommand("toggle_fps", "Toggles the framerate overlay.", "toggle_fps", () =>
        {
            FPSOverlay.Instance.ToggleOverlay();
        });

        ADD_LUNES = new DebugCommand<int>("add_lunes", "Adds a specified amount of lunes to your inventory.", "add_lunes <lunes_amount>", (x) =>
        {
            //Add money
        });

        commandList = new List<object>
        {
            HELP,
            TOGGLE_FPS,
            ADD_LUNES
        };

    }

    Vector2 scroll;
    
    private void OnGUI()
    {
        if (!showConsole)
        {
            return;
        }

        float height = 40;
        float width = 400;
        float y = Screen.height / 2f - height / 2f;
        float x = Screen.width / 2f - width / 2f;

        GUI.Box(new Rect(x, y, width, height), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        input = GUI.TextField(new Rect(x + 10f, y + height / 4f, width - 20f, height - 10f), input);

        y += 50;

        if (showHelp)
        {
            GUI.backgroundColor = new Color(0, 0, 0, 1f);
            GUI.Box(new Rect(x, y, width, 210f), "");
            Rect viewport = new Rect(x, y, width - 20f, height * commandList.Count);

            scroll = GUI.BeginScrollView(new Rect(x, y + 10f, width, 190f), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.commandFormat} - {command.commandDescription}";

                Rect labelRect = new Rect(x + 5f, y + 20f * i, viewport.width - 100f, 20f);

                GUI.Label(labelRect, label);
            }

            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.EndScrollView();            
        }
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.commandID))
            {
                if (commandList[i] is DebugCommand command)
                {
                    command.Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }
}
