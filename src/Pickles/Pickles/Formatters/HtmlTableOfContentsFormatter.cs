﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using NGenerics.DataStructures.Trees;

namespace Pickles.Formatters
{
    public class HtmlTableOfContentsFormatter
    {
        private XElement BuildListItems(XNamespace xmlns, Uri file, GeneralTree<FeatureNode> features)
        {
            var ul = new XElement(xmlns + "ul");

            foreach (var childNode in features.ChildNodes)
            {
                if (childNode.IsLeafNode)
                {
                    ul.Add(new XElement(xmlns + "li",
                                new XElement(xmlns + "a",
                                    new XAttribute("href", childNode.Data.Location.FullName != file.LocalPath ? file.MakeRelativeUri(childNode.Data.Url).ToString() : "#"))));
                }
                else
                {
                    ul.Add(BuildListItems(xmlns, file, childNode));
                }
            }

            return ul;
        }

        public XElement Format(FileInfo file, GeneralTree<FeatureNode> features)
        {
            return Format(new Uri(file.FullName), features);
        }

        public XElement Format(Uri file, GeneralTree<FeatureNode> features)
        {
            var xmlns = XNamespace.Get("http://www.w3.org/1999/xhtml");

            return new XElement(xmlns + "div",
                       new XAttribute("id", "toc"),
                       new XAttribute("class", "toc"),
                       BuildListItems(xmlns, file, features)
                   );
        }
    }
}
