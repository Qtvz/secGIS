using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;

namespace VioGIS
{
    public partial class DataBoard : Form
    {
        public DataBoard(string sDataName)
        {
            InitializeComponent();
            tbDataName.Text = sDataName;
        }
        private static DataTable CreatDataTableByLayer(ILayer pLayer, string tablename)
        {
            DataTable pDt = new DataTable(tablename);
            ITable pTable = pLayer as ITable;
            IField pFd = null;
            DataColumn pDc;
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pFd = pTable.Fields.get_Field(i);
                pDc = new DataColumn(pFd.Name);
                if (pFd.Name == pTable.OIDFieldName)
                {
                    pDc.Unique = true;
                }
                pDc.AllowDBNull = pFd.IsNullable;
                pDc.Caption = pFd.AliasName;
                pDc.DataType = System.Type.GetType(ParseFieldType(pFd.Type));
                pDc.DefaultValue = pFd.DefaultValue;
                if (pFd.VarType == 8)
                {
                    pDc.MaxLength = pFd.Length;
                }
                pDt.Columns.Add(pDc);
                pFd = null;
                pDc = null;
            }
            return pDt;
        }

        public static string ParseFieldType(esriFieldType FieldType)
        {
            switch (FieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DataTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                default:
                    return "System.String";
            }
        }

        public static DataTable CreatDataTable(ILayer pLayer, string tablename)
        {
            DataTable pDt = CreatDataTableByLayer(pLayer, tablename);
            string shapeType = getShapeType(pLayer);
            DataRow pDr = null;
            ITable pT = pLayer as ITable;
            ICursor pCr = pT.Search(null, false);
            IRow pR = pCr.NextRow();
            int n = 0;
            while (pR != null)
            {
                pDr = pDt.NewRow();
                for (int i = 0; i < pR.Fields.FieldCount; i++)
                {
                    if (pR.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDr[i] = shapeType;
                    }
                    else if (pR.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDr[i] = "Element";
                    }
                    else
                    {
                        pDr[i] = pR.get_Value(i);
                    }
                }
                pDt.Rows.Add(pDr);
                pDr = null;
                n++;
                if (n == 2000)
                {
                    pR = null;
                }
                else
                {
                    pR = pCr.NextRow();
                }
            }
            return pDt;
        }

        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer pFl = (IFeatureLayer)pLayer;
            switch (pFl.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "点";
                case esriGeometryType.esriGeometryPolyline:
                    return "线";
                case esriGeometryType.esriGeometryPolygon:
                    return "面";
                default:
                    return "";
            }
        }

        public DataTable attributeTable;

        public void CreateAttributeTable(ILayer pLayer)
        {
            string tablename;
            tablename = getValidFeatureClassName(pLayer.Name);
            attributeTable = CreatDataTable(pLayer, tablename);
            this.dataGridView1.DataSource = attributeTable;
            this.Text = "属性表[" + tablename + "]" + "记录数：" + attributeTable.Rows.Count.ToString();
        }

        public static string getValidFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }
    }
}
