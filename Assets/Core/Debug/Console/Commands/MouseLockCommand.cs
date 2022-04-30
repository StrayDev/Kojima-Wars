using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Console
{
    public class MouseLockCommand : IConsoleCommand
    {
        public string Description() => "/Mouse  :  Toggles Mouse Lock To Window. \n";

        public void Execute(string[] args)
        {
            if(Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        public bool IsHidden() => false;
        public string Name() => "mouse";

    }
}

