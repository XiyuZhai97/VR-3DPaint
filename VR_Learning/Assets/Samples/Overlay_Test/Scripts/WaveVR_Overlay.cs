using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using wvr_overlay;
using WVR_Log;

public class WaveVR_Overlay {
	private Texture2D wrapperTexture = null;
	private static string LOG_TAG = "WaveVR_Overlay";
	private int overlaytextureid = 0;
	private System.IntPtr wrapperTextureid;
	private MeshRenderer meshrenderer;
	private wvr_overlay.WVR_OverlayPosition_t position;
	private wvr_overlay.WVR_OverlayTexture_t mtexture;
	private bool isShowOverlay;

	private static WaveVR_Overlay mInstance = null;

	public static WaveVR_Overlay instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new WaveVR_Overlay();
			}

			return mInstance;
		}
	}

	private void GenOverlay()
	{
		wvr_overlay.WVR_OverlayError _result = Interop_overlay.WVR_GenOverlay(ref overlaytextureid);
		Log.d(LOG_TAG, "Gen OverlayError: " + _result.ToString() + ", overlaytextureid = " + overlaytextureid);
	}

	public void resetOverlayId()
	{
		overlaytextureid = 0;
		isShowOverlay = false;
	}

	private void setWrapperTexture(uint width, uint height)
	{
		wrapperTexture = Resources.Load<Texture2D>("WAVE_Overlay_test");
		wrapperTextureid = wrapperTexture.GetNativeTexturePtr();
		if (wrapperTexture != null)
		{
			mtexture.textureId = (uint)wrapperTextureid;
			mtexture.height = height;
			mtexture.width = width;
			updateOverlayTexture();
		} else
		{
			Log.e(LOG_TAG, "Overlay test image is null!");
		}
	}

	public void resetWrapperTexture()
	{
		if (wrapperTexture != null)
		{
			Resources.UnloadAsset(wrapperTexture);
			wrapperTexture = null;
			wrapperTextureid = System.IntPtr.Zero;
			mtexture.textureId = (uint)wrapperTextureid;
			mtexture.height = 0;
			mtexture.width = 0;
			Log.d(LOG_TAG, "resetWrapperTexture " + mtexture.textureId);
		} else
		{
			Log.e(LOG_TAG, "Overlay test image is null");
		}
	}

	private void setOverlayPosition()
	{
		position.x = 0.0f;
		position.y = 0.0f;
		position.z = -0.4f;
		wvr_overlay.WVR_OverlayError _resultposition = Interop_overlay.WVR_SetOverlayFixedPosition(overlaytextureid, ref position);
		if (_resultposition != wvr_overlay.WVR_OverlayError.WVR_OverlayError_None)
		{
			Log.e(LOG_TAG, "set OverlayPositionError: " + _resultposition.ToString());
		}
	}

	public void ShowOverlayTexture(uint width, uint height)
	{
		if (WaveVR_Utils.mCustomRenderThreadFunc != null)
		{
			Log.w(LOG_TAG, "WaveVR_Utils.mCustomRenderThreadFunc should be null! ");
			WaveVR_Utils.mCustomRenderThreadFunc = null;
		}
		Log.d(LOG_TAG, "WaveVR_Utils.mCustomRenderThreadFunc set delegate up!");
		WaveVR_Utils.mCustomRenderThreadFunc = executeFuncinRenderThread;

		bool isValid;
		isValid = Interop_overlay.WVR_IsOverlayValid(overlaytextureid);
		Log.d(LOG_TAG, isValid.ToString());
		if (isValid == false)
		{
			GenOverlay();
		}
		else
			Log.d(LOG_TAG, isValid.ToString());

		setWrapperTexture(width, height);
		setOverlayPosition();
		updateOverlayTexture();
	}

	public void HideOverlay()
	{
		wvr_overlay.WVR_OverlayError _result = Interop_overlay.WVR_HideOverlay(overlaytextureid);
		if (_result == wvr_overlay.WVR_OverlayError.WVR_OverlayError_None)
		{
			isShowOverlay = false;
		} else
		{
			Log.e(LOG_TAG, "WVR_HideOverlay fail: " + _result.ToString() + ", Overlayid: " + overlaytextureid);
		}
	}

	public bool getIsShowOverlay()
	{
		return isShowOverlay;
	}

	public void resetIsShowOverlay()
	{
		isShowOverlay = false;
	}

	public void DelOverlay()
	{
		wvr_overlay.WVR_OverlayError _result = Interop_overlay.WVR_DelOverlay(overlaytextureid);
		if (_result != wvr_overlay.WVR_OverlayError.WVR_OverlayError_None)
		{
			Log.d(LOG_TAG, "Del OverlayError " + _result.ToString());
		}
	}

	public int getOverlayTextureId()
	{
		return overlaytextureid;
	}

	public wvr_overlay.WVR_OverlayTexture_t getWrapperTexture()
	{
		return mtexture;
	}

	private void updateOverlayTexture()
	{
		WaveVR_Utils.SendRenderEvent(WaveVR_Utils.RENDEREVENTID_ExecuteCustomFunction);
	}

	public void executeFuncinRenderThread()
	{
		if ((WaveVR_Overlay.instance.mtexture.textureId != (uint)0) && (WaveVR_Overlay.instance.getOverlayTextureId() != (uint)0))
		{
			Log.d(LOG_TAG, "executeFuncinRenderThread overlay texture ID: " + WaveVR_Overlay.instance.getOverlayTextureId() + ", image texture ID: " + WaveVR_Overlay.instance.mtexture.textureId);
			wvr_overlay.WVR_OverlayError _resultSetTextureId = Interop_overlay.WVR_SetOverlayTextureId(WaveVR_Overlay.instance.getOverlayTextureId(), ref WaveVR_Overlay.instance.mtexture);

			if (_resultSetTextureId == wvr_overlay.WVR_OverlayError.WVR_OverlayError_None)
			{
				wvr_overlay.WVR_OverlayError _resultShowOverlay = Interop_overlay.WVR_ShowOverlay(overlaytextureid);
				Log.d(LOG_TAG, "show OverlayError " + _resultShowOverlay.ToString());

				if (_resultShowOverlay == wvr_overlay.WVR_OverlayError.WVR_OverlayError_None)
				{
					isShowOverlay = true;
				}
				else
				{
					Log.e(LOG_TAG, "Update overlay texture fail: " + _resultShowOverlay.ToString());
				}
			} else
			{
				Log.d(LOG_TAG, "set WVR_SetOverlayTextureId OverlayError: " + _resultSetTextureId.ToString());
			}
		}
		else
		{
			Log.e(LOG_TAG, "Neither overlay texture ID or image texture ID is 0!");
		}
	}
}
