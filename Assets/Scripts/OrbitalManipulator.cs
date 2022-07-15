using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitalManipulator : MonoBehaviour
{
    public Camera m_Camera;

    public float m_SensitivityMove = 0.2f;
    public float m_SensitivityScroll = 0.01f;

    private bool m_OrbitActive = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCameraStartupDistance(float lDistance)
    {
        m_Camera.transform.localPosition = new Vector3(0, 0, -lDistance);
    }

    void PerformOrbitalMove(Vector2 pDeltaMove)
    {
        if (m_OrbitActive)
        {
            transform.Rotate(Vector3.right, -pDeltaMove.y * m_SensitivityMove);
            transform.Rotate(Vector3.up, pDeltaMove.x * m_SensitivityMove);
        }
    }

    void PerformScroll(float pDeltaScrollMove)
    {
        m_Camera.transform.Translate(Vector3.forward * pDeltaScrollMove * m_SensitivityScroll, Space.Self);
    }

    public void OrbitalMove(InputAction.CallbackContext pContext)
    {
        Vector2 lDeltaMouse = pContext.action.ReadValue<Vector2>();
        PerformOrbitalMove(lDeltaMouse);
    }

    public void ScrollMove(InputAction.CallbackContext pContext)
    {
        float lDeltaScrollMouse = pContext.action.ReadValue<float>();
        PerformScroll(lDeltaScrollMouse);
    }

    public void ActivateOrbit(InputAction.CallbackContext pContext)
    {
        m_OrbitActive = pContext.ReadValueAsButton();
    }
}
