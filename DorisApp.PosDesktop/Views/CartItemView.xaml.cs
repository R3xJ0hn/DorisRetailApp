using DorisApp.PosDesktop.ViewModels;
using System;
using System.Windows.Controls;

namespace DorisApp.PosDesktop.Views
{
    /// <summary>
    /// Interaction logic for CartItemView.xaml
    /// </summary>
    public partial class CartItemView : UserControl
    {
        public CartItemView()
        {
            InitializeComponent();
        }
        private void ItemRemoved_Completed(object sender, EventArgs e)
        {
            var viewModel = (CartItemViewModel)DataContext;
            viewModel.CompletyRemoved();
        }
    }
}
