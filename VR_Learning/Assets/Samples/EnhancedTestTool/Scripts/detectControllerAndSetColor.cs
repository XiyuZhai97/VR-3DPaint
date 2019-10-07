using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using wvr;
using WVR_Log;
using UnityEngine.SceneManagement;
using System.Threading;

using System.IO;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

using System.Diagnostics;

// 四元数的基本数学方程为 : q = cos (a/2) + i(x * sin(a/2)) + j(y * sin(a/2)) + k(z * sin(a/2)) 其中a表示旋转角度，(x,y,z)表示旋转轴。注意：q^2 = 1.

public class detectControllerAndSetColor : MonoBehaviour
{
	public UnityEngine.Object m_WaveVR_PlatformObj;
	public UnityEngine.Object m_WaveVR_ControllerObj;	
	
	private static string LOG_TAG = "detectControllerAndSetColor";
	private bool FLAG_DEBUG = false;
	private bool IsLastTimeChangeColor = false;
	private bool IsMovingByPoseData = false;

	private bool isFixSvrHmdposeTimeStamp = true;
	private long  offsetTimeDSPToSystem = 0;//gQTimeToAndroidBoot
	private const long SET_MS_FromGetToGen = 2;
	
	private Thread detectPoseAndJudgeMovingThread;
	private bool bIsRunningThread=false;
	private bool bExitDetectPoseLoop=true;
	private WVR_DeviceType g_typeDevice = 0;
	private const int ReadPoseFrequence = 1000; //Hz 采样的密度与上下次采样旋转的角度有关系，也就是与闸值有关（gMotionToPhotonAccThreshold）和minValidImuXYZ
//	private int ReadPoseFrequence = 1000; //Hz 采样的密度与上下次采样旋转的角度有关系，也就是与闸值有关（gMotionToPhotonAccThreshold）和minValidImuXYZ
//	private const int ReadPoseFrequence = 100; //Hz 采样的密度与上下次采样旋转的角度有关系，也就是与闸值有关（gMotionToPhotonAccThreshold）和minValidImuXYZ
	private int nLastHightLightFrameCount = -1;

	
	const int NUM_SKIP_FRAMES =  10;
//	private string pathUpMenuScene = "Assets/Scenes/SelectTestItem.unity";
	private string pathUpMenuScene = "Assets/Samples/EnhancedTestTool/Scenes/EnhancedTestTool.unity";
	private int nSkipFrameCount;
	public bool m_isHmd;
	public bool m_isUseSvrWraperRender;
	
	private string RED_LED_DEV = "//sys//class//leds//red//brightness";
//	private string GREEN_LED_DEV = "//sys//class//leds//green//brightness";
//	private string BLUE_LED_DEV = "//sys//class//leds//blue//brightness";
//	private string LCD_BACKLIGHT_DEV = "//sys//class//leds//lcd-backlight//brightness";

//	private string RED_LED_DEV = "/sys/class/leds/red/brightness";
	private string GREEN_LED_DEV = "/sys/class/leds/green/brightness";
	private string BLUE_LED_DEV = "/sys/class/leds/blue/brightness";
	private string LCD_BACKLIGHT_DEV = "/sys/class/leds/lcd-backlight/brightness";

	private Color   FirstTargetColor; 
	private Color   targetColor; 
	public const string LEFTEYE = "Eye Left";
	public const string RIGHTEYE = "Eye Right";
	Camera leftCamera=null;
	Camera rightCamera=null;
	Transform head;// = new Transform();
	private Color gray = new Color(0.30f, 0.30f, 0.37f, 1.0f);
	private Color black = new Color(0.0f, 0.0f, 0.0f, 1.0f);


	
//	VAR(float, gMotionToPhotonAccThreshold, 0.999998f, kVariableNonpersistent);  //Minimum threshold for motion to be considered significant enough to light the display
//	const float gMotionToPhotonAccThreshold = 0.999999f;// cos(0.0810285度)=0.99999902522441504714715077799828 这是SVRsensor的采样频率为500Hz,旋转角度为2*0.08=0.16度
											  //0.9999989

//	const float gMotionToPhotonAccThreshold = 0.99999898672074237001559758173048;//这是最小的浮点表示角度 fIMUDegreesDiff1=0.1631294 cos(0.0815647度)
	const float gMotionToPhotonAccThreshold = 0.9999985f;// cos(0.1度)=0.99999847691329 这是SVRsensor的采样频率为500Hz,旋转角度为2*0.1=0.2度
											  //0.9999989
//	const float gMotionToPhotonAccThreshold = 0.9986f;// cos9(3度)=0.99862953475457387378449205843944 这是SVRsensor的采样频率为500Hz
	const float gMinDegreeMoving = (0.1631294f-0.0000010f); // 0.1631294f;////这是最小的浮点表示角度
	const float minValidImuXYZ = 0.0005f;//(x * sin(a/2))^2+(y * sin(a/2))^2+(z * sin(a/2))^2=(x^2+y^2+z^2)* sin(a/2)^2=sin(a/2)^2
										 //sin(0.1度)=0.00174532836589830883577820272085 sin(0.1度)^2 =3.0461711048092610090346068792767e-6 ==3.046e-6 
										 //set minValidImuXYZ =  开根号(sin(a/2)^2/3) == 0.00100763750095623839546411356484
										 //如果a=0.1度，sin(a/2)即sin(0.05度)=8.7266451523514954330458929907378e-4，sin(0.05度)^2 =7.6154335615059854937603714361717e-7
										 //set minValidImuXYZ =  开根号(sin(a/2)^2/3) == 5.0383309278324786294360272627746e-4=0.000503
										 //避免四元素全值都为0的无效值作为有效值判断。
	const float gPositionThresholdMotionToPhoton = 	1.0e-4f;	


	private bool cptEnabled = false;


	private Scene MyNowScene;
	
	private string StoreLcdBackLightValue;
	
	
	internal WVR_PoseState_t LastPose;
	internal WVR_PoseState_t NowPose;
	
	Quaternion preQuaternion =  new Quaternion(0,0,0,0);
	Quaternion currentQuaternion =  new Quaternion(0,0,0,0);
	Quaternion detaQuaternion =  new Quaternion(0,0,0,0);
	Vector3	prePosition  = new Vector3(0,0,0);
	Vector3	currentPosition  = new Vector3(0,0,0);
	Vector3	detaPosition  = new Vector3(0,0,0);

	Quaternion preRawQuaternion =  new Quaternion(0,0,0,0);
	Quaternion currentRawQuaternion =  new Quaternion(0,0,0,0);


	private static void PrintDebugLog(string msg)
	{
		Log.d (LOG_TAG, msg);
	}

	private void RestoreLightLED()
	{
		StreamReader sr;
		FileInfo t = new FileInfo(LCD_BACKLIGHT_DEV);	 
		if(!t.Exists)		  
		{			
			Log.d (LOG_TAG, "LED:"+LCD_BACKLIGHT_DEV+" is not exist!!");

		}		
		else	  
		{
			sr = File.OpenText(LCD_BACKLIGHT_DEV);
			StoreLcdBackLightValue = sr.ReadLine();
			sr.Close();
			sr.Dispose();
		}
	}
	
	private void ResetLightLED()
	{
		StreamWriter sw;
		FileInfo t = new FileInfo(LCD_BACKLIGHT_DEV);	 
		if(!t.Exists)		  
		{			
			Log.d (LOG_TAG, "LED:"+LCD_BACKLIGHT_DEV+" is not exist!!");

		}		
		else	  
		{
			sw = t.AppendText();
			sw.WriteLine(StoreLcdBackLightValue);
			sw.Close();
			sw.Dispose();
		}
	}

	private bool openLightLED(bool on)
	{
		bool isCanopenLcdBKLight = false;
		StreamWriter sw;
		StreamReader sr;
		
		try{
			FileInfo t = new FileInfo(LCD_BACKLIGHT_DEV);	 
			if(!t.Exists)		  
			{			
				Log.d (LOG_TAG, "LED:"+LCD_BACKLIGHT_DEV+" is not exist!!");
				isCanopenLcdBKLight = false;
			}		
			else	  
			{
				sw = t.AppendText();
				if(null != sw){
					if(on){
						sw.WriteLine("255");
					}else{
						sw.WriteLine("0");
					}				
					sw.Close();
					sw.Dispose();
					sr = File.OpenText(LCD_BACKLIGHT_DEV);
					Log.d(LOG_TAG,LCD_BACKLIGHT_DEV+"is ="+sr.ReadLine()+".");
					sr.Close();
					sr.Dispose();
					isCanopenLcdBKLight = true;
				}				
			}
		}
		catch(Exception e)
		{
			Log.d (LOG_TAG, "LED:"+LCD_BACKLIGHT_DEV+" ,openLightLED(it) is Error!e="+e.ToString());
			isCanopenLcdBKLight = false;
		}
		openOtherLightLEDs(on);	
		return isCanopenLcdBKLight;
	}
	 

	private bool openOtherLightLEDs(bool on)
	{
		bool isCanopenOtherLightLEDs = false;
		StreamWriter sw;	 
		FileInfo t = new FileInfo(RED_LED_DEV);	 
//		FileInfo t = new FileInfo(GREEN_LED_DEV);	 
		if(!t.Exists)		  
		{			
			Log.d (LOG_TAG, "LED:"+RED_LED_DEV+" is not exist!!");
			isCanopenOtherLightLEDs = false;
		}		
		else	  
		{
			sw = t.AppendText();
			if(null == sw){
				isCanopenOtherLightLEDs = false;
				return isCanopenOtherLightLEDs;
			}				
			if(on)
				sw.WriteLine("255");
			else
				sw.WriteLine("0");				
			sw.Close();
			sw.Dispose();			
			if(!on){
				t = new FileInfo(GREEN_LED_DEV);
				if(t.Exists)
				{
					sw = t.AppendText();
					sw.WriteLine("0");				
					sw.Close();
					sw.Dispose();			
				}
				t = new FileInfo(BLUE_LED_DEV);
				if(t.Exists)
				{
					sw = t.AppendText();
					sw.WriteLine("0");				
					sw.Close();
					sw.Dispose();			
				}
			}
		} 
		isCanopenOtherLightLEDs = true;
		return isCanopenOtherLightLEDs;
	}

	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	static extern long GetTimeMonotonic ();

	/*private static long GetNanoTime ()
	{
		long wvr_nano;
		long diff_nano;
		long nano = 10000L * Stopwatch.GetTimestamp ();
		nano /= TimeSpan.TicksPerMillisecond;
		nano *= 100L;
		wvr_nano = Interop.WVR_GetNanoTime ();
		if (wvr_nano > nano)
			diff_nano = wvr_nano - nano;
		else
			diff_nano = nano - wvr_nano;

		Log.d (LOG_TAG, "GetNanoTime, diff_nano=" + diff_nano + " , wvr_nano=" + wvr_nano + " , nano=" + nano);

		GetNanoTime2 ();
	   
		return nano;
	}*/

	private static long GetNanoTime ()
	{
		#if TESTNANO
		long wvr_nano;
		long diff_nano;
		long nano = (long)(Stopwatch.GetTimestamp () / (Stopwatch.Frequency / 1000000000.0));

		wvr_nano = Interop.WVR_GetNanoTime ();
		if (wvr_nano > nano)
			diff_nano = wvr_nano - nano;
		else
			diff_nano = nano - wvr_nano;

		Log.d (LOG_TAG, "GetNanoTime, diff_nano=" + diff_nano + " , wvr_nano=" + wvr_nano + " , nano=" + nano); 
		return nano;
		#else
		long nano = (long)(Stopwatch.GetTimestamp () / (Stopwatch.Frequency / 1000000000.0));

		PrintDebugLog("GetNanoTime, nano=" + nano); 
		return nano;
		#endif
	}

	private void DetectPoseLoop()
	{
		float fDegreesDiff = 0.0f;
//		WVR_PoseState_t LastPose;
//		WVR_PoseState_t NowPose;
		long tNsNowGet = 0;
		long tNsGenPose = 0;
		long tNsFromGenToGet = 0;
		long tMsFromGenToGet = 0;

		long tNsLastTimeGet = 0;
		long tNsLastTimeGen = 0;
		long tNsFromLastGenToNowGet = 0;
		long tMsFromLastGenToNowGet = 0;
		long tNsJudgePose = 0;
		long tMsJudgePose = 0;
		bool bConvertMatrixToQuaternion = false;

		bool needPrintTimeLog = false;
		
		offsetTimeDSPToSystem = 0;//gQTimeToAndroidBoot
		bIsRunningThread = true;
		while (!bExitDetectPoseLoop) {
			Interop.WVR_GetPoseState (g_typeDevice, WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnTrackingObserver, 0, ref NowPose);
//			Interop.WVR_GetPoseState (g_typeDevice,WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF,0,ref NowPose);
			if (NowPose.IsValidPose) {
				if (tNsGenPose == NowPose.PoseTimestamp_ns) { //得到上次有效的同样的数据
					Thread.Sleep (1000 / ReadPoseFrequence);
					continue;
				}
				//tNsJudgePose = il2cpp.icalls.mscorlib.System.DateTime.GetTimeMonotonic();
				//tNsJudgePose = GetTimeMonotonic();
				//tNsJudgePose = System.DateTime.Now.Millisecond * 1000000l;
				tNsJudgePose = GetNanoTime ();
				//tNsJudgePose *= 10^6;
				if (FLAG_DEBUG)
					DumpPoseState (NowPose);
//				if(!m_isHmd  && 0 == NowPose.RawPose.rotation.w &&
				if (0 == NowPose.RawPose.rotation.w &&
				0 == NowPose.RawPose.rotation.x &&
				0 == NowPose.RawPose.rotation.y &&
				0 == NowPose.RawPose.rotation.z) {  //use RawPose// Controller not get RawPose;
					bConvertMatrixToQuaternion = true;
					Interop.WVR_ConvertMatrixQuaternion (ref NowPose.PoseMatrix, ref NowPose.RawPose.rotation, true); //WVR_Quatf_t
					if (FLAG_DEBUG)
						Log.d (LOG_TAG, "Convert Matrix To Quaternion!!!"); 
					if (FLAG_DEBUG)
						DumpPoseState (NowPose);
				}
				currentQuaternion.Set (NowPose.RawPose.rotation.x, NowPose.RawPose.rotation.y, NowPose.RawPose.rotation.z, NowPose.RawPose.rotation.w);
//				currentQuaternion.w = NowPose.RawPose.rotation.w;
//				currentQuaternion.x = NowPose.RawPose.rotation.x;
//				currentQuaternion.y = NowPose.RawPose.rotation.y;
//				currentQuaternion.z = NowPose.RawPose.rotation.z;

				fDegreesDiff = Quaternion.Angle (preQuaternion, currentQuaternion);
				if (NowPose.Is6DoFPose) {
					currentPosition.x = NowPose.RawPose.position.v0;
					currentPosition.y = NowPose.RawPose.position.v1;
					currentPosition.z = NowPose.RawPose.position.v2;
					detaPosition = currentPosition - prePosition;
					
				}
				
				if ((Mathf.Abs (fDegreesDiff) > gMinDegreeMoving) || (
					detaPosition.x > gPositionThresholdMotionToPhoton ||
					detaPosition.y > gPositionThresholdMotionToPhoton ||
					detaPosition.z > gPositionThresholdMotionToPhoton
				)) {
//					openLightLED(true);
					changeColor ();
					needPrintTimeLog = true;
					IsMovingByPoseData = true;
					nLastHightLightFrameCount = Time.frameCount;
					Log.d (LOG_TAG, "High Light LED and LCD BackLight!!fDegreesDiff=" + fDegreesDiff + ",bConvertMatrixToQuaternion=" + bConvertMatrixToQuaternion + ".");
					/*Log.d (LOG_TAG, "Delay Time:: offsetTimeDSPToSystem="+offsetTimeDSPToSystem+
							",tNsNowGet="+tNsNowGet+",tNsGenPose="+tNsGenPose+
							 ",tNsFromGenToGet="+tNsFromGenToGet+",tNsJudgePose="+tNsJudgePose+
							 ",tNsFromLastGenToNowGet="+tNsFromLastGenToNowGet+
							 ",tMsFromGenToGet="+tMsFromGenToGet+
							 ",tMsFromLastGenToNowGet="+tMsFromLastGenToNowGet+".");*/
					if (Mathf.Abs (fDegreesDiff) <= gMinDegreeMoving)
						Log.d (LOG_TAG, "detaPosition is ={" + detaPosition.x + "," + detaPosition.y + "," + detaPosition.z + "}.");
					DumpQuatertion (preQuaternion);
					DumpQuatertion (currentQuaternion);
				} else {
					if (Time.frameCount > nLastHightLightFrameCount + 2) { //确保至少亮一帧以上时间以便捕捉 再延长1帧
//						openLightLED(false);
						clearColor ();
					}
					IsMovingByPoseData = false;
				}
				if (FLAG_DEBUG) {
					Log.d (LOG_TAG, "fDegreesDiff=" + fDegreesDiff + ",preQuaternion+currentQuaternion is ");
					DumpQuatertion (preQuaternion);
					DumpQuatertion (currentQuaternion);
				}
				 
				preQuaternion = currentQuaternion;
				prePosition = currentPosition;
				LastPose = NowPose;

				if (m_isHmd && isFixSvrHmdposeTimeStamp) {
					if (0 == tNsLastTimeGen) {
						if (tNsGenPose > tNsNowGet)
							offsetTimeDSPToSystem = tNsNowGet - tNsGenPose - (SET_MS_FromGetToGen * 1000000); //tNsFromGenToGet = tNsNowGet - tNsGenPose;
					}
				}

				//tNsNowGet = GetTimeMonotonic();
				//tNsNowGet = System.DateTime.Now.Millisecond * 1000000l;
				tNsNowGet = GetNanoTime ();
				tNsJudgePose = tNsNowGet - tNsJudgePose;
				tMsJudgePose = tNsJudgePose / 1000000;
				if (m_isHmd && isFixSvrHmdposeTimeStamp) {
					tNsFromLastGenToNowGet = tNsNowGet - (tNsLastTimeGen + offsetTimeDSPToSystem);//nTime from LastPoseGenTimeNs to NowGetDataTimeNs 
				} else
					tNsFromLastGenToNowGet = tNsNowGet - tNsLastTimeGen;//nTime from LastPoseGenTimeNs to NowGetDataTimeNs 
				tMsFromLastGenToNowGet = tNsFromLastGenToNowGet / 1000000;//nTime from LastPoseGenTimeNs to NowGetDataTimeNs 
				
				tNsGenPose = NowPose.PoseTimestamp_ns;
				if (m_isHmd && isFixSvrHmdposeTimeStamp) {
					tNsFromGenToGet = tNsNowGet - (tNsGenPose + offsetTimeDSPToSystem);
				} else
					tNsFromGenToGet = tNsNowGet - tNsGenPose;
					
				tMsFromGenToGet = tNsFromGenToGet / 1000000;
				tNsLastTimeGen = tNsGenPose;
				tNsLastTimeGet = tNsNowGet;
				
				if (needPrintTimeLog) {
					Log.d (LOG_TAG, "Delay Time:: offsetTimeDSPToSystem=" + offsetTimeDSPToSystem +
					",tNsNowGet=" + tNsNowGet + ",tNsGenPose=" + tNsGenPose +
					",tNsFromGenToGet=" + tNsFromGenToGet + ",tNsJudgePose=" + tNsJudgePose +
					",tNsFromLastGenToNowGet=" + tNsFromLastGenToNowGet +
					",tMsFromGenToGet=" + tMsFromGenToGet +
					",tMsFromLastGenToNowGet=" + tMsFromLastGenToNowGet + ".");
				}

				preRawQuaternion = currentRawQuaternion;
				if (tMsFromGenToGet - tMsJudgePose > 10) {
					Log.d (LOG_TAG, "Get Time is long!!tMsFromGenToGet=" + tMsFromGenToGet + ",tMsJudgePose=" + tMsJudgePose + ";");
				}
				if (tMsJudgePose > 10) {
					Log.d (LOG_TAG, "Judge Pose Time is long!!tNsJudgePose=" + tMsJudgePose + ";");
				}

				needPrintTimeLog = false;
				Thread.Sleep (1000 / ReadPoseFrequence);
			}
		}
	}
	
	void onUninit()
	{
		if(bIsRunningThread)
		{
			bExitDetectPoseLoop = true;
			detectPoseAndJudgeMovingThread.Join();
			bIsRunningThread = false;
		}
		
		WaveVR_Utils.Event.Remove (WaveVR_Utils.Event.ALL_VREVENT, OnEvent);	
		if(m_isHmd && m_isUseSvrWraperRender)
			WaveVR_Utils.SetSubmitOptionalArgument(null, (int)WVR_SubmitExtend.WVR_SubmitExtend_Default);//WVR_SubmitExtend_Default=0,WVR_SubmitExtend_DisableDisortion=1
		nSkipFrameCount = 0;
		cptEnabled = false;
		ResetLightLED();
	}

	void onInit()
	{
		WaveVR_Utils.Event.Listen (WaveVR_Utils.Event.ALL_VREVENT, OnEvent);
		nSkipFrameCount = 0;
		cptEnabled = true;
		
		bExitDetectPoseLoop = false;
		detectPoseAndJudgeMovingThread =  new Thread(DetectPoseLoop);
		bIsRunningThread = false;
		RestoreLightLED();
		
	}
	
	// Use this for initialization
	void Start () {
		WaveVR.Device HmdDevice = null;
		WaveVR.Device RightController = null;
		WaveVR.Device LeftController = null;	
		GameObject vr;
		
		Log.d (LOG_TAG, "++Start++,m_isHmd="+m_isHmd+",m_isUseSvrWraperRender="+m_isUseSvrWraperRender);
		onInit();

		vr = (GameObject)Instantiate(m_WaveVR_PlatformObj);  
		
		vr.transform.position = new Vector3 (0f,4f,0f);
		vr.transform.eulerAngles = new Vector3(0,90,0);
		head = vr.transform.Find ("head");
		Log.d (LOG_TAG, "m_WaveVR_PlatformObj="+m_WaveVR_PlatformObj+",vr="+vr+",head="+head);

		HmdDevice = WaveVR.Instance.getDeviceByType (WVR_DeviceType.WVR_DeviceType_HMD);
		if (null != HmdDevice && HmdDevice.connected)
		{
			g_typeDevice = WVR_DeviceType.WVR_DeviceType_HMD;
			Log.d (LOG_TAG, "Find WVR_DeviceType_HMD!");
		}
		RightController = WaveVR.Instance.getDeviceByType (WVR_DeviceType.WVR_DeviceType_Controller_Right);
		if (null != RightController && RightController.connected)
		{
			g_typeDevice = WVR_DeviceType.WVR_DeviceType_Controller_Right;
			Log.d (LOG_TAG, "Find WVR_DeviceType_Controller_Right!");
		}
		LeftController = WaveVR.Instance.getDeviceByType (WVR_DeviceType.WVR_DeviceType_Controller_Left);
		if(null != LeftController && LeftController.connected)
		{
			g_typeDevice = WVR_DeviceType.WVR_DeviceType_Controller_Left;
			Log.d (LOG_TAG, "Find WVR_DeviceType_Controller_Left!");
		}		
		if(m_isHmd){
			g_typeDevice = WVR_DeviceType.WVR_DeviceType_HMD;
			FirstTargetColor = new Color(1,1,0,1);//Color.blue;
			targetColor = Color.cyan;
			Log.d (LOG_TAG, "Is Testing Hmd!! g_typeDevice="+g_typeDevice);
		}else{
//			ReadPoseFrequence = 100; //for Controller set 100Hz.
			FirstTargetColor = Color.red;
			targetColor = Color.magenta;//Color.yellow;
			Log.d (LOG_TAG, "Is Testing Controller!! g_typeDevice="+g_typeDevice);
		}
		if(0 != g_typeDevice)
		{
			Interop.WVR_GetPoseState (g_typeDevice,WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnTrackingObserver,0,ref NowPose);
			if(NowPose.IsValidPose)
			{
				if(0 == NowPose.RawPose.rotation.w &&
				0 == NowPose.RawPose.rotation.x &&
				0 == NowPose.RawPose.rotation.y &&
				0 == NowPose.RawPose.rotation.z )  //use RawPose
				{ // Controller not get RawPose;
					Interop.WVR_ConvertMatrixQuaternion(ref NowPose.PoseMatrix,ref NowPose.RawPose.rotation,true); //WVR_Quatf_t
				}
				currentQuaternion.Set(NowPose.RawPose.rotation.x,NowPose.RawPose.rotation.y,NowPose.RawPose.rotation.z,NowPose.RawPose.rotation.w);
				preQuaternion = currentQuaternion;
			}
		}

		
		
		MyNowScene = SceneManager.GetActiveScene ();

		Log.d (LOG_TAG, "--Start--");
		nSkipFrameCount = 0;
		cptEnabled = true;
	}

	// Update is called once per frame
	void Update () {
	
		if(nSkipFrameCount < NUM_SKIP_FRAMES)
		{
			nSkipFrameCount++;
			return;
		}

		if(m_isHmd && m_isUseSvrWraperRender)
			WaveVR_Utils.SetSubmitOptionalArgument(null, unchecked((int) 0xF9E8D7C6));
		else if(!bIsRunningThread){
			detectPoseAndJudgeMovingThread.Start();	
			bIsRunningThread =  true;
			SetLcdRedColor();
		}else{
			SetLcdRedColor();
		}
		if (Input.anyKeyDown)
		{
			
			Log.d (LOG_TAG, "Input.anyKeyDown is pressed.");
			GoBackSelectItemOrExitGame();
/*			
			foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKeyDown(keyCode))
				{
					Debug.LogError("Current Key is : " + keyCode.ToString());
				}
			}
*/			
		}
	}

	void OnEvent(params object[] args)
	{
		WVR_Event_t _event = (WVR_Event_t)args[0];
		Log.d (LOG_TAG, "OnEvent() " + _event.common.type+",nSkipFrameCount="+nSkipFrameCount);
		if(nSkipFrameCount < NUM_SKIP_FRAMES)
			return;

		switch (_event.common.type)
		{
		case WVR_EventType.WVR_EventType_ButtonPressed:
//		case WVR_EventType.WVR_EventType_ButtonUnpressed:
			// Get system key
//			if (_event.input.inputId == WVR_InputId.WVR_InputId_Alias1_System) //any Button Unpress
			{
				Log.d (LOG_TAG, "OnEvent() WVR_InputId_Alias1_System is pressed.");
				GoBackSelectItemOrExitGame();
			}
			break;
		case WVR_EventType.WVR_EventType_RecenterSuccess:
		case WVR_EventType.WVR_EventType_RecenterSuccess3DoF:
			Log.d (LOG_TAG, "OnEvent() recentered.");
			break;
		case WVR_EventType.WVR_EventType_DeviceConnected:
			if((0 == g_typeDevice) && (WVR_DeviceType.WVR_DeviceType_HMD != _event.device.type))
			{
				g_typeDevice = _event.device.type;
			}
			break;
		case WVR_EventType.WVR_EventType_DeviceDisconnected:
			break;
		}
	}	
	

	private void checkCamera()
	{
		if (leftCamera == null) {
			leftCamera = head.Find (LEFTEYE).GetComponent<Camera> ();
			if (leftCamera != null) leftCamera.clearFlags = CameraClearFlags.SolidColor;
			if (leftCamera != null)
				 Log.d (LOG_TAG, "get matrix vr left camera..");
		}
		if (rightCamera == null) {
			rightCamera = head.Find (RIGHTEYE).GetComponent<Camera> (); 
			if (rightCamera != null) rightCamera.clearFlags = CameraClearFlags.SolidColor;
			if (rightCamera != null)
				Log.d (LOG_TAG, "get matrix vr right camera..");
		}

	}

		
	private void SetLcdRedColor ()
	{
		if (leftCamera == null) {
			Transform tform = head.Find (LEFTEYE);
			if (tform) {
				//tform.gameObject.SetActive (true);
				leftCamera = tform.GetComponent<Camera> ();
				if (leftCamera != null) {
					leftCamera.clearFlags = CameraClearFlags.SolidColor;
					Log.d (LOG_TAG, "get matrix vr left camera..");
					leftCamera.backgroundColor = FirstTargetColor;//Color.red;
				}
			}
		}
		if (rightCamera == null) {
			Transform tform = head.Find (RIGHTEYE);
			if (tform) {
				//tform.gameObject.SetActive (true);
				rightCamera = tform.GetComponent<Camera> (); 
				if (rightCamera != null) {
					rightCamera.clearFlags = CameraClearFlags.SolidColor;
					Log.d (LOG_TAG, "get matrix vr right camera..");
					rightCamera.backgroundColor = FirstTargetColor;//Color.red;
				}
			}
		}
	}
	
	private void changeColor()
	{
		if(!openLightLED(true))
		{
			checkCamera();
			if(leftCamera !=null)  leftCamera.backgroundColor = targetColor;//Color.red;
			if(rightCamera !=null) rightCamera.backgroundColor =  targetColor;//Color.red;
		}
		if(!IsLastTimeChangeColor) Log.d (LOG_TAG, "changeColor!!!!!!!!!!!!!!!!!!!!!");
		IsLastTimeChangeColor = true;
	}

	private void clearColor()
	{
		if(!openLightLED(false))
		{
			checkCamera();
			if(leftCamera !=null)  leftCamera.backgroundColor = Color.black;  //gray;
			if(rightCamera !=null) rightCamera.backgroundColor = Color.black;  //gray;
		}
		if(IsLastTimeChangeColor) Log.d (LOG_TAG, "clearColor!!!!!!!!!!!!!!!!!!!!!");
		IsLastTimeChangeColor = false;
	}

	private void GoBackSelectItemOrExitGame()	
	{
		Scene UpMenuScene = SceneManager.GetSceneByPath(pathUpMenuScene);
		 
		Log.d (LOG_TAG, "Goto Back -- SelectTestItem!!,UpMenuScene="+UpMenuScene);
		onUninit();
		if(null != UpMenuScene)
		{
			SceneManager.LoadScene(pathUpMenuScene); //,LoadSceneMode.Single);
	//		SceneManager.LoadScene(0); //,LoadSceneMode.Single);
		}else{
			Log.d (LOG_TAG, "ExitGame!!");
			Application.Quit();
		}
	}

	private void DumpQuatertion(Quaternion InQuaternion)
	{
		Log.d (LOG_TAG,"DumpQuatertion:: Rotation={"+InQuaternion.w+","+InQuaternion.x+","+InQuaternion.y+","+InQuaternion.z+"}"+
		".<end>");		
	}

	private void DumpRigidTransform(WaveVR_Utils.RigidTransform InRtPose)
	{
		Vector3 Position;
		Quaternion Rotation;
		
		if(null == InRtPose) return;
		 
		 Position = InRtPose.pos;
		 Rotation = InRtPose.rot;	

		Log.d (LOG_TAG,"RigidTransform:: Position={"+Position.x+","+Position.y+","+Position.z+"}"+ 
		",Rotation={"+Rotation.x+","+Rotation.y+","+Rotation.z+","+Rotation.w+"}"+
		".<end>");
	
//		Log.d (LOG_TAG,"Position="+Position+ 
//		",Rotation="+Rotation+
//		".<end>");
	}


   private void DumpPoseState(WVR_PoseState_t tPoseState)
   {
//	   if(tPoseState == null || !tPoseState.IsValidPose ) return;
	   
	   bool tIsValidPose =  tPoseState.IsValidPose;
	   WVR_Matrix4f_t tPoseMatrix = tPoseState.PoseMatrix;
	   WVR_Vector3f_t tVelocity =  tPoseState.Velocity;
	   WVR_Vector3f_t tAngV =  tPoseState.AngularVelocity;
	   bool tIs6DoFPose =  tPoseState.Is6DoFPose;
	   long tStamp_ns =  tPoseState.PoseTimestamp_ns;
	   WVR_Vector3f_t tAcceleration =  tPoseState.Acceleration;
	   WVR_Vector3f_t tAngAcc =  tPoseState.AngularAcceleration;
	   float tPredictedMilliSec =  tPoseState.PredictedMilliSec;
	   WVR_PoseOriginModel tOriginModel =  tPoseState.OriginModel;
	   WVR_Pose_t tRawPose =  tPoseState.RawPose;
	   WVR_Vector3f_t tPosition =  tRawPose.position;
	   WVR_Quatf_t tRotation =  tRawPose.rotation;


	   Log.d (LOG_TAG,"PoseState:: IsValidPose="+tIsValidPose+ 
			  	",Stamp_ns="+tStamp_ns+ 
				",RawPose.Postion(x,y,z)="+tPosition.v0+","+tPosition.v1+","+tPosition.v2 + 
				",RawPose.Rotation(w,x,y,z)="+tRotation.w+","+tRotation.x+","+tRotation.y+","+tRotation.z+ 
 				",Velocity(x,y,z)="+tVelocity.v0+","+tVelocity.v1+","+tVelocity.v2 + 
				",AngularVelocity(x,y,z)="+tAngV.v0+","+tAngV.v1+","+tAngV.v2 + 
				",Acc(x,y,z)="+tAcceleration.v0+","+tAcceleration.v1+","+tAcceleration.v2 + 
				",AngAcc(x,y,z)="+tAngAcc.v0+","+tAngAcc.v1+","+tAngAcc.v2 + 
				",OriginModel="+tOriginModel + 
				",PredictedMilliSec="+tPredictedMilliSec + 
				",PoseMatrix(4X1)="+tPoseMatrix.m0+","+tPoseMatrix.m1+","+tPoseMatrix.m2+","+tPoseMatrix.m3 + 
				",PoseMatrix(4X2)="+tPoseMatrix.m4+","+tPoseMatrix.m5+","+tPoseMatrix.m6+","+tPoseMatrix.m7 + 
				",PoseMatrix(4X3)="+tPoseMatrix.m8+","+tPoseMatrix.m9+","+tPoseMatrix.m10+","+tPoseMatrix.m11 + 
				",PoseMatrix(4X4)="+tPoseMatrix.m12+","+tPoseMatrix.m13+","+tPoseMatrix.m14+","+tPoseMatrix.m15+
				".<end>");	   
   }

   private void DumpDevicePosePair(WVR_DevicePosePair_t InPairPose)
   {
//	   if(InPairPose == null) return;
	  	
   	   WVR_PoseState_t tPoseState = InPairPose.pose;
	   
	   bool tIsValidPose =  tPoseState.IsValidPose;
	   WVR_Matrix4f_t tPoseMatrix = tPoseState.PoseMatrix;
	   WVR_Vector3f_t tVelocity =  tPoseState.Velocity;
	   WVR_Vector3f_t tAngV =  tPoseState.AngularVelocity;
	   bool tIs6DoFPose =  tPoseState.Is6DoFPose;
	   long tStamp_ns =  tPoseState.PoseTimestamp_ns;
	   WVR_Vector3f_t tAcceleration =  tPoseState.Acceleration;
	   WVR_Vector3f_t tAngAcc =  tPoseState.AngularAcceleration;
	   float tPredictedMilliSec =  tPoseState.PredictedMilliSec;
	   WVR_PoseOriginModel tOriginModel =  tPoseState.OriginModel;
	   WVR_Pose_t tRawPose =  tPoseState.RawPose;

	   WVR_Vector3f_t tPosition =  tRawPose.position;
	   WVR_Quatf_t tRotation =  tRawPose.rotation;

	   Log.d (LOG_TAG,"DevicePosePair:: type="+InPairPose.type+ 
			  	",IsValidPose="+tIsValidPose+ 
			  	",Stamp_ns="+tStamp_ns+ 
				",RawPose.Postion(x,y,z)="+tPosition.v0+","+tPosition.v1+","+tPosition.v2 + 
				",RawPose.Rotation(w,x,y,z)="+tRotation.w+","+tRotation.x+","+tRotation.y+","+tRotation.z+ 
 				",Velocity(x,y,z)="+tVelocity.v0+","+tVelocity.v1+","+tVelocity.v2 + 
				",AngularVelocity(x,y,z)="+tAngV.v0+","+tAngV.v1+","+tAngV.v2 + 
				",Acc(x,y,z)="+tAcceleration.v0+","+tAcceleration.v1+","+tAcceleration.v2 + 
				",AngAcc(x,y,z)="+tAngAcc.v0+","+tAngAcc.v1+","+tAngAcc.v2 + 
				",OriginModel="+tOriginModel + 
				",PredictedMilliSec="+tPredictedMilliSec + 
				",PoseMatrix(4X1)="+tPoseMatrix.m0+","+tPoseMatrix.m1+","+tPoseMatrix.m2+","+tPoseMatrix.m3 + 
				",PoseMatrix(4X2)="+tPoseMatrix.m4+","+tPoseMatrix.m5+","+tPoseMatrix.m6+","+tPoseMatrix.m7 + 
				",PoseMatrix(4X3)="+tPoseMatrix.m8+","+tPoseMatrix.m9+","+tPoseMatrix.m10+","+tPoseMatrix.m11 + 
				",PoseMatrix(4X4)="+tPoseMatrix.m12+","+tPoseMatrix.m13+","+tPoseMatrix.m14+","+tPoseMatrix.m15+
				".<end>");
   	}
	
	private void DumpWVR_Matrix4f(WVR_Matrix4f_t tPoseMatrix)
	{
	   Log.d (LOG_TAG,"WVR_Matrix4f="+
				"(4X1)={"+tPoseMatrix.m0+","+tPoseMatrix.m1+","+tPoseMatrix.m2+","+tPoseMatrix.m3 + "}"+
				"(4X2)={"+tPoseMatrix.m4+","+tPoseMatrix.m5+","+tPoseMatrix.m6+","+tPoseMatrix.m7 +  "}"+
				"(4X3)={"+tPoseMatrix.m8+","+tPoseMatrix.m9+","+tPoseMatrix.m10+","+tPoseMatrix.m11 +  "}"+
				"(4X4)={"+tPoseMatrix.m12+","+tPoseMatrix.m13+","+tPoseMatrix.m14+","+tPoseMatrix.m15+ "}"+
				".<end>");
	}

}
