﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Markdown.Avalonia.StyleCollections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Markdown.Avalonia
{
    public static class MarkdownStyle
    {
        static IStyle LoadXaml(string name)
        {
            return new StyleInclude(new Uri("avares://Markdown.Avalonia/"))
            {
                Source = new Uri("avares://Markdown.Avalonia/" + name)
            };
        }

        public static Styles Standard
        {
            get => new MarkdownStyleStandard();
        }

        public static Styles DefaultTheme
        {
            get => new MarkdownStyleDefaultTheme();
        }

        public static Styles FluentTheme
        {
            get => new MarkdownStyleFluentTheme();
        }

        public static Styles GithubLike
        {
            get => new MarkdownStyleGithubLike();
        }
    }
}
