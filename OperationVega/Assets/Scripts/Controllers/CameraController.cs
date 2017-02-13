
namespace Assets.Scripts.Controllers
{
    using UnityEngine;

    /// <summary>
    /// The camera controller class.
    /// Handles the camera movement functionality
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// The mouse position x.
        /// </summary>
        private float mousePosX;

        /// <summary>
        /// The speed at which the camera moves.
        /// </summary>
        /// [HideInInspector]
        public uint MoveSpeed;

        /// <summary>
        /// The rotate speed of the camera.
        /// </summary>
        /// [HideInInspector]
        public uint RotateSpeed;

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            this.ZoomCamera();
            this.PanCamera();

            this.MoveSpeed = (uint)Mathf.Clamp(this.MoveSpeed, 2, 5);
            this.RotateSpeed = (uint)Mathf.Clamp(this.RotateSpeed, 2, 5);
        }

        /// <summary>
        /// The late update function.
        /// </summary>
        private void LateUpdate()
        {
            this.RotateCamera();
            this.mousePosX = Input.mousePosition.x;
        }

        /// <summary>
        /// The pan camera function.
        /// Moves the camera based on key presses
        /// </summary>
        private void PanCamera()
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.position += this.transform.forward * this.MoveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.position -= this.transform.forward * this.MoveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.position -= this.transform.right * this.MoveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.position += this.transform.right * this.MoveSpeed * Time.deltaTime;
            }
        }

        /// <summary>
        /// The zoom camera.
        /// </summary>
        private void ZoomCamera()
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                Camera.main.orthographicSize += 0.25f;
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                 Camera.main.orthographicSize -= 0.25f;
            }

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3, 10);
        }

        /// <summary>
        /// The rotate camera.
        /// </summary>
        private void RotateCamera()
        {
            if (Input.GetMouseButton(1))
            {
                if (Input.mousePosition.x != this.mousePosX)
                {
                    float camroty = (Input.mousePosition.x - this.mousePosX) * this.RotateSpeed * Time.deltaTime;
                    this.transform.Rotate(0.0f, camroty, 0.0f);
                }
            }
        }
    }
}
