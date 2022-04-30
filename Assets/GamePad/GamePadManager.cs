using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadManager : MonoBehaviour
{
    [SerializeField] Gamepad gamepad;

    bool gamePadConnected;

    [SerializeField] GameObject cursor;

    void Start()
    {
        //Debug.Log("IT'S ALIVE!!!");

        cursor.SetActive(false);

        if (Gamepad.current != null)
        {
            gamepad = Gamepad.current;
            //Debug.Log("Gamepad already connected");
            gamePadConnected = true;
            cursor.SetActive(true);
        }
        else
        {
            //Debug.Log("No gamepad connected");
        }

        InputSystem.onDeviceChange += (device, change) =>
        {
            //if(device.GetType() == )
            if (device.GetType() == typeof(Mouse)) return; 

            switch (change)
            {
                case InputDeviceChange.Added:
                    Debug.Log("New device added: " + device + " , type: " + device.GetType());
                    gamePadConnected = true;
                    gamepad = (Gamepad)device;
                    cursor.SetActive(true);

                    break;

                case InputDeviceChange.Reconnected:
                    Debug.Log("Device reconnected: " + device + " , type: " + device.GetType());
                    gamePadConnected = true;
                    gamepad = (Gamepad)device;
                    cursor.SetActive(true);

                    break;

                case InputDeviceChange.Removed:
                    Debug.Log("Device removed: " + device + " , type: " + device.GetType());
                    gamePadConnected = false;
                    //gamepad = (Gamepad)device;
                    gamepad = null;
                    cursor.SetActive(false);

                    break;
            }
        };
    }

    public Gamepad GetGamepad()
    {
        return gamepad;
    }

    public bool GamepadConnected()
    {
        return gamePadConnected;
    }
}
