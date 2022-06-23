using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class AdaptViewportToContent : MonoBehaviour
{

    public float m_xMin = -1;
    public float m_xMax = 1;
    public float m_yMin = -1;
    public float m_yMax = 1;
    public float m_zMin = -1;
    public float m_zMax = 1;

    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.orthographic = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Adapt to ratio
        float lCamRatio = m_Camera.pixelRect.size.x / m_Camera.pixelRect.size.y;

        float lContentRatio = (m_xMax-m_xMin) / (m_yMax-m_yMin);

        if (lContentRatio > lCamRatio)
        {
            float lHalfHeight = (m_xMax-m_xMin) / (lCamRatio * 2.0f);
            float lCenter = (m_yMin + m_yMax) / 2.0f;

            setOrtho(m_Camera, m_xMin, m_xMax, lCenter - lHalfHeight, lCenter + lHalfHeight, m_zMin, m_zMax);
        }
        else
        {
            float lHalfWidth = (m_yMax - m_yMin) * (lCamRatio / 2.0f);
            float lCenter = (m_xMin + m_xMax) / 2.0f;

            setOrtho(m_Camera, lCenter - lHalfWidth, lCenter + lHalfWidth, m_yMin, m_yMax, m_zMin, m_zMax);
        }

        m_Camera.nearClipPlane = m_zMin;
        m_Camera.farClipPlane = m_zMax;

        //setOrtho(m_Camera, m_xMin, m_xMax, m_zMin, m_zMax, m_yMin, m_yMax);
    }

    static void setOrtho(Camera pCamera, float pLeft, float pRight, float pBottom, float pTop, float pNear, float pFar)
    {
        Matrix4x4 lProjectionMatrix = new Matrix4x4();

        lProjectionMatrix.m00 = 2.0f / (pRight - pLeft);
        lProjectionMatrix.m10 = 0.0f;
        lProjectionMatrix.m20 = 0.0f;
        lProjectionMatrix.m30 = 0.0f;

        lProjectionMatrix.m01 = 0.0f;
        lProjectionMatrix.m11 = 2.0f / (pTop - pBottom);
        lProjectionMatrix.m21 = 0.0f;
        lProjectionMatrix.m31 = 0.0f;
        
        lProjectionMatrix.m02 = 0.0f;
        lProjectionMatrix.m12 = 0.0f;
        lProjectionMatrix.m22 = -2.0f / (pFar - pNear);
        lProjectionMatrix.m32 = 0.0f;
        
        lProjectionMatrix.m03 = -(pRight + pLeft) / (pRight - pLeft);
        lProjectionMatrix.m13 = -(pTop + pBottom) / (pTop - pBottom);
        lProjectionMatrix.m23 = -(pFar + pNear) / (pFar - pNear);
        lProjectionMatrix.m33 = 1.0f;


        pCamera.projectionMatrix = lProjectionMatrix;
    }
}
