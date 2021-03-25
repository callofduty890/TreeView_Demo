using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TreeView_Demo
{
    public class TreeViewNode : INotifyPropertyChanged
    {

        //响应的事件
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 属性字段
        private int id;
        /// <summary>
        /// 节点ID
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        private int parentId;
        /// <summary>
        /// 父节点ID
        /// </summary>
        public int ParentId
        {
            get
            {
                return parentId;
            }
            set
            {
                parentId = value;
            }
        }

        private string nodeName;
        /// <summary>
        /// 节点名称（最多六个字符）
        /// </summary>
        public string NodeName
        {
            get
            {
                return nodeName;
            }
            set
            {
                nodeName = value;
                if (nodeName.Length > 6)
                {
                    //非添加项考虑字符长度
                    if (this.isNodeAdd != true && this.isChildNodeAdd != true)
                    {
                        nodeName = nodeName.Substring(0, 6);
                    }
                }
                OnPropertyChanged("NodeName");
            }
        }

        private bool isChildNode;
        /// <summary>
        /// 是否是子节点
        /// </summary>
        public bool IsChildNode
        {
            get
            {
                return isChildNode;
            }
            set
            {
                isChildNode = value;
            }
        }

        private bool isNodeAdd;
        /// <summary>
        /// 是否添加节点
        /// </summary>
        public bool IsNodeAdd
        {
            get
            {
                return isNodeAdd;
            }
            set
            {
                isNodeAdd = value;
            }
        }

        private bool isChildNodeAdd;
        /// <summary>
        /// 是否是添加子节点
        /// </summary>
        public bool IsChildNodeAdd
        {
            get
            {
                return isChildNodeAdd;
            }
            set
            {
                isChildNodeAdd = value;
            }
        }
        #endregion

        private ObservableCollection<TreeViewNode> childNodes;
        /// <summary>
        /// 子节点数据
        /// </summary>
        public ObservableCollection<TreeViewNode> ChildNodes
        {
            get
            {
                if (childNodes == null)
                {
                    childNodes = new ObservableCollection<TreeViewNode>();
                    childNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnMoreStuffChanged);
                }
                return childNodes;
            }
            set
            {
                childNodes = value;
            }
        }
        private void OnMoreStuffChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                TreeViewNode stuff = (TreeViewNode)e.NewItems[0];
                stuff.ParentId = this.Id;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                TreeViewNode stuff = (TreeViewNode)e.OldItems[0];
                if (stuff.ParentId == this.Id)
                {
                    stuff.ParentId = 0;
                }
            }
        }

        #region 界面展示相关属性
        //根据节点类型设置Margin
        public string Margining
        {
            get
            {
                double padLeft;
                if (this.isChildNode == true || this.isNodeAdd == true)
                {
                    padLeft = 36;
                }
                else
                {
                    padLeft = 10;
                }
                return string.Format("{0},0,0,0", padLeft);
            }
        }

        //添加节点按钮是否展示
        public Visibility ShowAddButton
        {
            get
            {
                if (this.isChildNode == false && this.isNodeAdd == true && this.isChildNodeAdd == false)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        //根据节点设置分隔线
        public string ShowBorderThickness
        {
            get
            {
                if (this.isChildNode == false && this.isChildNodeAdd == false)
                    return string.Format("0,1,0,0");
                else
                    return string.Format("0,0,0,0");
            }
        }

        //根据子父节点设置字体大小
        public int SetFontSize
        {
            get
            {
                if (this.isChildNode == true)
                    return 12;
                else
                    return 14;
            }
        }

        //根据子父节点设置字体宽度
        public string SetFontWeight
        {
            get
            {
                if (this.isChildNode == true)
                    return "Normal";
                else
                    return "Bold";
            }
        }

        //根据子父节点设置字体颜色
        public string SetForeground
        {
            get
            {
                if (this.isChildNode == true || this.isNodeAdd == true)
                    return "#999999";
                else
                    return "#000000";
            }
        }

        //根据子父节点设置背景颜色
        public string SetBackground
        {
            get
            {
                if (this.isChildNode == true || this.isNodeAdd == true)
                    return "#ffffff";
                else
                    return "#87CEEB";
            }
        }

        //节点是否展开
        public bool SetIsExpanded
        {
            get
            {
                if (this.isChildNode != true || this.isNodeAdd != true)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        #region 构造函数
        public TreeViewNode()
        {
        }
        public TreeViewNode(int _id, int _parentId, bool _isChildNode, bool _isChildNodeAdd, bool _isNodeAdd, string _nodeName)
        {
            this.id = _id;
            this.parentId = _parentId;
            this.isChildNode = _isChildNode;
            this.isChildNodeAdd = _isChildNodeAdd;
            this.isNodeAdd = _isNodeAdd;
            this.nodeName = _nodeName;
        }
        #endregion
    }
}
