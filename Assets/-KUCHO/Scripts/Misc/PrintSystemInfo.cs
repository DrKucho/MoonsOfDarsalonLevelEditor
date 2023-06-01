using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrintSystemInfo : MonoBehaviour {

    
	void Print () {
        print("batteryLevel " + SystemInfo.batteryLevel);
        print("batteryStatus " + SystemInfo.batteryStatus);
        print("deviceModel " + SystemInfo.deviceModel);
        print("deviceName " + SystemInfo.deviceName);
        print("deviceType " + SystemInfo.deviceType);
        print("deviceUniqueIdentifier " + SystemInfo.deviceUniqueIdentifier);
        print("graphicsDeviceID " + SystemInfo.graphicsDeviceID);
        print("graphicsDeviceName " + SystemInfo.graphicsDeviceName);
        print("graphicsDeviceType " + SystemInfo.graphicsDeviceType);
        print("graphicsDeviceVendor " + SystemInfo.graphicsDeviceVendor);
        print("graphicsDeviceVendorID " + SystemInfo.graphicsDeviceVendorID);
        print("graphicsDeviceVersion " + SystemInfo.graphicsDeviceVersion);
        print("graphicsMemorySize " + SystemInfo.graphicsMemorySize);
        print("graphicsMultiThreaded " + SystemInfo.graphicsMultiThreaded);
        print("graphicsShaderLevel " + SystemInfo.graphicsShaderLevel);
        print("maxTextureSize " + SystemInfo.maxTextureSize);
        print("operatingSystem " + SystemInfo.operatingSystem);
        print("operatingSystemFamily " + SystemInfo.operatingSystemFamily);
        print("processorCount " + SystemInfo.processorCount);
        print("processorFrequency " + SystemInfo.processorFrequency);
        print("processorType " + SystemInfo.processorType);
        print("systemMemorySize " + SystemInfo.systemMemorySize);


    }
}
