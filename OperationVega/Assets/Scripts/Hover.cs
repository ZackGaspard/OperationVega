
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The hover class.
    /// This will be attached to the cooked food allowing it to follow the mouse.
    /// </summary>
    public class Hover : MonoBehaviour
    {
        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            // Disable the collider so ray casts can still work while the cooked food
            // is attached to the mouse.
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            // Set the position to the mouse
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            this.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }
}
