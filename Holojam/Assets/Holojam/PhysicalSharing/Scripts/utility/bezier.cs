using UnityEngine;
using System.Collections;

public class bezier {
	

		int pointCount;

		Vector3 _p0;

		Vector3 _p1;

		Vector3 _p2;

		Vector3 _p3;

	public bezier ()
		{
			_p0 = new Vector3 ();
			_p1 = new Vector3 ();
			_p2 = new Vector3 ();
			_p3 = new Vector3 ();
			pointCount = 0;
		}

	public bezier (Vector3 p0, Vector3 p1)
		{
			_p0 = p0;
			_p1 = p1;
			pointCount = 2;
		}

	public bezier (Vector3 p0, Vector3 p1, Vector3 p2)
		{
			_p0 = p0;
			_p1 = p1;
			_p2 = p2;
			pointCount = 3;
		}

	public bezier (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			_p0 = p0;
			_p1 = p1;
			_p2 = p2;
			_p3 = p3;
			pointCount = 4;
		}

		public void addPoint (Vector3 p)
		{
			switch (pointCount) {
			case 0:
				_p0 = p;
				break;
			case 1:
				_p1 = p;
				break;
			case 2:
				_p2 = p;
				break;
			case 3:
				_p3 = p;
				break;
			default:
				break;
			}
			++pointCount;
		}

		public Vector3 getPoint (float t)
		{
			t = Mathf.Clamp01 (t);
			float oneMinusT = 1f - t;
			Vector3 v,v2;

			switch (pointCount) {
			case 2:
				return Vector3.Lerp (_p0, _p1, t);
			case 3:
				v = Mathf.Pow (t, 2f) * (_p0 - _p1 + _p2) +
					t * (-2f * _p0 + 2f * _p1) +
					_p0;
				v2 = 
					oneMinusT * oneMinusT * _p0 +
					2f * oneMinusT * t * _p1 +
					t * t * _p2;

				return
					//oneMinusT * oneMinusT * _p0 +
					//2f * oneMinusT * t * _p1 +
					//t * t * _p2;
					Mathf.Pow (t, 2f) * (_p0 - 2f * _p1 + _p2) +
					t * (-2f * _p0 + 2f * _p1) +
					_p0;
			case 4:
				return
					Mathf.Pow (t, 3f) * (-_p0 + 3f * _p1 - 3f * _p2 + _p3) +
					Mathf.Pow (t, 2f) * (3f * _p0 - 6f * _p1 + 3f * _p2) +
					t * (-3f * _p0 + 3f * _p1) +
					_p0;
			default:
				break;
			}
			return new Vector3 ();
		}


}
