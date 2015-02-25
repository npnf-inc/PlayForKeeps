using System;
using UnityEngine;
using NPNF.Core.Utils;
using NPNF.Core.Users.Tracking;

namespace NPNF
{
		public class UserTrackingController: IUserTrackingController
		{

				public DeviceProfile DeviceProfile { get; internal set; }
				public UnityBridgePlugin bridgeObj;


				public UserTrackingController (UnityBridgePlugin bridge)
				{
						bridgeObj = bridge;
				}
		}
}

