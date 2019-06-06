using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Display;

namespace VioGIS
{
    /// <summary>
    /// Summary description for MapZoomOutTool.
    /// </summary>
    [Guid("8408abc0-1705-4e22-9caa-9e0572db28e7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("VioGIS.MapZoomOutTool")]
    public sealed class MapZoomOutTool : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion
        private INewEnvelopeFeedback m_feedBack;
        private IPoint m_point;
        private Boolean m_isMouseDown;
        private IHookHelper m_hookHelper;

        public MapZoomOutTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "缩小"; //localizable text 
            base.m_caption = "缩小";  //localizable text 
            base.m_message = "缩小";  //localizable text
            base.m_toolTip = "缩小";  //localizable text
            base.m_name = "缩小";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            base.m_category = "GeoMapPlane";
            base.m_bitmap = new System.Drawing.Bitmap(string.Format("{0}\\软件开发统一图标\\ZoomOut.bmp",Application.StartupPath));
            base.m_cursor = new System.Windows.Forms.Cursor(string.Format("{0}\\软件开发统一图标\\ZoomOut.cur",Application.StartupPath));
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add MapZoomOutTool.OnCreate implementation
        }

        public override bool Enabled
        {
            get
            {
                if (m_hookHelper == null)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add MapZoomOutTool.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add MapZoomOutTool.OnMouseDown implementation
            if (m_hookHelper.ActiveView == null)
                return;
            if (m_hookHelper.ActiveView is IActiveView)
            {
                IPoint pPt = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X,Y) as IPoint;
                IMap pMap = m_hookHelper.ActiveView.HitTestMap(pPt);
                if (pMap == null)
                    return;
                if (pMap != m_hookHelper.FocusMap)
                {
                    m_hookHelper.ActiveView.FocusMap = pMap;
                    m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics,null,null);
                }
            }
            IActiveView pAcv = (IActiveView)m_hookHelper.FocusMap;
            m_point = pAcv.ScreenDisplay.DisplayTransformation.ToMapPoint(X,Y);
            m_isMouseDown = true;
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add MapZoomOutTool.OnMouseMove implementation
            if (!m_isMouseDown)
                return;
            IActiveView pAcv = m_hookHelper.FocusMap as IActiveView;
            if (m_feedBack == null)
            {
                m_feedBack = new NewEnvelopeFeedbackClass();
                m_feedBack.Display = pAcv.ScreenDisplay;
                m_feedBack.Start(m_point);
            }
            base.m_cursor = new System.Windows.Forms.Cursor(string.Format("{0}\\软件开发统一图标\\MoveZoomOut.cur",Application.StartupPath));
            m_feedBack.MoveTo(pAcv.ScreenDisplay.DisplayTransformation.ToMapPoint(X,Y));
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add MapZoomOutTool.OnMouseUp implementation
            base.m_cursor = new System.Windows.Forms.Cursor(string.Format("{0}\\软件统一开发图标\\ZoomOut.cur",Application.StartupPath));
            if (!m_isMouseDown)
                return;
            IEnvelope pEnv;
            IEnvelope pFeedenv;
            double newWidth, newHeight;
            IActiveView pAcv = (IActiveView)m_hookHelper.FocusMap;
            if (m_feedBack == null)
            {
                pEnv = pAcv.Extent;
                pEnv.Expand(1.5, 1.5, true);
                pEnv.CenterAt(m_point);
            }
            else
            {
                pFeedenv = m_feedBack.Stop();
                if (pFeedenv.Width == 0 || pFeedenv.Height == 0)
                {
                    m_feedBack = null;
                    m_isMouseDown = false;
                }
                newWidth = pAcv.Extent.Width * (pAcv.Extent.Width / pFeedenv.Width);
                newHeight=pAcv.Extent.Height*(pAcv.Extent.Height/pFeedenv.Height);
                pEnv = new EnvelopeClass();
                pEnv.PutCoords(pAcv.Extent.XMin - ((pFeedenv.XMin - pAcv.Extent.XMin) * (pAcv.Extent.Width / pFeedenv.Width)),
                    pAcv.Extent.YMin - ((pFeedenv.YMin - pAcv.Extent.YMin) * (pAcv.Extent.Height / pFeedenv.Height)),
                    (pAcv.Extent.XMin - ((pFeedenv.XMin - pAcv.Extent.XMin) * (pAcv.Extent.Width / pFeedenv.Width))) + newWidth,
                    (pAcv.Extent.YMin - ((pFeedenv.YMin - pAcv.Extent.YMin) * (pAcv.Extent.Height / pFeedenv.Height))) + newHeight);
            }
            pAcv.Extent = pEnv;
            pAcv.Refresh();
            m_feedBack = null;
            m_isMouseDown = false;
        }
        #endregion
    }
}
