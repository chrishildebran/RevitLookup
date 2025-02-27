﻿// Copyright 2003-2024 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Windows.Input;
using RevitLookup.UI.Framework.Views.Windows;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase : INavigationAware
{
    /// <summary>
    ///     Callback when navigating to the current page
    /// </summary>
    public Task OnNavigatedToAsync()
    {
        var host = _intercomService.GetHost();
        host.PreviewKeyDown += OnPageKeyPressed;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Callback when navigating from this page
    /// </summary>
    public Task OnNavigatedFromAsync()
    {
        var host = _intercomService.GetHost();
        host.PreviewKeyDown -= OnPageKeyPressed;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Page shortcuts
    /// </summary>
    private void OnPageKeyPressed(object sender, KeyEventArgs args)
    {
        HandleRefreshShortcut(args);
        if (args.Handled) return;

        HandleFocusSearchShortcut(sender, args);
    }

    private void HandleRefreshShortcut(KeyEventArgs args)
    {
        if (args.Key != Key.F5) return;

        ViewModel.RefreshMembersAsync();
        args.Handled = true;
    }

    private void HandleFocusSearchShortcut(object sender, KeyEventArgs args)
    {
        if (SearchBoxControl.IsKeyboardFocused) return;
        if (args.KeyboardDevice.Modifiers != ModifierKeys.None) return;

        var rootWindow = (RevitLookupView) sender;
        if (rootWindow.RootContentDialog.Content is not null) return;

        if (args.Key is >= Key.D0 and <= Key.Z or >= Key.NumPad0 and <= Key.NumPad9)
        {
            SearchBoxControl.Focus();
        }
    }
}