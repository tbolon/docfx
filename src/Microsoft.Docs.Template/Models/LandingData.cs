// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Docs.Build
{
    [PageSchema]
    public class LandingData
    {
        public string Title { get; set; }

        public string TitleSuffix { get; set; }

        public JObject Metadata { get; set; }

        public LandingDataAbstract Abstract { get; set; }

        public LandingDataSection[] Sections { get; set; }

        public string DocumentType { get; set; }

        [JsonExtensionData(WriteData = false)]
        public JObject ExtensionData { get; set; }
    }

    public sealed class LandingDataAbstract
    {
        public string Description { get; set; }

        public LandingDataAside Aside { get; set; }

        public LandingDataMenu Menu { get; set; }
    }

    public sealed class LandingDataAside
    {
        public string Title { get; set; }

        public string Width { get; set; }

        public string Href { get; set; }

        public LandingDataImage Image { get; set; }
    }

    public sealed class LandingDataImage
    {
        public string Href { get; set; }

        public string Src { get; set; }

        public string Alt { get; set; }

        public string Height { get; set; }

        public string Width { get; set; }

        public string Role { get; set; }
    }

    public sealed class LandingDataMenu
    {
        public string Title { get; set; }

        public LandingDataMenuItem[] Items { get; set; }
    }

    public sealed class LandingDataMenuItem
    {
        public string Href { get; set; }

        public string Text { get; set; }
    }

    public enum LandingDataType
    {
        Paragraph,
        List,
        Table,
        Markdown,
        Text,
    }

    public sealed class LandingDataSection
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public string Text { get; set; }

        public LandingDataItem[] Items { get; set; }
    }

    public sealed class LandingDataItem
    {
        public LandingDataType Type { get; set; }

        public string Text { get; set; }

        public string Style { get; set; }

        public string ClassName { get; set; }

        public LandingDataListItem[] Items { get; set; }

        public JToken Columns { get; set; }

        public LandingDataRow[] Rows { get; set; }

        public string Html { get; set; }
    }

    public sealed class LandingDataColumn
    {
        public LandingDataImage Image { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
    }

    public sealed class LandingDataRow
    {
        public string Title { get; set; }

        public LandingDataRowValue[] Values { get; set; }
    }

    public sealed class LandingDataRowValue
    {
        public string Href { get; set; }
    }

    public sealed class LandingDataListItem
    {
        public string Text { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Html { get; set; }

        public string Href { get; set; }

        public LandingDataImage Image { get; set; }
    }

}