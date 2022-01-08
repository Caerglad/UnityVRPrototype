using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Scenes.SampleScene {
    [DisallowMultipleComponent]
    public class LaserPointer : MonoBehaviour {
        public Color color;
        public VrItemsMenu vrItemsMenu;

        public SteamVR_Behaviour_Pose pose;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
        public SteamVR_Action_Boolean rightJoystickNorth = SteamVR_Input.GetBooleanAction("RightJoystickNorth");
        public SteamVR_Action_Boolean rightJoystickSouth = SteamVR_Input.GetBooleanAction("RightJoystickSouth");
        public SteamVR_Action_Boolean invokeMenu = SteamVR_Input.GetBooleanAction("InvokeMenu");

        private Ray ray;
        private bool isActive;
        private GameObject holder;
        private GameObject pointer;
        private bool activatedMenuInLastFrame;
        private MeshRenderer pointerMeshRenderer;
        private readonly Color clickColor = Color.green;
        private CustomInteractable currentlyHovered;
        private CustomInteractable currentlyHolding;

        private const float Dist = 100f;
        private const float Thickness = 0.002f;

        private void Start() {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);
            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);
            if (rightJoystickNorth == null)
                Debug.LogError("No right joystick north action has been set on this component.", this);
            if (rightJoystickSouth == null)
                Debug.LogError("No right joystick south action has been set on this component.", this);

            holder = new GameObject() {
                transform = {
                    parent = transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity
                }
            };

            CreatePointer();
        }

        private void CreatePointer() {
            Teleport.instance.CancelTeleportHint();
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(Thickness, Thickness, Dist);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            pointerMeshRenderer = pointer.GetComponent<MeshRenderer>();

            var pointerCollider = pointer.GetComponent<BoxCollider>();
            if (pointerCollider) {
                pointerCollider.isTrigger = true;
            }

            var pointerRigidBody = pointer.AddComponent<Rigidbody>();
            pointerRigidBody.isKinematic = true;

            var newPointerMaterial = new Material(Shader.Find("Unlit/Color"));
            newPointerMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newPointerMaterial;

            ray = new Ray(transform.position, transform.forward);
        }

        private void Update() {
            if (!isActive) {
                isActive = true;
                transform.GetChild(0).gameObject.SetActive(true);
            }

            if (vrItemsMenu.active)
                return;

            if (invokeMenu.GetState(pose.inputSource)) {
                pointer.SetActive(false);
                activatedMenuInLastFrame = true;
                return;
            } else if (activatedMenuInLastFrame) {
                pointer.SetActive(true);
                activatedMenuInLastFrame = false;
            }

            UpdateRayPositionAndOrigin();

            var didRayHit = Physics.Raycast(ray, out var rayHit);
            HandlePossibleEndOfCustomInteractableHold();
            HandlePossibleEndOfObjectHover(didRayHit, rayHit);

            UpdatePointerPositionAndScale(didRayHit, rayHit);

            HandlePossibleObjectHover(didRayHit, rayHit);
            HandlePossibleHoveredCustomInteractableHold();

            HandlePossibleHeldCustomInteractableMove();
        }

        private void HandlePossibleHeldCustomInteractableMove() {
            if (currentlyHolding)
                HandleMoveHeldCustomInteractableForwardsOrBackwardsOnUserInput();
        }

        private void UpdateRayPositionAndOrigin() {
            ray.origin = transform.position;
            ray.direction = transform.forward;
        }

        private void UpdatePointerPositionAndScale(bool didRayHit, RaycastHit raycastHit) {
            if (currentlyHolding != null)
                return;

            var rayDistance = Dist;
            if (didRayHit && raycastHit.distance < Dist &&
                raycastHit.transform != pointer.transform) {
                rayDistance = raycastHit.distance;
            }

            if (interactWithUI.GetState(pose.inputSource)) {
                pointer.transform.localScale = new Vector3(Thickness * 5f, Thickness * 5f, rayDistance);
                pointerMeshRenderer.material.color = clickColor;
            } else {
                pointer.transform.localScale = new Vector3(Thickness * 1.5f, Thickness * 1.5f, rayDistance);
                pointerMeshRenderer.material.color = color;
            }

            pointer.transform.localPosition = new Vector3(0f, 0f, rayDistance / 2f);
        }

        private void HandlePossibleEndOfCustomInteractableHold() {
            if (currentlyHolding != null && !interactWithUI.GetState(pose.inputSource)) {
                currentlyHolding.OnRayEndClick();

                // TODO: moving the object using the joint seems to break it for some reason,
                // for now a naive fix is to just re-instantiate the object while we finish moving it

                var copy = Instantiate(currentlyHolding.originalPrefab);
                var itemColliders = copy.GetComponents<Collider>();
                if (itemColliders != null && itemColliders.Length > 0) {
                    foreach (var itemCollider in itemColliders) {
                        Destroy(itemCollider);
                    }
                }
                copy.transform.position = currentlyHolding.transform.position;
                var customInteractable = copy.AddComponent<CustomInteractable>();
                customInteractable.originalPrefab = currentlyHolding.originalPrefab;
                copy.AddComponent<MeshCollider>();
                
                currentlyHolding.gameObject.SetActive(false);
                Destroy(currentlyHolding);
                currentlyHolding = null;
                copy.SetActive(true);
                //end naive fix
            }
        }

        private void HandlePossibleObjectHover(bool isHit, RaycastHit raycastHit) {
            if (!isHit)
                return;

            var customInteractable = raycastHit.transform.gameObject.GetComponent<CustomInteractable>();
            if (customInteractable == null)
                return;

            // we're already touching something and a different object is on our way => ignore
            if (currentlyHovered != null && currentlyHovered != customInteractable)
                return;

            HandleCustomInteractableHover(customInteractable);
        }

        private void HandleCustomInteractableHover(CustomInteractable customInteractable) {
            currentlyHovered = customInteractable;
            customInteractable.OnRayHover();
        }

        private void HandlePossibleHoveredCustomInteractableHold() {
            if (!currentlyHovered)
                return;

            if (currentlyHovered == currentlyHolding)
                return;

            if (interactWithUI.GetState(pose.inputSource)) {
                currentlyHolding = currentlyHovered;
                currentlyHolding.OnRayBeginClick(pointer.transform);
            }
        }

        private void HandleMoveHeldCustomInteractableForwardsOrBackwardsOnUserInput() {
            var newRayDistance = pointer.transform.localScale.z;
            if (rightJoystickSouth.GetState(pose.inputSource)) {
                if (newRayDistance <= 5f)
                    return;
                newRayDistance -= 0.5f;
            } else if (rightJoystickNorth.GetState(pose.inputSource)) {
                if (newRayDistance >= 20f)
                    return;
                newRayDistance += 0.5f;
            } else {
                return;
            }

            pointer.transform.localScale = new Vector3(Thickness * 5f, Thickness * 5f, newRayDistance);
            pointerMeshRenderer.material.color = clickColor;
            pointer.transform.localPosition = new Vector3(0f, 0f, newRayDistance / 2f);
        }

        private void HandlePossibleEndOfObjectHover(bool isHit, RaycastHit raycastHit) {
            // we have nothing we think we're interacting with
            if (currentlyHovered == null)
                return;

            // we're touching something, but it's still the same object we know of
            // if the object we're touching is the object itself, it means we're just touching the ray => ignore
            if (isHit && (raycastHit.transform == currentlyHovered.transform ||
                          raycastHit.transform == pointer.transform))
                return;

            HandleEndOfCustomInteractableHover();
        }

        private void HandleEndOfCustomInteractableHover() {
            currentlyHovered.OnRayEndHover();
            currentlyHovered = null;
        }
    }
}