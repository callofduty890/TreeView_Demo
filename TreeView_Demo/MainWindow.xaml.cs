using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeView_Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<TreeViewNode> TreeViewNodeList = null;//数据源
        private TreeViewNode TopNode = null;//顶层节点
        private int TopNodeId = -1;//顶层节点编号
        private int TopNodeParentId = -999;//顶层节点父节点编号
        private int MaxNodeIndex = 999;//最大节点编号

        private Point lastMouseDownPoint;//上一次鼠标左键点击位置
        private TreeViewNode targetNode;//目标节点

        public MainWindow()
        {
            InitializeComponent();
            InitTreeView();
        }

        private void InitTreeView()
        {
            //创建TreeView数据源对象LIst列表
            TreeViewNodeList = new ObservableCollection<TreeViewNode>();
            this.treeView.ItemsSource = TreeViewNodeList;//进行绑定

            //初始化顶层节点数据
            TopNode = new TreeViewNode();
            TopNode.Id = TopNodeId;//设置顶点ID
            TopNode.ParentId = TopNodeParentId;///顶层节点父节点编号

            //加入添加节点项
            TreeViewNode emptyNode = new TreeViewNode(MaxNodeIndex, TopNodeId, false, false, true, "双击添加新节点");
            emptyNode.ChildNodes = new ObservableCollection<TreeViewNode>();//子节点对象集合
            //TreeView 添加节点信息
            TreeViewNodeList.Add(emptyNode);
            //从顶点添加子节点
            TopNode.ChildNodes.Add(emptyNode);
        }


        /// <summary>
        /// 设置折叠状态
        /// </summary>
        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //获取选中的索引对象
            TreeViewItem SelectedfItem = new TreeViewItem();
            //获取点击的类型
            if (sender.GetType() == typeof(ToggleButton))
            {
                //构建一个byte
                ToggleButton btn = (ToggleButton)sender;
                System.Windows.Controls.ContentPresenter CP = (System.Windows.Controls.ContentPresenter)btn.Tag;
                SelectedfItem = (TreeViewItem)CP.TemplatedParent;
            }
            else if (sender.GetType() == typeof(Border))
            {
                //进行里式转换
                Border btn = (Border)sender;
                System.Windows.Controls.ContentPresenter CP = (System.Windows.Controls.ContentPresenter)btn.Tag;
                SelectedfItem = (TreeViewItem)CP.TemplatedParent;
            }
            else if (e.Source.GetType() == typeof(TreeViewItem))
            {
                SelectedfItem = (TreeViewItem)sender;
            }

            if (SelectedfItem == null || ((TreeViewNode)SelectedfItem.DataContext).IsChildNode || ((TreeViewNode)SelectedfItem.DataContext).IsNodeAdd)
            {
                SelectedfItem.IsExpanded = SelectedfItem.IsExpanded == true ? false : true;
            }
        }

        /// <summary>
        /// 双击添加节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //获取选中的节点Item
            TreeViewNode selectNode = (TreeViewNode)this.treeView.SelectedItem;

            if (selectNode == null)
                return;

            if (selectNode.IsNodeAdd)//双击添加节点
            {
                AddNode();
            }
            if (selectNode.IsChildNodeAdd)//双击添加子节点
            {
                AddChildNode();
            }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        private void AddNode()
        {
            TreeViewNode selectedItem = (TreeViewNode)this.treeView.SelectedItem;
            //获取罪行的节点编号
            int index = GetNodeIndex(TopNode.ChildNodes, selectedItem);

            //添加新节点
            int nodeIndex = 1;
            string newNodeName = "节点" + nodeIndex;
            //循环添加名称，检查不为fals的时候退出
            while (!CheckNodeNameAdd(TopNode, newNodeName))
            {
                nodeIndex++;
                newNodeName = "节点" + nodeIndex;
            }
            //添加一个新的节点
            TreeViewNode newNode = new TreeViewNode(TreeViewNodeList.Count - 1, -1, false, false, false, newNodeName);
            //添加子节点
            TreeViewNode childNode = new TreeViewNode(0, newNode.Id, true, false, false, "子节点1");
            newNode.ChildNodes.Add(childNode);
            //添加子节点Bottom项
            TreeViewNode actionNodeEmpty = new TreeViewNode(MaxNodeIndex, newNode.Id, true, true, false, "双击添加子节点");
            //新节点下添加子节点
            newNode.ChildNodes.Add(actionNodeEmpty);
            //插入一个新的子节点
            TopNode.ChildNodes.Insert(index, newNode);
            //Treeview的List列表添加子节点
            TreeViewNodeList.Insert(index, newNode);
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        private void AddChildNode()
        {
            //获取选择的子选中的独享
            TreeViewNode selectedItem = (TreeViewNode)this.treeView.SelectedItem;
            TreeViewNode selectParentNode = TopNode.ChildNodes[selectedItem.ParentId];
            //获取对应的索引 
            int index = GetNodeIndex(selectParentNode.ChildNodes, selectedItem);

            //添加子节点
            int nodeIndex = 1;
            string newNodeName = "子节点" + nodeIndex;
            //叠加到找不到同名的为止
            while (!CheckNodeNameAdd(selectParentNode, newNodeName))
            {
                nodeIndex++;
                newNodeName = "子节点" + nodeIndex;
            }
            //添加子节点
            TreeViewNode childNode = new TreeViewNode(selectParentNode.ChildNodes.Count - 1, selectedItem.ParentId, true, false, false, newNodeName);
            selectParentNode.ChildNodes.Insert(index, childNode);
        }

        /// <summary>
        /// 获取相应节点的编号
        /// </summary>
        private int GetNodeIndex(ObservableCollection<TreeViewNode> nodeList, TreeViewNode targetNode)
        {
            //判断如果输入的为空直接返回
            if (nodeList == null || nodeList.Count <= 0)
                return 0;
            for (int i = 0; i < nodeList.Count; i++)//遍历每一额节点
            {
                if (nodeList[i].Equals(targetNode))//判断是否想定然后返回,返回节点对应的编号
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// 节点重名检查
        /// </summary>
        private bool CheckNodeNameAdd(TreeViewNode node, string name)
        {
            //遍历所有的子控件名称
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                //判断如果找到相等的就返回false
                if (node.ChildNodes[i].NodeName == name)
                {
                    return false;
                }
            }
            //找不到返回真
            return true;
        }



        /// <summary>
        /// 鼠标左键按下事件响应
        /// </summary>
        private void TreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                lastMouseDownPoint = e.GetPosition(this.treeView);
            }
        }

        /// <summary>
        /// 拖拽进入，改变字体颜色
        /// </summary>
        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            TreeViewItem container = GetNearestContainer(e.OriginalSource as UIElement);
            if (container != null)
            {
                container.Foreground = new SolidColorBrush(Colors.Orange);
            }
        }

        /// <summary>
        /// 拖拽离开，恢复成原来的字体颜色
        /// </summary>
        private void TreeView_DragLeave(object sender, DragEventArgs e)
        {
            TreeViewItem container = GetNearestContainer(e.OriginalSource as UIElement);
            if (container != null)
            {
                if (((TreeViewNode)container.DataContext).IsChildNode || ((TreeViewNode)container.DataContext).IsNodeAdd)
                    container.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
                else
                    container.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        /// <summary>
        /// 拖拽释放
        /// </summary>
        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                TreeViewItem container = GetNearestContainer(e.OriginalSource as UIElement);
                if (container != null)
                {
                    //恢复成原来的字体颜色
                    if (((TreeViewNode)container.DataContext).IsChildNode || ((TreeViewNode)container.DataContext).IsNodeAdd)
                        container.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
                    else
                        container.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    TreeViewNode _sourceNode = (TreeViewNode)e.Data.GetData(typeof(TreeViewNode));//起始节点
                    TreeViewNode _targetNode = (TreeViewNode)container.Header;//目标节点

                    if (_sourceNode.Equals(_targetNode))//起始节点与目标节点相同则返回
                        return;

                    if ((_sourceNode != null) && (_targetNode != null))
                    {
                        if (!_targetNode.IsNodeAdd && !_targetNode.IsChildNodeAdd)
                        {
                            targetNode = _targetNode;
                            e.Effects = DragDropEffects.Move;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 拖拽移动
        /// </summary>
        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this.treeView);
                //超出2个位移点时可移动
                if ((Math.Abs(currentPosition.X - lastMouseDownPoint.X) > 2.0) || (Math.Abs(currentPosition.Y - lastMouseDownPoint.Y) > 2.0))
                {
                    TreeViewNode selectedItem = (TreeViewNode)this.treeView.SelectedItem;
                    if ((selectedItem != null) && selectedItem.IsNodeAdd == false && selectedItem.IsChildNodeAdd == false)
                    {
                        TreeViewItem container = GetContainerFromNode(selectedItem);
                        if (container != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(container, selectedItem, DragDropEffects.Move);
                            if ((finalDropEffect == DragDropEffects.Move) && (targetNode != null))
                            {
                                TreeViewNode _targetParentNode = targetNode.ParentId == TopNode.Id ? TopNode : TopNode.ChildNodes[targetNode.ParentId];//目标父节点
                                TreeViewNode _selectParentNode = selectedItem.ParentId == TopNode.Id ? TopNode : TopNode.ChildNodes[selectedItem.ParentId];//当前父节点
                                if (_targetParentNode != null && _selectParentNode != null && _targetParentNode.Equals(_selectParentNode))
                                {
                                    int index = GetNodeIndex(_targetParentNode.ChildNodes, targetNode);
                                    int indexold = GetNodeIndex(_selectParentNode.ChildNodes, selectedItem);
                                    _selectParentNode.ChildNodes.Remove(selectedItem);
                                    _targetParentNode.ChildNodes.Insert(index, selectedItem);
                                    if (targetNode.ParentId == TopNode.Id)//如果为父节点则需要同步更新数据源
                                    {
                                        TreeViewNodeList.Remove(selectedItem);
                                        TreeViewNodeList.Insert(index, selectedItem);
                                    }

                                    ChangeID(_targetParentNode);
                                    MessageBox.Show(string.Format("位置已改变，目标ID：{0}，目标父ID：{1}", targetNode.Id, targetNode.ParentId));
                                }
                                else
                                {
                                    MessageBox.Show("不能改变节点层级");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取最近的控件
        /// </summary>
        private TreeViewItem GetNearestContainer(UIElement element)
        {
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        /// <summary>
        /// 根据Node获取最近的控件
        /// </summary>
        private TreeViewItem GetContainerFromNode(TreeViewNode node)
        {
            try
            {
                Stack<TreeViewNode> _stack = new Stack<TreeViewNode>();
                _stack.Push(node);

                if (node.ParentId >= 0)
                {
                    TreeViewNode parent = TopNode.ChildNodes[node.ParentId];
                    _stack.Push(parent);
                }

                ItemsControl container = this.treeView;
                while ((_stack.Count > 0) && (container != null))
                {
                    TreeViewNode top = _stack.Pop();
                    container = (ItemsControl)container.ItemContainerGenerator.ContainerFromItem(top);
                }

                return container as TreeViewItem;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 改变节点ID
        /// </summary>
        private void ChangeID(TreeViewNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Id == MaxNodeIndex) continue;
                node.ChildNodes[i].Id = i;
                for (int j = 0; j < node.ChildNodes[i].ChildNodes.Count; j++)
                {
                    node.ChildNodes[i].ChildNodes[j].ParentId = i;
                }
            }
        }



    }
}
