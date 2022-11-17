using Android.Bluetooth;
using technoleight_THandy.Interface;
using technoleight_THandy.Event;
using technoleight_THandy.Data;
using Java.IO;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using technoleight_THandy.Common;

[assembly: Dependency(typeof(technoleight_THandy.Droid.BluetoothManager))]

namespace technoleight_THandy.Droid
{
    public class BluetoothManager : IBluetoothManager
    {
        public event EventHandler<DataEventArgs> DataReceived;
        public event EventHandler<DataEventArgs> NotifyConnet;
        private string strSockeState = Common.Const.C_CONNET_NG;
        private BluetoothSocket socket = null;

        public void BTConnet(BTDevice bTDevice)
        {
            BluetoothAdapter btAdapter = BluetoothAdapter.DefaultAdapter;
            ICollection<BluetoothDevice> devices = btAdapter.BondedDevices;
            if (devices != null && devices.Count > 0)
            {
                foreach (BluetoothDevice device in devices)
                {
                    try
                    {
                        // 設定したデバイスが対象
                        if (bTDevice.strName.Equals(device.Name) && bTDevice.strUuid.Equals(device.GetUuids().FirstOrDefault().ToString()))
                        {
                            // 前回値がconnet済みでなければconnet
                            //   実際は状態監視を入れないとconnetしているかどうかは分からないが、
                            //   今回は過去にconnetしたかどうかで判断する。
                            //   再connetしたければ一回切断すればよいだけ
                            if (strSockeState != Common.Const.C_CONNET_OK)
                            {

                                // Device情報通知
                                strSockeState = Common.Const.C_CONNET_TRY;
                                NotifyConnet?.Invoke(this, new DataEventArgs(strSockeState));

                                // socket作成
                                socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString(bTDevice.strUuid));

                                //System.Console.WriteLine("#socket State {0}", btAdapter.State.ToString());
                                //System.Console.WriteLine("#socket IsEnabled {0}", btAdapter.IsEnabled.ToString());
                                //System.Console.WriteLine("#socket IsDiscovering {0}", btAdapter.IsDiscovering.ToString());
                                //System.Console.WriteLine("#socket Address {0}", btAdapter.Address.ToString());
                                //System.Console.WriteLine("#socket BondState {0}", device.BondState.ToString());

                                Task.Run(() =>
                                {
                                    try
                                    {
                                        // 接続
                                        System.Console.WriteLine("#socket try to connect {0}:{1}:{2}", DateTime.Now.ToString(), bTDevice.strName, bTDevice.strUuid);
                                        socket.Connect();
                                        strSockeState = Common.Const.C_CONNET_OK;
                                        NotifyConnet?.Invoke(this, new DataEventArgs(strSockeState));
                                        System.Console.WriteLine("#socket complete connect {0}:{1}:{2}", DateTime.Now.ToString(), device.Name, bTDevice.strUuid);
                                        // ループで読取待ち
                                        BufferedReader br = new BufferedReader(new InputStreamReader(socket.InputStream));
                                        string data;
                                        while ((data = br.ReadLine()) != null)
                                        {
                                            DataReceived?.Invoke(this, new DataEventArgs(data));
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        System.Console.WriteLine("#socket Exception \n{0}", e.ToString());
                                    }
                                    finally
                                    {
                                        socket.Close();
                                        strSockeState = Common.Const.C_CONNET_NG;
                                        NotifyConnet?.Invoke(this, new DataEventArgs(strSockeState));
                                        System.Console.WriteLine("#socket close socket {0}:{1}:{2}", DateTime.Now.ToString(), bTDevice.strName, bTDevice.strUuid);
                                    }
                                });
                            }
                            else
                            {
                                // 接続状態と通知
                                NotifyConnet?.Invoke(this, new DataEventArgs(strSockeState));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("#socket Err {0}", e.ToString());
                    }
                }
            }
        }

        public void BTDisConnet()
        {
            try
            {
                if (null != socket)
                {
                    socket.Close();

                    strSockeState = Common.Const.C_CONNET_NG;
                    NotifyConnet?.Invoke(this, new DataEventArgs(strSockeState));
                    System.Console.WriteLine("#socket BTDisConnet close success");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("#socket BTDisConnet Err {0}", e.ToString());
            }
        }

        // ペアリング情報取得
        private List<BTDevice> result = new List<BTDevice>();
        public List<BTDevice> GetBondedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter; //アダプター作成
            ICollection<BluetoothDevice> listBondedDevices = adapter.BondedDevices;              //ペアリング済みデバイスの取得
            result.Clear();

            if (listBondedDevices != null && listBondedDevices.Count > 0)
            {
                foreach (var device in listBondedDevices)
                {
                    BTDevice btDevice = new BTDevice();
                    btDevice.strUuid = device.GetUuids().FirstOrDefault().ToString();
                    btDevice.strName = device.Name;
                    result.Add(btDevice);
                }
            }
            return result;
        }
    }
}