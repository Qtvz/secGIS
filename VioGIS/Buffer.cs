using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;

namespace VioGIS
{
    public partial class Buffer : Form
    {
        public IMap m_map;
        public IMapControl3 m_mapControl;

        public Buffer(IMap imap)
        {
            InitializeComponent();
            m_map = imap;
        }

        private void Buffer_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                comboBoxLayer.Items.Add(m_map.get_Layer(i).Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double bufferDistance;
            double.TryParse(txtBufferDistance.Text, out bufferDistance);
            if (0.0 == bufferDistance)
            {
                MessageBox.Show("距离设置错误", "Error!");
                return;
            }
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtOutputPath.Text)) || ".shp" != System.IO.Path.GetExtension(txtOutputPath.Text))
            {
                MessageBox.Show("输出格式错误！");
                return;
            }
            if (comboBoxLayer.Items.Count <= 0)
            {
                return;
            }
            DataOperator pDo = new DataOperator(m_map, null);
            IFeatureLayer pFl = (IFeatureLayer)pDo.GetLayerbyName(comboBoxLayer.SelectedItem.ToString());
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            gp.AddOutputsToMap = true;
            string unit = "Kilometers";
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(pFl, txtOutputPath.Text, Convert.ToString(bufferDistance) + "" + unit);
            try
            {
                IGeoProcessorResult results = (IGeoProcessorResult)gp.Execute(buffer, null);
            }
            catch
            { 
            }
            string fileDirectory = txtOutputPath.Text.ToString().Substring(0, txtOutputPath.Text.LastIndexOf("\\"));
            int j;
            j = txtOutputPath.Text.LastIndexOf("\\");
            string tmpstr = txtOutputPath.Text.ToString().Substring(j + 1);
            IWorkspaceFactory pWsf = new ShapefileWorkspaceFactory() as IWorkspaceFactory;
            IWorkspace pWs = pWsf.OpenFromFile(fileDirectory, 0);
            IFeatureWorkspace pFs = pWs as IFeatureWorkspace;
            IFeatureClass pFc = pFs.OpenFeatureClass(tmpstr);
            IFeatureLayer pfl = new FeatureLayer() as IFeatureLayer;
            pfl.FeatureClass = pFc;
            IRgbColor pColor = new RgbColor() as IRgbColor;
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            ILineSymbol pOutline = new SimpleLineSymbol();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            pColor = new RgbColor();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 100;
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbol();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            pFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
            ISimpleRenderer pRen;
            IGeoFeatureLayer pGeofl = pfl as IGeoFeatureLayer;
            pRen = pGeofl.Renderer as ISimpleRenderer;
            pRen.Symbol = pFillSymbol as ISymbol;
            pGeofl.Renderer = pRen as IFeatureRenderer;
            ILayerEffects pLayerEffects = pfl as ILayerEffects;
            pLayerEffects.Transparency = 150;
            m_mapControl.AddLayer((ILayer)pfl, 0);
            MessageBox.Show(comboBoxLayer.SelectedText + "缓冲区生成成功！");
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog pSfd = new SaveFileDialog();
            pSfd.Title = "输出要素类";
            pSfd.Filter = "要素类|*.shp";
            pSfd.ShowDialog();
            string sFilePath = pSfd.FileName;
            txtOutputPath.Text = sFilePath;
        }
    }
}
