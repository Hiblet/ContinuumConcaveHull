using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlteryxGuiToolkit.Plugins;
using System.Xml;

namespace ContinuumConcaveHull
{
    public partial class ConcaveHullUserControl : UserControl, IPluginConfiguration
    {
        public ConcaveHullUserControl()
        {
            InitializeComponent();
        }

        public Control GetConfigurationControl(
            AlteryxGuiToolkit.Document.Properties docProperties,
            XmlElement eConfig,
            XmlElement[] eIncomingMetaInfo,
            int nToolId,
            string strToolName)
        {
            // Call LoadFromConfiguration to get the xml file name and field information from eConfig.
            XmlInputConfiguration xmlConfig = XmlInputConfiguration.LoadFromConfiguration(eConfig);

            if (xmlConfig == null)
                return this;

            // Populate the Group ComboBox with field names
            // If there is no incoming connection, use what is stored

            string groupField = xmlConfig.GroupField;

            if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)
            {
                string groupFieldNames = xmlConfig.GroupFieldNames;
                string[] arrGroupFieldNames = groupFieldNames.Split(',');

                comboboxGroupField.Items.Clear();
                foreach (string groupFieldName in arrGroupFieldNames)
                    comboboxGroupField.Items.Add(groupFieldName);

                // Select the saved field                
                if (!string.IsNullOrWhiteSpace(groupField))
                {
                    int selectedIndex = comboboxGroupField.FindStringExact(groupField);
                    if (selectedIndex > 0)
                        comboboxGroupField.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                comboboxGroupField.Items.Clear();

                var xmlElementMetaInfo = eIncomingMetaInfo[0];
                var xmlElementRecordInfo = xmlElementMetaInfo.FirstChild;

                foreach (XmlElement elementChild in xmlElementRecordInfo)
                {
                    string fieldName = elementChild.GetAttribute("name");
                    //string fieldType = elementChild.GetAttribute("type");

                    //if (isStringType(fieldType))
                    //{
                        comboboxGroupField.Items.Add(fieldName);
                    //}
                }

                // If the selectedField matches a possible field in the combo box,
                // make it the selected field.
                // If the selectedField does not match, do not select anything and 
                // blank the selectedField.
                if (!string.IsNullOrWhiteSpace(groupField))
                {
                    int selectedIndex = comboboxGroupField.FindStringExact(groupField);
                    if (selectedIndex == -1)
                    {
                        // Not Found
                        XmlElement xmlElementGroupField = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.GROUPFIELDKEY);
                        xmlElementGroupField.InnerText = "";
                    }
                    else
                    {
                        // Found
                        comboboxGroupField.SelectedIndex = selectedIndex;
                    }
                }
            } // end of "if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)"


            // Populate the XField Combobox with Fieldnames

            string xField = xmlConfig.XField;

            if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)
            {
                string xFieldNames = xmlConfig.XFieldNames;
                string[] arrXFieldNames = xFieldNames.Split(',');

                comboboxXField.Items.Clear();
                foreach (string xFieldName in arrXFieldNames)
                    comboboxXField.Items.Add(xFieldName);

                // Select the saved field                
                if (!string.IsNullOrWhiteSpace(xField))
                {
                    int selectedIndex = comboboxXField.FindStringExact(xField);
                    if (selectedIndex > 0)
                        comboboxXField.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                comboboxXField.Items.Clear();

                var xmlElementMetaInfo = eIncomingMetaInfo[0];
                var xmlElementRecordInfo = xmlElementMetaInfo.FirstChild;

                foreach (XmlElement elementChild in xmlElementRecordInfo)
                {
                    string fieldName = elementChild.GetAttribute("name");
                    //string fieldType = elementChild.GetAttribute("type");

                    //if (isStringType(fieldType))
                    //{
                    comboboxXField.Items.Add(fieldName);
                    //}
                }

                // If the selectedField matches a possible field in the combo box,
                // make it the selected field.
                // If the selectedField does not match, do not select anything and 
                // blank the selectedField.
                if (!string.IsNullOrWhiteSpace(xField))
                {
                    int selectedIndex = comboboxXField.FindStringExact(xField);
                    if (selectedIndex == -1)
                    {
                        // Not Found
                        XmlElement xmlElementXField = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.XFIELDKEY);
                        xmlElementXField.InnerText = "";
                    }
                    else
                    {
                        // Found
                        comboboxXField.SelectedIndex = selectedIndex;
                    }
                }
            } // end of "if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)"



            // Populate the YField Combobox with Fieldnames

            string yField = xmlConfig.YField;

            if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)
            {
                string yFieldNames = xmlConfig.YFieldNames;
                string[] arrYFieldNames = yFieldNames.Split(',');

                comboboxYField.Items.Clear();
                foreach (string yFieldName in arrYFieldNames)
                    comboboxYField.Items.Add(yFieldName);

                // Select the saved field                
                if (!string.IsNullOrWhiteSpace(yField))
                {
                    int selectedIndex = comboboxYField.FindStringExact(yField);
                    if (selectedIndex > 0)
                        comboboxYField.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                comboboxYField.Items.Clear();

                var xmlElementMetaInfo = eIncomingMetaInfo[0];
                var xmlElementRecordInfo = xmlElementMetaInfo.FirstChild;

                foreach (XmlElement elementChild in xmlElementRecordInfo)
                {
                    string fieldName = elementChild.GetAttribute("name");
                    //string fieldType = elementChild.GetAttribute("type");

                    //if (isStringType(fieldType))
                    //{
                    comboboxYField.Items.Add(fieldName);
                    //}
                }

                // If the selectedField matches a possible field in the combo box,
                // make it the selected field.
                // If the selectedField does not match, do not select anything and 
                // blank the selectedField.
                if (!string.IsNullOrWhiteSpace(yField))
                {
                    int selectedIndex = comboboxYField.FindStringExact(yField);
                    if (selectedIndex == -1)
                    {
                        // Not Found
                        XmlElement xmlElementYField = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.YFIELDKEY);
                        xmlElementYField.InnerText = "";
                    }
                    else
                    {
                        // Found
                        comboboxYField.SelectedIndex = selectedIndex;
                    }
                }
            } // end of "if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)"



            // Populate the Concavity Textbox and ConcavityWillUseValue

            string concavity = xmlConfig.Concavity;
            decimal dConcavity = 90;

            try { dConcavity = Convert.ToDecimal(concavity); } catch { }

            if (dConcavity > 180) dConcavity = 180;
            if (dConcavity < 0) dConcavity = 0;

            string sConcavity = ToTrimmedString(dConcavity);

            textboxConcavity.Text = sConcavity;
            labelConcavityWillUseValue.Text = sConcavity;


            // Populate the ScaleFactor textbox and ScaleFactorWillUseValue

            string scaleFactor = xmlConfig.ScaleFactor;
            decimal dScaleFactor = 0;

            if (scaleFactor == "NULL")
            {
                textboxScaleFactor.Text = "";
                labelScaleFactorWillUseValue.Text = "NULL";
            }
            else
            {
                try { dScaleFactor = Convert.ToDecimal(scaleFactor); } catch { }
                if (dScaleFactor > 0)
                {
                    string sScaleFactor = ToTrimmedString(dScaleFactor);

                    textboxScaleFactor.Text = sScaleFactor;
                    labelScaleFactorWillUseValue.Text = sScaleFactor;
                }
                else
                {
                    textboxScaleFactor.Text = "";
                    labelScaleFactorWillUseValue.Text = "NULL";
                }
            }

            return this;
        }



        // Helper
        private static bool isStringType(string fieldType)
        {
            return string.Equals(fieldType, "string", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fieldType, "v_string", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fieldType, "wstring", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fieldType, "v_wstring", StringComparison.OrdinalIgnoreCase);
        }

        public void SaveResultsToXml(XmlElement eConfig, out string strDefaultAnnotation)
        {

            // GROUP FIELD
            XmlElement xmlElementGroupFieldNames = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.GROUPFIELDNAMESKEY);
            List<string> groupFieldNames = new List<string>();
            foreach (var item in comboboxGroupField.Items)
                groupFieldNames.Add(item.ToString());

            xmlElementGroupFieldNames.InnerText = string.Join(",", groupFieldNames);

            XmlElement xmlElementGroupField = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.GROUPFIELDKEY);
            var selectedGroupFieldItem = comboboxGroupField.SelectedItem;
            string groupField = "";
            if (selectedGroupFieldItem != null) groupField = comboboxGroupField.SelectedItem.ToString();
            xmlElementGroupField.InnerText = groupField;


            // X FIELD
            XmlElement xmlElementXFieldNames = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.XFIELDNAMESKEY);
            List<string> xFieldNames = new List<string>();
            foreach (var item in comboboxXField.Items)
                xFieldNames.Add(item.ToString());

            xmlElementXFieldNames.InnerText = string.Join(",", xFieldNames);

            XmlElement xmlElementXField = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.XFIELDKEY);
            var selectedXFieldItem = comboboxXField.SelectedItem;
            string xField = "";
            if (selectedXFieldItem != null) xField = comboboxXField.SelectedItem.ToString();
            xmlElementXField.InnerText = xField;


            // Y FIELD
            XmlElement xmlElementYFieldNames = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.YFIELDNAMESKEY);
            List<string> yFieldNames = new List<string>();
            foreach (var item in comboboxYField.Items)
                yFieldNames.Add(item.ToString());

            xmlElementYFieldNames.InnerText = string.Join(",", yFieldNames);

            XmlElement xmlElementYField = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.YFIELDKEY);
            var selectedYFieldItem = comboboxYField.SelectedItem;
            string yField = "";
            if (selectedYFieldItem != null) yField = comboboxYField.SelectedItem.ToString();
            xmlElementYField.InnerText = yField;


            // CONCAVITY
            XmlElement xmlElementConcavity = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.CONCAVITYKEY);
            xmlElementConcavity.InnerText = labelConcavityWillUseValue.Text;

            // SCALE FACTOR
            XmlElement xmlElementScaleFactor = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.SCALEFACTORKEY);
            xmlElementScaleFactor.InnerText = labelScaleFactorWillUseValue.Text;


            // Set the default annotation
            strDefaultAnnotation = "Concave Hull";
        }

        public static string ToTrimmedString(decimal target)
        {
            string strValue = target.ToString(); //Get the stock string

            //If there is a decimal point present
            if (strValue.Contains("."))
            {
                //Remove all trailing zeros
                strValue = strValue.TrimEnd('0');

                //If all we are left with is a decimal point
                if (strValue.EndsWith(".")) //then remove it
                    strValue = strValue.TrimEnd('.');
            }

            return strValue;
        }



        /////////////
        // HANDLERS

        private void textboxConcavity_KeyUp(object sender, KeyEventArgs e)
        {
            // User has typed something into the box.
            decimal dConcavity = 0;

            try { dConcavity = Convert.ToDecimal(textboxConcavity.Text); } catch { }

            if (dConcavity > 180) dConcavity = 180;
            if (dConcavity < 0) dConcavity = 0;

            labelConcavityWillUseValue.Text = ToTrimmedString(dConcavity); 
        }

        private void textboxConcavity_Leave(object sender, EventArgs e)
        {
            decimal dConcavity = 0;

            try { dConcavity = Convert.ToDecimal(textboxConcavity.Text); } catch { }

            if (dConcavity > 180) dConcavity = 180;
            if (dConcavity < 0) dConcavity = 0;

            labelConcavityWillUseValue.Text = ToTrimmedString(dConcavity);
        }

        private void textboxScaleFactor_KeyUp(object sender, KeyEventArgs e)
        {
            decimal dScaleFactor = -1;

            try { dScaleFactor = Convert.ToDecimal(textboxScaleFactor.Text); } catch { }

            if (dScaleFactor > 0)
            {
                labelScaleFactorWillUseValue.Text = ToTrimmedString(dScaleFactor);
            }
            else
            {
                labelScaleFactorWillUseValue.Text = "NULL";
            }
        }

        private void textboxScaleFactor_Leave(object sender, EventArgs e)
        {
            decimal dScaleFactor = -1;

            try { dScaleFactor = Convert.ToDecimal(textboxScaleFactor.Text); } catch { }

            if (dScaleFactor > 0)
            {
                labelScaleFactorWillUseValue.Text = ToTrimmedString(dScaleFactor);
            }
            else
            {
                labelScaleFactorWillUseValue.Text = "NULL";
            }
        }

        private void labelConcavityWillUseValue_Click(object sender, EventArgs e)
        {

        }
    }
}
