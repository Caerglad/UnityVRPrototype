using NUnit.Framework;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Scenes.SampleScene {
    [DisallowMultipleComponent]
    public class LaserPointer : MonoBehaviour {
        public Color color;

        public SteamVR_Behaviour_Pose pose;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
        public SteamVR_Action_Vector2 rightJoystickPosition = SteamVR_Input.GetVector2Action("RightJoystickPosition");

        private Ray ray;
        private bool isActive;
        private GameObject holder;
        private GameObject pointer;
        private MeshRenderer pointerMeshRenderer;
        private readonly Color clickColor = Color.green;
        private CustomInteractable currentInteractable;

        private const float Dist = 100f;
        private const float Thickness = 0.002f;

        private void Start() {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);
            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);

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

            UpdateRayPositionAndOrigin();

            var didRayHit = Physics.Raycast(ray, out var rayHit);
            HandlePossibleEndOfObjectInteraction(didRayHit, rayHit);
            UpdatePointerPositionAndScale(didRayHit, rayHit);
            HandlePossibleEndOfRayClick();
            HandlePossibleObjectInteraction(didRayHit, rayHit);
        }

        private void UpdateRayPositionAndOrigin() {
            ray.origin = transform.position;
            ray.direction = transform.forward;
        }

        private void UpdatePointerPositionAndScale(bool didRayHit, RaycastHit raycastHit) {
            if (currentInteractable != null)
                return;

            var rayDistance = Dist;
            if (didRayHit && raycastHit.distance < Dist &&
                raycastHit.transform != pointer.transform) {
                rayDistance = raycastHit.distance;
            }

            if (interactWithUI != null && interactWithUI.GetState(pose.inputSource)) {
                pointer.transform.localScale = new Vector3(Thickness * 5f, Thickness * 5f, rayDistance);
                pointerMeshRenderer.material.color = clickColor;
            } else {
                pointer.transform.localScale = new Vector3(Thickness * 1.5f, Thickness * 1.5f, rayDistance);
                pointerMeshRenderer.material.color = color;
            }

            pointer.transform.localPosition = new Vector3(0f, 0f, rayDistance / 2f);
        }

        private void HandlePossibleEndOfRayClick() {
            if (currentInteractable != null && interactWithUI != null && !interactWithUI.GetState(pose.inputSource)) {
                currentInteractable.OnRayEndClick();
            }
        }

        private void HandlePossibleObjectInteraction(bool isHit, RaycastHit raycastHit) {
            if (!isHit)
                return;

            var customInteractable = raycastHit.transform.gameObject.GetComponent<CustomInteractable>();
            if (customInteractable == null)
                return;

            // we're already touching something and a different object is on our way => ignore
            if (currentInteractable != null && currentInteractable != customInteractable)
                return;

            HandleCustomInteractableInteraction(customInteractable);
        }

        private void HandleCustomInteractableInteraction(CustomInteractable customInteractable) {
            bool interactableDidChange = currentInteractable != customInteractable;
            currentInteractable = customInteractable;
            currentInteractable.OnRayHover();

            if (interactableDidChange && interactWithUI != null && interactWithUI.GetState(pose.inputSource)) {
                currentInteractable.OnRayBeginClick(pointer.transform);
            }

            HandleMoveCustomInteractableForwardsOrBackwardsOnUserInput();
        }

        private void HandleMoveCustomInteractableForwardsOrBackwardsOnUserInput() {
            if (!rightJoystickPosition.active)
                throw new AssertionException("Right Joystick Position action is not active, set it up correctly.");

            var newRayDistance = pointer.transform.localScale.z;
            var rightJoysticksYAxisPosition = rightJoystickPosition.axis.y;
            if (rightJoysticksYAxisPosition < -0.15f) {
                if (newRayDistance <= 5f)
                    return;
                newRayDistance -= 0.5f;
            } else if (rightJoysticksYAxisPosition > 0.15f) {
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

        private void HandlePossibleEndOfObjectInteraction(bool isHit, RaycastHit raycastHit) {
            // we have nothing we think we're interacting with
            if (currentInteractable == null)
                return;

            // we're touching something, but it's still the same object we know of
            // if the object we're touching is the object itself, it means we're just touching the ray => ignore
            if (isHit && (raycastHit.transform == currentInteractable.transform ||
                          raycastHit.transform == pointer.transform))
                return;

            HandleEndOfCustomInteractableInteraction();
        }

        private void HandleEndOfCustomInteractableInteraction() {
            currentInteractable.OnRayEndHover();
            currentInteractable = null;
        }
    }
}