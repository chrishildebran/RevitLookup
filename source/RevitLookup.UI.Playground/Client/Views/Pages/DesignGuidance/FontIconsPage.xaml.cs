﻿using System.Windows;
using System.Windows.Input;
using RevitLookup.UI.Playground.Client.ViewModels.Pages.DesignGuidance;

namespace RevitLookup.UI.Playground.Client.Views.Pages.DesignGuidance;

public sealed partial class FontIconsPage
{
    static FontIconsPage()
    {
        CommandManager.RegisterClassCommandBinding(typeof(FontIconsPage), new CommandBinding(ApplicationCommands.Copy, OnCopyContentClicked));
    }

    public FontIconsPage(FontIconsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private static void OnCopyContentClicked(object sender, RoutedEventArgs args)
    {
        var routedArgs = (ExecutedRoutedEventArgs)args;
        var parameter = routedArgs.Parameter.ToString();

        if (!string.IsNullOrEmpty(parameter))
        {
            try
            {
                Clipboard.SetText(parameter);
            }
            catch
            {
                // ignored
            }
        }
    }
}