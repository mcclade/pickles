﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NGenerics.DataStructures.Trees;
using NGenerics.Patterns.Visitor;
using System.Xml;
using Pickles.Formatters;
using System.Xml.Linq;

namespace Pickles
{
    public class HtmlDocumentationBuilder : IDocumentationBuilder
    {
        private readonly FeatureCrawler featureCrawler;
        private readonly HtmlDocumentFormatter htmlDocumentFormatter;

        public HtmlDocumentationBuilder(FeatureCrawler featureCrawler, HtmlDocumentFormatter htmlDocumentFormatter)
        {
            this.featureCrawler = featureCrawler;
            this.htmlDocumentFormatter = htmlDocumentFormatter;
        }

        public void Build(DirectoryInfo inputPath, DirectoryInfo outputPath)
        {
            var features = this.featureCrawler.Crawl(inputPath);
            var actionVisitor = new ActionVisitor<FeatureNode>(node =>
                {
                    var nodePath = Path.Combine(outputPath.FullName, node.RelativePathFromRoot);

                    if (!node.IsDirectory)
                    {
                        var htmlFilePath = nodePath.Replace(".feature", ".xhtml");
                        using (var writer = new StreamWriter(htmlFilePath))
                        {
                            var document = this.htmlDocumentFormatter.Format(node, features);
                            document.Save(writer);
                            writer.Close();
                        }
                    }
                    else 
                    {
                        Directory.CreateDirectory(nodePath);
                    }
                });
            features.AcceptVisitor(actionVisitor);
        }
    }
}
