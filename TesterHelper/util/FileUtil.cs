using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using TesterHelper.constant;
using TesterHelper.context;
using TesterHelper.exception;
using TesterHelper.util.logger;

namespace TesterHelper.util
{
    /// <summary>
    /// 文件管理类
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// 获取当前程序目录
        /// </summary>
        /// <returns></returns>
        public static string getBaseProDir() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void createDir(string path) {
            if (checkDir(path)) {
                return;
            }
            try {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex) {
                throw new ServiceException(RCode.FILE_ERROR_CREATE, ex);
            }
        }

        public static void createFile(string file) {
            if (checkFile(file)) {
                return;
            }
            try {
                if (!checkDir(SystemConstant.systemSaveInfoPath)) {
                    Directory.CreateDirectory(SystemConstant.systemSaveInfoPath);
                }
                File.Create(file);
            }
            catch (Exception ex) {
                throw new ServiceException(RCode.FILE_ERROR_CREATE, ex);
            }
        }
        
        /// <summary>
        /// 当前目录寻找文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns>非空字符路径</returns>
        public static string findFile(string path, string fileName) {
            StringUtil.checkStr(path, fileName);
            if (!checkDir(path)) {
                throw new ServiceException(RCode.DIR_NOTFOUND);
            }
            string result = null;
            foreach (string file in Directory.GetFiles(path)) {
                if (Path.GetFileName(file).Equals(fileName)) {
                    result = file;
                    break;
                }
            }
            if (result == null) {
                throw new ServiceException(RCode.FILE_NOTFOUND);
            }
            return result;
        }
        /// <summary>
        /// 递归寻找文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns>非空字符路径</returns>
        public static string findFileRecur(string path, string fileName) {
            StringUtil.checkStr(path, fileName);
            if (!checkDir(path)) {
                throw new ServiceException(RCode.DIR_NOTFOUND);
            }
            string result = null, tmp;
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0) {
                tmp = queue.Dequeue();
                foreach (string dir in Directory.GetDirectories(tmp)) {
                    queue.Enqueue(dir);
                }
                foreach (string f in Directory.GetFiles(tmp)) {
                    if (Path.GetFileName(f).Equals(fileName)) {
                        result = f;
                        queue.Clear();
                        break;
                    }
                }
            }

            if (result == null) {
                throw new ServiceException(RCode.FILE_NOTFOUND);
            }
            return result;
        }

        public static void deleteDirectory(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
                return;
            }

            if (!Directory.Exists(path)) {
                return;
            }

            foreach (var file in Directory.GetFiles(path)) {
                File.Delete(file);
            }

            foreach (var dir in Directory.GetDirectories(path)) {
                deleteDirectory(dir);
            }
            Directory.Delete(path);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="file">文件绝对路径</param>
        /// <returns></returns>
        public static bool checkFile(string file) {
            StringUtil.checkStr(file);
            return File.Exists(file);
        }

        /// <summary>
        /// 检查文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool checkDir(string path) {
            StringUtil.checkStr(path);
            return Directory.Exists(path);
        }
        
    }
}