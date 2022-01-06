using UnityEngine;
using Valve.VR;

namespace Scenes.SampleScene {
    [DisallowMultipleComponent]
    public class LaserPointer : MonoBehaviour {
        public Color color;
        public bool addRigidBody;

        public SteamVR_Behaviour_Pose pose;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

        private bool isActive;
        private GameObject holder;
        private GameObject pointer;
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

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(Thickness, Thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pointer.GetComponent<BoxCollider>();
            if (addRigidBody) {
                if (collider) {
                    collider.isTrigger = true;
                }

                Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            } else if (collider) {
                Destroy(collider);
            }

            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
        }

        private void Update() {
            if (!isActive) {
                isActive = true;
                transform.GetChild(0).gameObject.SetActive(true);
            }

            var raycast = new Ray(transform.position, transform.forward);
            var bHit = Physics.Raycast(raycast, out var hit);

            if (interactWithUI != null && interactWithUI.GetState(pose.inputSource)) {
                pointer.transform.localScale = new Vector3(Thickness * 5f, Thickness * 5f, Dist);
                pointer.GetComponent<MeshRenderer>().material.color = clickColor;
            } else {
                pointer.transform.localScale = new Vector3(Thickness, Thickness, Dist);
                pointer.GetComponent<MeshRenderer>().material.color = color;
            }

            pointer.transform.localPosition = new Vector3(0f, 0f, Dist / 2f);
            
            HandlePossibleEndOfObjectInteraction(bHit, hit);
            
            if (currentInteractable != null && interactWithUI != null && !interactWithUI.GetState(pose.inputSource)) {
                currentInteractable.OnRayEndClick();
            }

            HandlePossibleObjectInteraction(bHit, hit);
        }

        private void HandlePossibleObjectInteraction(bool isHit, RaycastHit raycastHit) {
            if (!isHit)
                return;

            var customInteractable = raycastHit.transform.gameObject.GetComponent<CustomInteractable>();
            if (customInteractable == null)
                return;

            HandleCustomInteractableInteraction(customInteractable);
        }

        private void HandleCustomInteractableInteraction(CustomInteractable customInteractable) {
            currentInteractable = customInteractable;
            currentInteractable.OnRayHover();

            if (interactWithUI != null && interactWithUI.GetState(pose.inputSource)) {
                currentInteractable.OnRayClick(pointer.transform);
            }
        }

        private void HandlePossibleEndOfObjectInteraction(bool isHit, RaycastHit raycastHit) {
            // we have nothing we think we're interacting with
            if (currentInteractable == null)
                return;

            // we're touching something, but it's still the same object we know of
            // if the object we're touching is the object itself, it means we're just touching the ray => ignore
            if (isHit && raycastHit.transform == currentInteractable.transform || raycastHit.transform == pointer.transform)
                return;

            HandleEndOfCustomInteractableInteraction();
        }

        private void HandleEndOfCustomInteractableInteraction() {
            currentInteractable.OnRayEndHover();
            currentInteractable = null;
        }
    }
}