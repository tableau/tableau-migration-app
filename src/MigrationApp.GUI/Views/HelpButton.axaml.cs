using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;



namespace MigrationApp.GUI.Views
{
    public partial class HelpButton : UserControl
    {

        public static readonly StyledProperty<string> HelpTextProperty =
                    AvaloniaProperty.Register<HelpButton, string>(nameof(HelpText));

        public static readonly StyledProperty<string> DetailsUrlProperty =
                    AvaloniaProperty.Register<HelpButton, string>(nameof(DetailsUrl));

        public string DetailsUrl
        {
            get => GetValue(DetailsUrlProperty);
            set => SetValue(DetailsUrlProperty, value);
        }

        public string HelpText
        {
            get => GetValue(HelpTextProperty);
            set => SetValue(HelpTextProperty, value);
        }

        public HelpButton()
        {
            DataContext = this;
            InitializeComponent();

            Initialized += (sender, e) =>
            {
                var linkTextBlock = this.FindControl<TextBlock>("LinkTextBlock");
                if (linkTextBlock != null)
                {
                    linkTextBlock.PointerPressed += OnLinkClicked;
                }
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnLinkClicked(object? sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DetailsUrl))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = DetailsUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to open URL: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("DetailsUrl is not set or empty.");
            }
        }
    }
}