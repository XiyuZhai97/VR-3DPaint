using System;
using UnityEngine;
using wvr;
using WVR_Log;

public class MovieMode : MonoBehaviour {

	public bool lockHMDPosition = false;
	bool needUpdatePose = false;

	public bool needSmoothMove = false;
	public bool needSmoothMoveAccelerate = false;
	public int smoothMoveFrameCount = 25;
	public int smoothMoveNoActionFrameCount = 10;
	int smoothMoveFrame = 0;

	private WVR_PoseState_t[] StillPoses = new WVR_PoseState_t[1];
	private WVR_PoseState_t[] MovePoses = new WVR_PoseState_t[1];

	// Used by UpdatPose2.  Not smooth enough.
	private WVRMatrix SmoothMovePosesFrom;
	private WVRMatrix SmoothMovePosesTo;

	private MatrixSpline smSpline;
	private MatrixSpline.MatrixData smCurrent;


	void OnEnable()
	{
		WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.AFTER_NEW_POSES, UpdatePose);
	}

	void OnDisable()
	{
		WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.AFTER_NEW_POSES, UpdatePose);
		WaveVR_Render.ResetPoseUsedOnSubmit();
	}

	public bool GetMovieModeStatus()
	{
		return enabled && lockHMDPosition;
	}

	public void EnableMovieMode()
	{
		if (!enabled || lockHMDPosition)
			return;

		lockHMDPosition = true;
		needUpdatePose = true;
	}

	public void DisableMovieMode()
	{
		if (!enabled || !lockHMDPosition)
			return;

		lockHMDPosition = false;
		WaveVR_Render.ResetPoseUsedOnSubmit();
	}

	public void ResetMovieModeCenter()
	{
		if (!enabled || !lockHMDPosition)
			return;

		needUpdatePose = true;
	}

	public void ToggleMovieMode()
	{
		if (!enabled)
			return;
		if (lockHMDPosition)
			DisableMovieMode();
		else
			EnableMovieMode();
	}

	class WVRMatrix
	{
		// Always save in GL form
		Matrix4x4 matrix;

		public static Matrix4x4 ToMatrix44(WVR_Matrix4f_t pose)
		{
			var m = Matrix4x4.identity;

			m[0, 0] = pose.m0;
			m[0, 1] = pose.m1;
			m[0, 2] = pose.m2;
			m[0, 3] = pose.m3;

			m[1, 0] = pose.m4;
			m[1, 1] = pose.m5;
			m[1, 2] = pose.m6;
			m[1, 3] = pose.m7;

			m[2, 0] = pose.m8;
			m[2, 1] = pose.m9;
			m[2, 2] = pose.m10;
			m[2, 3] = pose.m11;

			m[3, 0] = pose.m12;
			m[3, 1] = pose.m13;
			m[3, 2] = pose.m14;
			m[3, 3] = pose.m15;

			return m;
		}

		public static WVR_Matrix4f_t ToWVRMatrix(Matrix4x4 m)
		{
			WVR_Matrix4f_t pose;

			pose.m0 = m[0, 0];
			pose.m1 = m[0, 1];
			pose.m2 = m[0, 2];
			pose.m3 = m[0, 3];

			pose.m4 = m[1, 0];
			pose.m5 = m[1, 1];
			pose.m6 = m[1, 2];
			pose.m7 = m[1, 3];

			pose.m8 = m[2, 0];
			pose.m9 = m[2, 1];
			pose.m10 = m[2, 2];
			pose.m11 = m[2, 3];

			pose.m12 = m[3, 0];
			pose.m13 = m[3, 1];
			pose.m14 = m[3, 2];
			pose.m15 = m[3, 3];

			return pose;
		}

		public static Matrix4x4 UnityGLConvert(Matrix4x4 m)
		{
			m[0, 2] = -m[0, 2];
			m[1, 2] = -m[1, 2];
			m[2, 0] = -m[2, 0];
			m[2, 1] = -m[2, 1];
			m[2, 3] = -m[2, 3];
			return m;
		}

		public WVRMatrix(WVR_Matrix4f_t mat)
		{
			matrix = ToMatrix44(mat);
		}

		public WVRMatrix(Matrix4x4 mat, bool unityToGL)
		{
			matrix = unityToGL ? UnityGLConvert(mat) : mat;
		}

		public WVRMatrix(Vector4 position, Quaternion rotation, bool unityToGL)
		{
			Matrix4x4 mat = Matrix4x4.Rotate(rotation);
			mat.SetColumn(3, position);
			matrix = unityToGL ? UnityGLConvert(mat) : mat;
		}

		public WVRMatrix()
		{
			matrix = Matrix4x4.identity;
		}

		public Matrix4x4 GetMatrix()
		{
			return matrix;
		}

		public WVR_Matrix4f_t GetWVRMatrix()
		{
			return ToWVRMatrix(matrix);
		}

		public WVR_Vector3f_t GetWVRPosition()
		{
			var pos = matrix.GetPosition();
			return new WVR_Vector3f_t() { v0 = pos.x, v1 = pos.y, v2 = pos.z };
		}

		public WVR_Quatf_t GetWVRRotation()
		{
			var rot = matrix.GetRotation();
			return new WVR_Quatf_t() { x = rot.x, y = rot.y, z = rot.z, w = rot.w };
		}
	}

	class MatrixSpline
	{
		public class MatrixData
		{
			public Vector4 pos;
			public Quaternion rot;

			public MatrixData(Matrix4x4 mat)
			{
				pos = mat.GetPosition();
				rot = mat.GetRotation();
			}

			public MatrixData(Vector4 p, Quaternion r)
			{
				pos = p;
				rot = r;
			}

			public Matrix4x4 ToMatrix()
			{
				Matrix4x4 mat = Matrix4x4.Rotate(rot);
				mat.SetColumn(3, pos);
				return mat;
			}

			public static MatrixData Lerp(MatrixData matFrom, MatrixData matTo, float t)
			{
				return new MatrixData(
					Vector4.Lerp(matFrom.pos, matTo.pos, t),
					Quaternion.Lerp(matFrom.rot, matTo.rot, t));
			}

			public static MatrixData Spline(MatrixData matFrom, MatrixData matMid, MatrixData matTo, float t)
			{
				return Lerp(Lerp(matFrom, matMid, t), Lerp(matMid, matTo, t), t);
			}
		};

		MatrixData m1, m2, m3;
		public MatrixSpline(Matrix4x4 matFrom, Matrix4x4 matTo)
		{
			m1 = new MatrixData(matFrom);
			m2 = new MatrixData(matTo);
			m3 = MatrixData.Lerp(m1, m2, 0.5f);
		}

		public MatrixSpline(MatrixData matFrom, Matrix4x4 matTo)
		{
			m1 = matFrom;
			m2 = new MatrixData(matTo);
			m3 = MatrixData.Lerp(m1, m2, 0.5f);
		}

		public void UpdateFromTo(MatrixData matFrom, Matrix4x4 matTo)
		{
			m1 = matFrom;
			m3 = m2;
			m2 = new MatrixData(matTo);
		}

		public MatrixData Spline(float t)
		{
			return MatrixData.Spline(m1, m3, m2, t);
		}
	}

	void UpdatePose(params object[] args)
	{
		if (lockHMDPosition && needUpdatePose)
		{
			needUpdatePose = false;
			StillPoses[0] = WaveVR.Instance.hmd.pose.pose;
			WaveVR_Render.SetPoseUsedOnSubmit(StillPoses[0]);
		}

		if (lockHMDPosition && needSmoothMove)
		{
			if (smoothMoveFrame >= smoothMoveFrameCount || smSpline == null)
			{
				smoothMoveFrame = 0;

				// Copy old value
				MovePoses[0] = StillPoses[0];

				SmoothMovePosesFrom = new WVRMatrix(MovePoses[0].PoseMatrix);
				SmoothMovePosesTo = new WVRMatrix(WaveVR.Instance.hmd.pose.pose.PoseMatrix);

				smSpline = new MatrixSpline(SmoothMovePosesFrom.GetMatrix(), SmoothMovePosesTo.GetMatrix());
			
				// save a new one
				StillPoses[0] = WaveVR.Instance.hmd.pose.pose;
			}

			smoothMoveFrame++;
			//Log.d("MovieMode", "smoothMoveFrame=" + smoothMoveFrame);

			if (smoothMoveFrame < smoothMoveNoActionFrameCount)
				return;

			int total = smoothMoveFrameCount - smoothMoveNoActionFrameCount;
			int time = smoothMoveFrame - smoothMoveNoActionFrameCount;

			// This will accelerate the speed.
			if (needSmoothMoveAccelerate && smCurrent != null)
			{
				SmoothMovePosesTo = new WVRMatrix(WaveVR.Instance.hmd.pose.pose.PoseMatrix);
				smSpline.UpdateFromTo(smCurrent, SmoothMovePosesTo.GetMatrix());
			}

			smCurrent = smSpline.Spline(time / (float)total);

			//Log.d("MovieMode", "From=" + SmoothMovePosesFrom.rot + ", To=" + SmoothMovePosesTo.rot + ", lerp=" + rot);

			WVRMatrix wvrMatrix = new WVRMatrix(smCurrent.pos, smCurrent.rot, false);
			MovePoses[0].PoseMatrix = wvrMatrix.GetWVRMatrix();
			MovePoses[0].RawPose.position = wvrMatrix.GetWVRPosition();
			MovePoses[0].RawPose.rotation = wvrMatrix.GetWVRRotation();

			WaveVR_Render.SetPoseUsedOnSubmit(MovePoses[0]);
		}
	}

	void UpdatePose2(object[] args)
	{
		if (lockHMDPosition && needUpdatePose)
		{
			needUpdatePose = false;
			WaveVR_Render.SetPoseUsedOnSubmit(WaveVR.Instance.hmd.pose.pose);
		}

		if (lockHMDPosition && needSmoothMove)
		{
			if (smoothMoveFrame >= smoothMoveFrameCount)
			{
				smoothMoveFrame = 0;

				// Copy old value
				MovePoses[0] = StillPoses[0];

				SmoothMovePosesFrom = new WVRMatrix(MovePoses[0].PoseMatrix);
				SmoothMovePosesTo = new WVRMatrix(WaveVR.Instance.hmd.pose.pose.PoseMatrix);

				// Record a new one
				StillPoses[0] = WaveVR.Instance.hmd.pose.pose;
			}

			smoothMoveFrame++;
			//Log.d("MovieMode", "smoothMoveFrame=" + smoothMoveFrame);

			if (smoothMoveFrame < smoothMoveNoActionFrameCount)
				return;

			int total = smoothMoveFrameCount - smoothMoveNoActionFrameCount;
			int time = smoothMoveFrame - smoothMoveNoActionFrameCount;

			var from = SmoothMovePosesFrom.GetMatrix();
			var to = SmoothMovePosesTo.GetMatrix();
			var pos = Vector3.Lerp(from.GetPosition(), to.GetPosition(), time / (float)total);
			var rot = Quaternion.Lerp(from.GetRotation(), to.GetRotation(), time / (float)total);

			//Log.d("MovieMode", "From=" + SmoothMovePosesFrom.rot + ", To=" + SmoothMovePosesTo.rot + ", lerp=" + rot);

			WVRMatrix wvrMatrix = new WVRMatrix(pos, rot, false);
			MovePoses[0].PoseMatrix = wvrMatrix.GetWVRMatrix();
			MovePoses[0].RawPose.position = wvrMatrix.GetWVRPosition();
			MovePoses[0].RawPose.rotation = wvrMatrix.GetWVRRotation();

			WaveVR_Render.SetPoseUsedOnSubmit(MovePoses[0]);
		}
	}
}
