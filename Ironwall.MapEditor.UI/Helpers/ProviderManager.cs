using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.ViewModels.ContentControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.Helpers
{
    public static class ProviderManager
    {
        /// <summary>
        /// ControllerProvider를 이용하여, 가장 큰 Controller 아이디 값을 반환하는 메소드
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static int GetMaxControllerID(ControllerProvider provider)
        {
            var idController = 0;
            provider.Select(t => t).ToList().ForEach(t =>
            {
                if (t.IdController > idController)
                    idController = t.IdController;
            });
            return idController;
        }
        /// <summary>
        /// SensorProvider를 이용하여, 선택된 Controller 중 가장 큰 Sensor 아이디 값을 반환하는 메소드
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="idController"></param>
        /// <returns></returns>
        public static int GetMaxSensorID(SensorProvider provider, int idController)
        {
            var idSensor = 0;
            provider.Where(t => t.IdController == idController && idController != 0).Select(t => t).ToList().ForEach(t =>
            {
                if (t.IdSensor > idSensor)
                    idSensor = t.IdSensor;
            });
            return idSensor;
        }
        /// <summary>
        /// MapProvider를 이용하여, MapNumber 중 가장 큰 값을 반환하는 메소드
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static int GetMaxMapID(MapProvider provider)
        {
            var idMap = 0;
            provider.Select(t => t).ToList().ForEach(t =>
             {
                 if (t.MapNumber > idMap)
                     idMap = t.MapNumber;
             });
            return idMap;
        }

        public static string GetMaxNameArea(GroupProvider provider)
        {
            var nameArea = "0";

            var nameAreaValue = 0;
            var value = 0;
            provider.Select(t => t).ToList().ForEach(t =>
            {
                try
                {
                    ///Provider에 등록된 NameArea 정보를 int type으로 변경
                    ///단 Untitle있을 경우 Exception이 예상됨
                    int.TryParse(t.NameArea, out value);
                    int.TryParse(nameArea, out nameAreaValue);

                    if (value > nameAreaValue)
                        nameArea = t.NameArea;
                }
                catch (Exception)
                {
                }

                
            });
            return nameArea;
        }
        
    }
}
