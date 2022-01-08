using UnityEngine;

namespace Scenes.SampleScene {
    [DisallowMultipleComponent]
    public class CustomInteractable : MonoBehaviour {
        public GameObject originalPrefab { get; set; }
        private FixedJoint rayJoint;
        private Color originalColor;
        private bool isHighlighted;

        public void OnRayHover() {
            Highlight();
        }

        public void OnRayEndHover() {
            EndHighlight();
        }

        public void OnRayBeginClick(Transform ray) {
            if (rayJoint != null)
                return;

            rayJoint = gameObject.AddComponent<FixedJoint>();
            rayJoint.connectedBody = ray.gameObject.GetComponent<Rigidbody>();
        }

        public void OnRayEndClick() {
            if (rayJoint == null)
                return;

            Destroy(rayJoint);
            // Adding fixed joint seems to add rigid body to the object
            Destroy(gameObject.GetComponent<Rigidbody>());
        }

        private void Highlight() {
            if (isHighlighted)
                return;

            var myRendererMaterial = GetComponent<Renderer>().material;
            originalColor = myRendererMaterial.color;
            myRendererMaterial.color = Color.yellow;
            isHighlighted = true;
        }

        private void EndHighlight() {
            if (!isHighlighted)
                return;

            var myRendererMaterial = GetComponent<Renderer>().material;
            myRendererMaterial.color = originalColor;
            isHighlighted = false;
        }
    }
}