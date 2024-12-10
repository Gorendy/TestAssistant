using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TesterHelper.exception;
using TesterHelper.util.logger;

namespace TesterHelper.util
{
    /// <summary>
    /// 用来解析xml文件内容
    /// </summary>
    public class XmlUtil
    {
        /// <summary>
        /// 复制节点时，需要修改节点内容
        /// </summary>
        public delegate string modifyContent(XmlNode node);

        /// <summary>
        /// 修改节点属性
        /// </summary>
        public delegate string modifyAttribute(XmlAttribute attribute);

        /// <summary>
        /// 将xml文件节点中待替换字符，根据类属性名替换成类属性对应的值
        /// xml节点代替换格式#{fieldName}（只可替换参数值和节点值）
        /// 存在多个相同ID格式，填充一致
        /// </summary>
        /// <param name="node"></param>
        /// <param name="o"></param>
        /// <returns>xml代替换节点数量是否大于实体类有效参数值数量。null:成功，string:未更新节点</returns>
        /// <exception cref="ServiceException">实体参数为空或xml替换的数量与参数数量不一致</exception>
        public static string consistXmlInfo(XmlNode node, object o) {
            if (node == null || o == null) {
                throw new ServiceException(RCode.PARAM_NOTFOUND);
            }
            // 获取实体类型参数
            var type = o.GetType();
            Dictionary<string, string> fields = new Dictionary<string, string>();
            foreach (var f in type.GetProperties()) {
                object var1 = f.GetValue(o);
                if (var1 == null) {
                    continue;
                }
                // 将参数格式成xml一样格式
                fields.Add($"#{{{f.Name}}}", var1.ToString());
            }
            // 判断实体类中是否存在参数
            if (fields.Count == 0) {
                throw new ServiceException($"fields in {type} is empty");
            }
            // 遍历节点，并替换
            Queue<XmlNode> queue = new Queue<XmlNode>(10);
            HashSet<string> copyVar = new HashSet<string>();// 替换节点值
            HashSet<string> xmlUpdateValues = new HashSet<string>();// xml节点代替换值数量
            queue.Enqueue(node);
            // 假设所有文件信息中不存在#开头的值
            while (queue.Count > 0) {
                var tmpNode = queue.Dequeue();
                if (tmpNode.NodeType == XmlNodeType.Comment) {// 跳过注释
                    continue;
                }
                // 遍历节点参数
                var attrs = tmpNode.Attributes;
                if (attrs != null && attrs.Count > 0) {
                    foreach (XmlAttribute attr in attrs) {
                        if (string.IsNullOrEmpty(attr.Value)) {
                            continue;
                        }
                        if (attr.Value[0] == '#') {
                            xmlUpdateValues.Add(attr.Value);
                        }
                        if (!fields.TryGetValue(attr.Value, out var value)) continue;
                        attr.Value = value;
                        copyVar.Add(attr.Value);
                    }
                }
                // 判断当前是否存在子节点
                if (tmpNode.HasChildNodes) {
                    foreach (XmlNode var2 in tmpNode.ChildNodes) {
                        queue.Enqueue(var2);
                    }
                }
                else {
                    // 更新节点内值
                    if (string.IsNullOrEmpty(tmpNode.InnerText)) {
                        continue;
                    }

                    if (tmpNode.InnerText[0] == '#') {
                        xmlUpdateValues.Add(tmpNode.InnerText);
                    }

                    if (!fields.TryGetValue(tmpNode.InnerText, out var value)) continue;
                    tmpNode.InnerText = value;
                    copyVar.Add(tmpNode.InnerText);
                }
            }

            string result = null;
            // 将xml文件中未替换的值转换成字符串
            if (copyVar.Count < xmlUpdateValues.Count) {
                foreach (var s in copyVar) {
                    xmlUpdateValues.Remove(s);
                }

                StringBuilder si = new StringBuilder();
                foreach (var s in xmlUpdateValues) {
                    si.Append('\'').Append(s).Append('\'').Append(',');
                }

                result = si.ToString();
            }
            return result;
        }

        #region 检索节点

        public static XmlNodeList selectNodeByName(XmlDocument doc, string nodeName) {
            
            return doc.SelectNodes($"//*[local-name()='{nodeName}']");
        }

        public static XmlNode selectSingleNodeByName(XmlDocument doc, string nodeName) {
            var list = doc.SelectNodes($"//*[local-name()='{nodeName}']");
            return list.Item(0);
        }

        public static XmlNode selectSingleNodeByPattern(XmlDocument doc, string pattern) {
            return doc.SelectSingleNode(pattern);
        }

        public static XmlNodeList selectNodeByPattern(XmlDocument doc, string pattern) {
            return doc.SelectNodes(pattern);
        }

        public static XmlNodeList retrievesPattern(XmlNode node, string pattern) {
            return node.SelectNodes(pattern);
        }

        #endregion

        #region 复制

        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode) {
            copyNode(targetDoc, targetNode, sourceNode, sourceNode.Name, true, null,null);
        }
        
        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode, string newNode) {
            copyNode(targetDoc, targetNode, sourceNode, newNode?? sourceNode.Name, true, null,null);
        }
        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode, string newNode, modifyContent contentCondition) {
            copyNode(targetDoc, targetNode, sourceNode, newNode?? sourceNode.Name, true, contentCondition,null);
        }
        /// <summary>
        /// 将节点复制到另一个文件中的某一节点上
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetNode"></param>
        /// <param name="sourceNode"></param>
        /// <param name="newName"></param>
        /// <param name="attributeCopy"></param>
        /// <param name="condition">修改节点内容条件</param>
        /// <exception cref="Exception"></exception>
        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode, string newName,
            bool attributeCopy, modifyContent contentCondition, modifyAttribute attributeCondition) {
            if (targetDoc == null || targetNode == null || sourceNode == null) {
                return;
            }

            if (sourceNode.NodeType != XmlNodeType.Element) {
                return;
            }
            // 添加子节点
            XmlElement newNode = targetDoc.CreateElement(newName);
            if (newName == null) {
                throw new Exception();
            }

            // 如果父节点存在参数，判断是否复制参数
            if (sourceNode.Attributes != null && attributeCopy) {
                if (sourceNode.Attributes.Count != 0) {
                    if (attributeCondition == null) {// 非条件复制
                        
                        // 添加子节点属性
                        foreach (XmlAttribute att in sourceNode.Attributes) {
                            newNode.SetAttribute(att.Name, att.Value);
                        }
                    }
                    else { // 条件复制
                        // 添加子节点属性
                        foreach (XmlAttribute att in sourceNode.Attributes) {
                            string value = attributeCondition(att);
                            newNode.SetAttribute(att.Name, string.IsNullOrEmpty(value) == null ? att.Value : value);
                            
                        }
                    }
                }
            }
            // 存在子节点，对子节点复制
            if (sourceNode.HasChildNodes) {
                if (contentCondition != null) {
                    copyChildNode(targetDoc, newNode, sourceNode, contentCondition);
                }
                else {
                    copyChildNode(targetDoc, newNode, sourceNode);
                }
            }
            else {
                string si = null;
                if (contentCondition != null) {
                    si = contentCondition(newNode);
                }

                if (string.IsNullOrEmpty(si)) {
                    if (!string.IsNullOrEmpty(sourceNode.InnerText)) {
                        newNode.InnerText = sourceNode.InnerText;
                    }
                }
                else {
                    newNode.InnerText = si;
                }
            }

            targetNode.AppendChild(newNode);
        }

        /// <summary>
        /// 复制子节点
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetNode"></param>
        /// <param name="sourceNode"></param>
        private static void copyChildNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode) {
            foreach (XmlNode child in sourceNode.ChildNodes) {
                if (child.NodeType == XmlNodeType.Comment) {
                    continue;
                }
                XmlNode newChild = targetDoc.ImportNode(child, true);
                
                targetNode.AppendChild(newChild);
                
            }
        }

        /// <summary>
        /// 复制子节点，会将源节点中的参数同时复制
        /// 只能修改节点内容，暂时不能修改节点属性
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetNode"></param>
        /// <param name="sourceNode"></param>
        /// <param name="contentCondition"></param>
        private static void copyChildNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode,
            modifyContent contentCondition) {
            if (contentCondition == null) {
                return;
            }
            string s = null;
            XmlNode newChild;
            foreach (XmlNode child in sourceNode.ChildNodes) {
                if (child.NodeType == XmlNodeType.Comment) {
                    continue;
                }
                // 节点不含有子节点
                newChild = targetDoc.ImportNode(child, false);
                targetNode.AppendChild(newChild);
                if (hasChileNode(child)) { // 节点含有子节点
                    copyChildNode(targetDoc, newChild, child, contentCondition);
                    continue;
                }
                
                s = contentCondition(child);
                newChild.InnerText = !string.IsNullOrEmpty(s) ? s : child.InnerText;
            }
        }

        #endregion

        #region 修改节点内容，包括节点内容

        public static void updateNode(XmlDocument targetDoc, XmlNode updateNode, modifyContent contentCondition) {
            if (contentCondition == null || targetDoc == null || updateNode == null) {
                return;
            }

            if (!updateNode.HasChildNodes) {// 节点不包括子节点
                updateNode.InnerText = contentCondition(updateNode);
                return;
            }
            // 对所有子节点进行内容修改
            Queue<XmlNode> queue = new Queue<XmlNode>();
            queue.Enqueue(updateNode);
            XmlNode tmp;
            while (queue.Count != 0) {
                tmp = queue.Dequeue();
                if (hasChileNode(tmp)) {
                    foreach (XmlNode child in tmp.ChildNodes) {
                        if (child.NodeType != XmlNodeType.Element) {
                            continue;
                        }
                        queue.Enqueue(child);
                    }
                }
                else {
                    string s = contentCondition(tmp);
                    if (!string.IsNullOrEmpty(s)) {
                        tmp.InnerText = s;
                    }
                }
            }
            
        }

        #endregion

        #region 插入node节点
        /// <summary>
        /// 根据xpath规则，将节点插入到相同规则之后
        /// </summary>
        /// <param name="targetDoc">配置文件</param>
        /// <param name="targetParentNodeXpath">待插入节点的父节点xpath</param>
        /// <param name="par">待插入节点</param>
        /// <param name="newRootNodeName">新节点名称</param>
        public static void insertNode(XmlDocument targetDoc, string targetParentNodeXpath, XmlNode par, string newRootNodeName = null) {
            if (CommonUtil.checkParams(targetDoc, targetParentNodeXpath, par)) {
                throw new ServiceException(RCode.PARAM_NOTFOUND);
            }
            // 获取父节点
            XmlNode parent = targetDoc.SelectSingleNode(targetParentNodeXpath);
            if (parent == null) {
                throw new ServiceException(RCode.CONF_ERROR_FIND_XMLNODE);
            }

            try {
                XmlNode newChild = null;
                if (string.IsNullOrEmpty(newRootNodeName)) {
                    // 找到父节点的最后一个子节点
                    newChild = targetDoc.ImportNode(par, true);
                }
                else { // 创建新节点名称
                    newChild = targetDoc.CreateElement(newRootNodeName);
                    if (par.HasChildNodes) {
                        // 子节点添加
                        foreach (XmlNode child in par.ChildNodes) {
                            newChild.AppendChild(targetDoc.ImportNode(child, true));
                        }
                    }
                    else {
                        newChild.InnerText = par.InnerText;
                    }
                    
                    // 添加根节点属性
                    if (par.Attributes != null) {
                        foreach (XmlNode attr in par.Attributes) {
                            ((XmlElement)newChild).SetAttribute(attr.Name, attr.Value);
                        }
                    }
                }
                parent.AppendChild(newChild);
            }
            catch (Exception e) {
                throw new ServiceException(RCode.CONF_ERROR_ADD_XMLNODE, e);
            }
        }
        

        #endregion
        /// <summary>
        /// 判断节点是否含有子标签节点，不包括type=test的类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool hasChileNode(XmlNode node) {
            if (node.NodeType != XmlNodeType.Element) {
                return false;
            }

            bool result = node.HasChildNodes;
            if (result) {
                if (node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text) {
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// 删除节点, 单个，多层
        /// 必须要有父节点
        /// </summary>
        /// <param name="node"></param>
        public static void delNode(XmlNode node) {
            var parent = node.ParentNode;
            if (parent != null) {
                parent.RemoveChild(node);
            }
            else {// 如果删除根节点
                node.RemoveAll();
            }
        }
        
    }
}