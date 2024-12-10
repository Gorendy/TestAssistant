using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using TesterHelper.context;
using TesterHelper.domain.config;
using TesterHelper.domain.dto;
using TesterHelper.domain.vo;
using TesterHelper.service;
using TesterHelper.util;
using TesterHelper.util.logger;

namespace TestProject
{
    [TestFixture]
    public class Tests
    {
        public delegate void TestDelegate();
    
        public Tests() {
            LogFactory.initLog();
        }
        private const string rootPath = @"C:\localfile\test\testhelpe";
        //[Test]
        public void FileTest() {
            var fileService = new FileService();
            TestAll(() => {
                fileService.createDirectory(rootPath, @"a\b\tast");
            });
        }

        //[Test]
        public void serializationTest() {
            var config = new Configuration() {
                systemConfig = new SystemConfig(),
                simulatorConfigs = new List<SimulatorConfig>()
            };
            config.systemConfig.deviceSaveLogPath = rootPath;
            config.systemConfig.devicePaperPath = rootPath;
            config.systemConfig.logBasePath = rootPath;
            config.systemConfig.currentUseSimulatorID = "Test";
            var si = new SimulatorConfig() {
                id = "Test",
                basePath = rootPath,
                programConfigName = "test",
                executor = "test.exe",
                tempXpath = "//test",
                template = "testemplate"
            };
            config.simulatorConfigs.Add(si);
            TestAll(() => {
                SerializeUtil.serialization(config, Path.Combine(rootPath, "TestHelperConfig.xml"));
            });
        }

        //[Test]
        public void deSerializationTest() {
            TestAll(() => {
                var a = SerializeUtil.deserialization<Configuration>(Path.Combine(rootPath, "TestHelperConfig.xml"));
                Console.WriteLine(a);
            });
        }

        //[Test]
        public void simulatorConfigTest() {
            var service = new SimulatorService();
            TestAll(() => {
                var a = SerializeUtil.deserialization<Configuration>(Path.Combine(rootPath, "TestHelperConfig.xml"));
                SystemContext.setConfiguration(a);
                var d = new DeviceDTO() {
                    ip = "192.168.23.112",
                    deviceCode = "1",
                    deviceId = "123",
                    deviceOwner = "test",
                    deviceNum = "123num"
                };
                service.addTestDeviceInfo(a.simulatorConfigs[0], d);
            });
        }
        //[Test]
        public void delSimulatorConfigTest() {
            var service = new SimulatorService();
            TestAll(() => {
                var a = SerializeUtil.deserialization<Configuration>(Path.Combine(rootPath, "TestHelperConfig.xml"));
                var d = new DeviceDTO() {
                    ip = "192.168.23.112",
                    deviceCode = "1",
                    deviceId = "123",
                    deviceOwner = "test",
                    deviceNum = "123num"
                };
                service.delDeviceInfo(a.simulatorConfigs[0], d.simulatorNodeName);
            });
        }
        //[Test]
        public void reflexTest() {
            object a = new DeviceVO() {
                ip = "127.0.0.1",
                deviceCode = "1",
                deviceId = "123",
                deviceOwner = "test"
            };
            Type type = a.GetType();
            foreach (var info in type.GetProperties()) {
                Console.WriteLine($"{info.Name}, {info.GetValue(a)}");
            }
        }
        [Test]
        public void fileFindTest() {
            foreach (var f in Directory.GetFiles(@"C:\localfile\work\ware\tool\SECSComEnable")) {
                Console.WriteLine($"file name :'{Path.GetFileName(f)}'");
            }
        }
        //[Test]
        public void processTest() {
            TestAll(() => {
                new SystemService().runProcess(@"C:\localfile\work\ware\tool\SECSComEnable\SEComSimulator.exe");
            });
        }
        
        public void TestAll(TestDelegate test) {
            var t = new Stopwatch();
            Console.WriteLine("start,,,,,");
            try {
                t.Start();
                test();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            finally {
                t.Stop();
            }
            Console.WriteLine("end,,,,,,");
            Console.WriteLine($"program run time is {t.ElapsedMilliseconds} ms");
        }
    }
}