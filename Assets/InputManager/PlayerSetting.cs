#region [Copyright (c) 2018 Cristian Alexandru Geambasu]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2018 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
using UnityEngine;

namespace Luminosity.IO.Examples {
    public class PlayerSetting : MonoBehaviour {
        [Header("Amount of Input Detected")]
        public string[] names;

        [Header("Who is Choosing Player")]
        public int ChoosePlayer = 0;
        public bool isSinglePlayer = false;
        public bool isChoosing = true;

        [Header("Player set to variable")]
        public int player1 = 99;
        [SerializeField] private string p1ControllerName;
        public int player2 = 99;
        [SerializeField] private string p2ControllerName;

        [Header("Detection Variable")]
        [SerializeField] private char detectedNumber;
        [SerializeField] private string detectedInput;

        void Start() { }

        void Update() {
            names = Input.GetJoystickNames();

            if (isChoosing) {
                System.Array values = System.Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode code in values) {
                    if (Input.GetKeyDown(code)) {
                        if (System.Enum.GetName(typeof(KeyCode), code).Contains("Joystick")) {
                            //print(System.Enum.GetName(typeof(KeyCode), code));
                            detectedInput = System.Enum.GetName(typeof(KeyCode), code).ToString();
                            detectedNumber = System.Enum.GetName(typeof(KeyCode), code).ToString()[8];
                        }
                    }
                }
                if (!isSinglePlayer) {
                    SetPlayer();
                }
                else {
                    SetSinglePlayer();
                }
            } 
        }

        void SetSinglePlayer() {
            print("Set Single Player");
            int controlInput = 99;
            int.TryParse(detectedNumber.ToString(), out controlInput);

            if (InputManager.anyKeyDown && controlInput >= 0) {

                if (detectedInput.Contains("Joystick"))
                    p1ControllerName = names[controlInput - 1];
                else
                    return;

                InputManager.GetControlScheme(PlayerID.One).ChangeJoystick(controlInput - 1);
                player1 = System.Convert.ToInt16(controlInput - 1);
                SetControllerScheme(p1ControllerName, PlayerID.One);
                ChoosePlayer++;
            }
        }

        void SetPlayer() {
            int controlInput = 99;
            int.TryParse(detectedNumber.ToString(), out controlInput);

            switch (ChoosePlayer) {
                case 0:
                    if (InputManager.anyKeyDown && controlInput >= 0) {

                        if (detectedInput.Contains("Joystick"))
                            p1ControllerName = names[controlInput - 1];
                        else
                            return;

                        InputManager.GetControlScheme(PlayerID.One).ChangeJoystick(controlInput - 1);
                        player1 = System.Convert.ToInt16(controlInput - 1);
                        SetControllerScheme(p1ControllerName, PlayerID.One);
                        ChoosePlayer++;
                    }
                    break;
                case 1:
                    if (InputManager.anyKeyDown && controlInput >= 0) {
                        if (int.Parse(detectedNumber.ToString()) != player1 + 1) {

                            if (detectedInput.Contains("Joystick"))
                                p2ControllerName = names[controlInput - 1];
                            else
                                return;

                            InputManager.GetControlScheme(PlayerID.Two).ChangeJoystick(controlInput - 1);
                            player2 = System.Convert.ToInt16(controlInput - 1);
                            SetControllerScheme(p2ControllerName, PlayerID.Two);
                            isChoosing = false;
                        }
                    }
                    break;
            }
        }

        void SetControllerScheme(string controllerName, PlayerID playerID) {
            if (controllerName == "Controller (XBOX 360 For Windows)") {
                print(playerID + " Changed to XBOX Input");
                InputManager.GetControlScheme(playerID).GetAction("Fire1").GetBinding(0).Type = InputType.AnalogButton;
                InputManager.GetControlScheme(playerID).GetAction("Fire1").GetBinding(0).Axis = 9;

                InputManager.GetControlScheme(playerID).GetAction("Sprint").GetBinding(0).Type = InputType.AnalogButton;
                InputManager.GetControlScheme(playerID).GetAction("Sprint").GetBinding(0).Axis = 8;

                InputManager.GetControlScheme(playerID).GetAction("PauseMenu").GetBinding(0).Type = InputType.Button;
                InputManager.GetControlScheme(playerID).GetAction("PauseMenu").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button7");

                InputManager.GetControlScheme(playerID).GetAction("Upgrade").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button3");
                InputManager.GetControlScheme(playerID).GetAction("Skill").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button2");

                if (playerID == PlayerID.One) {
                    InputManager.GetControlScheme(playerID).GetAction("UI_Submit").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button0");
                    InputManager.GetControlScheme(playerID).GetAction("UI_Cancel").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button1");

                    InputManager.GetControlScheme(playerID).GetAction("UI_Up").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Up").GetBinding(1).Axis = 6;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Down").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Down").GetBinding(1).Axis = 6;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Left").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Left").GetBinding(1).Axis = 5;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Right").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Right").GetBinding(1).Axis = 5;
                }
            }
            else if (controllerName == "Wireless Controller") {
                print(playerID + " Changed to PS4 Input");
                InputManager.GetControlScheme(playerID).GetAction("Fire1").GetBinding(0).Type = InputType.Button;
                InputManager.GetControlScheme(playerID).GetAction("Fire1").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button7");

                InputManager.GetControlScheme(playerID).GetAction("Sprint").GetBinding(0).Type = InputType.Button;
                InputManager.GetControlScheme(playerID).GetAction("Sprint").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button6");

                InputManager.GetControlScheme(playerID).GetAction("PauseMenu").GetBinding(0).Type = InputType.Button;
                InputManager.GetControlScheme(playerID).GetAction("PauseMenu").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button9");

                InputManager.GetControlScheme(playerID).GetAction("Upgrade").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button0");
                InputManager.GetControlScheme(playerID).GetAction("Skill").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button3");

                if (playerID == PlayerID.One) {
                    InputManager.GetControlScheme(playerID).GetAction("UI_Submit").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button1");
                    InputManager.GetControlScheme(playerID).GetAction("UI_Cancel").GetBinding(0).Positive = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + detectedNumber + "Button2");

                    InputManager.GetControlScheme(playerID).GetAction("UI_Up").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Up").GetBinding(1).Axis = 7;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Down").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Down").GetBinding(1).Axis = 7;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Left").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Left").GetBinding(1).Axis = 6;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Right").GetBinding(1).Type = InputType.AnalogButton;
                    InputManager.GetControlScheme(playerID).GetAction("UI_Right").GetBinding(1).Axis = 6;
                }
            }
        }
    }
}