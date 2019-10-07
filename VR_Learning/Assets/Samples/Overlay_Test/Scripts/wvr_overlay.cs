// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Runtime.InteropServices;

//WVR_SetOverlayTextureId(SplashOverlayId, &texture);
//WVR_SetOverlayFixedPosition(SplashOverlayId, &position);
//WVR_ShowOverlay(SplashOverlayId);
//WVR_HideOverlay(SplashOverlayId);


namespace wvr_overlay
{
	//public const WVR_OverlayPosition_t position = { 0.0f, 0.0f, -0.4f };
	//const WVR_OverlayTexture_t texture = { SplashWrapperId, textureWidth, textureHeight }; //SplashWrapperId是透過Engine製作Texture並且把圖透過shader畫在此Texture上之後取得其id
	public class Interop_overlay
	{
		//wvr_overlay.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GenOverlay", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_OverlayError WVR_GenOverlay(ref int overlayId);

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_DelOverlay", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_OverlayError WVR_DelOverlay(int overlayId);

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetOverlayTextureId", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_OverlayError WVR_SetOverlayTextureId(int overlayId, ref WVR_OverlayTexture_t texture);

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ShowOverlay", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_OverlayError WVR_ShowOverlay(int overlayId);

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetOverlayFixedPosition", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_OverlayError WVR_SetOverlayFixedPosition(int overlayId,ref WVR_OverlayPosition_t position);

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_HideOverlay", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_OverlayError WVR_HideOverlay(int overlayId);

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsOverlayValid", CallingConvention = CallingConvention.Cdecl)]
		public static extern Boolean WVR_IsOverlayValid(int overlayId);
	}

	public enum WVR_OverlayError
	{
		WVR_OverlayError_None = 0,
		WVR_OverlayError_UnknownOverlay = 10,
		WVR_OverlayError_OverlayUnavailable = 11,
		WVR_OverlayError_InvalidParameter = 20,
	}

	public struct WVR_OverlayTexture_t
	{
		public uint textureId;
		public uint width;
		public uint height;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_OverlayPosition_t
	{
		public float x;
		public float y;
		public float z;
	}
}
