using Ironwall.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Helpers
{
    public static class DataConvertionHelper
    {
        public static EnumDataType DeviceToDataConverter(EnumDeviceType device)
        {
            switch (device)
            {
                case EnumDeviceType.NONE:
                    return EnumDataType.Group;
                case EnumDeviceType.Controller:
                    return EnumDataType.Controller;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    return EnumDataType.Sensor;
                case EnumDeviceType.IpCamera:
                    return EnumDataType.Camera;

                default:
                    return EnumDataType.None; 
            }
        }

        public static EnumDataType IntDeviceToDataConverter(int device)
        {
            switch (device)
            {
                case ((int)EnumDeviceType.NONE):
                    return EnumDataType.Group;
                case ((int)EnumDeviceType.Controller):
                    return EnumDataType.Controller;
                case ((int)EnumDeviceType.Multi):
                case ((int)EnumDeviceType.Fence):
                case ((int)EnumDeviceType.Underground):
                case ((int)EnumDeviceType.Contact):
                case ((int)EnumDeviceType.PIR):
                case ((int)EnumDeviceType.IoController):
                case ((int)EnumDeviceType.Laser):
                case ((int)EnumDeviceType.Cable):
                    return EnumDataType.Sensor;
                case ((int)EnumDeviceType.IpCamera):
                    return EnumDataType.Camera;

                default:
                    return EnumDataType.None;
            }
        }
    }
}
