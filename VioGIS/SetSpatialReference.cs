using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF;

namespace VioGIS
{
    public partial class SetSpatialReference : Form
    {
        private ISpatialReferenceFactory3 spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
        public ISpatialReference pSpatialReference = new ProjectedCoordinateSystemClass();
        private IParameter[] parameterArray = new IParameter[5];
        private IGeographicCoordinateSystem geographicCoordinateSystem;
        private IProjectionGEN projection;
        private ILinearUnit unit;
        private object name;

        public SetSpatialReference()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void SetSpatialReference_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double ll0 = 0.0;
            int zoneNum = 0;
            try
            {
                ll0 = double.Parse(txtCenterLongitude.Text);
                if (ll0 <= 0 || ll0 >= 180 || txtCenterLongitude.Text.Contains("."))
                {
                    MessageBox.Show("请输入0-180之间的整数");
                    txtCenterLongitude.Text = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入0-180之间的整数");
                txtCenterLongitude.Text = null;
                return;
            }
            //计算带号
            if (rbtn6degree.Checked == true)
            {
                GeoRegister.StrTripType = "Strip6D";

            }
            else if (rbtn3degree.Checked == true)
            {
                GeoRegister.StrTripType = "Strip3D";
            }

            if (GeoRegister.StrTripType == "Strip6D")
            {
                //6度带带号
                zoneNum = GeoRegister.RoundOff((ll0 + 3) / 6);
            }
            else if (GeoRegister.StrTripType == "Strip3D")
            {
                //3度带带号
                zoneNum = GeoRegister.RoundOff(ll0 / 3);
            }
            //}
            //是否有带号
            if (chbNumber.Checked == true)
            {
                GeoRegister.BoolTripNum = true;
            }
            if (GeoRegister.BoolTripNum == true)
            {
                parameterArray[0] = spatialReferenceFactory.CreateParameter((int)esriSRParameterType.esriSRParameter_FalseEasting);
                parameterArray[0].Value = Convert.ToDouble(zoneNum.ToString() + "500000");
            }
            else
            {
                parameterArray[0] = spatialReferenceFactory.CreateParameter((int)esriSRParameterType.esriSRParameter_FalseEasting);
                parameterArray[0].Value = 500000;
            }
            if (rbtnBeijing.Checked == true)
            {
                GeoRegister.StrCoordiType = "BeiJing54";
            }
            else if (rbtnXian.Checked == true)
            {
                GeoRegister.StrCoordiType = "Xian80";
            }
            else if (rbtnWGS.Checked == true)
            {
                GeoRegister.StrCoordiType = "WGS84";
            }

            if (GeoRegister.StrCoordiType == "BeiJing54")
            {

                geographicCoordinateSystem = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_Beijing1954);
                if (GeoRegister.BoolTripNum == true)
                    name = "Beijing_1954_3_Degree_GK_Zone_" + zoneNum.ToString();
                else
                    name = "Beijing_1954_3_Degree_GK_CM_" + ll0.ToString() + "E";

            }
            else if (GeoRegister.StrCoordiType == "Xian80")
            {

                geographicCoordinateSystem = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCS3Type.esriSRGeoCS_Xian1980);
                if (GeoRegister.BoolTripNum == true)
                    name = "Xian_1980_3_Degree_GK_Zone_" + zoneNum.ToString();
                else
                    name = "Xian_1980_3_Degree_GK_CM_" + ll0.ToString() + "E";

            }
            else if (GeoRegister.StrCoordiType == "WGS84")
            {

                geographicCoordinateSystem = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                name = "WGS_1984_GK_Zone_" + zoneNum.ToString() + "N";

            }

            parameterArray[1] = spatialReferenceFactory.CreateParameter((int)esriSRParameterType.esriSRParameter_FalseNorthing);
            parameterArray[1].Value = 0;
            parameterArray[2] = spatialReferenceFactory.CreateParameter((int)esriSRParameterType.esriSRParameter_CentralMeridian);
            parameterArray[2].Value = ll0;
            parameterArray[3] = spatialReferenceFactory.CreateParameter((int)esriSRParameterType.esriSRParameter_LatitudeOfOrigin);
            parameterArray[3].Value = 0;
            parameterArray[4] = spatialReferenceFactory.CreateParameter((int)esriSRParameterType.esriSRParameter_ScaleFactor);
            parameterArray[4].Value = 1.0;
            projection = spatialReferenceFactory.CreateProjection((int)esriSRProjectionType.esriSRProjection_GaussKruger) as IProjectionGEN;
            unit = spatialReferenceFactory.CreateUnit((int)esriSRUnitType.esriSRUnit_Meter) as ILinearUnit;
            pSpatialReference = CreateProjectedCoordinateSystem(projection, parameterArray,
                                                                geographicCoordinateSystem, unit, name);
            this.Close();
            MessageBox.Show(pSpatialReference.Name);
        
        }
        private IProjectedCoordinateSystem CreateProjectedCoordinateSystem(IProjectionGEN projection,
         IParameter[] parameterArray, IGeographicCoordinateSystem geographicCoordinateSystem, ILinearUnit unit,
         object ProName)
        {
            IProjectedCoordinateSystem projectedCoordinateSystem = new ProjectedCoordinateSystemClass();
            IProjectedCoordinateSystemEdit projectedCoordinateSystemEdit = projectedCoordinateSystem as IProjectedCoordinateSystemEdit;
            object name = ProName;
            object alias = "GK";
            object abbreviation = "GK";
            object remarks = "his PCS is Gauss_Kruger";
            object usage = "";
            object geographicCoordinateSystemObject = geographicCoordinateSystem;
            object projectedUnitObject = unit;
            object projectionObject = projection;
            object parametersObject = parameterArray;


            projectedCoordinateSystemEdit.Define(ref name,
                                                 ref alias,
                                                 ref abbreviation,
                                                 ref remarks,
                                                 ref usage,
                                                 ref geographicCoordinateSystemObject,
                                                 ref projectedUnitObject,
                                                 ref projectionObject,
                                                 ref parametersObject);
            return projectedCoordinateSystemEdit as IProjectedCoordinateSystem;
        }





        public static bool Checked { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    class GeoRegister
    {
        public static string strCoordiType = string.Empty;
        public static string strTripType = string.Empty;
        public static string strUnitType = string.Empty;
        public static bool boolTripNum = false;
        static string GeoreferenceType = string.Empty;

        public static string StrCoordiType
        {
            get { return strCoordiType; }
            set { strCoordiType = value; }
        }
        public static string StrTripType
        {
            get { return strTripType; }
            set { strTripType = value; }
        }
        public static string StrUnitType
        {
            get { return strUnitType; }
            set { strUnitType = value; }
        }
        public static bool BoolTripNum
        {
            get { return boolTripNum; }
            set { boolTripNum = value; }
        }
        public static int RoundOff(double source)
        {
            int integer = Convert.ToInt32(source);
            if (source - integer >= 0.5)
            {
                integer++;
            }
            return integer;

        }
    }
}

    

