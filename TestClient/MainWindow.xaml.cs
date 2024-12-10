using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TesterHelper.context;
using TesterHelper.controller;
using TesterHelper.domain.vo;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly TesterController controller;
        private readonly bool isRunnable = false;
        public MainWindow() {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;//在中间显示
            // 读取配置信息
            try {
                controller = new TesterController();
                isRunnable = true;
            }
            catch (Exception e) {
                MsgDialog.showError($"can not init config.\n {e}");
            }
        }

        
        
        
        private void LoadSavedDevices()
        {
            //tester.ItemsSource = new List<string>();
        }

        private void getDeviceInfo(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            // if (tester.SelectedItem != null)
            // {
            //     string selectedDeviceInfo = tester.SelectedItem.ToString();
            //     
            // }
        }

        #region 窗口事件
        
        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            if (!isRunnable) {
                MsgDialog.showWarn("can not run because not init config program");
                return;
            }

            if (checkInvalid()) {
                MsgDialog.showWarn("存在参数值为空，不可执行");
                return;
            }
            var vo = new DeviceVO() {
                ip = txtIP.Text,
                deviceNum = txtDeviceNum.Text,
                deviceOwner = txtDeviceOwner.Text,
                deviceId = txtDeviceId.Text,
                port = txtPort.Text,
                deviceCode = txtDeviceCode.Text
            };
            controller.startUpTester(vo);
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
                this.DragMove();
            }
        }
        #endregion

        private bool checkInvalid() {
            bool result = string.IsNullOrEmpty(txtDeviceOwner.Text) || string.IsNullOrEmpty(txtPort.Text) || string.IsNullOrEmpty(txtDeviceNum.Text)
                          || string.IsNullOrEmpty(txtDeviceId.Text)  || string.IsNullOrEmpty(txtIP.Text);
            return result;
        }
    }
}