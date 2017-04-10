
namespace Assets.Scripts
{
    using System.Collections;

    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The floating text class.
    /// Handles the floating text for the completed objectives.
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        /// <summary>
        /// The fade time reference.
        /// The fade time will be 2 seconds.
        /// </summary>
        private const float Fadetime = 2.0f;

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.StartCoroutine(this.FadeOut());
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            this.transform.position += Vector3.up * 25 * Time.deltaTime;
        }

        /// <summary>
        /// The fade out function.
        /// Handles the text fading out.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        private IEnumerator FadeOut()
        {
            // Get current alpha
            float startalpha = this.GetComponent<Text>().color.a;

            // The rate to fade at
            float rate = 1.0f / Fadetime;

            // Progress of the fade
            float progress = 0.0f;

            // While its less than 1
            while (progress < 1.0)
            {
                // Get the current color
                Color tmpColor = this.GetComponent<Text>().color;

                // Set the color to the updated alpha
                this.GetComponent<Text>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startalpha, 0, progress));

                // Increase the progress
                progress += rate * Time.deltaTime;

                yield return null;
            }

            // Destroy the object
            Destroy(this.gameObject);
        }
    }
}
