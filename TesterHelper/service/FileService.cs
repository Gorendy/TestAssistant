using System;
using System.IO;
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
        

        public void moveFile() {
            try {

            }
            catch (Exception e) {
                
            }
        }
    }
}