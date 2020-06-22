using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;
using AlteryxRecordInfoNet;
using System.Net;
using System.Drawing;



namespace ContinuumConcaveHull
{
    public class ConcaveHullNetPlugin : INetPlugin, IIncomingConnectionInterface
    {

        private int _toolID; // Integer identifier provided by Alteryx, this tools tool number.
        private EngineInterface _engineInterface; // Reference provided by Alteryx so that we can talk to the engine.
        private XmlElement _xmlProperties; // Xml configuration for this custom tool

        private PluginOutputConnectionHelper _outputHelper;

        private RecordInfo _recordInfoIn;
        private RecordInfo _recordInfoOut;



        private string _groupField = Constants.DEFAULTGROUPFIELD;
        private string _xField = Constants.DEFAULTXFIELD;
        private string _yField = Constants.DEFAULTYFIELD;

        private string _sConcavity = Constants.DEFAULTCONCAVITY;
        private string _sScaleFactor = Constants.DEFAULTSCALEFACTOR;

        private Dictionary<string, List<Tuple<Double, Double>>> _dicPoints             
            = new Dictionary<string, List<Tuple<Double, Double>>>();


        public void PI_Init(int nToolID, EngineInterface engineInterface, XmlElement pXmlProperties)
        {
            DebugMessage($"PI_Init() Entering; ToolID={_toolID}");

            _toolID = nToolID;
            _engineInterface = engineInterface;
            _xmlProperties = pXmlProperties;

            // Use the information in the pXmlProperties parameter to get the input xml field name
            XmlElement configElement = XmlHelpers.GetFirstChildByName(_xmlProperties, "Configuration", true);
            if (configElement != null)
            {
                getConfigSetting(configElement, Constants.GROUPFIELDKEY, ref _groupField);
                getConfigSetting(configElement, Constants.XFIELDKEY, ref _xField);
                getConfigSetting(configElement, Constants.YFIELDKEY, ref _yField);

                getConfigSetting(configElement, Constants.CONCAVITYKEY, ref _sConcavity);
                getConfigSetting(configElement, Constants.SCALEFACTORKEY, ref _sScaleFactor);
            }

            // Re-initialise dictionary if the tool is run a second time.
            _dicPoints = new Dictionary<string, List<Tuple<Double, Double>>>();

            _outputHelper = new PluginOutputConnectionHelper(_toolID, _engineInterface);

            DebugMessage($"PI_Init() Exiting; ToolID={_toolID}");
        }

        public IIncomingConnectionInterface PI_AddIncomingConnection(string pIncomingConnectionType, string pIncomingConnectionName)
        {
            DebugMessage($"PI_AddIncomingConnection() has been called; Name={pIncomingConnectionName}");
            return this;
        }

        public bool PI_AddOutgoingConnection(string pOutgoingConnectionName, OutgoingConnection outgoingConnection)
        {
            // Add the outgoing connection to our PluginOutputConnectionHelper so it can manage it.
            _outputHelper.AddOutgoingConnection(outgoingConnection);

            DebugMessage($"PI_AddOutgoingConnection() has been called; Name={pOutgoingConnectionName}");
            return true;
        }

        public bool PI_PushAllRecords(long nRecordLimit)
        {
            // Should not be called
            DebugMessage($"PI_PushAllRecords() has been called; THIS SHOULD NOT BE CALLED FOR A TOOL WITH AN INPUT CONNECTION!");
            return true;
        }

        public void PI_Close(bool bHasErrors)
        {
            // Release any resources used by the control
            _outputHelper.Close();
            DebugMessage("PI_Close() has been called; OutputHelper has been closed.");
        }

        public bool ShowDebugMessages()
        {
            // Return true to help us debug our tool. This should be set to false for general distribution.
            #if DEBUG
                return true;
            #else
                return false;      
            #endif
        }

        public XmlElement II_GetPresortXml(XmlElement pXmlProperties)
        {
            DebugMessage($"II_GetPresortXml() has been called");

            return null;
        }

        public bool II_Init(RecordInfo recordInfo)
        {
            DebugMessage($"II_Init() Entering; ToolID={_toolID}");

            _recordInfoIn = recordInfo;

            prep();

            DebugMessage($"II_Init() Exiting; ToolID={_toolID};");
            return true;
        }


        // Receive an inbound record and process it.
        // Information about the record is held in the recordInfo object
        // that was passed in to II_Init(), and (hopefully) cached.
        public bool II_PushRecord(AlteryxRecordInfoNet.RecordData recordDataIn)
        {
            DebugMessage($"II_PushRecord() Entering; ToolID={_toolID}");

            // If we don't have a group field, no problem, optional

            // We need an X and Y field, pump out errors if points are not numbers

            // We will have a concavity, defaulting to zero

            // Scale Factor still un-known.

            // Might want an optional max number of points in enclosing polygon


            // Clear the collection
            //_dicPoints = new Dictionary<string, List<Tuple<Double, Double>>>();

            // Get the point
            if (string.IsNullOrWhiteSpace(_xField))
            {
                ErrorMessage($"X Field not selected");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_yField))
            {
                ErrorMessage($"Y Field not selected");
                return false;
            }

            FieldBase xFieldBase = null;
            try { xFieldBase = _recordInfoIn.GetFieldByName(_xField, false); }
            catch
            {
                ErrorMessage($"X Field [{_xField}] does not exist.");
                return false;
            }

            FieldBase yFieldBase = null;
            try { yFieldBase = _recordInfoIn.GetFieldByName(_yField, false); }
            catch
            {
                ErrorMessage($"Y Field [{_yField}] does not exist.");
                return false;
            }

            if (xFieldBase == null)
            {
                ErrorMessage($"X Field [{_xField}] exists but the FieldBase could not be accessed.");
                return false;
            }

            if (yFieldBase == null)
            {
                ErrorMessage($"Y Field [{_yField}] exists but the FieldBase could not be accessed.");
                return false;
            }


            // Got an X and Y fieldbase

            string xText = "";
            try { xText = xFieldBase.GetAsString(recordDataIn); }
            catch
            {
                ErrorMessage($"X Field [{_xField}] exists but the FieldBase did not yield a value string.");
                return false;
            }

            string yText = "";
            try { yText = yFieldBase.GetAsString(recordDataIn); }
            catch
            {
                ErrorMessage($"Y Field [{_yField}] exists but the FieldBase did not yield a value string.");
                return false;
            }

            if (string.IsNullOrEmpty(xText))
            {
                ErrorMessage($"X Field [{_xField}] has missing data; Ensure all rows entering this tool have point data; Remove empty rows with a Filter tool ahead of this tool.");
                return false;
            }

            if (string.IsNullOrEmpty(yText))
            {
                ErrorMessage($"Y Field [{_yField}] has missing data; Ensure all rows entering this tool have point data; Remove empty rows with a Filter tool ahead of this tool.");
                return false;
            }

            // Got an X and Y string
            // Convert to double

            double dX = 0;
            double dY = 0;

            try { dX = Convert.ToDouble(xText); }
            catch
            {
                ErrorMessage($"X Field [{_xField}] data element [{xText}] could not be converted to a double value.");
                return false;
            }

            try { dY = Convert.ToDouble(yText); }
            catch
            {
                ErrorMessage($"Y Field [{_yField}] data element [{yText}] could not be converted to a double value.");
                return false;
            }

            // Got an X and Y double value;
            // On to the Groups...

            // Read group if it exists
            string currentGroup = "DEFAULT";

            if (!string.IsNullOrWhiteSpace(_groupField))
            {
                FieldBase groupFieldBase = null;
                try { groupFieldBase = _recordInfoIn.GetFieldByName(_groupField, false); }
                catch
                {
                    ErrorMessage($"Group Field [{_groupField}] does not exist.");
                    return false;
                }

                // Got the fieldbase for the Group Field
                if (groupFieldBase == null)
                {
                    ErrorMessage($"Group Field [{_groupField}] exists but the FieldBase could not be accessed.");
                    return false;
                }

                // Got a non-null fieldbase
                string groupText = "";
                try { groupText = groupFieldBase.GetAsString(recordDataIn); }
                catch
                {
                    ErrorMessage($"Group Field [{_groupField}] could not be accessed.");
                    return false;
                }

                if (!string.IsNullOrEmpty(groupText)) currentGroup = groupText;                   
            }

            // Get existing list or create new list
            List<Tuple<Double,Double>> points = null;
            if (!_dicPoints.TryGetValue(currentGroup, out points))
            {
                // No list yet
                points = new List<Tuple<Double,Double>>();
                _dicPoints.Add(currentGroup, points);
            }

            // Should either have the current group list, or a new list.

            points.Add(new Tuple<Double, Double>(dX, dY));

            DebugMessage($"Added point ({xText},{yText}) for Group [{currentGroup}]");

            DebugMessage($"II_PushRecord() Exiting; ToolID={_toolID}");
            return true;
        }




        public void II_UpdateProgress(double dPercent)
        {
            // Since our progress is directly proportional to he progress of the
            // upstream tool, we can simply output it's percentage as our own.
            if (_engineInterface.OutputToolProgress(_toolID, dPercent) != 0)
            {
                // If this returns anything but 0, then the user has canceled the operation.
                throw new AlteryxRecordInfoNet.UserCanceledException();
            }

            // Have the PluginOutputConnectionHelper ask the downstream tools to update their progress.
            _outputHelper.UpdateProgress(dPercent);
            DebugMessage($"II_UpdateProgress() has been called; dPercent={dPercent}; ToolID={_toolID}");
        }

        public void II_Close()
        {
            DebugMessage($"II_Close() has been called; ToolID={_toolID}");

            // Get Concavity and ScaleFactor
            double concavity = 90;
            try { concavity = Convert.ToDouble(_sConcavity); }
            catch
            {
                WarningMessage($"Failed to convert concavity [{_sConcavity}] to double value;  Using 90 degrees;");
                concavity = 0;
            }
            /*
            Int32 scaleFactor = 1;
            try { scaleFactor = Convert.ToInt32(_sScaleFactor); }
            catch
            {
                WarningMessage($"Failed to convert scale factor [{_sScaleFactor}] to integer value;  Using 1;");
                scaleFactor = 1;
            }

            if (scaleFactor < 1)
            {
                WarningMessage($"Scale factor [{_sScaleFactor}] was unacceptable;  Positive non-zero integer required; Using 1;");
                scaleFactor = 1;
            }
            */

            foreach (KeyValuePair<string,List<Tuple<Double,Double>>> kvp in _dicPoints)
            {
                DebugMessage($"Group [{kvp.Key}] has [{kvp.Value.Count}] points.  Would process at this point.");

                

                ////////////////////
                // HULL PROCESSING

                List<Node> dot_list = new List<Node>();
                int id = 0;
                foreach(Tuple<double,double> element in kvp.Value)
                {
                    ++id;
                    Node node = new Node(element.Item1, element.Item2, id);
                    dot_list.Add(node);
                }

                var hull = new Hull();
                hull.makeHull(dot_list,concavity);
                List<Node> way = hull.getWay();

                string diag = hull.dumpWay(way);

                // END HULL PROCESSING
                ////////////////////////

                int seq = 0;
                foreach (Node node in way)
                {
                    Record recordOut = _recordInfoOut.CreateRecord();
                    recordOut.Reset();

                    // Group
                    FieldBase fieldBase = _recordInfoOut[0];
                    fieldBase.SetFromString(recordOut, kvp.Key);

                    // Sequence Number Dummy
                    fieldBase = _recordInfoOut[1];
                    fieldBase.SetFromInt32(recordOut, seq);

                    // XCoord
                    fieldBase = _recordInfoOut[2];
                    fieldBase.SetFromDouble(recordOut, node.x);

                    // YCoord
                    fieldBase = _recordInfoOut[3];
                    fieldBase.SetFromDouble(recordOut, node.y);

                    // Write record
                    _outputHelper.PushRecord(recordOut.GetRecord());

                    ++seq;
                }
                
            }
            

            DebugMessage($"II_Close() is exiting; ToolID={_toolID}");
        }

        private void prep()
        {
            // Exit if already done (safety)
            if (_recordInfoOut != null)
                return;

            _recordInfoOut = new AlteryxRecordInfoNet.RecordInfo(); // Make a new record

            //_recordInfoOut = new AlteryxRecordInfoNet.RecordInfo();

            /*
            // Copy the fieldbase structure of the incoming record
            uint countFields = _recordInfoIn.NumFields();
            for (int i = 0; i < countFields; ++i)
            {
                FieldBase fbIn = _recordInfoIn[i];
                var currentFieldName = fbIn.GetFieldName();

                _recordInfoOut.AddField(currentFieldName, fbIn.FieldType, (int)fbIn.Size, fbIn.Scale, fbIn.GetSource(), fbIn.GetDescription());
            }
            */

            // Add the output columns at the end
            _recordInfoOut.AddField("ConcaveHull_Polygon_Group", FieldType.E_FT_V_WString, Constants.LARGEOUTPUTFIELDSIZE, 0, "", "");
            _recordInfoOut.AddField("ConcaveHull_Polygon_Sequence", FieldType.E_FT_Int32, Constants.INT32_FIELDSIZE, 0, "", "");
            _recordInfoOut.AddField("ConcaveHull_Polygon_XCoord", FieldType.E_FT_Double, Constants.DOUBLE_FIELDSIZE, 0, "", "");
            _recordInfoOut.AddField("ConcaveHull_Polygon_YCoord", FieldType.E_FT_Double, Constants.DOUBLE_FIELDSIZE, 0, "", "");


            //_recordCopier.DoneAdding();

            _outputHelper.Init(_recordInfoOut, "Output", null, _xmlProperties);
        }

        /*
        private void prep()
        {
            // Exit if already done (safety)
            if (_recordInfoOut != null)
                return;

            _recordInfoOut = new AlteryxRecordInfoNet.RecordInfo();

            populateRecordInfoOut();

            _recordCopier = new RecordCopier(_recordInfoOut, _recordInfoIn, true);

            uint countFields = _recordInfoIn.NumFields();
            for (int i = 0; i < countFields; ++i)
            {
                var fieldName = _recordInfoIn[i].GetFieldName();

                var newFieldNum = _recordInfoOut.GetFieldNum(fieldName, false);
                if (newFieldNum == -1)
                    continue;

                _recordCopier.Add(newFieldNum, i);
            }

            _recordCopier.DoneAdding();

            _outputHelper.Init(_recordInfoOut, "Output", null, _xmlProperties);
        }
        */

            /*
        private void populateRecordInfoOut()
        {
            _recordInfoOut = new AlteryxRecordInfoNet.RecordInfo();

            
            // Copy the fieldbase structure of the incoming record
            uint countFields = _recordInfoIn.NumFields();
            for (int i = 0; i < countFields; ++i)
            {
                FieldBase fbIn = _recordInfoIn[i];
                var currentFieldName = fbIn.GetFieldName();

                _recordInfoOut.AddField(currentFieldName, fbIn.FieldType, (int)fbIn.Size, fbIn.Scale, fbIn.GetSource(), fbIn.GetDescription());
            }
            

            // Add the output columns at the end
            _recordInfoOut.AddField("ConcaveHull_Polygon_Sequence", FieldType.E_FT_Int32, Constants.INT32_FIELDSIZE, 0, "", "");
            _recordInfoOut.AddField("ConcaveHull_Polygon_XCoord", FieldType.E_FT_Double, Constants.DOUBLE_FIELDSIZE, 0, "", "");
            _recordInfoOut.AddField("ConcaveHull_Polygon_YCoord", FieldType.E_FT_Double, Constants.DOUBLE_FIELDSIZE, 0, "", "");
        }
        */


        //////////// 
        // HELPERS


        private void getConfigSetting(XmlElement configElement, string key, ref string memberToSet)
        {
            XmlElement xe = XmlHelpers.GetFirstChildByName(configElement, key, false);
            if (xe != null)
            {
                if (!string.IsNullOrWhiteSpace(xe.InnerText))
                    memberToSet = xe.InnerText;
            }
        }

        public static bool isTrueString(string target)
        {
            string cleanTarget = target.Trim().ToUpper();
            switch (cleanTarget)
            {
                case "Y":
                case "TRUE":
                case "1":
                    return true;
                default:
                    break;
            }
            return false;
        }

        /////////////////
        // HULL HELPERS

        /*
        private List<Node> convertPolygonToWay(List<Line> polygon)
        {
            int seq = 0;
            List<Node> nodes = new List<Node>();

            foreach(Line line in polygon)
            {
                Node node = new Node(line.nodes[0].x, line.nodes[0].y, seq);
                nodes.Add(node);
                ++seq;
            }

            // Add the node at the end of the last line, 
            // which should be the same as the start node.
            Node lastNode = new Node(polygon[0].nodes[0].x, polygon[0].nodes[0].y, seq);
            nodes.Add(lastNode);

            return nodes;
        }
        */


        //////////////////////// 
        // Message Boilerplate

        public void Message(string message, MessageStatus messageStatus = MessageStatus.STATUS_Info)
        {
            this._engineInterface?.OutputMessage(this._toolID, messageStatus, message);
        }

        public void ErrorMessage(string message, MessageStatus messageStatus = MessageStatus.STATUS_Error)
        {
            this._engineInterface?.OutputMessage(this._toolID, messageStatus, message);
        }

        public void WarningMessage(string message, MessageStatus messageStatus = MessageStatus.STATUS_Warning)
        {
            this._engineInterface?.OutputMessage(this._toolID, messageStatus, message);
        }

        public void DebugMessage(string message)
        {
            if (ShowDebugMessages()) this.Message(message);
        }
    }
}
