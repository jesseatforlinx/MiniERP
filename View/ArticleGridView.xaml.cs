using Microsoft.Extensions.DependencyInjection;
using MiniERP.UI.ViewModel;
using System.Windows.Controls;

namespace MiniERP.UI.View
{
    /// <summary>
    /// ArticleGridView.xaml 的交互逻辑
    /// </summary>
    public partial class ArticleGridView : UserControl
    {
        public ArticleGridView()
        {
            InitializeComponent();
            
            // 从依赖注入容器获取ViewModel
            var viewModel = App.ServiceProvider.GetRequiredService<ArticleViewModel>();
            DataContext = viewModel;
        }
    }
}
