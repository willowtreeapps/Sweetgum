// <copyright file="FontFamilyFactory.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using Avalonia.Media;

namespace WillowTree.Sweetgum.Client.Fonts
{
    public static class FontFamilyFactory
    {
        public static FontFamily CreateCustom(string ttfName, string familyName)
        {
            return new FontFamily(
                new Uri("avares://Client/App.axaml"),
                $"avares://Client/Assets/Fonts/{ttfName}.ttf#{familyName}");
        }
    }
}
