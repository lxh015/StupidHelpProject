using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stupid
{
    /// <summary>
    /// 系统通用方法
    /// </summary>
    public class SystemCommon
    {
        /// <summary>
        /// 声音相关
        /// </summary>
        public class VoiceCommon
        {
            /// <summary>
            /// 播放声音
            /// </summary>
            /// <param name="frequency">声音频率（从37Hz到32767Hz）。在windows95中忽略</param>  
            /// <param name="duration">声音的持续时间，以毫秒为单位。</param>  
            /// <returns></returns>
            [DllImport("Kernel32.dll")] //引入命名空间 using System.Runtime.InteropServices;  
            public static extern bool Beep(int frequency, int duration);
        }

        /// <summary>
        /// 电脑信息相关
        /// </summary>
        public class ComputerInformationCommon
        {
            /// <summary>
            /// 获取计算机主机名称
            /// </summary>
            /// <returns></returns>
            public string GetHostName()
            {
                return System.Net.Dns.GetHostName();
            }


            /// <summary>
            /// 取CPU编号
            /// </summary>
            /// <returns></returns>
            public string GetCpuID()
            {
                try
                {
                    ManagementClass mc = new ManagementClass("Win32_Processor");
                    ManagementObjectCollection moc = mc.GetInstances();
                    String strCpuID = null;
                    foreach (ManagementObject mo in moc)
                    {
                        strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                        break;
                    }
                    return strCpuID;
                }
                catch
                {
                    return "";
                }
            }

            /// <summary>
            /// 获取全部硬盘编号
            /// </summary>
            /// <returns></returns>
            public List<string> GetHardDiskID()
            {
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                    String strHardDiskID = null;
                    List<string> data = new List<string>();
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        strHardDiskID = mo["SerialNumber"].ToString().Trim();
                        data.Add(strHardDiskID);
                    }
                    return data;
                }
                catch
                {
                    return new List<string>();
                }
            }


        }

    }
}
