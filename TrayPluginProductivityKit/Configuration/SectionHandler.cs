﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TrayPluginProductivityKit.Configuration
{
  public class SectionHandler : System.Configuration.ConfigurationSection
  {
    public XmlDocument XmlRepresentation { get; set; }

    protected override void DeserializeSection(XmlReader reader)
    {
      var document = new XmlDocument();
      document.Load(reader);
      XmlRepresentation = document;
    }
  }
}
