using HomeSeerAPI;
using Hspi;
using Scheduler.Classes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HSPI_UdpSeerLight
{
    internal enum DeviceId
    {
        Temperature = 1357,
        TemperatureRising = 1365,
        CommonCurrent = 1363,
        SmallMotor = 1362,
        L1Motor = 1366,
        L2Motor = 1364,
        MotorPower = 1371
        //1357=Temp
        //1365=TempRising
        //1363=common
        //1362=small motor
        //1366=L1 motor
        //1364=L2 motor
        //1371=MotorPower
    }

    internal class HSPI:HspiBase
    {

        private static CancellationTokenSource _cancellationTokenSource;
        private static UdpClient _udpClient;
        private Task _listeningTask;
        private int UdpPort = 49999;

        private const string PluginName = "UdpSeerLight";
        protected override string GetName()
        {
            return PluginName;
        }

        public override string InitIO(string port)
        {
            // debug
            HS.WriteLog(PluginName, "Entering InitIO");

            // initialise everything here, return a blank string only if successful, or an error message
            _udpClient = new UdpClient(UdpPort);
            _cancellationTokenSource = new CancellationTokenSource();

            _listeningTask = ListenForUdpDataAsync(_cancellationTokenSource.Token);



            // debug
            HS.WriteLog(PluginName, "Completed InitIO");
            return "";
        }

        public async Task ListenForUdpDataAsync(CancellationToken cancellationToken)
        {
            if (_udpClient == null) return;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await _udpClient.ReceiveAsync();

                    string receivedData = Encoding.UTF8.GetString(result.Buffer);
                    Console.WriteLine($"[{result.RemoteEndPoint}] Received: {receivedData}");

                    ProcessUdpData(receivedData, result.RemoteEndPoint);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("UDP listening cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex.Message}");
            }
        }

        private void ProcessUdpData(string data, IPEndPoint remoteEndPoint)
        {
            // Process received UDP data here
            // Example: Parse device commands, sensor data, etc.
            Console.WriteLine($"Processing data from {remoteEndPoint}: {data}");
            HS.WriteLog(PluginName, $"Processing data from {remoteEndPoint}: {data}");

            SplitDataAndUpdateDevices(data);

            //Split data into segments and update devices
            //1357=Temp
            //1365=TempRising
            //1363=common
            //1362=small motor
            //1366=L1 motor
            //1364=L2 motor
            //1371=MotorPower

        }

        private void SplitDataAndUpdateDevices(string data)
        {
            var result = new JsonSplitter().SplitJsonString(data);
            UpdateDevicesWithValues(result);
        }

        private void UpdateDevicesWithValues(FanData result)
        {

            UpdateDevicesWithDecimalValues(result.Temperature, (int)DeviceId.Temperature);
            UpdateDevicesWithIntValues(result.TempRising, (int)DeviceId.TemperatureRising);
            UpdateDevicesWithIntValues(result.CommonCurrent, (int)DeviceId.CommonCurrent);
            UpdateDevicesWithIntValues(result.SmallMotor, (int)DeviceId.SmallMotor);
            UpdateDevicesWithIntValues(result.L1Motor, (int)DeviceId.L1Motor);
            UpdateDevicesWithIntValues(result.L2Motor, (int)DeviceId.L2Motor);
            UpdateDevicesWithIntValues(result.MotorPower, (int)DeviceId.MotorPower);
            







          
        }

        private void UpdateDevicesWithIntValues(int valueToUpdate, int dvRef)
        {
            var childDevice = GetDeviceByHSRefId(dvRef);
            if (childDevice != null)
            {
                HS.SetDeviceValueByRef(dvRef,valueToUpdate , true);
                
                //if (!string.IsNullOrWhiteSpace(stringToUpdateTo))
                //{
                //    HS.SetDeviceString(dvRef, stringToUpdateTo, true);
                //}
            }
        }

        private void UpdateDevicesWithDecimalValues(decimal valueToUpdateTo, int dvRef)
        {
            var childDevice = GetDeviceByHSRefId(dvRef);
            if (childDevice != null)
            {
                HS.SetDeviceValueByRef(dvRef, (double)valueToUpdateTo, true);
                
                //var stringToUpdateTo = unitData.GetStringToUpdateTo();
                //if (!string.IsNullOrWhiteSpace(stringToUpdateTo))
                //{
                //    HS.SetDeviceString(dvRef, stringToUpdateTo, true);
                //}
            }
        }

        private DeviceClass GetDeviceByHSRefId(int deviceId)
        {
            var foundDeviceByRef = (DeviceClass)HS.GetDeviceByRef(deviceId);
            return foundDeviceByRef;
        }

        private void ShutdownListener()
        {
            try
            {
                _cancellationTokenSource?.Cancel();

                if (_listeningTask != null)
                {
                    _listeningTask.Wait(5000);
                }

                _udpClient?.Dispose();
                _cancellationTokenSource?.Dispose();

                Console.WriteLine($" shut down successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error shutting down plugin: {ex.Message}");
            }
        }

        public override void ShutdownIO()
        {
            HS.WriteLog(Name, "Entering ShutdownIO");

            // shut everything down here
            ShutdownListener();

            // let our console wrapper know we are finished
            Shutdown = true;

            // debug
            HS.WriteLog(Name, "Completed ShutdownIO");
        }



        public override string GetPagePlugin(string page, string user, int userRights, string queryString)
        {
            return "";// _ecowittPlugin.GetPagePlugin(page, user, userRights, queryString);
        }

        public override string PostBackProc(string page, string data, string user, int userRights)
        {
            return "";//_ecowittPlugin.PostBackProc(page, data, user, userRights); ;
        }
        public override object PluginPropertyGet(string propertyName, object[] parameters)
        {
            return null;//_ecowittPlugin.PluginPropertyGet(propertyName, parameters);
        }

        public override void PluginPropertySet(string propertyName, object value)
        {
            return;// _ecowittPlugin.PluginPropertySet(propertyName, value);
        }


        protected override bool GetHscomPort()
        {
            return false;
        }

        public override string InstanceFriendlyName()
        {
            return string.Empty;
        }

        public override IPlugInAPI.strInterfaceStatus InterfaceStatus()
        {
            return new IPlugInAPI.strInterfaceStatus { intStatus = IPlugInAPI.enumInterfaceStatus.OK };
        }

        public override int Capabilities()
        {
            return (int)Enums.eCapabilities.CA_IO;
        }

        public override int AccessLevel()
        {
            return 1;
        }

        public override bool SupportsAddDevice()
        {
            return false;
        }

        public override bool SupportsConfigDevice()
        {
            return false;
        }

        public override bool SupportsConfigDeviceAll()
        {
            return false;
        }

        public override bool SupportsMultipleInstances()
        {
            return false;
        }

        public override bool SupportsMultipleInstancesSingleEXE()
        {
            return false;
        }

        public override bool RaisesGenericCallbacks()
        {
            return false;
        }

        public override void HSEvent(Enums.HSEvent eventType, object[] parameters)
        {
        }



        public override IPlugInAPI.PollResultInfo PollDevice(int deviceId)
        {
            // return the value of a device on demand

            return new IPlugInAPI.PollResultInfo
            {
                Result = IPlugInAPI.enumPollResult.Device_Not_Found,
                Value = 0
            };
        }

        public override void SetIOMulti(List<CAPI.CAPIControl> colSend)
        {
            // HomeSeer will inform us when the one of our devices has changed.  Push that change through to the field.
        }


        public override SearchReturn[] Search(string searchString, bool regEx)
        {
            return null;
        }

        public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)
        {
            return "";
        }

        public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return true;
        }

        public override int ActionCount()
        {
            return 0;
        }

        public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return "";
        }

        public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo actionInfo)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public override bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            return false;
        }

        public override string get_ActionName(int actionNumber)
        {
            return "";
        }

        public override bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return false;
        }

        public override bool get_HasConditions(int triggerNumber)
        {
            return false;
        }

        public override string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
        {
            return "";
        }

        public override string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return "";
        }

        public override IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo actionInfo)
        {
            return new IPlugInAPI.strMultiReturn();
        }

        public override bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
        {
            return false;
        }

        public override bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return false;
        }

        public override int get_SubTriggerCount(int triggerNumber)
        {
            return 0;
        }

        public override string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
        {
            return "";
        }

        public override bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return true;
        }

        public override string get_TriggerName(int triggerNumber)
        {
            return "";
        }

        public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return false;
        }

        public override void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
        {
        }

        public override void SpeakIn(int deviceId, string txt, bool w, string host)
        {
        }

        public override string GenPage(string link)
        {
            return "";
        }

        public override string PagePut(string data)
        {
            return "";
        }


        public override string ConfigDevice(int deviceId, string user, int userRights, bool newDevice)
        {
            return "";
        }

        public override Enums.ConfigDevicePostReturn ConfigDevicePost(int deviceId, string data, string user, int userRights)
        {
            return Enums.ConfigDevicePostReturn.DoneAndCancel;
        }

        public override object PluginFunction(string functionName, object[] parameters)
        {
            return null;
        }



        protected override bool GetHasTriggers()
        {
            return false;
        }

        protected override int GetTriggerCount()
        {
            return 0;
        }



        public override void SetDeviceValue(int deviceId, double value, bool trigger = true)
        {
        }
    }
}
