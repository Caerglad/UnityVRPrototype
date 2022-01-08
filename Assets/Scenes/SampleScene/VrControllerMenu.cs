using System;
using System.Collections.Generic;
using Scenes.MainMenu;
using UnityEngine;
using Valve.VR;

namespace Scenes.SampleScene {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(GameObject))]
    [RequireComponent(typeof(VrItemsMenu))]
    public class VrControllerMenu : MonoBehaviour {
        public Camera vrCamera;
        public VrItemsMenu vrItemsMenu;
        public SceneLoader sceneLoader;
        public GameObject menuOptionPrefab;

        public SteamVR_Behaviour_Pose pose;
        public SteamVR_Action_Boolean invokeMenu = SteamVR_Input.GetBooleanAction("InvokeMenu");
        public SteamVR_Action_Boolean rightAButton = SteamVR_Input.GetBooleanAction("rightAButton");
        public SteamVR_Action_Boolean rightJoystickNorth = SteamVR_Input.GetBooleanAction("RightJoystickNorth");
        public SteamVR_Action_Boolean rightJoystickSouth = SteamVR_Input.GetBooleanAction("RightJoystickSouth");

        private GameObject menu;
        private List<VrRadialMenuOption> menuOptions;
        private MenuOptionController menuOptionController;

        private const float SingleCellDistance = 0.15f;

        private static readonly Vector3 ChosenOptionEnlargeSize = new Vector3(0.001f, 0.001f);
        private static readonly Vector3 DistanceFromParent = new Vector3(0.05f, 0.05f, 0.3f);

        private void Start() {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);
            if (invokeMenu == null)
                Debug.LogError("No invoke menu action has been set up for this component", this);
            if (rightJoystickNorth == null)
                Debug.LogError("No right joystick north action has been set on this component.", this);
            if (rightJoystickSouth == null)
                Debug.LogError("No right joystick south action has been set on this component.", this);

            CreateMenu();
        }

        private void CreateMenu() {
            menu = new GameObject("Vr Controller Menu");
            menu.SetActive(false);

            menuOptions = new List<VrRadialMenuOption> {
                new OpenItemsMenuOption(vrItemsMenu, menu),
                new ExitGameOption(sceneLoader)
            };

            var cellYPosition = DistanceFromParent.y;
            foreach (var option in menuOptions) {
                var cell = Instantiate(menuOptionPrefab, menu.transform);
                cell.name = "Menu Option Cell";
                cell.transform.position = new Vector3(0f, cellYPosition);
                cellYPosition -= SingleCellDistance;

                var textHolder = cell.transform.Find("TextHolder").GetComponent<TextMesh>();
                textHolder.text = option.text;
                option.Init(cell, textHolder);
            }

            menuOptionController = new MenuOptionController(menuOptions);
        }

        private void Update() {
            if (!invokeMenu.GetState(pose.inputSource)) {
                if (menu.activeSelf)
                    menu.SetActive(false);

                return;
            }

            if (invokeMenu.GetState(pose.inputSource) && vrItemsMenu.active)
                return;

            if (!menu.activeSelf)
                menu.SetActive(true);

            UpdateMenuPositionAndRotation();
            UpdatePossibleMenuClicks();
        }

        private void UpdateMenuPositionAndRotation() {
            var menuPosition = transform.position + DistanceFromParent;

            menu.transform.position = menuPosition;
            menu.transform.LookAt(vrCamera.transform);

            if (rightJoystickSouth.GetState(pose.inputSource)) {
                menuOptionController.ChooseNext();
            } else if (rightJoystickNorth.GetState(pose.inputSource)) {
                menuOptionController.ChoosePrevious();
            }
        }

        private void UpdatePossibleMenuClicks() {
            if (rightAButton.GetState(pose.inputSource)) {
                menuOptionController.ClickCurrent();
            }
        }

        [Serializable]
        private abstract class VrRadialMenuOption {
            public string text { get; }

            private GameObject cell;
            private bool initialized;
            private TextMesh cellTextHolder;

            protected VrRadialMenuOption(string text) {
                this.text = text;
            }

            public void Init(GameObject optionCell, TextMesh textHolder) {
                cell = optionCell;
                cellTextHolder = textHolder;
                initialized = true;
            }

            public void Choose() {
                if (!initialized)
                    return;

                cell.transform.localScale += ChosenOptionEnlargeSize;
                cellTextHolder.fontStyle = FontStyle.Bold;
            }

            public void UnChoose() {
                if (!initialized)
                    return;

                cell.transform.localScale -= ChosenOptionEnlargeSize;
                cellTextHolder.fontStyle = FontStyle.Normal;
            }

            public abstract void Click();
        }

        private class OpenItemsMenuOption : VrRadialMenuOption {
            private readonly VrItemsMenu vrItemsMenu;
            private GameObject vrControllerMenu;

            public OpenItemsMenuOption(VrItemsMenu vrItemsMenu, GameObject vrControllerMenu) : base("Open Items") {
                this.vrItemsMenu = vrItemsMenu;
                this.vrControllerMenu = vrControllerMenu;
            }

            public override void Click() {
                vrItemsMenu.Show();
                vrControllerMenu.SetActive(false);
            }
        }

        private class ExitGameOption : VrRadialMenuOption {
            private readonly SceneLoader sceneLoader;

            public ExitGameOption(SceneLoader sceneLoader) : base("Exit") {
                this.sceneLoader = sceneLoader;
            }

            public override void Click() {
                sceneLoader.LoadScene("MainMenu");
            }
        }

        private class MenuOptionController {
            private int index;
            private readonly List<VrRadialMenuOption> menuOptions;

            public MenuOptionController(List<VrRadialMenuOption> options) {
                menuOptions = options;
                menuOptions[0].Choose();
            }

            public void ClickCurrent() {
                menuOptions[index].Click();
            }

            public void ChooseNext() {
                if (index + 1 >= menuOptions.Count)
                    return;

                menuOptions[index].UnChoose();
                menuOptions[++index].Choose();
            }

            public void ChoosePrevious() {
                if (index - 1 < 0)
                    return;

                menuOptions[index].UnChoose();
                menuOptions[--index].Choose();
            }
        };
    }
}