//source : https://handwiki.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline

using System.Collections.Generic;
using UnityEngine;

// Draws a catmull-rom spline in the scene view,
// along the child objects of the transform of this component
public class CatmullRomSpline
{

	[Range(0, 1)] public float alpha = 0.5f;

	float mEtremitiesTension = 1f;

	public List<Vector3> m_CurvePoints = new List<Vector3>();

	Vector3 GetPoint(int i) => m_CurvePoints[i];


	public Vector3[] Generate(int pNumSubdivisions)
	{
		List<Vector3> lVerticesList = new List<Vector3>();
		List<Vector3> lCurveToProcess = new List<Vector3>();

		lCurveToProcess.Add(m_CurvePoints[0] + (m_CurvePoints[0] - m_CurvePoints[1]) * mEtremitiesTension);
		lCurveToProcess.AddRange(m_CurvePoints);
        lCurveToProcess.Add(m_CurvePoints[m_CurvePoints.Count-1] + (m_CurvePoints[m_CurvePoints.Count - 1] - m_CurvePoints[m_CurvePoints.Count - 2]) * mEtremitiesTension);

		for (int i = 0; i < lCurveToProcess.Count - 3; ++i)
        {
			CatmullRomCurve lCurve = new CatmullRomCurve(lCurveToProcess[i], lCurveToProcess[i + 1], lCurveToProcess[i + 2], lCurveToProcess[i + 3], alpha);// GetCurve(i);
			for (int j = 0; j < pNumSubdivisions; ++j)
			{
				float t = j / (pNumSubdivisions - 1f);
				lVerticesList.Add(lCurve.GetPoint(t));
			}
		}

		return lVerticesList.ToArray();
	}
}