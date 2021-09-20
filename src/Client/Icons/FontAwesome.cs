// <copyright file="FontAwesome.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Avalonia.Media;
using WillowTree.Sweetgum.Client.Fonts;

namespace WillowTree.Sweetgum.Client.Icons
{
    /// <summary>
    /// The font awesome font families and glyph characters.
    /// </summary>
    public static class FontAwesome
    {
        /// <summary>
        /// The unicode character for the chevron-right glyph.
        /// For more information, see: https://fontawesome.com/v5.15/icons/chevron-right?style=solid.
        /// </summary>
        public const string ChevronRight = "\uf054";

        /// <summary>
        /// The regular font family.
        /// Please note that you still need to specify the "Normal" weight for the text element.
        /// </summary>
        public static readonly FontFamily FontFamilyRegular = FontFamilyFactory.CreateCustom(
            "FontAwesomeFreeRegular",
            "Font Awesome 5 Free");

        /// <summary>
        /// The solid font family.
        /// Please note that you still need to specify the "Black" weight for the text element.
        /// </summary>
        public static readonly FontFamily FontFamilySolid = FontFamilyFactory.CreateCustom(
            "FontAwesomeFreeSolid",
            "Font Awesome 5 Free");
    }
}
