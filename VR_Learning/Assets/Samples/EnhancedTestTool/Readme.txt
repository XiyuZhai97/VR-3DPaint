1.安装并运行M2P_Latecy_Test.apk
2.选择<Hmd> button则进入Hmd的Motion到Render的延迟测试，检测到ACC的(x,y,z)任意数据比上一帧大0.1f，则屏幕变成黄色，否则黑屏；
  按Controller上任意键，则退回到主菜单；
3.选择<Controller> button则进入 Controller 的Motion到Render的延迟测试，检测到ACC的(x,y,z)任意数据比上一帧大0.1f，则屏幕变成红色，否则黑屏；
  按Controller上任意键，则退回到主菜单；
4.选择<M2P(InSvrWrapper)> button则进入 原来的HMD的Motion到Phone的在SVR Wrapper中实现的延迟测试，有motion数据变化则闪屏，否则黑屏；
  按Controller上任意键，则退回到主菜单；
5. 注意：
    当前2是采用检测WaveVR_Utils.NEW_POSES 的Event来改变motion数据；--与屏幕刷新率有关，这与正常svr去Motion数据有差异，4更接近；
    当前3是采用检测在Update()里面去读取连接Controller的数据来改变motion数据；--与屏幕刷新率有关，与使用unity APP基本一致；
	      （当前测试发现 从WaveVR_Utils.NEW_POSES 的Event 得不到 Controller的motion）
    当前4是采用是在SVR的render的Wrapper进程中，直接读取Sensor的Motion数据和闪屏的，并且去掉正常的一些等待时间；

按照刷新来说，Controller只需要每帧（屏幕刷新率）都能拿到最新的数据就基本满足要求，所以Controller的产生频率应该在屏幕刷新率的1倍以上，2倍以内即可；

	
  
  
  