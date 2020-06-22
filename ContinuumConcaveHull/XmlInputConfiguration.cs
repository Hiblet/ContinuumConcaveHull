using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace ContinuumConcaveHull
{
    public class XmlInputConfiguration
    {
        public string GroupField { get; private set; }
        public string GroupFieldNames { get; private set; }

        public string XField { get; private set; }
        public string XFieldNames { get; private set; }

        public string YField { get; private set; }
        public string YFieldNames { get; private set; }

        public string Concavity { get; private set; }
        public string ScaleFactor { get; private set; }


        // Note that the constructor is private.  Instances are created through the LoadFromConfigration method.
        private XmlInputConfiguration(
            string groupField, 
            string groupFieldNames, 
            string xField,
            string xFieldNames,
            string yField,
            string yFieldNames,
            string concavity,
            string scaleFactor)
        {
            GroupField = groupField;
            GroupFieldNames = groupFieldNames;

            XField = xField;
            XFieldNames = xFieldNames;

            YField = yField;
            YFieldNames = yFieldNames;

            Concavity = concavity;
            ScaleFactor = scaleFactor;
        }

        public static XmlInputConfiguration LoadFromConfiguration(XmlElement eConfig)
        {
            string groupField = getStringFromConfig(eConfig, Constants.GROUPFIELDKEY, Constants.DEFAULTGROUPFIELD);
            string groupFieldNames = getStringFromConfig(eConfig, Constants.GROUPFIELDNAMESKEY, Constants.DEFAULTGROUPFIELDNAMES);

            string xField = getStringFromConfig(eConfig, Constants.XFIELDKEY, Constants.DEFAULTXFIELD);
            string xFieldNames = getStringFromConfig(eConfig, Constants.XFIELDNAMESKEY, Constants.DEFAULTXFIELDNAMES);

            string yField = getStringFromConfig(eConfig, Constants.YFIELDKEY, Constants.DEFAULTYFIELD);
            string yFieldNames = getStringFromConfig(eConfig, Constants.YFIELDNAMESKEY, Constants.DEFAULTYFIELDNAMES);

            string concavity = getStringFromConfig(eConfig, Constants.CONCAVITYKEY, Constants.DEFAULTCONCAVITY);
            string scaleFactor = getStringFromConfig(eConfig, Constants.SCALEFACTORKEY, Constants.DEFAULTSCALEFACTOR);

            return new XmlInputConfiguration(
                groupField, 
                groupFieldNames,
                xField,
                xFieldNames,
                yField,
                yFieldNames,
                concavity,
                scaleFactor);
        }

        public static string getStringFromConfig(XmlElement eConfig, string key, string valueDefault)
        {
            string sReturn = valueDefault;

            XmlElement xe = eConfig.SelectSingleNode(key) as XmlElement;
            if (xe != null)
            {
                if (!string.IsNullOrEmpty(xe.InnerText))
                    sReturn = xe.InnerText;
            }

            return sReturn;
        }

        // Property Name Accessor
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

    }
}
