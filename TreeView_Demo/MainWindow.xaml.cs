using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
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

    }
}
