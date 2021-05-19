using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGS.AdvancedRenderSystem.Examples
{
    public class DesktopCameraController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 6;
        [SerializeField] private float _rotationSpeed = 50;
        [SerializeField] private float _height = 3;

        private float _addRotationX;


        private void Update()
        {
            ComputeMovement();
            ComputeRotation();
            ComputeHeight();
        }

        private void ComputeMovement()
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) direction.z += 1;
            if (Input.GetKey(KeyCode.S)) direction.z -= 1;
            if (Input.GetKey(KeyCode.A)) direction.x -= 1;
            if (Input.GetKey(KeyCode.D)) direction.x += 1;

            direction = direction.normalized;

            if (Mathf.Approximately(direction.x, 0) && Mathf.Approximately(direction.z, 0))
                return;

            transform.position += Quaternion.Euler(0, transform.eulerAngles.y, 0) * (direction * _movementSpeed * Time.deltaTime);
        }

        private void ComputeRotation()
        {
            Vector2 direction = Vector2.zero;

            if (Input.GetKey(KeyCode.UpArrow)) direction.y += 1;
            if (Input.GetKey(KeyCode.DownArrow)) direction.y -= 1;
            if (Input.GetKey(KeyCode.RightArrow)) direction.x += 1;
            if (Input.GetKey(KeyCode.LeftArrow)) direction.x -= 1;

            direction = direction.normalized;

            if (Mathf.Approximately(direction.x, 0) && Mathf.Approximately(direction.y, 0))
                return;

            Vector3 rotation = transform.eulerAngles;

            if ((_addRotationX > -40 && _addRotationX < 40) || (_addRotationX < -40 && direction.y > 0) || (_addRotationX > 40 && direction.y < 0))
            {
                rotation.x -= direction.y * _rotationSpeed * Time.deltaTime;

                _addRotationX += direction.y * _rotationSpeed * Time.deltaTime;
            }

            rotation.y += direction.x * _rotationSpeed * Time.deltaTime;

            transform.eulerAngles = rotation;
        }

        private void ComputeHeight()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, LayerMask.GetMask("Water")))
                transform.position = new Vector3(transform.position.x, hit.point.y + _height, transform.position.z);
        }
    }
}
