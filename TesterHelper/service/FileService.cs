using System;
using System.Collections.Generic;
using System.IO;
using TesterHelper.constant;
using TesterHelper.context;
using TesterHelper.domain.config;
using TesterHelper.domain.dto;
using TesterHelper.domain.po;
using TesterHelper.exception;
using TesterHelper.util;
using TesterHelper.util.logger;

namespace TesterHelper.service
{
    public class FileService
    {
        private readonly ILogger logger = LogFactory.getLogger(typeof(FileService));
        public void createDirectory(string baseDir, string dirName) {
            if (string.IsNullOrEmpty(baseDir) || string.IsNullOrEmpty(dirName)) {
                throw new ServiceException(RCode.PARAM_NOTFOUND);
            }
            logger.debug("createDirectory : start,params:{},{}", baseDir, dirName);
            string path = Path.Combine(baseDir, dirName);
            logger.info("createDirectory : start,params:{}", path);
            if (FileUtil.checkDir(path)) {
                logger.debug("createDirectory : directory is exist {}", path);
                logger.warn(RCode.DIR_EXISTS, "exists dir is {}", path);
                return;
            }
            try {
                logger.debug("createDirectory : create directory start..");
                Directory.CreateDirectory(path);
                logger.info("createDirectory : create directory success..");
                logger.debug("createDirectory : create directory end..");
            }
            catch (Exception e) {
                logger.error(RCode.FILE_ERROR_CREATE, "params:{},{},{}",baseDir,dirName, e);
                return;
            }
            logger.debug("createDirectory : end----------------");
        }

        public string findFile(string baseDir, string fileName) {
            if (string.IsNullOrEmpty(baseDir) || string.IsNullOrEmpty(fileName)) {
                throw new ServiceException(RCode.PARAM_NOTFOUND);
            }
            logger.debug("findFile : start,params:{},{}", baseDir, fileName);
            logger.info("findFile : start,params:{},{}", baseDir, fileName);
            string file = null;
            try {
                file = FileUtil.findFileRecur(baseDir, fileName);
            }
            catch (Exception e) {
                logger.error(RCode.FILE_NOT_EXIST, "can not find '{}' file in {}.\n{}", fileName, baseDir, e);
                return null;
            }
            logger.info("findFile : find file success");
            logger.debug("findFile : find file end ----------------");
            return file;
        }

        /// <summary>
        /// 保存新增设备到文件
        /// </summary>
        /// <param name="dto"></param>
        public bool saveTestingDeviceInfo(DeviceDTO dto, out int index) {
            index = 0;
            if (SystemContext.deviceF == null) {
                //FileUtil.createDir(SystemConstant.systemSaveInfoPath);
                SystemContext.deviceF = new DeviceF {
                    devices = new List<Device>(2),
                    deviceIndex = SystemConstant.idStartIndex
                };
            }
            logger.debug("saveTestingDeviceInfo : start saving ,params:{}", dto);
            logger.info("saveTestingDeviceInfo : start saving ,params:{}", dto);
            SystemContext.deviceF.devices.Add(new Device(SystemContext.deviceF.deviceIndex, dto));
            if (updateTestingDevice()) {
                logger.info("saveTestingDeviceInfo : save success");
                logger.debug("saveTestingDeviceInfo : save end -------------");
            }

            index = SystemContext.deviceF.deviceIndex++;
            return true;
        }

        public bool updateTestingDevice() {
            logger.info("updateTestingDevice : start");
            logger.debug("updateTestingDevice : start -------------");
            bool result = false;
            try {
                // 修改程序保存文件的设备信息
                result = SerializeUtil.serialization(SystemContext.deviceF, SystemConstant.testingDeviceFile);
                logger.info($"updateTestingDevice : device info class serialization - {result}");
            }
            catch (Exception e) {
                logger.error("updateTestingDevice : 文件序列化失败，无法保存测试设备信息", e);
                result = false;
            }
            logger.info("updateTestingDevice : save success");
            logger.debug("updateTestingDevice : save end -------------");
            return result;
        }

        public void deleteDirectory(string path) {
            logger.info("deleteDirectory : start");
            logger.debug("deleteDirectory : start -------------");
            try {
                FileUtil.deleteDirectory(path);
            }
            catch (Exception e) {
                logger.error("deleteDirectory : delete directory error", e);
                return;
            }
            logger.info("deleteDirectory : end");
            logger.debug("deleteDirectory : end -------------");
        }
        public void moveFile() {
            try {

            }
            catch (Exception e) {
                
            }
        }
    }
}