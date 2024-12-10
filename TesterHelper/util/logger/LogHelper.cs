using System;
using System.Collections.Generic;

namespace TesterHelper.util.logger
{
    /// <summary>
    /// 区分异常种类
    /// </summary>
    public enum RCode
    {
        #region 未定义类型

        // 未定义类型
        NONE,
        WARN,
        PARAM_NOTFOUND,

        #endregion

        #region 文件操作相关

        // unknown
        FILE_WARN,
        FILE_ERROR,
        FILE_OPERATION,
        FILE_NOT_EXIST,
        // info
        FILE_EXIST,
        FILE_INFO_COPY,
        // ok
        FILE_OK_COPY,
        FILE_OK_DELETE,
        FILE_OK_UNZIP,
        FILE_OK_MOVE,
        // warn
        FILE_WARN_NOTEXIST,
        DIR_EXISTS,
        // error
        FILE_NOTFOUND,
        DIR_NOTFOUND,
        FILE_ERROR_CATEGORY,
        FILE_ERROR_DELETE,
        FILE_ERROR_COPY,
        FILE_ERROR_CREATE,
        FILE_ERROR_UNZIP,
        FILE_ERROR_MOVE,
        #endregion

        #region 配置文件操作

        // unknown
        CONF_ERROR,
        CONF_INIT_ERROR,
        CONF_WARN,
        // info
        CONF_INFO_SERIALIZATION,
        CONF_INFO_DESERIALIZATION,
        CONF_INFO_FIND,
        CONF_INFO_ADD_XMLNODE,
        CONF_INFO_UPT_XMLNODE,
        CONF_INFO_DEL_XMLNODE,
        // ok
        CONF_OK_SERIALIZATION,
        CONF_OK_DESERIALIZATION,
        CONF_OK_FIND,
        CONF_OK_ADD_XMLNODE,
        CONF_OK_DEL_XMLNODE,
        CONF_OK_UPT_XMLNODE,
        // warn
        CONF_WARN_UPDATE,
        CONF_WARN_XMLNODE_NOTFOUND,
        CONF_WARN_XMLNODE,
        // error
        CONF_ERROR_OPERATION,
        CONF_ERROR_SERIALIZATION,
        CONF_ERROR_DESERIALIZATION,
        CONF_ERROR_FIND,
        CONF_ERROR_FIND_XMLNODE,
        CONF_ERROR_ADD_XMLNODE,
        CONF_ERROR_DEL_XMLNODE,
        CONF_ERROR_UPT_XMLNODE,
        #endregion

        #region 目标程序执行

        // 程序执行
        EXE_COMPLETE,
        EXE_ERROR,

        #endregion
    }

    /// <summary>
    /// 异常种类对应信息
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// 根据类型分成不同编号和信息 00_00_00
        /// 一级分类
        /// 0：未定义错误
        /// 1：文件类
        /// 2：配置文件操作
        /// 3：程序执行
        /// 二级分类
        /// 0. unknown 1. info 2.ok, 3. warn 4.error
        /// </summary>
        private static readonly Dictionary<RCode, MessageBlock> messageList =
            new Dictionary<RCode, MessageBlock>()
            {
                #region 未定义错误
                { RCode.NONE, new MessageBlock(00_00_00, "未知错误") },
                { RCode.WARN, new MessageBlock(00_30_10, "未知警告") },
                { RCode.PARAM_NOTFOUND, new MessageBlock(00_40_00, "参数错误，未发现参数") },
                #endregion

                #region 文件
                // unknown 0
                { RCode.FILE_WARN, new MessageBlock(10_00_01, "文件操作警告") },
                { RCode.FILE_ERROR, new MessageBlock(10_00_04, "文件操作错误") },
                { RCode.FILE_OPERATION, new MessageBlock(10_00_03, "文件操作") },
                { RCode.FILE_NOT_EXIST, new MessageBlock(10_00_02, "文件不存在") },
                // info 1
                { RCode.FILE_EXIST, new MessageBlock(10_10_01, "文件已存在") },
                { RCode.FILE_INFO_COPY, new MessageBlock(10_10_02, "文件复制") },
                // ok 2
                { RCode.FILE_OK_COPY, new MessageBlock(10_20_01, "文件复制成功") },
                { RCode.FILE_OK_DELETE, new MessageBlock(10_20_02, "文件删除成功") },
                { RCode.FILE_OK_UNZIP, new MessageBlock(10_20_03, "文件解压缩成功") },
                { RCode.FILE_OK_MOVE, new MessageBlock(10_20_04, "文件移动成功") },
                // warn 3
                { RCode.FILE_WARN_NOTEXIST, new MessageBlock(10_30_01, "文件不存在") },
                { RCode.DIR_EXISTS, new MessageBlock(10_30_02, "文件夹已存在") },
                // error 4
                { RCode.FILE_NOTFOUND, new MessageBlock(10_40_01, "指定文件未找到") },
                { RCode.DIR_NOTFOUND, new MessageBlock(10_40_02, "文件夹未找到") },
                { RCode.FILE_ERROR_CATEGORY, new MessageBlock(10_40_03, "文件类型错误") },
                { RCode.FILE_ERROR_DELETE, new MessageBlock(10_40_04, "文件删除失败") },
                { RCode.FILE_ERROR_COPY, new MessageBlock(10_40_04, "文件复制失败") },
                { RCode.FILE_ERROR_CREATE, new MessageBlock(10_40_05, "文件创建失败") },
                { RCode.FILE_ERROR_UNZIP, new MessageBlock(10_40_05, "文件解压缩失败") },
                { RCode.FILE_ERROR_MOVE, new MessageBlock(10_40_06, "文件移动失败") },
                #endregion

                #region 配置文件
                // unknown 0
                { RCode.CONF_ERROR, new MessageBlock(20_00_01, "配置文件错误") },
                { RCode.CONF_WARN, new MessageBlock(20_00_02, "配置文件警告") },
                { RCode.CONF_INIT_ERROR, new MessageBlock(20_00_03, "配置文件未初始化错误") },
                // info 1
                {RCode.CONF_INFO_SERIALIZATION, new MessageBlock(20_10_01, "文件序列化")},
                {RCode.CONF_INFO_DESERIALIZATION, new MessageBlock(20_10_02, "文件反序列化")},
                {RCode.CONF_INFO_FIND, new MessageBlock(20_10_03, "配置文件查找")},
                {RCode.CONF_INFO_ADD_XMLNODE, new MessageBlock(20_10_04, "添加xml节点")},
                {RCode.CONF_INFO_DEL_XMLNODE, new MessageBlock(20_10_05, "删除xml节点")},
                {RCode.CONF_INFO_UPT_XMLNODE, new MessageBlock(20_10_06, "更新xml节点")},
                // ok 2
                {RCode.CONF_OK_SERIALIZATION, new MessageBlock(20_20_01, "文件序列化成功")},
                {RCode.CONF_OK_DESERIALIZATION, new MessageBlock(20_20_02, "文件反序列化成功")},
                {RCode.CONF_OK_FIND, new MessageBlock(20_20_03, "配置文件查找成功")},
                {RCode.CONF_OK_ADD_XMLNODE, new MessageBlock(20_20_04, "添加xml节点成功")},
                {RCode.CONF_OK_DEL_XMLNODE, new MessageBlock(20_20_05, "删除xml节点成功")},
                {RCode.CONF_OK_UPT_XMLNODE, new MessageBlock(20_20_06, "修改xml节点成功")},
                // warn 3
                { RCode.CONF_WARN_UPDATE, new MessageBlock(20_30_0, "配置文件更新警告") },
                { RCode.CONF_WARN_XMLNODE_NOTFOUND, new MessageBlock(20_30_1, "xml节点未发现") },
                { RCode.CONF_WARN_XMLNODE, new MessageBlock(20_30_2, "xml节点警告") },
                // error 4
                { RCode.CONF_ERROR_OPERATION, new MessageBlock(10_40_01, "配置文件操作错误") },
                { RCode.CONF_ERROR_SERIALIZATION, new MessageBlock(10_40_02, "序列化失败") },
                { RCode.CONF_ERROR_DESERIALIZATION, new MessageBlock(10_40_03, "反序列化失败") },
                { RCode.CONF_ERROR_FIND, new MessageBlock(10_40_04, "配置文件查找失败") },
                { RCode.CONF_ERROR_ADD_XMLNODE, new MessageBlock(10_40_05, "添加xml节点失败") },
                { RCode.CONF_ERROR_DEL_XMLNODE, new MessageBlock(10_40_06, "删除xml节点失败") },
                { RCode.CONF_ERROR_UPT_XMLNODE, new MessageBlock(10_40_07, "修改xml节点失败") },
                { RCode.CONF_ERROR_FIND_XMLNODE, new MessageBlock(10_40_08, "无法找到配置文件对应xml节点") },
                #endregion
                #region 目标程序
                { RCode.EXE_ERROR, new MessageBlock(30_00_01, "目标程序执行失败") },
                { RCode.EXE_COMPLETE, new MessageBlock(30_02_00, "目标程序执行成功，程序以运行") },                
                #endregion

            };

        public static MessageBlock detail(RCode code) {
            if (!messageList.ContainsKey(code)) {
                return messageList[RCode.NONE];
            }

            return messageList[code];
        }

        public class MessageBlock
        {
            public readonly int code;
            public readonly string message;

            public MessageBlock(int code, string message) {
                this.code = code;
                this.message = message;
            }

            public override string ToString() {
                return $"{code}:'{message}'";
            }
        }
    }

    public interface ILogger
    {
        
        void debug(RCode code, string message = null);
        void debug(string message);
        void debug(string format, params object[] pars);
        void debug(RCode code, string message, Exception e);
        void debug(RCode code, string format, params object[] pars);
        void info(RCode code, string message = null);
        void info(string message);
        void info(string format, params object[] pars);
        void info(RCode code, string message, Exception e);
        void info(RCode code, string format, params object[] pars);
        void warn(RCode code, string message = null);
        void warn(string message);
        void warn(RCode code, string message, Exception e);
        void warn(RCode code, string format, params object[] pars);
        void error(RCode code, string message = null);
        void error(string message);
        void error(RCode code, string message, Exception e);
        void error(RCode code, string format, params object[] pars);
        int getErrorNum();
        int getWarnNum();
        void reset();
    }
    
}