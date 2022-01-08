using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Scenes.SampleScene {
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(GameObject))]
    public class VrItemsMenu : MonoBehaviour {
        public bool active { get; private set; }

        public Camera vrCamera;
        public List<GameObject> items;
        public GameObject menuCirclePrefab;

        public SteamVR_Behaviour_Pose pose;
        public SteamVR_Action_Boolean rightJoystickWest = SteamVR_Input.GetBooleanAction("RightJoystickWest");
        public SteamVR_Action_Boolean rightJoystickEast = SteamVR_Input.GetBooleanAction("RightJoystickEast");
        public SteamVR_Action_Boolean rightAButton = SteamVR_Input.GetBooleanAction("RightAButton");
        public SteamVR_Action_Boolean rightBButton = SteamVR_Input.GetBooleanAction("RightBButton");

        private GameObject menu;
        private int currentItemIndex;
        private GameObject menuCircle;
        private GameObject currentItem;
        private bool readyToReadItemChoice;
        private DateTime menuActivatedTime;
        private DateTime menuItemSwitchedTime;
        private GameObject menuCircleItemHolder;
        private bool readyToNavigateMenuItem = true;
        private GameObject menuCircleNextItemButton;
        private GameObject menuCirclePreviousItemButton;

        private static readonly TimeSpan SafetyItemChoiceDelay = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan SafetyMenuNavigateChoiceDelay = TimeSpan.FromSeconds(0.3);
        private static readonly Vector3 DistanceFromCamera = new Vector3(0f, 0.05f, 1.8f);
        private static readonly Vector3 SpawnedItemDistanceFromCamera = new Vector3(0f, -0.02f, 2.2f);

        public void Show() {
            active = true;
            menu.SetActive(true);
            menu.transform.position = vrCamera.transform.position + DistanceFromCamera;
            menu.transform.LookAt(vrCamera.transform);
            menuActivatedTime = DateTime.Now;
        }

        private void Hide() {
            active = false;
            menu.SetActive(false);
            readyToReadItemChoice = false;
        }

        private void Start() {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();

            CreateMenu();
        }

        private void CreateMenu() {
            menu = new GameObject("Vr Items Menu");
            menu.SetActive(false);

            menuCircle = Instantiate(menuCirclePrefab, menu.transform);
            menuCircle.name = "Vr Items Menu Circle";
            menuCircleNextItemButton = menuCircle.transform.Find("Next Choice Button").gameObject;
            menuCirclePreviousItemButton =
                menuCircle.transform.Find("Previous Choice Button").gameObject;
            menuCircleItemHolder = menuCircle.transform.Find("Item Holder").gameObject;

            UpdateCircleMenu();
        }

        private void UpdateCircleMenu() {
            if (currentItem)
                Destroy(currentItem);

            PutCurrentItemOnCircleMenu();
            UpdateMenuButtons();
        }

        private void UpdateMenuButtons() {
            if (items.Count <= 1)
                return;

            if (currentItemIndex > 0)
                menuCirclePreviousItemButton.SetActive(true);
            if (currentItemIndex + 1 < items.Count)
                menuCircleNextItemButton.SetActive(true);
            if (currentItemIndex < 1)
                menuCirclePreviousItemButton.SetActive(false);
            if (currentItemIndex + 1 == items.Count)
                menuCircleNextItemButton.SetActive(false);
        }

        private void PutCurrentItemOnCircleMenu() {
            if (items.Count < 1)
                return;

            currentItem = Instantiate(items[currentItemIndex], menuCircleItemHolder.transform);
            var itemRigidBody = currentItem.GetComponent<Rigidbody>();
            if (itemRigidBody) {
                Destroy(itemRigidBody);
            }

            RemoveItemColliders(currentItem);

            currentItem.transform.localPosition = Vector3.zero;
            currentItem.AddComponent<Rotator>();
        }

        private void Update() {
            if (!active)
                return;

            if (rightBButton.GetState(pose.inputSource)) {
                Hide();
                return;
            }

            if (!readyToReadItemChoice)
                readyToReadItemChoice = DateTime.Now - menuActivatedTime >= SafetyItemChoiceDelay;

            UpdatePossibleMenuClicks();
        }

        private void UpdatePossibleMenuClicks() {
            if (!readyToNavigateMenuItem)
                readyToNavigateMenuItem = DateTime.Now - menuItemSwitchedTime >= SafetyMenuNavigateChoiceDelay;
            if (readyToNavigateMenuItem) {
                if (menuCircleNextItemButton.activeSelf && rightJoystickEast.GetState(pose.inputSource)) {
                    ++currentItemIndex;
                    UpdateCircleMenu();
                    readyToNavigateMenuItem = false;
                    menuItemSwitchedTime = DateTime.Now;
                } else if (menuCirclePreviousItemButton.activeSelf && rightJoystickWest.GetState(pose.inputSource)) {
                    --currentItemIndex;
                    UpdateCircleMenu();
                    readyToNavigateMenuItem = false;
                    menuItemSwitchedTime = DateTime.Now;
                }
            }

            if (readyToReadItemChoice && rightAButton.GetState(pose.inputSource)) {
                SpawnCurrentItem();
            }
        }

        private void SpawnCurrentItem() {
            Hide();
            var spawnedItem = Instantiate(items[currentItemIndex]);
            RemoveItemColliders(spawnedItem);
            spawnedItem.transform.position = menuCircleItemHolder.transform.position + SpawnedItemDistanceFromCamera;
            var customInteractable = spawnedItem.AddComponent<CustomInteractable>();
            customInteractable.originalPrefab = items[currentItemIndex];
            spawnedItem.AddComponent<MeshCollider>();
            spawnedItem.SetActive(true);
        }

        private static void RemoveItemColliders(GameObject item) {
            var itemColliders = item.GetComponents<Collider>();
            if (itemColliders != null && itemColliders.Length > 0) {
                foreach (var itemCollider in itemColliders) {
                    Destroy(itemCollider);
                }
            }
        }

        private class Rotator : MonoBehaviour {
            private void Update() {
                transform.Rotate(Vector3.up, 6.0f * Time.deltaTime);
            }
        }
    }
}