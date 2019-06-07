using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GeocodingTools;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using stdole;
using ESRI.ArcGIS.SpatialAnalyst;
using System.Media;
using Calling;

namespace VioGIS
{
    public partial class Form1 : Form
    {
        IMapDocument pMdt;
        IMap pMAP;
        IActiveView pActiveView;
        ILayer pMovelayer;
        int toindex = 0;
        ILayer pTempLayer;
        IMapDocument m_MapDocument;

        public static void Success()
        {
            MessageBox.Show("The program has been done.", "Success!");
        }

        public static void Error()
        {
            MessageBox.Show("There may be someting wrong,please try again.", "Error!");
        }
        public Form1()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            InitializeComponent();
        }

        string str1 = System.IO.Directory.GetCurrentDirectory();
        System.Media.SoundPlayer sp = new SoundPlayer();
        private void Form1_Load(object sender, EventArgs e)
        {           
            this.WindowState = FormWindowState.Maximized;
            axTOCControl1.SetBuddyControl(axMapControl1);
            axToolbarControl1.SetBuddyControl(axMapControl1);
            axToolbarControl1.AddItem(new GeoMapComm(),-1,-1,false,0,esriCommandStyles.esriCommandStyleIconOnly);
            sp.SoundLocation = str1 + @"\bgm\Canon.wav";
            sp.PlayLooping();
            loadMapDoc();
        }

        public void CreatBookmark(string BookMarkname)
        {
            IAOIBookmark pBm = new AOIBookmarkClass();
            if (pBm != null)
            {
                pBm.Location = axMapControl1.ActiveView.Extent;
                pBm.Name = BookMarkname;
            }
            IMapBookmarks pBms = axMapControl1.Map as IMapBookmarks;
            if (pBms != null)
            {
                pBms.AddBookmark(pBm);
                cbBookmarkList.Items.Add(pBm.Name);
            }
        }

        static public IFeatureClass OpenShapefileFeatureClass(string shpName, string shpPath)
        {
            IWorkspaceFactory pWsf = new ShapefileWorkspaceFactoryClass();
            IPropertySet pPpts = new PropertySetClass();
            pPpts.SetProperty("DATABASE", shpPath);
            IWorkspace pWs = pWsf.Open(pPpts, 0);
            IFeatureWorkspace pFws = pWs as IFeatureWorkspace;
            IFeatureClass pFc = pFws.OpenFeatureClass(shpName);
            return pFc;
        }
        
        private IRgbColor GetColor(int r, int g, int b, int t)
        {
            IRgbColor pRc = new RgbColorClass();
            pRc.Red = r;
            pRc.Green = g;
            pRc.Blue = b;
            pRc.Transparency = (byte)t;
            return pRc;
        }
        
        public void loadMapDoc()
        {
            m_MapDocument = new MapDocumentClass();
            OpenFileDialog Opfg = new OpenFileDialog();
            Opfg.Title = "打开地图文档";
            Opfg.Filter = "map documents(*.mxd)|*.mxd";
            Opfg.ShowDialog();
            string fp = Opfg.FileName;
            if (fp == string.Empty)
            {
                MessageBox.Show(fp + "不是有效的地图文档", "文档错误");
                return;
            }
            tSslL.Text = "状态：显示地图文档";
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerHourglass;
            m_MapDocument.Open(fp,"");
            for (int i = 0; i < m_MapDocument.MapCount; i++)
            {
                axMapControl1.Map = m_MapDocument.get_Map(i);
            }
            axMapControl1.Refresh();
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void 加载地图文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadMapDoc();
            tSslL.Text = "状态：完成";
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            IMap pMap;
            pMap = axMapControl1.Map;
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                axMapControl2.Map.AddLayer(axMapControl1.get_Layer(i));
            }
            axMapControl2.Extent = axMapControl2.FullExtent;
            GeoMapLoad.CopyAndOverwriteMap(axMapControl1, axPageLayoutControl1);
        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPt = new PointClass();
                pPt.X = e.mapX;
                pPt.Y = e.mapY;
                IEnvelope pEnv1 = axMapControl1.Extent as IEnvelope;
                pPt.PutCoords(e.mapX, e.mapY);
                pEnv1.CenterAt(pPt);
                axMapControl1.Extent = pEnv1;
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
            else if (e.button == 2)
            {
                IEnvelope pEnv2 = axMapControl2.TrackRectangle();
                axMapControl1.Extent = pEnv2;
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button != 1)
                return;
            IPoint pPt1 = new PointClass();
            pPt1.X = e.mapX;
            pPt1.Y = e.mapY;
            axMapControl1.CenterAt(pPt1);
            axMapControl2.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGpc = axMapControl2.Map as IGraphicsContainer;
            IActiveView pAv = pGpc as IActiveView;
            pGpc.DeleteAllElements();
            IElement pElm = new RectangleElementClass();
            pElm.Geometry = pEnv;
            ILineSymbol pOls = new SimpleLineSymbolClass();
            pOls.Width = 2;
            pOls.Color = GetColor(255, 0, 0, 255);
            IFillSymbol pFsb = new SimpleFillSymbolClass();
            pFsb.Color = GetColor(9, 0, 0, 0);
            pFsb.Outline = pOls;
            IFillShapeElement pFse = pElm as IFillShapeElement;
            pFse.Symbol = pFsb;
            pGpc.AddElement((IElement)pFse, 0);
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void AddShapefile_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFdlg = new OpenFileDialog();
            OpenFdlg.Title = "选择需要加载的地理数据库";
            OpenFdlg.Filter = "Shape格式文件（*.shp）|*.shp";
            OpenFdlg.ShowDialog();
            string strFileName = OpenFdlg.FileName;
            if (strFileName == string.Empty)
            {
                MessageBox.Show(strFileName + "不是有效的Shapefile", "文件错误");
                return;
            }
            string pathName = System.IO.Path.GetDirectoryName(strFileName);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(strFileName);
            IFeatureClass pFc = OpenShapefileFeatureClass(FileName, pathName);
            IDataset pDs = pFc as IDataset;
            IFeatureLayer pFl = new FeatureLayerClass();
            pFl.FeatureClass = pFc;
            pFl.Name = pDs.Name;
            ILayer pL = pFl as ILayer;
            this.axMapControl1.AddLayer(pL);

            axMapControl2.ClearLayers();
            axMapControl2.AddShapeFile(pathName, FileName);
            axMapControl2.Extent = axMapControl2.FullExtent;
        }

        private void cbBookmarkList_SelectedIndexChanged(object sender, EventArgs e)
        {
            IMapBookmarks pbm = axMapControl1.Map as IMapBookmarks;
            IEnumSpatialBookmark pEsb = pbm.Bookmarks;
            pEsb.Reset();
            ISpatialBookmark pSbm = pEsb.Next();
            while (pSbm != null)
            {
                if (cbBookmarkList.SelectedItem.ToString() == pSbm.Name)
                {
                    pSbm.ZoomTo((IMap)axMapControl1.ActiveView);
                    axMapControl1.ActiveView.Refresh();
                    break;
                }
                pSbm = pEsb.Next();
            }
        }

        private void AddAOIBookmark_Click(object sender, EventArgs e)
        {
            AdmitBookmarkName frmABN = new AdmitBookmarkName(this);
            frmABN.Show();
        }

        private void miRenderSimply_Click(object sender, EventArgs e)
        {
            DataOperator pDo = new DataOperator(axMapControl1.Map,SelectedLayer_TOC);
            ILayer pL = pDo.GetLayerbyName(axMapControl1.get_Layer(0).Name);

            IRgbColor pRGBc = new RgbColorClass
            {
                Red = 255,
                Green = 0,
                Blue = 0
            };

            ISymbol pSb = MapComposer.GetSymbolFromLayer(pL);
            IColor pC = pRGBc as IColor;

            bool bRes = MapComposer.RenderSimply(pL, pC);
            if (bRes)
            {
                axTOCControl1.ActiveView.ContentsChanged();
                axMapControl1.ActiveView.Refresh();
                miRenderSimply.Enabled = false;
            }
            else
            {
                MessageBox.Show("简单渲染图层失败");
            }
        }

        private void miGetRenderInfo_Click(object sender, EventArgs e)
        {
            DataOperator pDo = new DataOperator(axMapControl1.Map,SelectedLayer_TOC);
            ILayer pL = pDo.GetLayerbyName(axMapControl1.get_Layer(0).Name);
            MessageBox.Show(MapComposer.GetRendererTypeByLayer(pL));
        }

        private void miMap_Click(object sender, EventArgs e)
        {
            if (miMap.Checked == false)
            {
                axToolbarControl1.SetBuddyControl(axMapControl1.Object);
                axTOCControl1.SetBuddyControl(axMapControl1.Object);

                axMapControl1.Show();
                axPageLayoutControl1.Hide();

                miMap.Checked = true;
                miPageLayout.Checked = false;
                miPrint.Enabled = false;
                miOutput.Enabled = false;

            }
            else
            {
                axToolbarControl1.SetBuddyControl(axPageLayoutControl1.Object);
                axTOCControl1.SetBuddyControl(axPageLayoutControl1.Object);
                axMapControl1.Hide();
                axPageLayoutControl1.Show();

                miMap.Checked = false;
                miPageLayout.Checked = true;
                miPrint.Enabled = true;
                miOutput.Enabled = true;
            }
            tSslL.Text = "状态：显示地图";
        }

        private void miPageLayout_Click(object sender, EventArgs e)
        {
            if (miPageLayout.Checked == false||miPageLayout1.Checked==false)
            {
                axToolbarControl1.SetBuddyControl(axPageLayoutControl1.Object);
                axTOCControl1.SetBuddyControl(axPageLayoutControl1.Object);

                axPageLayoutControl1.Show();
                axMapControl1.Hide();

                miPageLayout.Checked = true;
                miMap.Checked = false;
                miPrint.Enabled = true;
                miOutput.Enabled = true;
            }
            else
            {
                axToolbarControl1.SetBuddyControl(axMapControl1.Object);
                axTOCControl1.SetBuddyControl(axMapControl1.Object);

                axPageLayoutControl1.Hide();
                axMapControl1.Show();

                miPageLayout.Checked = false;
                miMap.Checked = true;
                miPrint.Enabled = false;
                miOutput.Enabled = false;
            }
            tSslL.Text = "状态：显示页面布局";
        }

        private void miPrint_Click(object sender, EventArgs e)
        {
            IPrinter pPt = axPageLayoutControl1.Printer;
            if (pPt == null)
            {
                MessageBox.Show("获取默认打印机失败！");
            }

            string sMsg = "是否使用默认打印机：" + pPt.Name + "?";
            if (MessageBox.Show(sMsg, "", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }

            IPaper pPr = pPt.Paper;
            pPr.Orientation = 1;

            IPage pPg = axPageLayoutControl1.Page;
            pPg.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingScale;

            axPageLayoutControl1.PrintPageLayout(1, 1, 0);
        }

        private void miOutput_Click(object sender, EventArgs e)
        {
            IActiveView pAv;
            IExport pEp;
            IPrintAndExport pPae;
            int iOutputResolution = 300;

            if (miPageLayout.Checked)
            {
                pAv = axMapControl1.ActiveView;
            }
            else
            {
                pAv = axPageLayoutControl1.ActiveView;
            }

            pEp = new ExportJPEGClass();
            pPae = new PrintAndExportClass();

            pEp.ExportFileName = "C:\\Export.JPG";

            pPae.Export(pAv, pEp, iOutputResolution, true, null);

            MessageBox.Show("成功导出到C盘！");
        }

        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            IActiveView pAv = axPageLayoutControl1.ActiveView.FocusMap as IActiveView;
            IDisplayTransformation pDtm = pAv.ScreenDisplay.DisplayTransformation;
            pDtm.VisibleBounds = axMapControl1.Extent;
            axPageLayoutControl1.ActiveView.Refresh();
            GeoMapLoad.CopyAndOverwriteMap(axMapControl1, axPageLayoutControl1);
        }

        private void axMapControl1_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
        {
            try
            {
                axTOCControl1.Update();
                GeoMapLoad.CopyAndOverwriteMap(axMapControl1, axPageLayoutControl1);
            }
            catch (System.Windows.Forms.AxHost.InvalidActiveXStateException)
            {
            }
        }

        private void miCreatShapefile_Click(object sender, EventArgs e)
        {
            DataOperator pDo = new DataOperator(axMapControl1.Map,SelectedLayer_TOC);
            IFeatureClass pFc = pDo.CreateShapefile("C:\\", "ShapefileWorkspace", "ShapefileSample");
            if (pFc == null)
            {
                MessageBox.Show("创建Shape文件失败");
                return;
            }

            bool bRes = pDo.AddFeatureClassToMap(pFc, "Observation Stations");
            if (bRes)
            {
                miCreatShapefile.Enabled = false;
                return;
            }
            else
            {
                MessageBox.Show("将新建Shape文件加入地图失败");
                return;
            }
        }

        private void miAddFeature_Click(object sender, EventArgs e)
        {
            if (miAddFeature.Checked == false)
            {
                miAddFeature.Checked = true;
                AddLine.Checked = false;
                AddPolygon.Checked = false;
            }
            else
            {
                miAddFeature.Checked = false;
            }
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (miAddFeature.Checked == true)
            {
                IPoint pPt = new PointClass();
                pPt.PutCoords(e.mapX, e.mapY);
                IMap pMap;
                IActiveView pActiveView;
                pMap = axMapControl1.Map;
                pActiveView = pMap as IActiveView;

                IMarkerElement pMarkerElement;
                pMarkerElement = new MarkerElementClass();
                ISimpleMarkerSymbol pMarkerSymbol;
                pMarkerSymbol = new SimpleMarkerSymbolClass();

                pMarkerSymbol.Size = 2;
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                IElement pElement;
                pElement = pMarkerElement as IElement;
                pElement.Geometry = pPt;
                pMarkerElement.Symbol = pMarkerSymbol;
                IGraphicsContainer pGraphicsContainer;
                pGraphicsContainer = pMap as IGraphicsContainer;
                pGraphicsContainer.AddElement(pMarkerElement as IElement, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                DataOperator pDo = new DataOperator(axMapControl1.Map,SelectedLayer_TOC);
                pDo.AddFeatureToLayer("Observation Stations","观测站",pPt);
            }
            else if (AddLine.Checked == true)
            {
                IPoint pPt = new PointClass();
                pPt.PutCoords(e.mapX, e.mapY);
                IMap pMap;
                IActiveView pActiveView;
                pMap = axMapControl1.Map;
                pActiveView = pMap as IActiveView;

                IMarkerElement pMarkerElement;
                pMarkerElement = new MarkerElementClass();
                ISimpleMarkerSymbol pMarkerSymbol;
                pMarkerSymbol = new SimpleMarkerSymbolClass();

                pMarkerSymbol.Size = 2;
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                IElement pElement;
                pElement = pMarkerElement as IElement;
                pElement.Geometry = pPt;
                pMarkerElement.Symbol = pMarkerSymbol;
                IGraphicsContainer pGraphicsContainer;
                pGraphicsContainer = pMap as IGraphicsContainer;
                pGraphicsContainer.AddElement(pMarkerElement as IElement, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                miAddFeature.Checked = false;
                AddPolygon.Checked = false;
                IPolyline pPolyline;//绘制线
                pPolyline = axMapControl1.TrackLine() as IPolyline;//产生一个SimpleLineSymbol符号
                ISimpleLineSymbol pSimpleLineSym;
                pSimpleLineSym = new SimpleLineSymbolClass();
                pSimpleLineSym.Style = esriSimpleLineStyle.esriSLSSolid;
                //需要用户动态选择
                //设置符号颜色
                IRgbColor pColor = new RgbColorClass();//最好由用户动态选择
                pColor.Red = 120;
                pColor.Green = 200;
                pColor.Blue = 180;
                //pSimpleLineSym.Color = GetRGB(120, 200, 180);
                pSimpleLineSym.Width = 1;
                //产生一个PolylineElement对象
                ILineElement pLineEle;
                pLineEle = new LineElementClass();
                IElement pEle;
                pEle = pLineEle as IElement;
                pEle.Geometry = pPolyline;
                //将元素添加到Map对象中
                pGraphicsContainer.AddElement(pEle, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            else if (AddPolygon.Checked == true)
            {
                IPoint pPt = new PointClass();
                pPt.PutCoords(e.mapX, e.mapY);
                IMap pMap;
                IActiveView pActiveView;
                pMap = axMapControl1.Map;
                pActiveView = pMap as IActiveView;

                IMarkerElement pMarkerElement;
                pMarkerElement = new MarkerElementClass();
                ISimpleMarkerSymbol pMarkerSymbol;
                pMarkerSymbol = new SimpleMarkerSymbolClass();

                pMarkerSymbol.Size = 2;
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                IElement pElement;
                pElement = pMarkerElement as IElement;
                pElement.Geometry = pPt;
                pMarkerElement.Symbol = pMarkerSymbol;
                IGraphicsContainer pGraphicsContainer;
                pGraphicsContainer = pMap as IGraphicsContainer;
                pGraphicsContainer.AddElement(pMarkerElement as IElement, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                IPolygon pPolygion;//绘制面
                pPolygion = axMapControl1.TrackPolygon() as IPolygon;//产生一个SimpleFillSymbol符号
                ISimpleFillSymbol pSimpleFillSym;
                pSimpleFillSym = new SimpleFillSymbolClass();
                pSimpleFillSym.Style = esriSimpleFillStyle.esriSFSDiagonalCross;//需要用户动态选择

                IRgbColor pcolor = new RgbColorClass();
                //设置符号颜色
                pcolor.Red = 220;
                pcolor.Green = 112;
                pcolor.Blue = 60;
                // pSimpleFillSym.Color = GetRGB(220, 112, 60);
                IFillShapeElement pPolygonEle; //产生一个PolygonElement对象
                pPolygonEle = new PolygonElementClass();
                pPolygonEle.Symbol = pSimpleFillSym;
                IElement pElle;
                pElle = pPolygonEle as IElement;
                pElle.Geometry = pPolygion;
                //将元素添加到Map对象之中
                pGraphicsContainer.AddElement(pElle, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            if (e.button == 2)
            {
                int s = splitContainer2.Width;
                System.Drawing.Point pPts = new System.Drawing.Point();
                pPts.X = e.x + s;
                pPts.Y = e.y;
                pPts = this.axTOCControl1.PointToScreen(pPts);
                this.aMctms.Show(pPts);
            }

        }

        public ILayer SelectedLayer_TOC = null;

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            ILayer pL = new FeatureLayerClass();
            this.axTOCControl1.ContextMenuStrip = null;
            IBasicMap pBm = new MapClass();
            object other = null;
            object index = null;
            esriTOCControlItem pIm = esriTOCControlItem.esriTOCControlItemNone;
            try
            {
                this.axTOCControl1.HitTest(e.x, e.y, ref pIm, ref pBm, ref pL, ref other, ref index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            switch (e.button)
            {
                case 1:
                    if (pIm == esriTOCControlItem.esriTOCControlItemLegendClass)
                    {
                        if (pL is IAnnotationSublayer)
                        {
                            return;
                        }
                        else
                        {
                            pMovelayer = pL;
                        }
                    }
                    break;
                case 2:
                    if (pIm == esriTOCControlItem.esriTOCControlItemLayer)
                    {
                        SelectedLayer_TOC = pL;
                        System.Drawing.Point pPt = new System.Drawing.Point();
                        pPt.X = e.x;
                        pPt.Y = e.y;
                        pPt = this.axTOCControl1.PointToScreen(pPt);
                        this.cmTOC.Show(pPt);
                    }
                    break;
            }
        }

        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            if (e.button == 1)
            {
                esriTOCControlItem pIm = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap pBm = null;
                ILayer pL = null;
                object other = null;
                object index = null;
                axTOCControl1.HitTest(e.x,e.y,ref pIm,ref pBm,ref pL,ref other,ref index);
                IMap pMap = axMapControl1.ActiveView.FocusMap;
                if (pIm == esriTOCControlItem.esriTOCControlItemLayer || pL != null)
                {
                    if (pMovelayer != pL)
                        for (int i = 0; i < pMap.LayerCount; i++)
                        {
                            pTempLayer = pMap.get_Layer(i);
                            if (pTempLayer == pL)
                            {
                                toindex = i;
                            }
                        }
                    try
                    {
                        pMap.MoveLayer(pMovelayer, toindex);
                        axMapControl1.ActiveView.Refresh();
                        axMapControl1.Update();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            MapDocument pMd = new MapDocumentClass();
            SaveFileDialog pSfd = new SaveFileDialog();
            pSfd.Title = "新建空白地图";
            pSfd.Filter = "Map Document|*.mxd";
            pSfd.ShowDialog();
            string sFilePath = pSfd.FileName;
            if (sFilePath == "")
            {
                return;
            }
            else
            {
                pMd.New(sFilePath);
                pMd.Open(sFilePath, "");
                axMapControl1.Map = pMd.get_Map(0);
            }
        }

        private void SaveDocument_Click(object sender, EventArgs e)
        {
            SaveDoc();
        }

        public void SaveDoc()
        {
            IMapDocument pMapDocument = new MapDocumentClass();
            if (axMapControl1.DocumentFilename != null)
            {
                if (axMapControl1.CheckMxFile(axMapControl1.DocumentFilename))
                {
                    pMapDocument = new MapDocumentClass();
                    pMapDocument.Open(axMapControl1.DocumentFilename, "");
                    if (pMapDocument.get_IsReadOnly(pMapDocument.DocumentFilename))
                    {
                        MessageBox.Show("地图文档是只读的！");
                        pMapDocument.Close();
                        return;
                    }
                    pMapDocument.ReplaceContents((IMxdContents)axMapControl1.Map);
                    pMapDocument.Save(pMapDocument.UsesRelativePaths, true);
                    pMapDocument.Close();
                    MessageBox.Show("地图保存成功");
                }
            }
        }

        private void SaveAsDocument_Click(object sender, EventArgs e)
        {
            SaveAsDoc();
        }

        public void SaveAsDoc()
        {
            SaveFileDialog pSfd = new SaveFileDialog();
            pSfd.Title = "Save Document(*.mxd)|*.mxd";
            pSfd.Filter = "Map Document(*.mxd)|*.mxd";
            pSfd.ShowDialog();
            string sFilePath = pSfd.FileName;
            if (sFilePath == "")
            {
                return;
            }
            if (sFilePath == m_MapDocument.DocumentFilename)
            {
                SaveDoc();
            }
            else
            {
                m_MapDocument.SaveAs(sFilePath, true, true);
                MessageBox.Show("Document saved successfully!","Done!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (SelectedLayer_TOC != null)
            {
                axMapControl1.Map.DeleteLayer(SelectedLayer_TOC);
                axMapControl2.Map.DeleteLayer(SelectedLayer_TOC);
                SelectedLayer_TOC = null;
            }
        }

        private void pOpenTable_Click(object sender, EventArgs e)
        {
            sPtlP(SelectedLayer_TOC);
        }

        private ILayer m_pLayer;
        public void sPtlP(ILayer pLayer)
        {
            m_pLayer = pLayer;
            DataBoard pDb = new DataBoard(SelectedLayer_TOC.Name);
            pDb.CreateAttributeTable(m_pLayer);
            pDb.ShowDialog();

        }

        private void miSpatilFilter_Click(object sender, EventArgs e)
        {
            MapAnalysis pMa = new MapAnalysis();
            pMa.QueryIntersect("Word Cities","Continents",axMapControl1.Map,esriSpatialRelationEnum.esriSpatialRelationIntersection);
            IActiveView pAcv;
            pAcv = axMapControl1.ActiveView;
            pAcv.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,0,axMapControl1.Extent);
            Form1.Success();
        }

        private void miStatistic_Click(object sender, EventArgs e)
        {
            MapAnalysis pMa = new MapAnalysis();
            string sMsg;
            sMsg = pMa.Statistic("Continents","SQMI",axMapControl1.Map);
            MessageBox.Show(sMsg);
        }

        private void miBuffer_Click(object sender, EventArgs e)
        {
            /*Buffer bf = new Buffer(axMapControl1.Map);
            bf.Show();*/
            MapAnalysis pMa = new MapAnalysis();
            pMa.Buffer("World Cities","CITY_NAME='Beijing'",1,axMapControl1.Map);
            IActiveView pAv;
            pAv = axMapControl1.ActiveView;
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,0,axMapControl1.Extent);
        }

        private void openClassCMS_Click(object sender, EventArgs e)
        {
            DataOperator pDot = new DataOperator(axMapControl1.Map, SelectedLayer_TOC);
            DataBoard pDb = new DataBoard(SelectedLayer_TOC.Name);
            pDb.Show();
        }

        private void btnSymbolRenderer_Click(object sender, EventArgs e)
        {
            IGeoFeatureLayer pGeoFeatureLayer;
            IFeatureLayer pFeatureLayer;
            IProportionalSymbolRenderer pProportionalSymbolR;
            ITable pTable;
            IQueryFilter pQueryFilter;
            ICursor pCursor;
            IFillSymbol pFillSymbol;
            ICharacterMarkerSymbol pCharaterMarkerS;
            IDataStatistics pDataStatistics;
            IStatisticsResults pStatisticsResult;
            stdole.StdFont pFontDisp;
            IRotationRenderer pRotationRenderer;
            IMap pMap = axMapControl1.Map;
            pMap.ReferenceScale = 0;
            pFeatureLayer = (IGeoFeatureLayer)pMap.get_Layer(0);
            pGeoFeatureLayer = (IGeoFeatureLayer)pFeatureLayer;
            pTable = (ITable)pGeoFeatureLayer;
            pQueryFilter = new QueryFilterClass();
            pQueryFilter.AddField("");
            pCursor = pTable.Search(pQueryFilter, true);
            //使用statistics对象来计算最大值最小值
            pDataStatistics = new DataStatisticsClass();
            pDataStatistics.Cursor = pCursor;
            //设置要统计的字段名称
            pDataStatistics.Field = "POP_CLASS";
            //获取统计结果
            pStatisticsResult = pDataStatistics.Statistics;
            if (pStatisticsResult == null)
            {
                MessageBox.Show("获取对象失败");
                return;
            }
            //设置符号的背景填充色
            pFillSymbol = new SimpleFillSymbolClass();
            IRgbColor backColor = new RgbColorClass();
            backColor.Red = 239;
            backColor.Green = 228;
            backColor.Blue = 190;
            pFillSymbol.Color = backColor;
            //设置依比例符号的符号类型
            pCharaterMarkerS = new CharacterMarkerSymbolClass();
            pFontDisp = new stdole.StdFontClass();
            pFontDisp.Name = "ESRI Business";
            pFontDisp.Size = 20;
            pCharaterMarkerS.Font = (IFontDisp)pFontDisp;
            pCharaterMarkerS.CharacterIndex = 90;
            IRgbColor color = new RgbColorClass();
            backColor.Red = 0;
            backColor.Green = 0;
            backColor.Blue = 0;
            pCharaterMarkerS.Color = color;
            //创建一个新的比例变化的符号来对POP_CLASS字段进行渲染符号化
            pCharaterMarkerS.Size = 8;
            pProportionalSymbolR = new ProportionalSymbolRendererClass();
            pProportionalSymbolR.ValueUnit = esriUnits.esriUnknownUnits;
            pProportionalSymbolR.Field = "POP_CLASS";
            pProportionalSymbolR.FlanneryCompensation = false;
            pProportionalSymbolR.MinDataValue = pStatisticsResult.Minimum;
            pProportionalSymbolR.MaxDataValue = pStatisticsResult.Maximum;
            pProportionalSymbolR.BackgroundSymbol = pFillSymbol;
            pProportionalSymbolR.MinSymbol = (ISymbol)pCharaterMarkerS;
            pProportionalSymbolR.LegendSymbolCount = 5;
            pProportionalSymbolR.CreateLegendSymbols();
            //根据POP_CLASS的值来设置符号的旋转角度
            pRotationRenderer = (IRotationRenderer)pProportionalSymbolR;
            pRotationRenderer.RotationField = "POP_CLASS";
            pRotationRenderer.RotationType = esriSymbolRotationType.esriRotateSymbolArithmetic;
            //设置states图层为依比例变化符号渲染图层并刷新显示
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pProportionalSymbolR;
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void 缓冲区查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeatureClass pFcl;
            IFeature pF;
            IGeometry pGt;
            DataOperator pDo = new DataOperator(axMapControl1.Map, null);
            IFeatureLayer pFl = (IFeatureLayer)pDo.GetLayerbyName("World Cities");
            pFcl = pFl.FeatureClass;
            IQueryFilter pQf = new QueryFilterClass();
            pQf.WhereClause = "CITY_NAME='"+this.textBox1.Text.Trim()+"'";
            IFeatureCursor pFc;
            pFc = (IFeatureCursor)pFcl.Search(pQf, false);
            int count = pFcl.FeatureCount(pQf);
            pF = pFc.NextFeature();
            pGt = pF.Shape;
            ITopologicalOperator pTo = (ITopologicalOperator)pGt;
            IGeometry pGtBuffer = pTo.Buffer(1);
            ISpatialFilter pSf = new SpatialFilter();
            pSf.Geometry = pGtBuffer;
            pSf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIndexIntersects;
            IFeatureSelection pFs = (IFeatureSelection)pFl;
            pFs.SelectFeatures(pSf, esriSelectionResultEnum.esriSelectionResultNew, false);
        }

        private void 打开属性表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (miMap.Checked == false)
            {
                axToolbarControl1.SetBuddyControl(axMapControl1.Object);
                axTOCControl1.SetBuddyControl(axMapControl1.Object);

                axMapControl1.Show();
                axPageLayoutControl1.Hide();

                miMap.Checked = true;
                miPageLayout.Checked = false;
                miMap1.Checked = true;
                miPageLayout1.Checked = false;
            }
            else
            {
                axToolbarControl1.SetBuddyControl(axPageLayoutControl1.Object);
                axTOCControl1.SetBuddyControl(axPageLayoutControl1.Object);
                axMapControl1.Hide();
                axPageLayoutControl1.Show();

                miMap1.Checked = false;
                miPageLayout1.Checked = true;
                miMap.Checked = false;
                miPageLayout.Checked = true;
            }
            tSslL.Text = "状态：显示地图";
        }

        private void 布局视图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (miPageLayout.Checked == false||miPageLayout1.Checked==false)
            {
                axToolbarControl1.SetBuddyControl(axPageLayoutControl1.Object);
                axTOCControl1.SetBuddyControl(axPageLayoutControl1.Object);

                axPageLayoutControl1.Show();
                axMapControl1.Hide();

                miPageLayout1.Checked = true;
                miMap1.Checked = false;
                miPageLayout.Checked = true;
                miMap.Checked = false;
                miPrint.Enabled = true;
                miOutput.Enabled = true;
            }
            else
            {
                axToolbarControl1.SetBuddyControl(axMapControl1.Object);
                axTOCControl1.SetBuddyControl(axMapControl1.Object);

                axPageLayoutControl1.Hide();
                axMapControl1.Show();

                miPageLayout.Checked = false;
                miMap.Checked = true;
                miPageLayout1.Checked = false;
                miMap1.Checked = true;
            }
            tSslL.Text = "状态：显示页面布局";
        }

        private void axPageLayoutControl1_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            if (e.button == 2)
            {
                int s = splitContainer2.Width;
                System.Drawing.Point pPts = new System.Drawing.Point();
                pPts.X = e.x + s;
                pPts.Y = e.y;
                pPts = this.axTOCControl1.PointToScreen(pPts);
                this.aMctms.Show(pPts);
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint pPt;
            pPt = axMapControl1.ToMapPoint(e.x, e.y);
            if (pPt.X > 0 && pPt.Y > 0)
            {
                tSxy.Text = "                                                                                                                                          东经=" + pPt.X.ToString("F4") + "°   北纬=" + pPt.Y.ToString("F4") + "°";
            }
            else if (pPt.X > 0 && pPt.Y < 0)
            {
                tSxy.Text = "                                                                                                                                          东经=" + pPt.X.ToString("F4") + "°   南纬=" + (-pPt.Y).ToString("F4") + "°";
            }
            else if (pPt.X < 0 && pPt.Y > 0)
            {
                tSxy.Text = "                                                                                                                                          西经=" + (-pPt.X).ToString("F4") + "°   北纬=" + pPt.Y.ToString("F4") + "°";
            }
            else
            {
                tSxy.Text = "                                                                                                                                          西经=" + (-pPt.X).ToString("F4") + "°   南纬=" + (-pPt.Y).ToString("F4") + "°";
            }
        }

        private void 插入比例尺ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strBarType = "双线交换式比例尺";
            IEnvelope pEnv = new EnvelopeClass();
            pEnv.PutCoords(6, 2, 12.4, 3.4);
            AddScaleBar(pEnv, strBarType);
        }

        private void AddScaleBar(IEnvelope pEnv, string strBarType)
        {
            IScaleBar pScaleBar;
            IMapFrame pMapFrame;
            IMapSurroundFrame pMapSurroundFrame;
            IMapSurround pMapSurround;
            IElementProperties pElementPro;
            //产生一 个 UID对象,使用它产生不同的MapSurround对象
            UID pUID = new UIDClass();
            pUID.Value = "esriCarto.Scalebar";
            IPageLayout pPageLayout;
            pPageLayout = axPageLayoutControl1.PageLayout;
            IGraphicsContainer pGraphicsContainer;
            pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IActiveView pActiveView;
            pActiveView = pGraphicsContainer as IActiveView;
            IMap pMap;
            pMap = pActiveView.FocusMap;
            //获得与地图相关的MapFrame
            pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            //产生一 个 MapsurroundFrame
            pMapSurroundFrame = pMapFrame.CreateSurroundFrame(pUID, null);
            //依据传入参数的不同使用不同类型的比例尺
            switch (strBarType)
            {
                case "单线交互比括例尺 ":
                    pScaleBar = new AlternatingScaleBarClass();
                    break;
                case "双线交互式比例尺":
                    pScaleBar = new DoubleAlternatingScaleBarClass();
                    break;
                case "中空式比括例尺":
                    pScaleBar = new HollowScaleBarClass();
                    break;
                case "线式比例尺":
                    pScaleBar = new ScaleLineClass();
                    break;
                case "分割式比括例尺 ":
                    pScaleBar = new SingleDivisionScaleBarClass();
                    break;
                case "阶梯式比括例尺":
                    pScaleBar = new SteppedScaleLineClass();
                    break;
                default:
                    pScaleBar = new ScaleLineClass();
                    break;
            }
            //设置比括例尺属性 
            pScaleBar.Division = 4;
            pScaleBar.Divisions = 4;
            pScaleBar.LabelGap = 4;
            pScaleBar.LabelPosition = esriVertPosEnum.esriAbove;
            pScaleBar.Map = pMap;
            pScaleBar.Name = "";
            pScaleBar.Subdivisions = 2;
            pScaleBar.UnitLabel = "";
            pScaleBar.UnitLabelGap = 4;
            pScaleBar.UnitLabelPosition = esriScaleBarPos.esriScaleBarAbove;
            pScaleBar.Units = esriUnits.esriKilometers;
            pMapSurround = pScaleBar;
            pMapSurroundFrame.MapSurround = pMapSurround;
            pElementPro = pMapSurroundFrame as IElementProperties;
            pElementPro.Name = "myscalebar";
            //将 MapSurroundFrame对象添加到控件中
            axPageLayoutControl1.AddElement(pMapSurroundFrame as IElement, pEnv, Type.Missing, Type.Missing, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void 插入指北针ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IMap pMap;
            IActiveView pActiveView;
            pMap = axMapControl1.Map;
            pActiveView = pMap as IActiveView;
            pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
            pMap = pActiveView.FocusMap as IMap;
            IGraphicsContainer pGraphicsContainer;
            pGraphicsContainer = pActiveView as IGraphicsContainer;
            IMapFrame pMapFrame;
            pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapSurround pMapSurround;
            INorthArrow pNorthArrow;
            pNorthArrow = new MarkerNorthArrowClass();
            pMapSurround = pNorthArrow;
            pMapSurround.Name = "NorthArrow";
            //定义UID
            UID uid = new UIDClass();
            uid.Value = "esriCarto.MarkerNorthArrow";
            //定义MapSurroundFrame对象
            IMapSurroundFrame pMapSurroundFrame = pMapFrame.CreateSurroundFrame(uid, null);
            pMapSurroundFrame.MapSurround = pMapSurround;
            //定义Envelope设置摆放的位置
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope.PutCoords(16, 2, 19.4, 2.4);


            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnvelope;
            pGraphicsContainer.AddElement(pElement, 0);
        }

        private void 插入图例ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGraphicsContainer graphicsContainer = axPageLayoutControl1.ActiveView.GraphicsContainer;
            //得到MapFrame对象
            IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(axPageLayoutControl1.ActiveView.FocusMap);
            if (mapFrame == null) return;
            //生成一个图例
            UID uID = new UIDClass();
            uID.Value = "esriCarto.Legend";
            //从MapFrame中D生成一个MapSurroundFrame
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);
            if (mapSurroundFrame == null) return;
            if (mapSurroundFrame.MapSurround == null) return;
            //MapSurroundFrame名称
            mapSurroundFrame.MapSurround.Name = "图例";
            ILegend pleg;
            pleg = new Legend();
            pleg = mapSurroundFrame.MapSurround as ILegend;
            pleg.Title = "图 例";
            //设置图例的现实范围
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(2, 1.4, 4.4, 2.8);
            IElement element = (IElement)mapSurroundFrame;
            element.Geometry = envelope;
            //添加图例元素
            axPageLayoutControl1.ActiveView.GraphicsContainer.AddElement(element, 0);
            //PageLayoutControl刷新视图
            axPageLayoutControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void 插入标题ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double pagex = 0;
            double pagey = 0;
            IActiveView activeView;
            IGraphicsContainer graphicsContainer;
            ITextElement textElement;
            ITextSymbol textSymbol;
            IRgbColor color;
            IElement element;
            IEnvelope envelope;


            activeView = axPageLayoutControl1.PageLayout as IActiveView;
            envelope = new EnvelopeClass();
            envelope.PutCoords(0, 0, 5, 5);
            envelope.PutCoords(pagex - 15, pagey - 9, pagex + 35, pagey + 63);
            textElement = new TextElementClass();
            element = textElement as IElement;
            element.Geometry = envelope;
            textElement.Text = "我的地图";
            textSymbol = new TextSymbolClass();
            color = new RgbColorClass();
            color.Green = 0;
            color.Blue = 0;
            color.Red = 0;
            textSymbol.Color = color as IColor;
            textSymbol.Size = 30;
            textElement.Symbol = textSymbol;
            graphicsContainer = activeView as IGraphicsContainer;
            graphicsContainer.AddElement(element, 0);
            axPageLayoutControl1.Refresh();
        }

        private void AddLine_Click(object sender, EventArgs e)
        {
            if (AddLine.Checked == false)
            {
                AddLine.Checked = true;
                AddPolygon.Checked = false;
                miAddFeature.Checked = false;
            }
            else
            {
                AddLine.Checked = false;
            }
        }

        private void AddPolygon_Click(object sender, EventArgs e)
        {
            if (AddPolygon.Checked == false)
            {
                AddPolygon.Checked = true;
                AddLine.Checked = false;
                miAddFeature.Checked = false;
            }
            else
            {
                AddPolygon.Checked = false;
            }
        }

        private void 添加栅格数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                string fileName;
                openFile.Title = "添加栅格数据";
                openFile.Filter = "IMG图像(*.img)|*.img|TIFF图像（*tif)|*.tif|JPG(*.jpg)|*.jpg";
                openFile.ShowDialog();
                fileName = openFile.FileName;
                IRasterLayer rasterLayer = new RasterLayerClass();
                rasterLayer.CreateFromFilePath(fileName);
                axMapControl1.AddLayer(rasterLayer, 0);
                axMapControl2.ClearLayers();
                axMapControl2.AddLayer(rasterLayer, 0);
                axMapControl2.Extent = axMapControl1.FullExtent;

            }
            catch
            {
                MessageBox.Show("添加栅格数据错误！");
            }
        }

        private void axMapControl1_OnFullExtentUpdated(object sender, IMapControlEvents2_OnFullExtentUpdatedEvent e)
        {
           
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 拉伸渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IRasterLayer rasterLayer = new RasterLayerClass();
            rasterLayer = axMapControl1.get_Layer(0) as IRasterLayer;
            ChangeRender2RaseterStretchColorRampRender(rasterLayer);
            axMapControl1.ActiveView.Refresh();
        }

        private void ChangeRender2RaseterStretchColorRampRender(IRasterLayer rasterLayer)
        {
            IRaster raster=new Raster();
            raster= rasterLayer.Raster;
            IRasterStretchColorRampRenderer rasterStretchColorRampRender = new RasterStretchColorRampRendererClass();
            IRasterRenderer rasterRender = rasterStretchColorRampRender as IRasterRenderer;
            rasterRender.Raster = raster;
            rasterRender.Update();
            IColor fromColor = new RgbColorClass();
            fromColor = getRGB(255, 0, 0);
            IColor toColor = new RgbColorClass();
            toColor = getRGB(0, 255, 0);
            //创建起止颜色带
            IAlgorithmicColorRamp algorithmicColorRamp = new AlgorithmicColorRampClass();
            algorithmicColorRamp.Size = 255;
            algorithmicColorRamp.FromColor = fromColor;
            algorithmicColorRamp.ToColor = toColor;
            bool test = true;
            algorithmicColorRamp.CreateRamp(out test);
            //选择拉伸颜色带符号化波段
            rasterStretchColorRampRender.BandIndex = 0;
            //设置拉伸颜色带符号化所采用的颜色带
            rasterStretchColorRampRender.ColorRamp = algorithmicColorRamp;
            rasterRender.Update();
            rasterLayer.Renderer = rasterStretchColorRampRender as IRasterRenderer;
        }

        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;

        }

        private void 分级渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IRasterLayer rasterLayer = new RasterLayerClass();
            rasterLayer = axMapControl1.get_Layer(0) as IRasterLayer;
            funColorForRaster_Classify(rasterLayer);
            axMapControl1.ActiveView.Refresh();
        }


        public static void funColorForRaster_Classify(IRasterLayer pRasterLayer)
        {
            IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRenderer() as IRasterClassifyColorRampRenderer;
            IRasterRenderer pRRend = pRClassRend as IRasterRenderer;
            IRaster pRaster = pRasterLayer.Raster;
            IRasterBandCollection pRBandCol = pRaster as IRasterBandCollection;
            IRasterBand pRBand = pRBandCol.Item(0);
            if (pRBand.Histogram == null)
            {
                pRBand.ComputeStatsAndHist();
            }
            pRRend.Raster = pRaster;
            pRClassRend.ClassCount = 10;
            pRRend.Update();
            IRgbColor pFromColor = new RgbColor() as IRgbColor;
            pFromColor.Red = 200;
            pFromColor.Green = 10;
            pFromColor.Blue = 0;
            IRgbColor pToColor = new RgbColor() as IRgbColor;
            pToColor.Red = 0;
            pToColor.Green = 0;
            pToColor.Blue = 255;
            IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRamp() as IAlgorithmicColorRamp;
            colorRamp.Size = 10;
            colorRamp.FromColor = pFromColor;
            colorRamp.ToColor = pToColor;
            bool createColorRamp;
            colorRamp.CreateRamp(out createColorRamp);
            IFillSymbol fillSymbol = new SimpleFillSymbol() as IFillSymbol;
            for (int i = 0; i < pRClassRend.ClassCount; i++)
            {
                fillSymbol.Color = colorRamp.get_Color(i);
                pRClassRend.set_Symbol(i, fillSymbol as ISymbol);
                pRClassRend.set_Label(i, pRClassRend.get_Break(i).ToString("0.00"));
            }
            pRasterLayer.Renderer = pRRend;
        }

        private void 加ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IRasterLayer rasterLayer1 = new RasterLayerClass();
            IRasterLayer rasterLayer2 = new RasterLayerClass();
            rasterLayer1 = axMapControl1.get_Layer(0) as IRasterLayer;
            rasterLayer2 = axMapControl1.get_Layer(1) as IRasterLayer;
            IRasterLayer pOutRL = xiangjia(rasterLayer1, rasterLayer2);
            axMapControl1.AddLayer(pOutRL);
            //this.axMapControl1.Refresh();
            this.axMapControl1.ActiveView.Refresh();
            MessageBox.Show("加法计算结果!");

        }
        private static IRasterLayer xiangjia(IRasterLayer pRL1, IRasterLayer pRL2)
        {
            IRaster pR1 = pRL1.Raster;
            IRaster pR2 = pRL2.Raster;
            IGeoDataset pGeoDT1 = pR1 as IGeoDataset;
            IGeoDataset pGeoDT2 = pR2 as IGeoDataset;
            IMapAlgebraOp pRSalgebra = new RasterMapAlgebraOpClass();

            pRSalgebra.BindRaster(pGeoDT1, "raster1");
            pRSalgebra.BindRaster(pGeoDT2, "raster2");

            //MessageBox.Show("成功第一步");
            IGeoDataset pOutGeoDT = pRSalgebra.Execute("[raster1] + [raster2]");

            IRasterLayer pOutRL = new RasterLayerClass();
            pOutRL.CreateFromRaster(pOutGeoDT as IRaster);
            //MessageBox.Show("方法结束！");
            return pOutRL;
        }

        private void 减ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IRasterLayer rasterLayer1 = new RasterLayerClass();
            IRasterLayer rasterLayer2 = new RasterLayerClass();
            rasterLayer1 = axMapControl1.get_Layer(0) as IRasterLayer;
            rasterLayer2 = axMapControl1.get_Layer(1) as IRasterLayer;
            IRasterLayer pOutRL = xiangjian(rasterLayer1, rasterLayer2);
            axMapControl1.AddLayer(pOutRL);
            //this.axMapControl1.Refresh();
            this.axMapControl1.ActiveView.Refresh();
            MessageBox.Show("减法计算结果!");
        }

        private static IRasterLayer xiangjian(IRasterLayer pRL1, IRasterLayer pRL2)
        {
            IRaster pR1 = pRL1.Raster;
            IRaster pR2 = pRL2.Raster;
            IGeoDataset pGeoDT1 = pR1 as IGeoDataset;
            IGeoDataset pGeoDT2 = pR2 as IGeoDataset;
            IMapAlgebraOp pRSalgebra = new RasterMapAlgebraOpClass();

            pRSalgebra.BindRaster(pGeoDT1, "raster1");
            pRSalgebra.BindRaster(pGeoDT2, "raster2");

            //MessageBox.Show("成功第一步");
            IGeoDataset pOutGeoDT = pRSalgebra.Execute("[raster1] - [raster2]");

            IRasterLayer pOutRL = new RasterLayerClass();
            pOutRL.CreateFromRaster(pOutGeoDT as IRaster);
            //MessageBox.Show("方法结束！");
            return pOutRL;
        }

        private void 乘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IRasterLayer rasterLayer1 = new RasterLayerClass();
            IRasterLayer rasterLayer2 = new RasterLayerClass();
            rasterLayer1 = axMapControl1.get_Layer(0) as IRasterLayer;
            rasterLayer2 = axMapControl1.get_Layer(1) as IRasterLayer;
            IRasterLayer pOutRL = funReturnRasterLayerByRasterCalculatechengfa(rasterLayer1, rasterLayer2);
            axMapControl1.AddLayer(pOutRL);
            //this.axMapControl1.Refresh();
            this.axMapControl1.ActiveView.Refresh();
            MessageBox.Show("乘法计算结果!");
        }

        private static IRasterLayer funReturnRasterLayerByRasterCalculatechengfa(IRasterLayer pRL1, IRasterLayer pRL2)
        {

            IRaster pR1 = pRL1.Raster;
            IRaster pR2 = pRL2.Raster;
            IGeoDataset pGeoDT1 = pR1 as IGeoDataset;
            IGeoDataset pGeoDT2 = pR2 as IGeoDataset;
            IMapAlgebraOp pRSalgebra = new RasterMapAlgebraOpClass();

            pRSalgebra.BindRaster(pGeoDT1, "raster1");
            pRSalgebra.BindRaster(pGeoDT2, "raster2");

            //MessageBox.Show("成功第一步");
            IGeoDataset pOutGeoDT = pRSalgebra.Execute("[raster1] * [raster2]");

            IRasterLayer pOutRL = new RasterLayerClass();
            pOutRL.CreateFromRaster(pOutGeoDT as IRaster);
            //MessageBox.Show("方法结束！");
            return pOutRL;
        }

        private void 除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IRasterLayer rasterLayer1 = new RasterLayerClass();
            IRasterLayer rasterLayer2 = new RasterLayerClass();
            rasterLayer1 = axMapControl1.get_Layer(0) as IRasterLayer;
            rasterLayer2 = axMapControl1.get_Layer(1) as IRasterLayer;
            IRasterLayer pOutRL = xiangchu(rasterLayer1, rasterLayer2);
            axMapControl1.AddLayer(pOutRL);
            //this.axMapControl1.Refresh();
            this.axMapControl1.ActiveView.Refresh();
            MessageBox.Show("除法计算结果!");
        }

        private static IRasterLayer xiangchu(IRasterLayer pRL1, IRasterLayer pRL2)
        {
            IRaster pR1 = pRL1.Raster;
            IRaster pR2 = pRL2.Raster;
            IGeoDataset pGeoDT1 = pR1 as IGeoDataset;
            IGeoDataset pGeoDT2 = pR2 as IGeoDataset;
            IMapAlgebraOp pRSalgebra = new RasterMapAlgebraOpClass();

            pRSalgebra.BindRaster(pGeoDT1, "raster1");
            pRSalgebra.BindRaster(pGeoDT2, "raster2");

            //MessageBox.Show("成功第一步");
            IGeoDataset pOutGeoDT = pRSalgebra.Execute("[raster1] / [raster2]");

            IRasterLayer pOutRL = new RasterLayerClass();
            pOutRL.CreateFromRaster(pOutGeoDT as IRaster);
            //MessageBox.Show("方法结束！");
            return pOutRL;
        }

        private void 设置空间坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSpatialReference frmAB = new SetSpatialReference();
            frmAB.Show();

        }

        private void 查看空间坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pMAP = axMapControl1.Map;
            IFeatureLayer pLayer;
            pLayer = pMAP.get_Layer(0) as IFeatureLayer;
            IGeoDataset pGeoDataset;
            ISpatialReference pSpatialReference;
            pGeoDataset = pLayer as IGeoDataset;
            pSpatialReference = pGeoDataset.SpatialReference;
            MessageBox.Show(pSpatialReference.Name);

        }

        private void 改变空间坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IMap pMap;
            IActiveView pActiveView;
            pMap = axMapControl1.Map;
            pActiveView = pMap as IActiveView;
            IFeatureLayer player;
            player = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureClass pFeatureClass;
            pFeatureClass = player.FeatureClass;
            IGeoDataset pGeoDataset;
            pGeoDataset = pFeatureClass as IGeoDataset;
            IGeoDatasetSchemaEdit pGeoDatasetEdit;
            pGeoDatasetEdit = pGeoDataset as IGeoDatasetSchemaEdit;
            if (pGeoDatasetEdit.CanAlterSpatialReference == true)
            {
                ISpatialReferenceFactory2 pSpatRefFact;
                pSpatRefFact = new SpatialReferenceEnvironmentClass();
                IGeographicCoordinateSystem pGeoSys;
                pGeoSys = pSpatRefFact.CreateGeographicCoordinateSystem(4214);
                pGeoDatasetEdit.AlterSpatialReference(pGeoSys);
            }
            pActiveView.Refresh();
        }

        private void 开始编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            开始编辑ToolStripMenuItem.Enabled = false;
            EndEdit.Enabled = true;
            EditWork.Enabled = true;
            GeoShow.Enabled = true;
            ChangeWGS.Enabled = true;
            EditWGS.Enabled = true;
        }

        private void EndEdit_Click(object sender, EventArgs e)
        {
            开始编辑ToolStripMenuItem.Enabled = true;
            EndEdit.Enabled = false;
            EditWork.Enabled = false;
            GeoShow.Enabled = false;
            ChangeWGS.Enabled = false;
            EditWGS.Enabled = false;
        }

        private void 保存编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDoc();
        }

        private void 快捷键设计FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Keys k = new Keys();
            k.Show();
            return;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys k = new Keys();
            string s;
            k.lblM = new TextBox();
            s=k.lblM.Text;
            if (e.KeyCode.ToString() == s)
            {
                loadMapDoc();
            }
        }

        private void NoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noneToolStripMenuItem.Checked = true;
            cannonToolStripMenuItem.Checked = false;
            mINUETInGToolStripMenuItem.Checked = false;
            sp.Stop();
        }

        private void CannonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sp.Stop();
            noneToolStripMenuItem.Checked = false;
            cannonToolStripMenuItem.Checked = true;
            mINUETInGToolStripMenuItem.Checked = false;
            sp.SoundLocation = str1 + @"\bgm\Canon.wav";
            sp.PlayLooping();
        }

        private void MINUETInGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sp.Stop();
            noneToolStripMenuItem.Checked = false;
            cannonToolStripMenuItem.Checked = false;
            mINUETInGToolStripMenuItem.Checked = true;
            sp.SoundLocation = str1 + @"\bgm\MINUET in G.wav";
            sp.PlayLooping();
        }

        private void StePToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }

    //pageLayeout与map联动
    public class GeoMapLoad
    {
        public static void CopyAndOverwriteMap(AxMapControl axMapControl, AxPageLayoutControl axPageLayoutControl)
        {
            try
            {
                IObjectCopy pOc = new ObjectCopyClass();
                object toCopyMap = axMapControl.Map;
                object CopiedMap = pOc.Copy(toCopyMap);

                object overwriteMap = axPageLayoutControl.ActiveView.FocusMap;
                pOc.Overwrite(CopiedMap, ref overwriteMap);
            }
            catch (System.Windows.Forms.AxHost.InvalidActiveXStateException)
            {
            }
        }
    }
}
