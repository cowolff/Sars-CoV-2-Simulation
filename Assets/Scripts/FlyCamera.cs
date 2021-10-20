using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{

    private bool menu = true;

    private bool _active = true;

    private bool _enableRotation = true;

    private float _mouseSense = 1.8f;

    private bool _enableTranslation = true;

    private float _translationSpeed = 55f;

    private bool _enableMovement = true;

    private float _movementSpeed = 10f;

    private float _boostedSpeed = 50f;

    private KeyCode _boostSpeed = KeyCode.LeftShift;

    private KeyCode _moveUp = KeyCode.E;

    private KeyCode _moveDown = KeyCode.Q;

    private bool _enableSpeedAcceleration = true;

    private float _speedAccelerationFactor = 1.5f;

    private KeyCode _initPositonButton = KeyCode.R;

    private CursorLockMode _wantedMode;

    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;

    private Vector3 _initPosition;
    private Vector3 _initRotation;

    private void SetCursorState()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("f"))
        {
            if(Cursor.lockState == CursorLockMode.Confined){
                _wantedMode = CursorLockMode.Locked;
            } else {
                Cursor.lockState = _wantedMode = CursorLockMode.None;
            }
        }

        /*
        if (Input.GetMouseButtonDown(0))
        {
            _wantedMode = CursorLockMode.Locked;
        }
        */
        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = menu;
        if (!menu) {
            SetCursorState();

            if (Cursor.visible)
                return;

            // Translation
            if (_enableTranslation)
            {
                transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
            }

            // Movement
            if (_enableMovement)
            {
                Vector3 deltaPosition = Vector3.zero;
                float currentSpeed = _movementSpeed;

                if (Input.GetKey(_boostSpeed))
                    currentSpeed = _boostedSpeed;

                if (Input.GetKey(KeyCode.W))
                    deltaPosition += transform.forward;

                if (Input.GetKey(KeyCode.S))
                    deltaPosition -= transform.forward;

                if (Input.GetKey(KeyCode.A))
                    deltaPosition -= transform.right;

                if (Input.GetKey(KeyCode.D))
                    deltaPosition += transform.right;

                if (Input.GetKey(_moveUp))
                    deltaPosition += transform.up;

                if (Input.GetKey(_moveDown))
                    deltaPosition -= transform.up;

                // Calc acceleration
                CalculateCurrentIncrease(deltaPosition != Vector3.zero);

                transform.position += deltaPosition * currentSpeed * _currentIncrease;
            }

            // Rotation
            if (_enableRotation)
            {
                // Pitch
                transform.rotation *= Quaternion.AngleAxis(
                    -Input.GetAxis("Mouse Y") * _mouseSense,
                    Vector3.right
                );

                // Paw
                transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                    transform.eulerAngles.z
                );
            }

            // Return to init position
            if (Input.GetKeyDown(_initPositonButton))
            {
                transform.position = _initPosition;
                transform.eulerAngles = _initRotation;
            }
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }

    public void setActive(bool active){
        menu = active;
    }

    public bool getActive(){
        return menu;
    }
}