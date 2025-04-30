using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TesterHelper.context;
using TesterHelper.controller;
using TesterHelper.domain.po;
using TesterHelper.domain.vo;

namespace TestClient
{
    public enum MsgType
    {
        info,
        warn,
        error
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly TesterController controller;
        private readonly bool isRunnable = false;
        private readonly Regex regex = new Regex(@"^\[(.*)\].*$");
        
        // 消息
        public MainWindow() {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;//在中间显示
            // 读取配置信息
            try {
                controller = new TesterController();
                isRunnable = true;
                LoadSavedDevices();
            }
            catch (Exception e) {
                MsgDialog.showError($"can not init config.\n {e}");
            }
        }

        
        
        
        private void LoadSavedDevices()
        {
            var list = new List<string>(SystemContext.deviceF.devices.Count + 1);
            list.Add("");
            foreach (var device in SystemContext.deviceF.devices) {
                list.Add($"[{device.id}]({device.firstSpell}){device.deviceOwner}-{device.deviceNum}");
            }
            txtList.ItemsSource = list;
        }

        private void getDeviceInfo(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            try {
                if (txtList.SelectedItem != null)
                {
                    string selectedDeviceInfo = txtList.SelectedItem.ToString();
                    if (string.IsNullOrEmpty(selectedDeviceInfo)) {
                        clear();
                        return;
                    }
                    string id = regex.Match(selectedDeviceInfo).Groups[1].Value;
                    foreach (var device in SystemContext.deviceF.devices) {
                        if (id.Equals(device.id)) {
                            consistInfo(device);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) {
                MsgDialog.showError($"无法选中当前设备信息;/n{exception}");
            }
        }

        #region 窗口事件
        
        private void DeleteInfo(object sender, RoutedEventArgs e) {
            if (!isRunnable) {
                MsgDialog.showWarn("can not run because not init config program");
                return;
            }
            // 删除缓存中的数据
            if (string.IsNullOrEmpty(txtId.Text)) {
                MsgDialog.showInfo("success");
                return;
            }
            var device = findDeviceInfo(txtId.Text);
            if (device == null) {
                MsgDialog.showError($"device id :'{txtId.Text}' not found");
                return;
            }
            // 删除当前信息
            controller.deleteDeviceInfo(device);
            clear();
        }
        
        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            var vo = getVo();
            if (vo == null) {
                return;
            }
            try {
                // 判断当前设备信息是否存在与测试数据中
                if (string.IsNullOrEmpty(txtId.Text)) {
                    // 在list中添加当前信息
                    controller.startUpTester(vo, out var index);
                    //添加新程序
                    //txtList.Items.Add($"[{index}]{vo.deviceOwner}-{vo.deviceNum}");
                }
                else { // 修改程序内容
                    var device = findDeviceInfo(txtId.Text);
                    if (device == null) {
                        MsgDialog.showError($"device id :'{txtId.Text}' not found");
                        return;
                    }
                    // 修改并打开
                    if (device.updateInfo(vo)) { // 数据做改动
                        controller.updateDevice();
                        controller.updateSimulatorConfig(vo);
                    }
                    else {
                        controller.selectDeviceId(vo.toDTO());
                    }
                    controller.runSimulatorOfDevice();
                }
            }
            catch (Exception exception) {
                MsgDialog.showError($"run error: {exception.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            var vo = getVo();
            if (vo == null) {
                return;
            }
            try {
                // 判断当前设备信息是否存在与测试数据中
                if (string.IsNullOrEmpty(txtId.Text)) {
                    // 在list中添加当前信息
                    controller.startUpTester(vo, out var index, false);
                    //添加新程序
                    //txtList.Items.Add($"[{index}]{vo.deviceOwner}-{vo.deviceNum}");
                }
                else { // 修改程序内容
                    var device = findDeviceInfo(txtId.Text);
                    if (device == null) {
                        MsgDialog.showError($"device id :'{txtId.Text}' not found");
                        return;
                    }
                    // 修改并打开
                    if (device.updateInfo(vo)) { // 数据做改动
                        controller.updateDevice();
                        controller.updateSimulatorConfig(vo);
                    }
                }
                MsgDialog.showInfo("success");
            }
            catch (Exception exception) {
                MsgDialog.showError($"run error: {exception.Message}");
            }
        }
        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CloseButton(object sender, RoutedEventArgs e) {
            Close();
        }
        /// <summary>
        /// 监听窗口移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        #endregion

        private void consistInfo(Device device) {
            txtDeviceOwner.Text = device.deviceOwner;
            txtDeviceNum.Text = device.deviceNum;
            txtDeviceCode.Text = device.deviceCode;
            txtDeviceId.Text = device.deviceId;
            txtPort.Text = device.port;
            txtIP.Text = device.ip;
            txtId.Text = device.id;
        }

        private DeviceVO getVo() {
            if (!isRunnable) {
                MsgDialog.showWarn("can not run because not init config program");
                return null;
            }

            if (checkInvalid()) {
                MsgDialog.showWarn("存在参数值为空，不可执行");
                return null;
            }
            var vo = new DeviceVO() {
                ip = txtIP.Text,
                deviceNum = txtDeviceNum.Text,
                deviceOwner = txtDeviceOwner.Text,
                deviceId = txtDeviceId.Text,
                port = txtPort.Text,
                deviceCode = txtDeviceCode.Text
            };
            return vo;
        }

        private void clear() {
            txtDeviceOwner.Text = "";
            txtDeviceNum.Text = "";
            txtDeviceCode.Text = "";
            txtDeviceId.Text = "0";
            txtPort.Text = "5000";
            txtIP.Text = "";
            txtId.Text = "";
        }
        private bool checkInvalid() {
            bool result = string.IsNullOrEmpty(txtDeviceOwner.Text) || string.IsNullOrEmpty(txtPort.Text) || string.IsNullOrEmpty(txtDeviceNum.Text)
                          || string.IsNullOrEmpty(txtDeviceId.Text)  || string.IsNullOrEmpty(txtIP.Text);
            return result;
        }

        private Device findDeviceInfo(string id) {
            Device d = null;
            foreach (var device in SystemContext.deviceF.devices) {
                if (device.id.Equals(id)) {
                    d = device;
                    break;
                }
            }
            return d;
        }
    }
}