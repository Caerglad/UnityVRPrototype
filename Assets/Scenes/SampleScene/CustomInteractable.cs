using UnityEditor;
using UnityEngine;

namespace Scenes.SampleScene {
    [DisallowMultipleComponent]
    public class CustomInteractable : MonoBehaviour {
        private FixedJoint rayFixedJoint;
        private Color originalColor;
        private bool isHighlighted;

        public void OnRayHover() {
            Highlight();
        }

        public void OnRayEndHover() {
            EndHighlight();
        }

        public void OnRayClick(Transform ray) {
            if (rayFixedJoint != null)
                return;

            rayFixedJoint = gameObject.AddComponent<FixedJoint>();
            rayFixedJoint.connectedBody = ray.gameObject.GetComponent<Rigidbody>();
            // gameObject.GetComponent(FixedJoint).connectedBody=collision.rigidbody;
        }

        public void OnRayEndClick() {
            if (rayFixedJoint == null)
                return;

            Destroy(rayFixedJoint);
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