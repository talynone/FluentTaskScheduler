using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FluentTaskScheduler.ViewModels;

namespace FluentTaskScheduler
{
    public sealed partial class ScriptLibraryPage : Page
    {
        public ScriptLibraryViewModel ViewModel { get; } = new();

        public ScriptLibraryPage()
        {
            this.InitializeComponent();
        }

        private MainPage? _ownerMainPage;

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is MainPage mp) _ownerMainPage = mp;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageScrollViewer.IsScrollInertiaEnabled = FluentTaskScheduler.Services.SettingsService.SmoothScrolling;
            await ViewModel.LoadScriptsAsync();
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
             if (sender is Button btn && btn.Tag is ScriptTemplateModel template)
             {
                 (_ownerMainPage ?? MainPage.Current)?.OpenCreateTaskFromTemplate(template);
             }
        }
    }
}
