using Microsoft.UI.Xaml.Controls;
using FluentTaskScheduler.ViewModels;

namespace FluentTaskScheduler
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage()
        {
            this.InitializeComponent();
            ViewModel = new DashboardViewModel();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            PageScrollViewer.IsScrollInertiaEnabled = FluentTaskScheduler.Services.SettingsService.SmoothScrolling;
            if (ViewModel.TotalTasks == 0 && !ViewModel.IsLoading)
            {
                await ViewModel.LoadDashboardData();
            }
        }
        private void ActivityList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is FluentTaskScheduler.Models.TaskHistoryEntry entry && !string.IsNullOrEmpty(entry.TaskPath))
            {
                ViewModel.NavigateToTask(entry.TaskPath);
            }
        }
    }
}
