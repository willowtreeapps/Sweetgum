// <copyright file="FontFamilyFactory.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using Avalonia.Media;

namespace WillowTree.Sweetgum.Client.Fonts
{
    /// <summary>
    /// Methods which allow simplified convention-based construction of <see cref="FontFamily"/> instances.
    /// </summary>
    public static class FontFamilyFactory
    {
        /// <summary>
        /// Constructs an instance of <see cref="FontFamily"/> for a custom TTF font in the font directory inside the
        /// assets directory.
        /// </summary>
        /// <param name="ttfName">The file name of the TTF file inside the font directory inside the assets directory.</param>
        /// <param name="familyName">The font family name. This must be correct and match the TTF file details.
        /// You can use a program like "Font Book" on Mac to determine the family name.</param>
        /// <returns>An instance of <see cref="FontFamily"/>.</returns>
        public static FontFamily CreateCustom(string ttfName, string familyName)
        {
            return new FontFamily(
                new Uri("avares://Client/App.axaml"),
                $"avares://Client/Assets/Fonts/{ttfName}.ttf#{familyName}");
        }
    }
}
